using Cake.Controllers;
using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace Cake
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            ViewEngines.Engines.Clear();
            ViewEngines.Engines.Add(new RazorViewEngine());
        }

        private void Session_Start(object sender, EventArgs e)
        {
            Session.Timeout = 1200;
        }

        protected void Application_Error(Object sender, EventArgs e)
        {
            var raisedException = Server.GetLastError();
            var url = "";
            bool discardException = false;

            if (raisedException.Message.Contains("potentially"))
            {
                discardException = true;
            }

            string[] mobile = new string[] { "/m/", "/mobile/", "/mobil/" };

            if (HttpContext.Current != null)
            {
                url = HttpContext.Current.Request.Url.ToString();
            }

            string[] urlDiscard = new string[] { "undefined", "apple-app" };

            for (int i = 0; i < urlDiscard.Length; i++) { if (url.Contains(urlDiscard[i])) { discardException = true; } }

            Server.ClearError();
            Response.Clear();

            try
            {
                if (discardException == false)
                {
                    DatabaseController dc = new DatabaseController();
                    AccountController ac = new AccountController();

                    dc.DBQuerySetter("insert into ErrorLogs (ID,DateTime,ErrorID,URL,Exception) values ('" + Guid.NewGuid() + "','" + DateTime.Now.ToString() + "','" + Guid.NewGuid() + "','" + url + "','" + raisedException.Message.Replace("'", "") + "')");

                    String errorBody = @"
                    <div style=""width:90%; margin-left:5%; font-family:Helvetica, sans-serif;"">
                        <div style=""margin:auto; min-width:330px; max-width:600px;"">
                            <div style=""padding:40px 0px; width:100%; float:left; background:white;"">
                                <img src=""http://www.icaked.com/Images/Site/main_logo.png"" style=""width:150px; float:left;"" />
                            </div>
                            <div style=""width:100%; float:left; border-top:1px dashed #d6d6d6; border-bottom:1px dashed #d6d6d6;"">
                                <div style=""width:100%; float:left; text-align:justify; padding:40px 0px; background:#f2f2f2; text-align:center;  margin:20px 0px; border-radius:5px; position:relative;"">
                                    <div style=""width:100%; float:left;"">
                                        <label style=""font-size:16px; color:black; float:left; width:100%; text-align:center; padding-bottom:30px;"">Hata Bildirimi</label>
                                        <label style=""font-size:14px; color:black; float:left; width:100%; text-align:center; padding-bottom:35px;"">URL : " + url + @"</label>
                                        <label style=""font-size:14px; color:black; float:left; width:100%; text-align:center; padding-bottom:35px;"">Exception : " + raisedException.Message + @"</label>
                                    </div>
                                </div>
                            </div>
                            <div style=""background:white; width:100%; float:left; padding:20px 0px;"">
                                <a style=""text-decoration:none; float:right;"">
                                    <label style=""font-size:13px; float:right; color:black;"">2016 © www.iCaked.com</label>
                                </a>
                            </div>
                        </div>
                    </div>
                    ";
                    ac.sendMail("info@icaked.com", "iCaked.com - Hata Bildirimi", errorBody, null, null);
                }
            }
            catch (Exception ex) { }

            try
            {
                if (((System.Web.HttpException)raisedException).GetHttpCode() == 404)
                {
                    string path = HttpContext.Current.Request.Path.ToLower();
                    if (mobile.Contains(path)) { Response.Redirect("~/"); }
                    else if (path.Contains("pastaneler")) { Response.Redirect("~/pastane"); }
                    else { Response.Redirect("~/sayfa-bulunamadi"); }
                }
                else { Response.Redirect("~/HataSayfasi"); }
            }
            catch (Exception ex) { Response.Redirect("~/HataSayfasi"); }
        }

        protected void Application_BeginRequest(Object sender, EventArgs e)
        {
            string[] redirects = new string[] {
                "icaked.com",
                "icaked.azurewebsites.net"
            };

            bool redirectRequired = false;


            for (int i = 0; i < redirects.Length; i++)
            {
                if (redirects[i] == Request.ServerVariables["HTTP_HOST"]) { redirectRequired = true; }
            }

            if ((HttpContext.Current.Request.IsSecureConnection.Equals(false) && HttpContext.Current.Request.IsLocal.Equals(false)) | redirectRequired)
            {
                Response.RedirectPermanent("https://www.icaked.com" + HttpContext.Current.Request.RawUrl.ToLower());
            }
        }
    }
}