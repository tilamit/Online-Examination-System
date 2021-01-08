using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Stripe;
using System.Collections.Specialized;

namespace OnlineRevision.Controllers
{
    public class PayWithStripeController : Controller
    {
        // GET: PayWithStripe
        public ActionResult Index()
        {
            if (Request.HttpMethod == "POST")
            {
                NameValueCollection nvc = Request.Form;
                string hfStripeToken = nvc["hfStripeToken"];
                string package = nvc["package"];
                string email = nvc["email"];
                long price = Convert.ToInt64(nvc["total-price-pointer"]);

                var customers = new CustomerService();
                var charges = new ChargeService();

                var customer = customers.Create(new CustomerCreateOptions
                {
                    SourceToken = hfStripeToken
                });

                var options = new TokenCreateOptions
                {
                    CustomerId = customer.Id
                };

                var charge = charges.Create(new ChargeCreateOptions
                {
                    Amount = price * 100,
                    Description = package + " with email id: " + email,
                    Currency = "GBP",
                    CustomerId = customer.Id
                });

                if (!string.IsNullOrEmpty(hfStripeToken))
                {
                    Session["stripeToken"] = hfStripeToken;
                    Session["email"] = email;
                    Session["package"] = package;

                    return RedirectToAction("Index", "Payment");
                }
            }

            return View();
        }
    }
}