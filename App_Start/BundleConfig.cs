using System.Web.Optimization;

namespace Cake
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            BundleTable.EnableOptimizations = true;

            bundles.Add(new ScriptBundle("~/scripts/Prev").Include(
                    "~/scripts/DefaultScripts/jquery-1.10.2.min.js"
                ));

            bundles.Add(new ScriptBundle("~/scripts/Later").Include(
                "~/scripts/SiteScripts/Site.js",
                "~/scripts/DefaultScripts/jquery-ui.min.js",
                "~/scripts/UtilityScripts/owl.carousel.min.js",
                "~/scripts/UtilityScripts/agile-min.js",
                "~/scripts/UtilityScripts/jquery.ui.touch-punch.min.js",
                "~/scripts/UtilityScripts/jquery.maskedinput.min.js",
                "~/scripts/UtilityScripts/cookieconsent.min.js",
                "~/scripts/UtilityScripts/webslidemenu.js",
                "~/scripts/UtilityScripts/jquery.lazyload.js",
                "~/scripts/DefaultScripts/modernizr-2.6.2.js",
                "~/scripts/CakeScripts/jquery.mousewheel.min.js",
                "~/scripts/UtilityScripts/jquery.easeScroll.js"
                ));

            bundles.Add(new StyleBundle("~/Content/Prev").Include(
                "~/Content/bootstrap.min.css",
                "~/Content/Site.css",
                "~/Content/owl.carousel.css",
                "~/Content/webslidemenu.css"
            ));

            bundles.Add(new StyleBundle("~/Content/Later").Include(
                "~/Content/SiteMobile.css",
                "~/Content/owl.theme.css",
                "~/Content/jquery-ui.css",
                "~/Content/opensans.css",
                "~/Content/raleway.css"
            ));

            bundles.Add(new ScriptBundle("~/scripts/Idx").Include(
                "~/scripts/UtilityScripts/jquery.rateyo.min.js",
                "~/scripts/UtilityScripts/swiper.js",
                "~/scripts/UtilityScripts/instafeed.min.js"
            ));

            bundles.Add(new StyleBundle("~/Content/Idx").Include(
                "~/Content/jquery.rateyo.min.css",
                "~/Content/swiper.css"
            ));
        }
    }
}