using System.Web;
using System.Web.Optimization;

namespace OnlineRevision
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                       "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/site.css"));

            bundles.Add(new StyleBundle("~/Content/Registration").Include(
                     "~/bower_components/bootstrap/dist/css/bootstrap.min.css",
                     "~/bower_components/font-awesome/css/font-awesome.min.css",
                     "~/bower_components/Ionicons/css/ionicons.min.css",
                     "~/dist/css/AdminLTE.min.css",
                     "~/dist/css/skins/_all-skins.min.css",
                     "~/dist/css/skins/_all-skins.min.css"));

            bundles.Add(new ScriptBundle("~/Script/Registration").Include(
                  "~/Scripts/jquery-1.10.2.min.js",
                  "~/Scripts/jquery.validate.js",
                  "~/Scripts/jquery.validate.unobtrusive.min.js"));

            bundles.Add(new StyleBundle("~/Content/Layout").Include(
                    "~/bower_components/bootstrap/dist/css/bootstrap.min.css",
                    "~/bower_components/font-awesome/css/font-awesome.min.css",
                    "~/bower_components/Ionicons/css/ionicons.min.css",
                    "~/dist/css/AdminLTE.min.css",
                    "~/dist/css/skins/_all-skins.min.css",
                    "~/bower_components/morris.js/morris.css",
                    "~/bower_components/jvectormap/jquery-jvectormap.css",
                    "~/bower_components/bootstrap-datepicker/dist/css/bootstrap-datepicker.min.css",
                    "~/bower_components/bootstrap-daterangepicker/daterangepicker.css",
                    "~/plugins/bootstrap-wysihtml5/bootstrap3-wysihtml5.min.css"));


            bundles.Add(new ScriptBundle("~/Script/Layout").Include(
                    "~/bower_components/jquery/dist/jquery.min.js",
                    "~/bower_components/jquery-ui/jquery-ui.min.js",
                    "~/bower_components/bootstrap/dist/js/bootstrap.min.js",
                    "~/bower_components/raphael/raphael.min.js",
                    "~/bower_components/morris.js/morris.min.js",
                    "~/bower_components/jquery-sparkline/dist/jquery.sparkline.min.js",
                    "~/plugins/jvectormap/jquery-jvectormap-1.2.2.min.js",
                    "~/plugins/jvectormap/jquery-jvectormap-world-mill-en.js",
                    "~/bower_components/jquery-knob/dist/jquery.knob.min.js",
                    "~/bower_components/moment/min/moment.min.js",
                    "~/bower_components/bootstrap-daterangepicker/daterangepicker.js",
                    "~/bower_components/bootstrap-datepicker/dist/js/bootstrap-datepicker.min.js",
                    "~/plugins/bootstrap-wysihtml5/bootstrap3-wysihtml5.all.min.js",
                    "~/bower_components/jquery-slimscroll/jquery.slimscroll.min.js",
                    "~/bower_components/fastclick/lib/fastclick.js",
                    "~/dist/js/adminlte.min.js",
                    "~/dist/js/pages/dashboard.js",
                    "~/dist/js/demo.js"));

            bundles.Add(new ScriptBundle("~/Script/allPartialView").Include(
                  "~/Scripts/jquery-2.1.1.min.js",
                  "~/Scripts/bootstrap.js",
                  "~/Scripts/bootstrap.min.js"));

            bundles.Add(new ScriptBundle("~/Script/btnSwitch").Include(
                 "~/Scripts/jquery.btnswitch.js"));
        }
    }
}
