using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace OnlineRevision
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            StripeConfiguration.SetApiKey("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");
        }

        protected void Session_Start()
        {
        }
    }
}
