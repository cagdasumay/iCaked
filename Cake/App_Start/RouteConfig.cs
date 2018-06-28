using System.Web.Mvc;
using System.Web.Routing;

namespace Cake
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            #region Home

            routes.MapRoute(
                name: "Index",
                url: "",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "ankara",
                url: "ankara",
                defaults: new { controller = "Home", action = "Index", id = "Ankara" }
            );

            routes.MapRoute(
                name: "istanbul",
                url: "istanbul",
                defaults: new { controller = "Home", action = "Index", id = "İstanbul" }
            );

            routes.MapRoute(
                name: "Intro",
                url: "Intro",
                defaults: new { controller = "Home", action = "Intro", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "en",
                url: "en",
                defaults: new { controller = "Home", action = "En", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "MiniBakeries",
                url: "MiniBakeries",
                defaults: new { controller = "Home", action = "MiniBakeries", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "pastani-tasarla",
                url: "pastani-tasarla",
                defaults: new { controller = "Home", action = "PastaniTasarla", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "butik-pastalar",
                url: "butik-pastalar/{category}",
                defaults: new { controller = "Home", action = "Kategoriler", id = UrlParameter.Optional, category = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "tezgah-pastalari",
                url: "tezgah-pastalari",
                defaults: new { controller = "Home", action = "Kategoriler", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Editor",
                url: "Editor/{lang}",
                defaults: new { controller = "Home", action = "Editor", id = UrlParameter.Optional, lang = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "EditorENG",
                url: "EditorENG",
                defaults: new { controller = "Home", action = "EditorENG", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "EditorFrame",
                url: "EditorFrame",
                defaults: new { controller = "Home", action = "EditorFrame", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Editor2",
                url: "Editor2",
                defaults: new { controller = "Home", action = "Editor2", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Tasarla",
                url: "Tasarla",
                defaults: new { controller = "Home", action = "Tasarla", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Kategoriler",
                url: "Kategoriler",
                defaults: new { controller = "Home", action = "RP", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Urunler",
                url: "Urunler/{category}",
                defaults: new { controller = "Home", action = "Kategoriler", id = UrlParameter.Optional, category = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "HataSayfasi",
                url: "HataSayfasi",
                defaults: new { controller = "Home", action = "HataSayfasi", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "SiparisTakibi",
                url: "SiparisTakibi",
                defaults: new { controller = "Home", action = "SiparisTakibi", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Trials",
                url: "Trials",
                defaults: new { controller = "Home", action = "Trials", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "NotFound",
                url: "sayfa-bulunamadi",
                defaults: new { controller = "Home", action = "NotFound", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Iletisim",
                url: "Iletisim",
                defaults: new { controller = "Home", action = "Iletisim", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Hakkimizda",
                url: "Hakkimizda",
                defaults: new { controller = "Home", action = "Hakkimizda", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Blog",
                url: "Blog",
                defaults: new { controller = "Home", action = "RP", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "SSS",
                url: "SSS",
                defaults: new { controller = "Home", action = "SSS", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Oturum",
                url: "Oturum",
                defaults: new { controller = "Home", action = "Oturum", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "SizdenGelenler",
                url: "SizdenGelenler",
                defaults: new { controller = "Home", action = "Tasarim", type = UrlParameter.Optional, designName = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Tasarim",
                url: "tasarim-{type}/{designName}",
                defaults: new { controller = "Home", action = "Tasarim", type = UrlParameter.Optional, designName = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Gorusler",
                url: "Gorusler",
                defaults: new { controller = "Home", action = "Gorusler", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "OdemeSecenekleri",
                url: "odeme-secenekleri",
                defaults: new { controller = "Home", action = "OdemeSecenekleri", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Satis",
                url: "mesafeli-satis-sozlesmesi",
                defaults: new { controller = "Home", action = "Satis", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Gizlilik",
                url: "kullanici-ve-gizlilik-sozlesmesi",
                defaults: new { controller = "Home", action = "Gizlilik", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "TasarimIstek",
                url: "istek-tasarim",
                defaults: new { controller = "Home", action = "TasarimIstek", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "FigurIstek",
                url: "istek-figur",
                defaults: new { controller = "Home", action = "FigurIstek", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "SevgililerGunu",
                url: "sevgililer-gunu",
                defaults: new { controller = "Home", action = "SevgililerGunu", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "ResimliPasta",
                url: "resimli-pasta",
                defaults: new { controller = "Home", action = "ResimliPasta", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "BasindaBiz",
                url: "basinda-biz",
                defaults: new { controller = "Home", action = "BasindaBiz", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "DagitimAgi",
                url: "dagitim-agi",
                defaults: new { controller = "Home", action = "DagitimAgi", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "parti-sepeti",
                url: "parti-sepeti",
                defaults: new { controller = "Home", action = "PartiSepeti", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "ilan",
                url: "ilan",
                defaults: new { controller = "Order", action = "Ilan", id = UrlParameter.Optional }
            );

            #endregion Home

            #region Bakery

            routes.MapRoute(
                name: "BakeryLogin",
                url: "pastane-girisi",
                defaults: new { controller = "Account", action = "BakeryLogin", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "BakeryOrderSub",
                url: "BakeryOrderSub",
                defaults: new { controller = "Bakery", action = "BakeryOrderSub", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Product",
                url: "pastane/{bakeryName}/{category}/{productName}",
                defaults: new { controller = "Bakery", action = "Product", category = UrlParameter.Optional, bakeryName = UrlParameter.Optional, productName = UrlParameter.Optional }
            );

            #endregion Bakery

            #region Order

            routes.MapRoute(
                name: "Sepetim",
                url: "Sepetim",
                defaults: new { controller = "Order", action = "Sepetim", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Siparis",
                url: "Siparis",
                defaults: new { controller = "Order", action = "Siparis", id = UrlParameter.Optional }
            );

            #endregion Order

            #region Account

            routes.MapRoute(
                name: "UserProfile",
                url: "Profil/{profileName}",
                defaults: new { controller = "Account", action = "UserProfile", profileName = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Pastane",
                url: "Pastane/{profileName}",
                defaults: new { controller = "Account", action = "Pastane", profileName = UrlParameter.Optional }
            );

            #endregion Account

            #region Purchase

            routes.MapRoute(
                name: "Result",
                url: "Result",
                defaults: new { controller = "Purchase", action = "Result", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Odeme",
                url: "Odeme",
                defaults: new { controller = "Purchase", action = "Odeme", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "SecurePayment",
                url: "SecurePayment",
                defaults: new { controller = "Purchase", action = "SecurePayment", id = UrlParameter.Optional }
            );

            #endregion Purchase

            #region Lending

            routes.MapRoute(
                name: "kadinlar-gunu",
                url: "kadinlar-gunu",
                defaults: new { controller = "Landing", action = "KadinlarGunu", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Dogumgunu",
                url: "Dogumgunu",
                defaults: new { controller = "Landing", action = "Dogumgunu", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "OnlineTasarla",
                url: "online-tasarla",
                defaults: new { controller = "Landing", action = "OnlineTasarla", id = UrlParameter.Optional }
            );

            #endregion Lending

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}