$(document).ready(function () {
    function Validate() {
        var email = document.getElementById("txtEmail").value;

        if (email == "") {
            alert("Please provie email!");
        }
    }

    $(function () {
        $(".primary-btn").click(function () {
            $('#LoginModalUp').modal('show');
        });

        $('#radio1').click(function () {
            $('#btnPaypal').show();
            $('#btnCard').hide();
            $('#showPara').slideUp();
        });

        $('#radio2').click(function () {
            $('#btnPaypal').hide();
            $('#btnCard').show();
            $('#showPara').slideDown();
        });

        $('#link1').hide();
        $('#link2').hide();
        $('#lblMsg').hide();
        $('#showPara').hide();
        $('#btnPaypal').hide();
        $('#btnCard').hide();
    });

    $(function() {
        var creditly = Creditly.initialize(
          "#ExpDate",
        );
        $(".payment__confirm").click(function(e) {
            e.preventDefault();
            var output = creditly.validate();
            $ele = $("#ExpDate");
            var today = new Date();
            if (output) {
                // Your validated credit card output
                console.log(output);
                if(output.expiration_year > today.getFullYear()){
                    $ele.next().show().text("Card is expired.");
                }
                else{
                    $ele.next().hide();
                }
            }
        });
    });
    
    var Creditly = (function() {
        var getInputValue = function(e, selector) {
            var inputValue = $.trim($(selector).val());
            inputValue = inputValue + String.fromCharCode(e.which);
            return getNumber(inputValue);
        };

        var getNumber = function(string) {
            return string.replace(/[^\d]/g, "");
        };

        var reachedMaximumLength = function(e, maximumLength, selector) {
            return getInputValue(e, selector).length > maximumLength;
        };

        // Backspace, delete, tab, escape, enter, ., Ctrl+a, Ctrl+c, Ctrl+v, home, end, left, right
        var isEscapedKeyStroke = function(e) {
            return ( $.inArray(e.which,[46,8,9,0,27,13,190]) !== -1 ||
              (e.which == 65 && e.ctrlKey === true) || 
              (e.which == 67 && e.ctrlKey === true) || 
              (e.which == 86 && e.ctrlKey === true) || 
              (e.which >= 35 && e.which <= 39));
        };

        var isNumberEvent = function(e) {
            return (/^\d+$/.test(String.fromCharCode(e.which)));
        };

        var onlyAllowNumeric = function(e, maximumLength, selector) {
            e.preventDefault();
            // Ensure that it is a number and stop the keypress
            if (reachedMaximumLength(e, maximumLength, selector) || e.shiftKey || (!isNumberEvent(e))) {
                return false;
            }
            return true;
        };


        var shouldProcessInput = function(e, maximumLength, selector) {
            return (!isEscapedKeyStroke(e)) && onlyAllowNumeric(e, maximumLength, selector);
        };

        var Validation = (function() {
            var Validators = (function() {
                var expirationRegex = /(\d\d)\s*\/\s*(\d\d)/;

                var creditCardExpiration = function(selector, data) {
                    var expirationVal = $.trim($(selector).val());
                    var match = expirationRegex.exec(expirationVal);
                    var isValid = false;
                    var outputValue = ["", ""];
                    if (match && match.length === 3) {
                        var month = parseInt(match[1], 10);
                        var year = "20" + match[2];
                        if (month >= 0 && month <= 12) {
                            isValid = true;
                            var outputValue = [month, year];
                        }
                    }

                    return {
                        "is_valid": isValid,
                        "messages": [data["message"]],
                        "output_value": outputValue
                    };
                };


                return {
                    creditCardExpiration: creditCardExpiration,
                };
            })();

            var ValidationErrorHolder = (function() {
                var errorMessages = [];
                var selectors = [];

                var addError = function(selector, validatorResults) {
                    if (validatorResults.hasOwnProperty("selectors")) {
                        selectors = selectors.concat(validatorResults["selectors"]);
                    } else {
                        selectors.push(selector)
                    }

                    errorMessages.concat(validatorResults["messages"]);
                };

                var triggerErrorMessage = function() {
                    var errorsPayload = {
                        "selectors": selectors,
                        "messages": errorMessages
                    };
                    for (var i=0; i<selectors.length; i++) {
                        $(selectors[i]).addClass("has-error");
                    }
                    $("#ExpDate").next().show().text("Date is not valid.");
                    $("body").trigger("creditly_client_validation_error", errorsPayload);
                };

                return {
                    addError: addError,
                    triggerErrorMessage: triggerErrorMessage
                };
            });

            var ValidationOutputHolder = (function() {
                var output = {};

                var addOutput = function(outputName, value) {
                    var outputParts = outputName.split(".");
                    var currentPart = output;
                    for (var i=0; i<outputParts.length; i++) {
                        if (!currentPart.hasOwnProperty(outputParts[i])) {
                            currentPart[outputParts[i]] = {};
                        }

                        // Either place the value into the output, or continue going down the
                        // search space.
                        if (i === outputParts.length-1) {
                            currentPart[outputParts[i]] = value
                        } else {
                            currentPart = currentPart[outputParts[i]];
                        }
                    }
                };

                var getOutput = function() {
                    return output;
                };

                return {
                    addOutput: addOutput,
                    getOutput: getOutput
                }
            });

            var processSelector = function(selector, selectorValidatorMap, errorHolder, outputHolder) {
                if (selectorValidatorMap.hasOwnProperty(selector)) {
                    var currentMapping = selectorValidatorMap[selector];
                    var validatorType = currentMapping["type"];
                    var fieldName = currentMapping["name"];
                    var validatorResults = Validators[validatorType](selector, currentMapping["data"]);

                    if (validatorResults["is_valid"]) {
                        if (currentMapping["output_name"] instanceof Array) {
                            for (var i=0; i<currentMapping["output_name"].length; i++) {
                                outputHolder.addOutput(currentMapping["output_name"][i],
                                    validatorResults["output_value"][i]);
                            }
                        } else {
                            outputHolder.addOutput(currentMapping["output_name"],
                                validatorResults["output_value"]);
                        }
                    } else {
                        errorHolder.addError(selector, validatorResults);
                        return true;
                    }
                }
            };

            var validate = function(selectorValidatorMap) {
                var errorHolder = ValidationErrorHolder();
                var outputHolder = ValidationOutputHolder();
                var anyErrors = false;
                for (var selector in selectorValidatorMap) {
                    if (processSelector(selector, selectorValidatorMap, errorHolder, outputHolder)) {
                        anyErrors = true;
                    }
                }
                if (anyErrors) {
                    errorHolder.triggerErrorMessage();
                    return false;
                } else {
                    return outputHolder.getOutput();
                }
            };

            return {
                validate: validate
            };
        })();

        var ExpirationInput = (function() {
            var maximumLength = 4;
            var selector;

            var createExpirationInput = function(mainSelector) {
                selector = mainSelector
                $(selector).keypress(function(e) {
                    $(selector).removeClass("has-error");
                    if (shouldProcessInput(e, maximumLength, selector)) {
                        var inputValue = getInputValue(e, selector);
                        if (inputValue.length >= 2) {
                            var newInput = inputValue.slice(0, 2) + " / " + inputValue.slice(2);
                            $(selector).val(newInput);
                        } else {
                            $(selector).val(inputValue);
                        }
                    }
                });
            };

            var parseExpirationInput = function(expirationSelector) {
                var inputValue = getNumber($(expirationSelector).val());
                var month = inputValue.slice(0,2);
                var year = "20" + inputValue.slice(2);
                return {
                    'year': year,
                    'month': month
                };
            };

            return {
                createExpirationInput: createExpirationInput,
                parseExpirationInput: parseExpirationInput
            };
        })();


        var initialize = function(expirationSelector) {
            createSelectorValidatorMap(expirationSelector);
            ExpirationInput.createExpirationInput(expirationSelector);
            return this;
        };

        var selectorValidatorMap;

        var createSelectorValidatorMap = function(expirationSelector) {
            var optionValues = {};
            optionValues["expiration_message"] = optionValues["expiration_message"] || "Your credit card expiration is invalid";

            selectorValidatorMap = {};
            selectorValidatorMap[expirationSelector] = {
                "type": "creditCardExpiration",
                "data": {
                    "message": optionValues["expiration_message"]
                },
                "output_name": ["expiration_month", "expiration_year"]
            };
        };

        var validate = function() {
            return Validation.validate(selectorValidatorMap);
        };

        return {
            initialize: initialize,
            validate: validate,
        };
    })();

    debugger;
    $("body").on("click", "#btnCard", function (e) {
        var email = $('#txtEmail').val();
        var card = $('#cardnumber').val();
        var mnthYear = $('#ExpDate').val().split('/');
        var cvc = $('#cvc').val();

        console.log(card);
        console.log(mnthYear[0] + mnthYear[1]);
        console.log(cvc);

        var isValid = true;
        $('input[type="text"]').each(function () {
            if ($.trim($(this).val()) == '') {
                isValid = false;
                $(this).css({
                    "border": "1px solid red",
                    "background": "#FFCECE"
                });
            }
            else {
                $(this).css({
                    "border": "",
                    "background": ""
                });
            }
        });

        if (isValid == false)
        {
            e.preventDefault();
        }
        else
        {
            Stripe.setPublishableKey("pk_live_51HYH5zI7bYFbeO1sBNuO16x4CbLwkaBgj7XHsuaUKyrpr7ALghsmTAHyazYiTYAkaHdf5uikrvdsWOPFpKsBO1zn00y2ojMA8h");
            e.preventDefault();
            e.stopPropagation();

            Stripe.card.createToken({
                number: $('#cardnumber').val(),
                cvc: $('#cvc').val(),
                exp_month: mnthYear[0].trim(),
                exp_year: mnthYear[1].trim()
            }, stripeResponseHandler);
        }
    });

    function stripeResponseHandler(status, response) {
        var $form = $('#form1');
        if (response.error) {
            alert(response.error.message);
        } else {
            var token = response.id;
            $('#hfStripeToken').val(token);
            $('#total-price-pointer').val($('.price').text());
            $('#package').val($('.package').text());
            $('#email').val($('#txtEmail').val());
            $form.get(0).submit();
        }
    }
});