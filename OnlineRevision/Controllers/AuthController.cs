using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OnlineRevision.DbContext;
using System.Globalization;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;

namespace OnlineRevision.Controllers
{
    public class AuthController : Controller
    {

        // GET: Auth
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(string name, string pass)
        {
            string email = Request.Form["email"];
            string password = Request.Form["password"];

            using (OnlineRevisionEntities db = new OnlineRevisionEntities())
            {
                var query = (from c in db.UserDetails
                             where (c.Email == email) && c.Password == password && (c.PaymentId == null || c.PaymentId == "")
                             select c).ToList();

                DateTime today = DateTime.Now.Date;

                if (query.Count > 0)
                {
                    ViewBag.ErrorMessage = "Your payment is due. So your account isn't activated.";
                }
                else
                {
                    var result = (from c in db.UserDetails
                                  where (c.Email == email) && c.Password == password && c.Status == 1 && today <= EntityFunctions.TruncateTime(c.ValidTill)
                                  select c).ToList();

                    var queryResult = (from c in db.UserDetails
                                       where (c.Email == email) && c.Password == password && c.Status == 1 && today > EntityFunctions.TruncateTime(c.ValidTill)
                                       select c).ToList();

                    if (result.Count > 0)
                    {
                        foreach (var d in result)
                        {
                            Session["userName"] = d.UserName;
                            Session["userType"] = d.UserType.ToString();
                            Session["userId"] = d.UserId.ToString();
                        }

                        TrackUsers aTrackUsers = new TrackUsers();

                        aTrackUsers.Email = email;
                        aTrackUsers.Activity = "Login";
                        aTrackUsers.LoginTime = DateTime.Now;

                        try
                        {
                            db.TrackUsers.Add(aTrackUsers);
                            db.SaveChanges();
                        }

                        catch(Exception ex)
                        {
                            ex.ToString();
                        }

                        return RedirectToAction("Index", "Dashboard");
                    }
                    else if (queryResult.Count > 0)
                    {
                        ViewBag.ErrorMessage = "Your subscription period is over. Please subscribe using packages - <a href='/Payment/Packages'>Click here</a>";
                        return View();
                    }
                    else
                    {
                        ViewBag.ErrorMessage = "User id or password doesn't match!";
                        return View();
                    }
                }
            }

            return View();
        }

        public ActionResult Logout()
        {
            Session.Remove("userName");
            Session.Abandon();

            return RedirectToAction("Login", "Auth");
        }

        [HttpPost]
        public ActionResult Logout(string logout)
        {
            Session.Remove("userName");
            Session.Abandon();

            return RedirectToAction("Login", "Auth");
        }

        public ActionResult Registration()
        {
            return View();
        }
    }
}