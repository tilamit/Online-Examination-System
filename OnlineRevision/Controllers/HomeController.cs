using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Stripe;
using System.Collections.Specialized;

namespace OnlineRevision.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            if (string.IsNullOrEmpty(Session["userName"] as string))
            {
                Session["userName"] = "";
            }
            else
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
                        return RedirectToAction("Index", "Payment");
                    }
                }
            }

            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}