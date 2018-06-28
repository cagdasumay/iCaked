using nQuant;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Cake.Controllers
{
    public class AccountController : Controller
    {
        private DatabaseController dc = new DatabaseController();
        private OrderController oc = new OrderController();
        private UtilityController uc = new UtilityController();

        #region Constructor Methods

        public ActionResult DeleteAccount()
        {
            DataRow userinfo = (DataRow)Session["userinfo"];
            dc.DBQuerySetter("delete from Users where UserID = '" + new Guid(userinfo["UserID"].ToString()) + "'");
            return RedirectToAction("Index","Home");
        }

        public void DailyMailByBakery()
        {
            try
            {
                DataTable orders = dc.DBQueryGetter("select * from OrderLogs");
                DataTable dailyOrders = orders.Clone();
                DataTable bakeries = dc.DBQueryGetter("select * from Bakeries");

                for (int i = 0; i < orders.Rows.Count; i++)
                {
                    DataRow order = orders.Rows[i];
                    DateTime dt = Convert.ToDateTime(order["DeliveryDateTime"].ToString().Split(' ')[0]);
                    if (dt == DateTime.Today) { dailyOrders.Rows.Add(order.ItemArray); }
                }

                List<string> bakeryIds = new List<string>();
                List<List<DataRow>> ordersByBakery = new List<List<DataRow>>();

                for (int i = 0; i < bakeries.Rows.Count; i++)
                {
                    bakeryIds.Add(bakeries.Rows[i]["BakeryID"].ToString().ToLower());
                    ordersByBakery.Add(new List<DataRow>());
                }

                for (int i = 0; i < dailyOrders.Rows.Count; i++)
                {
                    DataRow order = dailyOrders.Rows[i];
                    int bakeryIndex = bakeryIds.IndexOf(order["Bakeries"].ToString().Split(' ')[0].ToString().ToLower());
                    ordersByBakery[bakeryIndex].Add(order);
                }

                for (int i = 0; i < ordersByBakery.Count; i++)
                {
                    if (ordersByBakery[i].Count != 0)
                    {
                        String orderBody = "";

                        for(int i2 = 0; i2 < ordersByBakery[i].Count; i2++)
                        {
                            String orderLink = "https://www.icaked.com/BakeryOrderSub?orderID=" + ordersByBakery[i][i2]["OrderID"].ToString();

                            String odemeSekli = ordersByBakery[i][i2]["PaymentType"].ToString();
                            if(odemeSekli == "POS") { odemeSekli = "İnternet"; }

                            String odemeDurumu = "ÖDENDİ";
                            if (ordersByBakery[i][i2]["PaymentApproved"].ToString() == "no") { odemeDurumu = "ÖDENMEDİ"; }

                            bool addressExists = false; DataRow address = null; String addressStr = ""; String adresWrapper1 = ""; String adresWrapper2 = "";
                            if (String.IsNullOrEmpty(ordersByBakery[i][i2]["AddressID"].ToString()) == false) {
                                DataTable temp = dc.DBQueryGetter("select * from AnonAddresses where ID = '" + ordersByBakery[i][i2]["AddressID"].ToString() + "'");
                                if(temp.Rows.Count == 0) { temp = dc.DBQueryGetter("select * from UserAddresses where ID = '" + ordersByBakery[i][i2]["AddressID"].ToString() + "'"); }
                                if(temp.Rows.Count != 0) { addressExists = true; address = temp.Rows[0]; }
                            }
                            if(addressExists == true) {
                                addressStr = address["ProvinceName"].ToString() + "," + address["DistrictName"].ToString() + "," + address["TownName"].ToString() + "," + address["DistrictName"].ToString();
                                adresWrapper1 = @"                                    
                                    <label style=""width:100%; float:left; font-size:14px; padding-bottom:10px; color:#92460f;"">Adres:</label>
                                    <label style=""width:100%; float:left; font-size:14px; color:#92460f; height:26px;""></label>
                                    <label style=""width:100%; float:left; font-size:14px; color:#92460f; height:26px;""></label>";
                                adresWrapper2 = @"
                                    <label style=""width:100%; float:left; font-size:14px; color:black; padding-bottom:10px;"">" + addressStr + @" TL</label>
                                    <label style=""width:100%; float:left; font-size:14px; color:black; padding-bottom:10px;"">" + address["SpecificAddress"].ToString() + @"</label>
                                    <label style=""width:100%; float:left; font-size:14px; color:black; padding-bottom:10px;"">" + address["Description"].ToString() + @"</label>";
                            }

                            orderBody = orderBody + @"
                                <div style=""width:100%; float:left; padding:20px 0px; border-bottom:1px solid #e6e6e6; margin-bottom:20px;"">
                                <div style=""float:left; width:100%; padding-bottom:20px;"">
                                    <a href=" + orderLink + @" style=""width:100%; float:left; font-size:14px; text-align:center; font-weight:600;"">Siparişin bilgilerinin tamamına buradan ulaşabilirsiniz</label>
                                </div>
                                    <div style=""width:120px; float:left;"">
                                        <label style=""width:100%; float:left; font-size:14px; padding-bottom:10px; color:#92460f;"">Sipariş No:</label>
                                        <label style=""width:100%; float:left; font-size:14px; padding-bottom:10px; color:#92460f;"">Teslimat Tarihi:</label>
                                        <label style=""width:100%; float:left; font-size:14px; padding-bottom:10px; color:#92460f;"">İletişim:</label>
                                        <label style=""width:100%; float:left; font-size:14px; color:#92460f; height:26px;""></label>
                                        <label style=""width:100%; float:left; font-size:14px; color:#92460f; height:26px;""></label>
                                        <label style=""width:100%; float:left; font-size:14px; padding-bottom:10px; color:#92460f;"">Ödeme Şekli:</label>
                                        <label style=""width:100%; float:left; font-size:14px; padding-bottom:10px; color:#92460f;"">Ödeme Durumu:</label>
                                        <label style=""width:100%; float:left; font-size:14px; padding-bottom:10px; color:#92460f;"">Tutar:</label>
                                        " + adresWrapper1 + @"
                                    </div>
                                    <div style=""float:left; width:400px;"">
                                        <label style=""width:100%; float:left; font-size:14px; color:black; padding-bottom:10px;"">" + ordersByBakery[i][i2]["OrderNumber"].ToString() + @"</label>
                                        <label style=""width:100%; float:left; font-size:14px; color:black; padding-bottom:10px;"">" + ordersByBakery[i][i2]["DeliveryDateTime"].ToString() + @"</label>
                                        <label style=""width:100%; float:left; font-size:14px; color:black; padding-bottom:10px;"">" + ordersByBakery[i][i2]["ContactName"] + " " + ordersByBakery[i][i2]["ContactSurname"] + @"</label>
                                        <label style=""width:100%; float:left; font-size:14px; color:black; padding-bottom:10px;"">" + ordersByBakery[i][i2]["ContactEmail"] + @"</label>
                                        <label style=""width:100%; float:left; font-size:14px; color:black; padding-bottom:10px;"">" + ordersByBakery[i][i2]["ContactPhone"] + @"</label>
                                        <label style=""width:100%; float:left; font-size:14px; color:black; padding-bottom:10px;"">" + odemeSekli + @"</label>
                                        <label style=""width:100%; float:left; font-size:14px; color:black; padding-bottom:10px; font-weight:600;"">" + odemeDurumu + @"</label>
                                        <label style=""width:100%; float:left; font-size:14px; color:black; padding-bottom:10px; font-weight:600;"">" + ordersByBakery[i][i2]["TotalPrice"].ToString() + @" TL</label>
                                        " + adresWrapper2 + @"
                                        
                                    </div>
                                </div>
                            ";
                        }

                        String mailBody = @"<div style=""width:90%; margin-left:5%; font-family:Helvetica, sans-serif;"">
                    <div style=""margin:auto; min-width:330px; max-width:600px;"">
                        <div style=""padding:20px 0px; width:100%; float:left; background:white; text-align:center;"">
                            <img src=""https://www.icaked.com/Images/Site/main_logo.png"" style=""width:150px;"" />
                        </div>
                        <div style=""width:100%; float:left; border-top:1px solid #e6e6e6; border-bottom:1px solid #e6e6e6;"">
                            <div style=""width:100%; padding:20px 0px; float:left; text-align:justify; text-align:center; margin:20px 0px; border-radius:5px; position:relative;"">
                                <div style=""width:100%; float:left; text-align:center;"">
                                    <img src=""https://www.icaked.com/Images/Site/thanks_icon.png"" style=""width:50px; margin:auto;"" />
                                </div>
                                <div style=""width:90%; margin-left:5%; float:left;"">
                                    <label style=""font-size:14px; color:#92460f; float:left; width:100%; text-align:center; padding-top:15px;"">Bugün için iCaked.com üzerinden alınmış siparişleriniz aşağıda sergilenmektedir.</label>
                                </div>
                            </div>
                        </div>
                        " + orderBody + @"
                        <div style=""background:white; width:100%; float:left; margin-bottom:50px;"">
                            <a href=""http://www.facebook.com/icaked""><img src=""https://www.icaked.com/Images/Site/fb_icon.png"" style=""float:left; margin:3px; width:25px; margin-top:0px;"" /></a>
                            <a href=""http://www.twitter.com/icakedcom""><img src=""https://www.icaked.com/Images/Site/tw_icon.png"" style=""float:left; margin:3px; width:25px; margin-top:0px;"" /></a>
                            <a href=""http://www.instagram.com/icakedcom""><img src=""https://www.icaked.com/Images/Site/in_icon.png"" style=""float:left; margin:3px; width:25px; margin-top:0px;"" /></a>
                            <a href=""http://www.pinterest.com/icakedcom""><img src=""https://www.icaked.com/Images/Site/pi_icon.png"" style=""float:left; margin:3px; width:25px; margin-top:0px;"" /></a>
                            <a href=""https://www.icaked.com"" style=""text-decoration:none; float:right; "">
                                <label style=""font-size:14px; float:right; color:black; line-height:30px;"">2016 © www.iCaked.com</label>
                            </a>
                        </div>
                    </div>
                </div>
                ";
                        //sendMail(bakeries.Rows[i]["Email"].ToString(), "iCaked - Günlük Siparişleriniz", mailBody, null, null);
                        sendMail("info@icaked.com", "iCaked - Günlük Siparişleriniz", mailBody, null, null);
                    }
                }
            }
            catch (Exception e) { }
        }

        public JsonResult DailyMail()
        {
            if (Session["userinfo"] != null)
            {
                if (((DataRow)Session["userinfo"])["Role"].ToString().ToLower().Trim() == "admin")
                {
                    DailyMailByBakery();
                }
            }

            return Json(true, JsonRequestBehavior.AllowGet);
        }

        //beyler caddesi dostkent sitesi no45 çayyolu çankaya ankara
        //0532 154 40 11
        //iyi ki doğdun kübra
        //15.04.2017 18:00-19:00

        public ActionResult BakeryLogin()
        {
            if (Session["bakeryinfo"] == null) { if (TempData["BakeryLogin"] != null) { ViewData["loginFailed"] = true; } return View(); }
            else { return RedirectToAction("Profile"); }
        }

        public ActionResult UserProfile(string profileName)
        {
            String whoseProfileIsThis = "user"; String whatIsThePermission = "";

            if (String.IsNullOrEmpty(profileName))
            {
                if (Session["userinfo"] == null)
                {
                    ViewData["profileExists"] = false;
                }
                else
                {
                    DataRow user = (DataRow)Session["userinfo"];
                    string profileId = user["UserID"].ToString();

                    string role = user["Role"].ToString().Trim().ToLower();

                    if (user["UserID"].ToString().ToLower() == profileId.ToLower())
                    {
                        whatIsThePermission = "own";
                        if (role == "admin") { whoseProfileIsThis = "admin"; }
                    }
                    else { whatIsThePermission = "user"; }

                    ArrangeFaves(profileId);

                    if (whatIsThePermission == "own" & whoseProfileIsThis == "admin")
                    {
                        DataTable products = dc.DBQueryGetter("select * from Products");
                        DataTable bakeries = dc.MemoryCacheByQuery("select * from Bakeries");
                        ViewData["products"] = products; ViewData["bakeries"] = bakeries; ViewData["categories"] = oc.arrangeCategories(products);
                    }

                    ViewData["userinfo"] = user;
                    ViewData["whoseProfileIsThis"] = whoseProfileIsThis;
                    ViewData["whatIsThePermission"] = whatIsThePermission;
                }
            }
            else
            {
                string temp = profileName.Split('-')[profileName.Split('-').Length - 1];
                DataTable userinfo = new DataTable();
                if (temp.Length == 8) { userinfo = dc.DBQueryGetter("select * from Users where UserID like '" + temp + "%'"); }

                if (userinfo.Rows.Count != 0)
                {
                    string profileId = userinfo.Rows[0]["UserID"].ToString();

                    if (Session["userinfo"] != null)
                    {
                        DataRow user = (DataRow)Session["userinfo"];
                        string role = user["Role"].ToString().Trim().ToLower();

                        if (user["UserID"].ToString().ToLower() == profileId.ToLower())
                        {
                            whatIsThePermission = "own";
                            if (role == "admin") { whoseProfileIsThis = "admin"; }
                        }
                        else { whatIsThePermission = "user"; }

                        if (role == "admin") { whatIsThePermission = "admin"; }
                    }
                    else { whatIsThePermission = "user"; }

                    ArrangeFaves(profileId);

                    if (whatIsThePermission == "own" & whoseProfileIsThis == "admin")
                    {
                        DataTable products = dc.DBQueryGetter("select * from Products");
                        DataTable bakeries = dc.MemoryCacheByQuery("select * from Bakeries");
                        ViewData["products"] = products; ViewData["bakeries"] = bakeries; ViewData["categories"] = oc.arrangeCategories(products);
                    }

                    ViewData["userinfo"] = userinfo.Rows[0];
                    ViewData["whoseProfileIsThis"] = whoseProfileIsThis;
                    ViewData["whatIsThePermission"] = whatIsThePermission;
                }
                else
                {
                    ViewData["profileExists"] = false;
                }
            }

            return View();
        }

        public void ArrangeFaves(String profileID)
        {
            DataTable faves = dc.DBQueryGetter("select * from Fav where UserID = '" + new Guid(profileID) + "'");
            List<DataRow> favedProducts = new List<DataRow>();
            List<DataRow> favedDesigns = new List<DataRow>();

            for (int i = 0; i < faves.Rows.Count; i++)
            {
                bool isDesign = false;
                DataTable dt = dc.DBQueryGetter("select * from Products where ProductID = '" + faves.Rows[i]["ProductID"] + "'");
                if (dt.Rows.Count == 0) { dt = dc.DBQueryGetter("select * from MadeCakes where MadeID = '" + faves.Rows[i]["ProductID"] + "'"); isDesign = true; }
                if (dt.Rows.Count != 0) { if (isDesign) { favedDesigns.Add(dt.Rows[0]); } else { favedProducts.Add(dt.Rows[0]); } }
            }

            ViewData["favedProducts"] = favedProducts;
            ViewData["favedDesigns"] = favedDesigns;
        }

        public ActionResult DeleteComment(Guid CommentID)
        {
            dc.DBQuerySetter("delete from ProductComments where ID = '" + CommentID + "'");
            return Redirect(Request.UrlReferrer.ToString());
        }

        public ActionResult EditComment(FormCollection form)
        {
            String approved = "no";
            if (form["admin-comment-approved"] != null) { if (form["admin-comment-approved"] == "on") { approved = "yes"; } }
            dc.DBQuerySetter("update ProductComments set NameSurname='" + form["commentName"] + "',Title='" + form["commentTitle"] + "',Comment='" + form["commentComment"] + "',Likes='" + form["commentLike"] + "',Dislikes='" + form["commentDislike"] + "',Approved = '" + approved + "' where ID ='" + form["commentID"] + "'");
            return Redirect(Request.UrlReferrer.ToString());
        }

        [ValidateInput(false)]
        public ActionResult EditBakeryInfo(FormCollection form)
        {
            String workHours = "";
            String cakePrice = form["bakery_cake_price"].ToString().Replace("TL", "").Trim();
            String cupcakePrice = form["bakery_cupcake_price"].ToString().Replace("TL", "").Trim();
            String cookiePrice = form["bakery_cookie_price"].ToString().Replace("TL", "").Trim();
            DataRow bakery = (DataRow)Session["bakeryinfo"]; bool isOpen = false; bool isBill = false;
            if (form["bakery_open"] != null) { if (form["bakery_open"] == "on") { isOpen = true; } }
            if (form["bakery_bill"] != null) { if (form["bakery_bill"] == "on") { isBill = true; } }
            workHours = form["bakeryTime1"] + " - " + form["bakeryTime2"];

            String designEarliest = form["bakery_earliest_design1"] + " " + form["bakery_earliest_design2"] + " " + form["bakery_earliest_design3"];

            dc.DBQuerySetter("update Bakeries set Description=N'" + form["bakery_description"] + "',Username = N'" + form["bakery_username"] + "',Password = N'" + form["bakery_password"] + "',isOpen = '" + isOpen + "',workDays = '" + form["bakery_weekdays"] + "',workHours = '" + workHours + "',earliestDelivery= '" + form["bakery_earliest"] + "',Phone = '" + form["bakery_phone"] + "',CakePrice = '" + cakePrice + "', CupcakePrice = '" + cupcakePrice + "', CookiePrice = '" + cookiePrice + "', Email = '" + form["bakery_email"] + "', SpecificAddress = N'" + form["bakery_address"] + "', MinPacket = '" + form["bakery_min_price"] + "', BillWanted = '" + isBill + "', EarliestDeliveryDesign = '" + designEarliest + "' where BakeryID = '" + bakery["BakeryID"] + "'");

            Session["bakeryinfo"] = dc.DBQueryGetter("select * from Bakeries where BakeryID = '" + bakery["BakeryID"] + "'").Rows[0];
            return Redirect(Request.UrlReferrer.ToString());
        }

        #endregion Constructor Methods

        #region Json Methods

        public JsonResult Login(String email_login, String password_login, String rememberMe)
        {
            bool isLoginSuccessful = false;
            DataTable user_table = dc.DBQueryGetter("select * from Users where (Email = '" + email_login + "') and Password = '" + password_login + "'");
            if (user_table.Rows.Count > 0) { Session["userinfo"] = user_table.Rows[0]; isLoginSuccessful = true; }

            if (rememberMe == "true")
            {
                HttpCookie email_cookie = new HttpCookie("email", email_login); email_cookie.Expires = DateTime.Now.AddDays(1);
                HttpCookie password_cookie = new HttpCookie("password", password_login); password_cookie.Expires = DateTime.Now.AddDays(1);
                HttpContext.Response.Cookies.Add(email_cookie); HttpContext.Response.Cookies.Add(password_cookie);
            }

            return Json(isLoginSuccessful, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CheckFBLogin()
        {
            bool result = false;
            if (Session["userinfo"] != null) { result = true; }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult FacebookLogin(String fbID, String nameSurname, String email)
        {
            string result = "";
            if (Session["userinfo"] == null)
            {
                DataTable users = dc.DBQueryGetter("select * from Users where SocialID = '" + fbID + "'");
                if (users.Rows.Count == 0)                       // no user - register
                {
                    string[] temp = nameSurname.Split(' ');
                    string surname = temp[temp.Length - 1];
                    string name = "";
                    if (temp.Length == 2) { name = temp[0]; }
                    else
                    {
                        for (int i = 0; i < temp.Length - 1; i++) { name = name + " " + temp[i]; }
                    }
                    Guid userid = Guid.NewGuid();
                    String newPass = Regex.Replace(Membership.GeneratePassword(6, 0), @"[^a-zA-Z0-9]", m => "9");
                    dc.DBQuerySetter("insert into Users (ID, UserID, SocialID, Email, Password, Name, Surname, Role, Thumbnail, Followers, Followings) values ('" + Guid.NewGuid() + "','" + userid + "','" + fbID + "','" + email + "',N'" + newPass + "',N'" + name + "',N'" + surname + "','User','" + userid + ".jpg', '0', '0')");
                    System.IO.File.Copy(Server.MapPath("/Images/Users/default_thumbnail.png"), Server.MapPath("/Images/Users/" + userid + ".jpg"));
                    Session["userinfo"] = dc.DBQueryGetter("select * from Users where SocialID = '" + fbID + "'").Rows[0];
                    sendMail(email, "iCaked.com - Hoşgeldiniz !", prepareRegisterMail(), null, null);
                }
                else { Session["userinfo"] = users.Rows[0]; }   // login user
            }
            else { result = "logon"; }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Register(String email_register, String name_register, String surname_register, String password_register)
        {
            Guid userid = Guid.NewGuid();
            bool result = dc.DBQuerySetter("insert into Users (ID, UserID, Email, Password, Name, Surname, Role, Thumbnail, Followers, Followings) values ('" + Guid.NewGuid() + "','" + userid + "','" + email_register + "',N'" + password_register + "',N'" + name_register + "',N'" + surname_register + "','User','" + userid + ".jpg', '0', '0')");
            if (result == true)
            {
                DataTable user_table = dc.DBQueryGetter("select * from Users where (Email = '" + email_register + "') and Password = '" + password_register + "'");
                if (user_table.Rows.Count > 0) { Session["userinfo"] = user_table.Rows[0]; }
                System.IO.File.Copy(Server.MapPath("/Images/Users/default_thumbnail.png"), Server.MapPath("/Images/Users/" + userid + ".jpg"));
                Session["userinfo"] = dc.DBQueryGetter("select * from Users where UserID = '" + userid + "'").Rows[0];
            }

            sendMail(email_register, "iCaked.com - Hoşgeldiniz !", prepareRegisterMail(), null, null);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public string prepareRegisterMail()
        {
            String productStr = ""; String madeStr = "";

            DataTable youDesigned = dc.DBQueryGetter("select * from MadeCakes where Name in (N'Yaramaz Minion',N'Müzik Pastası',N'Elsa Pastasi',N'Uçakli 1 Yas Pastasi')");
            DataTable cokSatanlar = dc.DBQueryGetter("select top 4 * from Products where Approved = 'yes' order by Rating asc");

            for (int i = 0; i < youDesigned.Rows.Count; i++)
            {
                String img = "https://www.icaked.com/Images/MadeCakes/" + youDesigned.Rows[i]["MadeID"] + "/" + youDesigned.Rows[i]["ImagePath"].ToString().Split(' ')[0];
                String madeLink = "https://www.icaked.com/Tasarim?designID=" + youDesigned.Rows[i]["MadeID"];

                madeStr = madeStr + @"<a href=" + madeLink + @" style=""text-decoration:none;"">
                        <div style=""float:left; width:22%; margin-right:3%; "">
                            <img src=" + img + @" style=""width:100%; border-top-left-radius:3px; border-top-right-radius:3px; float:left;"" />
                            <label style=""width:100%; float:left; text-align:center; padding:8px 0px; background:#f0f0f0; color:black !important; font-size:12px; border-bottom-left-radius:3px; border-bottom-right-radius:3px;"">" + youDesigned.Rows[i]["Name"] + @"</label>
                        </div>
                    </a>";
            }

            for (int i = 0; i < cokSatanlar.Rows.Count; i++)
            {
                String img = "https://www.icaked.com/Images/Products/" + cokSatanlar.Rows[i]["ProductID"] + "/" + cokSatanlar.Rows[i]["Thumbnail"].ToString().Split(' ')[0];
                String productLink = "https://www.icaked.com/Bakery/Product?ProductID=" + cokSatanlar.Rows[i]["ProductID"];

                productStr = productStr + @"<a href=" + productLink + @" style=""text-decoration:none;"">
                        <div style=""float:left; width:22%; margin-right:3%;"">
                            <img src=" + img + @" style=""width:100%; border-top-left-radius:3px; border-top-right-radius:3px; float:left;"" />
                            <label style=""width:100%; float:left; text-align:center; padding:8px 0px; background:#f0f0f0; color:black !important; font-size:12px; border-bottom-left-radius:3px; border-bottom-right-radius:3px;"">" + cokSatanlar.Rows[i]["Name"] + @"</label>
                        </div>
                    </a>";
            }

            String registerBody = @"
                <div style=""width:90%; margin-left:5%; font-family:Helvetica, sans-serif;"">
                    <div style=""margin:auto; min-width:330px; max-width:600px;"">
                        <div style=""padding:40px 0px; width:100%; float:left; background:white;"">
                            <img src=""https://www.icaked.com/Images/Site/main_logo.png"" style=""width:150px; float:left;"" />
                        </div>
                        <div style=""width:100%; float:left; border-top:1px dashed #d6d6d6; border-bottom:1px dashed #d6d6d6;"">
                            <div style=""width:100%; padding:20px 0px; float:left; text-align:justify; background:#f2f2f2; text-align:center; margin:20px 0px; border-radius:5px; position:relative;"">
                                <div style=""width:100%; float:left; text-align:center;"">
                                    <img src=""https://www.icaked.com/Images/Site/thanks_icon.png"" style=""width:50px; margin:auto;"" />
                                </div>
                                <div style=""width:90%; margin-left:5%; float:left;"">
                                    <label style=""font-size:22px; color:#92460f; float:left; width:100%; text-align:center; padding-bottom:30px; padding-top:15px;"">iCaked'e Hoşgeldiniz</label>
                                    <label style=""font-size:14px; color:#92460f; float:left; width:100%; text-align:center; padding:0px 20px; padding-bottom:30px;"">
                                        iCaked ile pastalarınız artık çok özel. <br /> İsterseniz Kendin Tasarla bölümünden istediğiniz pastayı zevklerinize göre tasarlayabilir, isterseniz binlerce tasarım pasta arasından en beğendiğinizi seçip zevkinize göre yapacağınız değişikliklerle sipariş verebilirsiniz.
                                        Bunlarla da sınırlı kalmayıp, Türkiye’nin en seçkin pastanelerinden dilediğiniz hazır ürünü sipariş verebilirsiniz. Hatta doğum günleriniz için özel olarak tasarlanan konsept parti setlerinden birine de anında ulaşmanız mümkün.
                                    </label>
                                </div>
                            </div>
                        </div>
                        <div style=""width:100%; float:left;"">
                            <label style=""float:left; width:100%; font-size:16px; color:#3f2518; text-align:left; padding-top:20px; padding-bottom:10px;"">Özel Tasarımlar</label>
                " + madeStr + @"
                        </div>
                        <div style=""width:100%; float:left;"">
                            <label style=""float:left; font-size:16px; width:100%; color:#3f2518; text-align:left; padding-top:20px; padding-bottom:10px;"">Hazır Ürünler</label>

                " + productStr + @"
                        </div>
                        <div style=""width:100%; float:left; text-align:center; padding:30px 0px; border-bottom:1px dashed #d6d6d6;"">
                            <a href=""https://www.icaked.com"" style=""text-decoration:none;"">
                                <label style=""border-radius:3px; padding:7px 15px; font-size:16px; background:#92460f; color:white; box-shadow: 0px 2px 5px -2px gray;"">Siz de Pastanızı Tasarlayın</label>
                            </a>
                        </div>
                        <div style=""background:white; width:100%; float:left; padding:15px 0px; background:#f0f0f0; margin:15px 0px;"">
                            <a href=""http://www.facebook.com/icaked""><img src=""https://www.icaked.com/Images/Site/fb_icon.png"" style=""float:left; margin:3px; width:25px; margin-top:0px; margin-left:15px;"" /></a>
                            <a href=""http://www.twitter.com/icakedcom""><img src=""https://www.icaked.com/Images/Site/tw_icon.png"" style=""float:left; margin:3px; width:25px; margin-top:0px;"" /></a>
                            <a href=""http://www.instagram.com/icakedcom""><img src=""https://www.icaked.com/Images/Site/in_icon.png"" style=""float:left; margin:3px; width:25px; margin-top:0px;"" /></a>
                            <a href=""http://www.pinterest.com/icakedcom""><img src=""https://www.icaked.com/Images/Site/pi_icon.png"" style=""float:left; margin:3px; width:25px; margin-top:0px;"" /></a>
                            <a href=""https://www.icaked.com"" style=""text-decoration:none; float:right; margin-right:20px;"">
                                <label style=""font-size:13px; float:right; color:black; line-height:30px;"">2016 © www.iCaked.com</label>
                            </a>
                        </div>
                    </div>
                </div>
                ";

            return registerBody;
        }

        public JsonResult SetDesignVisibility(String id, bool isVisible)
        {
            String visible = "no"; if (isVisible == true) { visible = "yes"; }
            dc.DBQuerySetter("update MadeCakes set Visible = '" + visible + "' where MadeID = '" + new Guid(id) + "'");
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CheckEmail(String email)
        {
            bool result = !(dc.DBQueryGetter("select * from Users where Email = '" + email + "'").Rows.Count > 0);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Forgot(String email)
        {
            String newPass = Regex.Replace(Membership.GeneratePassword(6, 0), @"[^a-zA-Z0-9]", m => "9");

            String forgotBody = @"
                <div style=""width:90%; margin-left:5%; font-family:Helvetica, sans-serif;"">
                    <div style=""margin:auto; min-width:330px; max-width:600px;"">
                        <div style=""padding:40px 0px; width:100%; float:left; background:white;"">
                            <img src=""https://www.icaked.com/Images/Site/main_logo.png"" style=""width:150px; float:left;"" />
                        </div>
                        <div style=""width:100%; float:left; border-top:1px dashed #d6d6d6; border-bottom:1px dashed #d6d6d6;"">
                            <div style=""width:100%; padding:20px 0px; float:left; text-align:justify; background:#f2f2f2; text-align:center; margin:20px 0px; border-radius:5px; position:relative;"">
                                <div style=""width:100%; float:left;"">
                                    <label style=""font-size:14px; color:#92460f; float:left; width:100%; text-align:center; padding:15px 0px;"">
                                        Bu e-posta iCaked.com üyeliğinizin şifresini güncellemek amacıyla gönderilmiştir.
                                    </label>
                                    <label style=""font-size:16px; color:#92460f; float:left; width:100%; text-align:center; padding:20px 0px;"">
                                        Yeni şifreniz <b>" + newPass + @"</b> olarak belirlenmiştir. <br /><br />
                                        Profilinizden şifrenizi değiştirebilirsiniz.
                                    </label>
                                    <a href=""https://www.icaked.com"" style=""width:100%; float:left; font-weight:bold; cursor:pointer; font-size:14px;"">Giriş yapmak için tıklayın</a>
                                </div>
                            </div>
                        </div>
                        <div style=""background:white; width:100%; float:left; padding:15px 0px; margin:15px 0px;"">
                            <a href=""http://www.facebook.com/icaked""><img src=""https://www.icaked.com/Images/Site/fb_icon.png"" style=""float:left; margin:3px; width:25px; margin-top:0px; margin-left:15px;"" /></a>
                            <a href=""http://www.twitter.com/icakedcom""><img src=""https://www.icaked.com/Images/Site/tw_icon.png"" style=""float:left; margin:3px; width:25px; margin-top:0px;"" /></a>
                            <a href=""http://www.instagram.com/icakedcom""><img src=""https://www.icaked.com/Images/Site/in_icon.png"" style=""float:left; margin:3px; width:25px; margin-top:0px;"" /></a>
                            <a href=""http://www.facebook.com/icakedcom""><img src=""https://www.icaked.com/Images/Site/pi_icon.png"" style=""float:left; margin:3px; width:25px; margin-top:0px;"" /></a>
                            <a href=""https://www.icaked.com"" style=""text-decoration:none; float:right; margin-right:20px;"">
                                <label style=""font-size:13px; float:right; color:black; line-height:30px;"">2016 © www.iCaked.com</label>
                            </a>
                        </div>
                    </div>
                </div>";

            bool result = dc.DBQuerySetter("update Users set Password='" + newPass + "' where Email = '" + email + "'");
            if (result == true) { sendMail(email, "iCaked.com - Şifre Güncelleme", forgotBody, null, null); }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ChangeDeclaration(String text, String declaration)
        {
            bool result = dc.DBQuerySetter("update Users set " + declaration + "='" + text + "' where Email = '" + ((DataRow)Session["userinfo"])["Email"] + "'");
            if (result == true) { ((DataRow)Session["userinfo"])[declaration] = text; }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ChangeEmail(String email, String password)
        {
            bool result = dc.DBQuerySetter("update Users set Email ='" + email + "' where Email = '" + ((DataRow)Session["userinfo"])["Email"] + "' and Password = '" + password + "'");
            if (result == true) { ((DataRow)Session["userinfo"])["Email"] = email; }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ChangePassword(String oldPass, String newPass)
        {
            String newPassBody = @"
                <div style=""width:90%; margin-left:5%; font-family:Helvetica, sans-serif;"">
                    <div style=""margin:auto; min-width:330px; max-width:600px;"">
                        <div style=""padding:40px 0px; width:100%; float:left; background:white;"">
                            <img src=""https://www.icaked.com/Images/Site/main_logo.png"" style=""width:150px; float:left;"" />
                        </div>
                        <div style=""width:100%; float:left; border-top:1px dashed #d6d6d6; border-bottom:1px dashed #d6d6d6;"">
                            <div style=""width:100%; padding:20px 0px; float:left; text-align:justify; background:#f2f2f2; text-align:center; margin:20px 0px; border-radius:5px; position:relative;"">
                                <div style=""width:100%; float:left;"">
                                    <label style=""font-size:14px; color:#92460f; float:left; width:100%; text-align:center; padding:15px 0px;"">
                                        Bu e-posta iCaked.com üyeliğinizin şifresini güncellemek amacıyla gönderilmiştir.
                                    </label>
                                    <label style=""font-size:16px; color:#92460f; float:left; width:100%; text-align:center; padding:20px 0px;"">
                                        Yeni şifreniz <b>" + newPass + @"</b> olarak değiştirilmiştir. <br />
                                    </label>
                                    <a href=""https://www.icaked.com"" style=""width:100%; float:left; font-weight:bold; cursor:pointer; font-size:14px;"">Giriş yapmak için tıklayın</a>
                                </div>
                            </div>
                        </div>
                        <div style=""background:white; width:100%; float:left; padding:15px 0px; margin:15px 0px;"">
                            <a href=""http://www.facebook.com/icaked""><img src=""https://www.icaked.com/Images/Site/fb_icon.png"" style=""float:left; margin:3px; width:25px; margin-top:0px; margin-left:15px;"" /></a>
                            <a href=""http://www.twitter.com/icakedcom""><img src=""https://www.icaked.com/Images/Site/tw_icon.png"" style=""float:left; margin:3px; width:25px; margin-top:0px;"" /></a>
                            <a href=""http://www.instagram.com/icakedcom""><img src=""https://www.icaked.com/Images/Site/in_icon.png"" style=""float:left; margin:3px; width:25px; margin-top:0px;"" /></a>
                            <a href=""http://www.facebook.com/icakedcom""><img src=""https://www.icaked.com/Images/Site/pi_icon.png"" style=""float:left; margin:3px; width:25px; margin-top:0px;"" /></a>
                            <a href=""https://www.icaked.com"" style=""text-decoration:none; float:right; margin-right:20px;"">
                                <label style=""font-size:13px; float:right; color:black; line-height:30px;"">2016 © www.iCaked.com</label>
                            </a>
                        </div>
                    </div>
                </div>";

            bool result = dc.DBQuerySetter("update Users set Password ='" + newPass + "' where Email = '" + ((DataRow)Session["userinfo"])["Email"] + "' and Password = '" + oldPass + "'");
            if (result == true)
            {
                ((DataRow)Session["userinfo"])["Password"] = newPass;
                sendMail(((DataRow)Session["userinfo"])["Email"].ToString(), "iCaked.com - Şifre Güncelleme", newPassBody, null, null);
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SaveChanges(String parent, String data)
        {
            String term = "";

            if (parent.Contains("surname")) { term = "Surname"; }
            else if (parent.Contains("name")) { term = "Name"; }
            else if (parent.Contains("birth")) { term = "Birth"; data = Convert.ToDateTime(data).Date.ToString("dd.MM.yyyy"); }
            else if (parent.Contains("gender")) { term = "Gender"; }
            bool result = dc.DBQuerySetter("update Users set " + term + "='" + data + "' where Email = '" + ((DataRow)Session["userinfo"])["Email"] + "' and Password = '" + ((DataRow)Session["userinfo"])["Password"] + "'");
            if (result == true) { ((DataRow)Session["userinfo"])[term] = data; }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SaveLocationChanges(String province, String district, String neighborhood)
        {
            bool result = dc.DBQuerySetter("update Users set Province = '" + province + "',District = '" + district + "',Neighborhood = '" + neighborhood + "' where Email = '" + ((DataRow)Session["userinfo"])["Email"] + "' and Password = '" + ((DataRow)Session["userinfo"])["Password"] + "'");

            if (result == true)
            {
                ((DataRow)Session["userinfo"])["province"] = province;
                ((DataRow)Session["userinfo"])["district"] = district;
                ((DataRow)Session["userinfo"])["neighborhood"] = neighborhood;
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        #endregion Json Methods

        #region Operational Methods

        [HttpPost]
        public ActionResult AdminMultipleUpload(FormCollection form, IEnumerable<HttpPostedFileBase> adminMultipleObjs)
        {
            List<string> objs = new List<string>(); List<string> imgs = new List<string>(); List<string> txts = new List<string>(); List<string> alps = new List<string>();

            string sourcePath = System.IO.Path.Combine(Server.MapPath("~/Objects/Sources/"));
            string imagePath = System.IO.Path.Combine(Server.MapPath("~/Objects/Images/"));
            string texturePath = System.IO.Path.Combine(Server.MapPath("~/Objects/Textures/"));
            string alphaPath = System.IO.Path.Combine(Server.MapPath("~/Objects/Textures/"));
            string path = "";

            for (int i = 0; i < ((HttpPostedFileBase[])adminMultipleObjs).Length; i++)
            {
                HttpPostedFileBase file = ((HttpPostedFileBase[])adminMultipleObjs)[i];

                if (file.FileName.Contains("obj"))
                {
                    string objPath = Path.Combine(sourcePath, file.FileName);
                    file.SaveAs(objPath); objs.Add(file.FileName);
                }
                else if (file.FileName.Contains("image"))
                {
                    string imgPath = Path.Combine(imagePath, file.FileName.Replace(".jpg", ".png"));
                    Bitmap b = (Bitmap)Bitmap.FromStream(file.InputStream);
                    using (MemoryStream ms = new MemoryStream())
                    {
                        b.Save(imgPath, ImageFormat.Png);
                    }
                    imgs.Add(file.FileName);
                }
                else if (file.FileName.Contains("texture"))
                {
                    string textPath = Path.Combine(texturePath, file.FileName.Replace(".jpg", ".png"));
                    Bitmap b = (Bitmap)Bitmap.FromStream(file.InputStream);
                    using (MemoryStream ms = new MemoryStream())
                    {
                        b.Save(textPath, ImageFormat.Png);
                    }
                    txts.Add(file.FileName);
                }
                else if (file.FileName.Contains("alpha"))
                {
                    string alpPath = Path.Combine(texturePath, file.FileName.Replace(".jpg", ".png"));
                    Bitmap b = (Bitmap)Bitmap.FromStream(file.InputStream);
                    using (MemoryStream ms = new MemoryStream())
                    {
                        b.Save(alpPath, ImageFormat.Png);
                    }
                    alps.Add(file.FileName);
                }
            }

            for (int i = 0; i < objs.Count; i++)
            {
                String properties = "<?xml version=\"1.0\" encoding=\"utf - 16\"?><ObjectProperties xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"><styles><scale>1</scale><color>FFFFFF</color></styles></ObjectProperties>";
                String imgPath = null; String txtPath = null; String alpPath = null;
                bool imgExists = imgs.Contains(objs[i].Replace(".obj", "") + "_image.png") | imgs.Contains(objs[i].Replace(".obj", "") + "_image.jpg");
                bool txtExists = txts.Contains(objs[i].Replace(".obj", "") + "_texture.png") | txts.Contains(objs[i].Replace(".obj", "") + "_texture.jpg");
                bool alpExists = alps.Contains(objs[i].Replace(".obj", "") + "_alpha.png") | alps.Contains(objs[i].Replace(".obj", "") + "_alpha.jpg");
                if (imgExists) { imgPath = objs[i].Replace(".obj", "") + "_image.png"; }
                if (txtExists) { txtPath = objs[i].Replace(".obj", "") + "_texture.png"; }
                if (alpExists) { alpPath = objs[i].Replace(".obj", "") + "_alpha.png"; }

                DataTable existsTable = dc.DBQueryGetter("select * from Objects where ObjectName = '" + objs[i].Replace(".obj", "") + "'");

                if (existsTable.Rows.Count > 0) // update
                {
                    dc.DBQuerySetter("update Objects set Type = '" + form["objectType"] + "', ImagePath = '" + imgPath + "', TexturePath = '" + texturePath + "', AlphaPath = '" + alphaPath + "' where ObjectName = '" + objs[i].Replace(".obj", "") + "'");
                }
                else                           // insert
                {
                    dc.DBQuerySetter("insert into Objects (ID,ObjectID,ObjectName,Type,ObjectPath,ImagePath,TexturePath,AlphaPath,Properties,Approved,Price,Currency) values ('" + Guid.NewGuid() + "','" + Guid.NewGuid() + "','" + objs[i].Replace(".obj", "") + "','" + form["objectType"] + "','" + objs[i] + "','" + imgPath + "','" + txtPath + "','" + alphaPath + "','" + properties + "','yes','4.99','TL')");
                }
            }

            return Redirect(Request.UrlReferrer.ToString());
        }

        public ActionResult BakeryLoginTrial(String bakery_username, String bakery_password)
        {
            DataTable bakery = dc.DBQueryGetter("select * from Bakeries where Username = '" + bakery_username + "' and Password = '" + bakery_password + "'");
            if (bakery.Rows.Count > 0)
            {
                Session["bakeryinfo"] = bakery.Rows[0];
                string profileName = bakery.Rows[0]["Name"].ToString();
                return RedirectToAction("Product", "Bakery", new { bakeryName = uc.cleanseUrlString(profileName) });
            }
            else
            {
                TempData["BakeryLogin"] = "failed";
                return RedirectToAction("BakeryLogin", "Account");
            }
        }

        public ActionResult Logout()
        {
            Session["userinfo"] = null; Session["bakeryinfo"] = null; Session["cart"] = null;
            HttpCookie email_cookie = new HttpCookie("email"); email_cookie.Expires = DateTime.Now.AddDays(-1);
            HttpCookie password_cookie = new HttpCookie("password"); password_cookie.Expires = DateTime.Now.AddDays(-1);
            Response.Cookies.Add(email_cookie); Response.Cookies.Add(password_cookie);
            return RedirectToAction("Index", "Home");
        }

        public ActionResult ChangeUserImage(HttpPostedFileBase upload_userImg)
        {
            string path = System.IO.Path.Combine(Server.MapPath("~/Images/Users/" + ((DataRow)Session["userinfo"])["Thumbnail"] + ""));
            Image sourceimage = Image.FromStream(upload_userImg.InputStream);
            var i = ScaleImage(sourceimage, 300, 300);
            if (System.IO.File.Exists(path)) { System.IO.File.Delete(path); }

            Bitmap bmp = new Bitmap(i);
            var quantizer = new WuQuantizer();
            using (var bitmap = bmp)
            {
                using (var quantized = quantizer.QuantizeImage(bitmap))
                {
                    quantized.Save(path, ImageFormat.Png); quantized.Save(path, ImageFormat.Jpeg);
                }
            }

            return Redirect(Request.UrlReferrer.ToString());
        }

        public static Image ScaleImage(Image image, int newWidth, int newHeight)
        {
            var newImage = new Bitmap(newWidth, newHeight);

            using (var graphics = Graphics.FromImage(newImage))
                graphics.DrawImage(image, 0, 0, newWidth, newHeight);

            return newImage;
        }

        public void sendMail(String to, String subject, String body, List<string> cc, List<string> bcc)
        {
            try
            {
                SmtpClient client = new SmtpClient("smtp.gmail.com", 25);
                client.EnableSsl = true;

                client.DeliveryMethod = SmtpDeliveryMethod.Network;

                MailMessage msg = new MailMessage("info@icaked.com", to, subject, body);
                msg.IsBodyHtml = true;
                if (cc != null) { for (int i = 0; i < cc.Count; i++) { msg.CC.Add(cc[i]); } }
                if (bcc != null) { for (int i = 0; i < bcc.Count; i++) { msg.Bcc.Add(bcc[i]); } }

                client.UseDefaultCredentials = true;
                client.Credentials = new NetworkCredential("info@icaked.com", "huzzud15Feposea");

                client.Send(msg);
            }
            catch (Exception e) { }
        }

        #endregion Operational Methods

        public ActionResult DeleteAddress(String AddressID)
        {
            dc.DBQuerySetter("delete from UserAddresses where Id = '" + AddressID + "'");
            return Redirect(Request.UrlReferrer.ToString());
        }

        [HttpPost]
        public ActionResult AddAddress(FormCollection form)
        {
            Guid UserID = new Guid(((DataRow)Session["userinfo"])["UserID"].ToString());
            String provinceName = ""; String districtName = ""; String neighborhoodName = ""; String townName = "";

            provinceName = dc.DBQueryGetter("select SehirAdi from Sehirler where SehirId = '" + Convert.ToInt32(form["provinceAddress"].ToString()) + "'").Rows[0][0].ToString();
            districtName = dc.DBQueryGetter("select IlceAdi from Ilceler where ilceId = '" + Convert.ToInt32(form["districtAddress"].ToString()) + "'").Rows[0][0].ToString();
            neighborhoodName = dc.DBQueryGetter("select MahalleAdi from SemtMah where SemtMahId = '" + Convert.ToInt32(form["neighborhoodAddress"].ToString()) + "'").Rows[0][0].ToString();
            townName = dc.DBQueryGetter("select SemtAdi from SemtMah where SemtMahId = '" + Convert.ToInt32(form["neighborhoodAddress"].ToString()) + "'").Rows[0][0].ToString();

            dc.DBQuerySetter("insert into UserAddresses (ID,UserID,Province,ProvinceName,District,DistrictName,Neighborhood,NeighborhoodName,TownName,SpecificAddress,Description) values ('" + Guid.NewGuid() + "','" + UserID + "','" + form["provinceAddress"] + "','" + provinceName + "','" + form["districtAddress"] + "','" + districtName + "','" + form["neighborhoodAddress"] + "','" + neighborhoodName + "','" + townName + "','" + form["openAddress"] + "','" + form["addressDescription"] + "')");
            return Redirect(Request.UrlReferrer.ToString());
        }

        [HttpPost]
        public ActionResult EditAddress(FormCollection form)
        {
            Guid UserID = new Guid(((DataRow)Session["userinfo"])["UserID"].ToString());
            String provinceName = ""; String districtName = ""; String neighborhoodName = ""; String townName = "";

            provinceName = dc.DBQueryGetter("select SehirAdi from Sehirler where SehirId = '" + Convert.ToInt32(form["editProvinceAddress"].ToString()) + "'").Rows[0][0].ToString();
            districtName = dc.DBQueryGetter("select IlceAdi from Ilceler where ilceId = '" + Convert.ToInt32(form["editDistrictAddress"].ToString()) + "'").Rows[0][0].ToString();
            neighborhoodName = dc.DBQueryGetter("select MahalleAdi from SemtMah where SemtMahId = '" + Convert.ToInt32(form["editNeighborhoodAddress"].ToString()) + "'").Rows[0][0].ToString();
            townName = dc.DBQueryGetter("select SemtAdi from SemtMah where SemtMahId = '" + Convert.ToInt32(form["editNeighborhoodAddress"].ToString()) + "'").Rows[0][0].ToString();

            dc.DBQuerySetter("update UserAddresses set Province = '" + form["editProvinceAddress"] + "',ProvinceName = '" + provinceName + "',District = '" + form["editDistrictAddress"] + "',DistrictName = '" + districtName + "',Neighborhood = '" + form["editNeighborhoodAddress"] + "',NeighborhoodName = '" + neighborhoodName + "',TownName = '" + townName + "',SpecificAddress = '" + form["editOpenAddress"] + "',Description ='" + form["editAddressDescription"] + "' where ID = '" + new Guid(form["AddressId"].ToString()) + "'");
            return Redirect(Request.UrlReferrer.ToString());
        }

        public JsonResult FollowUser(String userId)
        {
            Guid followerID = new Guid(((DataRow)Session["userinfo"])["UserID"].ToString());
            dc.DBQuerySetter("insert into Follows (ID,UserID,FollowerID,DateTime) values ('" + Guid.NewGuid() + "','" + new Guid(userId) + "','" + followerID + "','" + DateTime.Now.ToString() + "')");
            dc.DBQuerySetter("update Users set Followers = Followers + 1 where UserID = '" + new Guid(userId) + "'");

            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public JsonResult StopFollowingUser(String userId)
        {
            Guid followerID = new Guid(((DataRow)Session["userinfo"])["UserID"].ToString());
            dc.DBQuerySetter("delete from Follows where UserID = '" + new Guid(userId) + "' and FollowerID = '" + followerID + "'");
            dc.DBQuerySetter("update Users set Followers = Followers - 1 where UserID = '" + new Guid(userId) + "'");

            return Json(true, JsonRequestBehavior.AllowGet);
        }
    }
}