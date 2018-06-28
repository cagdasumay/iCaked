using System;
using System.Web.Mvc;

namespace Cake.Controllers
{
    public class LandingController : Controller
    {
        private AccountController ac = new AccountController();

        public ActionResult KadinlarGunu()
        {
            return View();
        }

        public ActionResult Dogumgunu()
        {
            return View();
        }

        public ActionResult OnlineTasarla()
        {
            return View();
        }

        public JsonResult IstekPasta(FormCollection form)
        {
            String mail = form["email"];
            String name = form["name"];
            String message = form["message"];

            String serverBody = @"
                <div style=""width:90%; margin-left:5%; font-family:Helvetica, sans-serif;"">
                    <div style=""margin:auto; min-width:330px; max-width:600px;"">
                        <div style=""padding:40px 0px; width:100%; float:left; background:white;"">
                            <img src=""https://www.icaked.com/Images/Site/main_logo.png"" style=""width:150px; float:left;"" />
                        </div>
                        <div style=""width:100%; float:left; border-top:1px dashed #d6d6d6; border-bottom:1px dashed #d6d6d6;"">
                            <div style=""width:100%; padding:20px 0px; float:left; text-align:justify; background:#f2f2f2; text-align:center; margin:20px 0px; border-radius:5px; position:relative;"">
                                <div style=""width:100%; float:left;"">
                                    <label style=""font-size:14px; color:#92460f; float:left; width:100%; text-align:center; padding:15px 0px;"">
                                        Lending page'den istek pasta mesajı geldi. <br /> Mesaj detayları aşağıdadır.
                                    </label>
                                    <label style=""font-size:14px; color:#92460f; float:left; width:100%; text-align:center; padding:10px 0px;"">
                                        " + name + @" - " + mail + @"
                                    </label>
                                    <label style=""font-size:14px; color:#92460f; float:left; width:100%; text-align:center; padding:10px 0px;"">
                                        " + message + @"
                                    </label>
                                    <a href=""https://www.icaked.com"" style=""width:100%; float:left; font-weight:bold; cursor:pointer; font-size:14px;"">iCaked.com</a>
                                </div>
                            </div>
                        </div>
                        <div style=""background:white; width:100%; float:left; padding:15px 0px; margin:15px 0px;"">
                            <a href=""http://www.facebook.com/icaked""><img src=""https://www.icaked.com/Images/Site/fb_icon.png"" style=""float:left; margin:3px; width:25px; margin-top:0px; margin-left:15px;"" /></a>
                            <a href=""http://www.twitter.com/icakedcom""><img src=""https://www.icaked.com/Images/Site/tw_icon.png"" style=""float:left; margin:3px; width:25px; margin-top:0px;"" /></a>
                            <a href=""http://www.instagram.com/icakedcom""><img src=""https://www.icaked.com/Images/Site/in_icon.png"" style=""float:left; margin:3px; width:25px; margin-top:0px;"" /></a>
                            <a href=""http://www.facebook.com/icakedcom""><img src=""https://www.icaked.com/Images/Site/pi_icon.png"" style=""float:left; margin:3px; width:25px; margin-top:0px;"" /></a>
                            <a href=""https://www.icaked.com"" style=""text-decoration:none; float:right; margin-right:20px;"">
                                <label style=""font-size:13px; float:right; color:black; line-height:30px;"">2017 © www.iCaked.com</label>
                            </a>
                        </div>
                    </div>
                </div>";

            String clientBody = @"
                <div style=""width:90%; margin-left:5%; font-family:Helvetica, sans-serif;"">
                    <div style=""margin:auto; min-width:330px; max-width:600px;"">
                        <div style=""padding:40px 0px; width:100%; float:left; background:white;"">
                            <img src=""https://www.icaked.com/Images/Site/main_logo.png"" style=""width:150px; float:left;"" />
                        </div>
                        <div style=""width:100%; float:left; border-top:1px dashed #d6d6d6; border-bottom:1px dashed #d6d6d6;"">
                            <div style=""width:100%; padding:20px 0px; float:left; text-align:justify; background:#f2f2f2; text-align:center; margin:20px 0px; border-radius:5px; position:relative;"">
                                <div style=""width:100%; float:left; text-align:center;"">
                                    <img src=""https://www.icaked.com/Images/Site/thanks_icon.png"" style=""width:50px; margin-top:20px;"" />
                                    <label style=""font-size:20px; font-weight:bold; color:#92460f; float:left; width:100%; text-align:center; padding:15px 0px;"">
                                        Teşekkürler !
                                    </label>
                                    <label style=""font-size:14px; color:#92460f; float:left; width:100%; text-align:center; padding:15px 0px;"">
                                        Pasta siparişiniz alınmıştır. En kısa sürede sizi tasarım bölümümüze de bekleriz.
                                    </label>
                                    <a href=""https://www.icaked.com"" style=""width:100%; float:left; font-weight:bold; cursor:pointer; font-size:16px; color:saddlebrown;"">iCaked.com</a>
                                </div>
                            </div>
                        </div>
                        <div style=""background:white; width:100%; float:left; padding:15px 0px; margin:15px 0px;"">
                            <a href=""http://www.facebook.com/icaked""><img src=""https://www.icaked.com/Images/Site/fb_icon.png"" style=""float:left; margin:3px; width:25px; margin-top:0px; margin-left:15px;"" /></a>
                            <a href=""http://www.twitter.com/icakedcom""><img src=""https://www.icaked.com/Images/Site/tw_icon.png"" style=""float:left; margin:3px; width:25px; margin-top:0px;"" /></a>
                            <a href=""http://www.instagram.com/icakedcom""><img src=""https://www.icaked.com/Images/Site/in_icon.png"" style=""float:left; margin:3px; width:25px; margin-top:0px;"" /></a>
                            <a href=""http://www.facebook.com/icakedcom""><img src=""https://www.icaked.com/Images/Site/pi_icon.png"" style=""float:left; margin:3px; width:25px; margin-top:0px;"" /></a>
                            <a href=""https://www.icaked.com"" style=""text-decoration:none; float:right; margin-right:20px;"">
                                <label style=""font-size:13px; float:right; color:black; line-height:30px;"">2017 © www.iCaked.com</label>
                            </a>
                        </div>
                    </div>
                </div>";

            ac.sendMail("info@icaked.com", "iCaked.com - İstek Pasta", serverBody, null, null);
            ac.sendMail(mail, "iCaked.com - İsteğiniz Alınmıştır", clientBody, null, null);
            return Json(1, JsonRequestBehavior.AllowGet);
        }
    }
}