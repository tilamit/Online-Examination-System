using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace OnlineRevision
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );

       //     routes.MapRoute(
       //  name: "item_details",
       //  url: "Exam/{id}",
       //  defaults: new { controller = "Exam", action = "Index" }
       //);

        //    routes.MapRoute(
        //    name: "Exam",
        //    url: "Exam/{id}",
        //    defaults: new { controller = "Exam", action = "Index" },
        //    namespaces: new[] { "App.Web.Controllers" }
        //);
        }
    }
}
