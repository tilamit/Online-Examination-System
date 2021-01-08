using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PayPal.Api;
using OnlineRevision.Config;

namespace OnlineRevision.Controllers
{
    public class PayWithPaypalController : Controller
    {
        string paymentId = "";
        string msg = "";

        // GET: PayWithPaypal
        public ActionResult Index()
        {
            APIContext apiContext = PaypalConfiguration.GetAPIContext();
            string val = "";

            try
            {
                string payerId = Request.Params["PayerID"];

                if (string.IsNullOrEmpty(payerId))
                {
                    string baseURI = Request.Url.Scheme + "://" + Request.Url.Authority + "/PayWithPaypal?";

                    var guid = Convert.ToString((new Random()).Next(100000));

                    var createdPayment = this.CreatePayment(apiContext, baseURI + "guid=" + guid);
                    var links = createdPayment.links.GetEnumerator();

                    string paypalRedirectUrl = null;

                    while (links.MoveNext())
                    {
                        Links lnk = links.Current;

                        if (lnk.rel.ToLower().Trim().Equals("approval_url"))
                        {
                            paypalRedirectUrl = lnk.href;
                        }
                    }

                    Session.Add(guid, createdPayment.id); //Payment Id Here
                    Session["id"] = createdPayment.id;

                    val = createdPayment.id;

                    Response.Redirect(paypalRedirectUrl);
                }
                else
                {
                    var guid = Request.Params["guid"];

                    var executedPayment = ExecutePayment(apiContext, payerId, Session[guid] as string);

                    /**On Transaction Failed, Redirect User Here**/
                    if (executedPayment.state.ToLower() != "approved")
                    {
                        Response.Redirect("FailureView.aspx?id=" + Session["id"].ToString());
                    }
                }
            }

            catch (Exception ex)
            {
                if (ex.Message != "")
                {
                    msg += ex.Message;
                    Session["msg"] = msg;
                }
                //Response.Redirect("FailureView.aspx?id=" + Session["id"].ToString());
            }

            if (msg != "")
            {
                //Response.Redirect("FailureView.aspx");
            }
            else
            {
                /**On Successful Transaction, Redirect User Here**/
                return RedirectToAction("Index", "Payment", new { id = Request.QueryString["paymentId"] });
            }

            return View("Index", "Payment", new { id = Request.QueryString["paymentId"] }); ;
        }

        [HttpPost]
        public JsonResult GetPaymentWithPaypal(string packageName, string price, string email)
        {
            Session["package"] = packageName;
            Session["price"] = price;
            Session["email"] = email;

            return Json(true);
        }

        public PayPal.Api.Payment payment;
        public Payment ExecutePayment(APIContext apiContext, string payerId, string paymentId)
        {
            var paymentExecution = new PaymentExecution() { payer_id = payerId };
            this.payment = new Payment() { id = paymentId };
            return this.payment.Execute(apiContext, paymentExecution);
        }

        public Payment CreatePayment(APIContext apiContext, string redirectUrl)
        {
            var itemList = new ItemList() { items = new List<Item>() };

            itemList.items.Add(new Item()
            {
                name = Session["package"].ToString() + " with email id: " + Session["email"].ToString(),
                currency = "GBP",
                price = Session["price"].ToString(),
                quantity = "1",
                sku = "sku"
            });

            var payer = new Payer() { payment_method = "paypal" };

            var redirUrls = new RedirectUrls()
            {
                cancel_url = redirectUrl + "&Cancel=true",
                return_url = redirectUrl
            };

            var details = new Details()
            {
                tax = "0",
                shipping = "0",
                subtotal = Session["price"].ToString()
            };

            var amount = new Amount()
            {
                currency = "GBP",
                total = Session["price"].ToString(),
                details = details
            };

            var transactionList = new List<Transaction>();

            transactionList.Add(new Transaction()
            {
                description = "Order-" + DateTime.Now.ToString("yyyyMMddhhmmss", System.Globalization.CultureInfo.InvariantCulture),
                invoice_number = "Order-" + DateTime.Now.ToString("yyyyMMddhhmmss", System.Globalization.CultureInfo.InvariantCulture),
                amount = amount,
                item_list = itemList
            });


            this.payment = new Payment()
            {
                intent = "sale",
                payer = payer,
                transactions = transactionList,
                redirect_urls = redirUrls
            };

            return this.payment.Create(apiContext);
        }
    }
}