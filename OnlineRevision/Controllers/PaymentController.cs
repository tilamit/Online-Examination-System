using OnlineRevision.DbContext;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OnlineRevision.Controllers
{
    public class PaymentController : Controller
    {
        public static string MyGlobalString { get; set; }

        // GET: Payment
        public ActionResult Index()
        {
            string val = Session["stripeToken"] as string;
            string val2 = RouteData.Values["id"] as string;

            if (!string.IsNullOrEmpty(val2) || Session["stripeToken"] != null)
            {
                if (!string.IsNullOrEmpty(val2))
                {
                    Session["id"] = "PayPal Generated Id: " + val2;

                    string email = Session["email"].ToString();
                    string package = Session["package"].ToString();

                    using (OnlineRevisionEntities db = new OnlineRevisionEntities())
                    {
                        UserDetails user = new UserDetails();

                        var result = db.UserDetails.Where(c => c.Email == email);

                        foreach (var item in result)
                        {
                            item.PaymentId = val2;
                            item.ValidTill = DateTime.Now.AddDays(GetDays(package));
                        }

                        PaymentDetails aPaymentDetails = new PaymentDetails();
                        aPaymentDetails.PaymentId = val2;
                        aPaymentDetails.ValidTill = DateTime.Now.AddDays(GetDays(package));
                        aPaymentDetails.email = email;
                        aPaymentDetails.AddedOn = DateTime.Now;
                        aPaymentDetails.Status = 1;


                        db.PaymentDetails.Add(aPaymentDetails);
                        db.SaveChanges();
                    }
                }
                else
                {
                    Session["id"] = "Stripe Generated Id: " + Session["stripeToken"].ToString();

                    string email = Session["email"].ToString();
                    string package = Session["package"].ToString();

                    using (OnlineRevisionEntities db = new OnlineRevisionEntities())
                    {
                        UserDetails user = new UserDetails();

                        var result = db.UserDetails.Where(c => c.Email == email);

                        foreach (var item in result)
                        {
                            item.PaymentId = Session["stripeToken"].ToString();
                            item.ValidTill = DateTime.Now.AddDays(GetDays(package));
                        }

                        PaymentDetails aPaymentDetails = new PaymentDetails();
                        aPaymentDetails.PaymentId = Session["stripeToken"].ToString();
                        aPaymentDetails.ValidTill = DateTime.Now.AddDays(GetDays(package));
                        aPaymentDetails.email = email;
                        aPaymentDetails.AddedOn = DateTime.Now;
                        aPaymentDetails.Status = 1;


                        db.PaymentDetails.Add(aPaymentDetails);
                        db.SaveChanges();
                        db.SaveChanges();
                    }
                }
            }
            else
            {
                Response.Redirect("FailureView.aspx");
            }

            return View();
        }

        public int GetDays(string package)
        {
            int days = 0;

            if (package == "1 week access")
            {
                days = 7;
            }
            else if (package == "2 weeks access")
            {
                days = 14;
            }
            else if (package == "1 month access")
            {
                days = 30;
            }

            return days;
        }

        public ActionResult Packages()
        {
            return View();
        }
    }
}