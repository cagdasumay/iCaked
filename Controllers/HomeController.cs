using Cake.Models;
using nQuant;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Cake.Controllers
{
    public class HomeController : Controller
    {
        private ObjectController oc = new ObjectController();
        private AccountController ac = new AccountController();
        private DatabaseController dc = new DatabaseController();
        private UtilityController uc = new UtilityController();

        #region Constructor Methods

        public ActionResult En()
        {
            Session["en"] = true;
            if (Session["city"] == null) { return View("Intro"); }
            else { return View("Index"); }
        }

        public ActionResult PastaniTasarla()
        {
            return View();
        }

        public ActionResult Intro()
        {
            if (Session["city"] == null) { return View(); }
            else { return View("Index"); }
        }

        public ActionResult PartiSepeti()
        {
            return View();
        }

        public ActionResult BasindaBiz()
        {
            return View();
        }

        public ActionResult DagitimAgi()
        {
            return View();
        }

        public ActionResult Kategoriler(string category)
        {
            if (String.IsNullOrEmpty(category)) { category = "Yaş Pasta"; }
            else { category = uc.categoryCorrector(category); }
            ViewData["category"] = category;

            String title = category + " Siparişi - En Lezzetli " + category;
            String keywords = category.ToLower() + "," + category.ToLower() + " siparişi, ankara istanbul " + category.ToLower() + " siparişi," + category.ToLower() + " kategorileri," + category.ToLower() + " ürünleri, tasarım pasta, pasta siparişi";
            String description = category.ToLower().Replace("ları", "sı") + " siparişi vermek artık çok kolay. Sayısız " + category.ToLower().Replace("ları","sı") + " seçeneği arasından siparişinizi verin kapınıza gelsin.";
            
            ViewData["title"] = title; ViewData["keywords"] = keywords; ViewData["description"] = description;

            return View();
        }

        public ActionResult TasarimIstek()
        {
            return View();
        }

        public ActionResult FigurIstek()
        {
            return View();
        }

        public ActionResult HataSayfasi()
        {
            return View();
        }

        public ActionResult SiparisTakibi()
        {
            return View();
        }

        public ActionResult Trials()
        {
            return View();
        }

        public ActionResult NotFound()
        {
            return View();
        }

        public ActionResult Hakkimizda()
        {
            return View();
        }

        public ActionResult Iletisim()
        {
            return View();
        }

        public ActionResult SSS()
        {
            return View();
        }

        public ActionResult Oturum()
        {
            return View();
        }

        public ActionResult Gorusler()
        {
            return View();
        }

        public ActionResult OdemeSecenekleri()
        {
            return View();
        }

        public ActionResult Satis()
        {
            return View();
        }

        public ActionResult Gizlilik()
        {
            return View();
        }

        public ActionResult SevgililerGunu()
        {
            return View();
        }

        public ActionResult Blog()
        {
            return RedirectPermanent("http://blog.icaked.com");
        }

        public ActionResult ResimliPasta()
        {
            if (Session != null)
            {
                if (Session["AllObjects"] != null) { Session["AllObjects"] = null; }
                if (Session["Objects"] != null) { Session["Objects"] = null; }
                if (Session["DesignID"] != null) { Session["DesignID"] = null; }
                if (Session["ObjectSizes"] != null) { Session["ObjectSizes"] = null; }
                if (Session["editorMode"] != null) { Session["editorMode"] = null; }
                if (Session["editorType"] != null) { Session["editorType"] = null; }
                if (Session["editorSize"] != null) { Session["editorSize"] = null; }
                if (Session["editorSaveType"] != null) { Session["editorSaveType"] = null; }
                if (Session["designNotes"] != null) { Session["designNotes"] = null; }
                if (Session["designImages"] != null) { Session["designImages"] = null; }
                if (Session["editorOutput"] != null) { Session["editorOutput"] = null; }
                if (Session["editorAutoSaveString"] != null) { Session["editorAutoSaveString"] = null; }
                if (Session["editorImg"] != null) { Session["editorImg"] = null; }
                if (Session["editorText"] != null) { Session["editorText"] = null; }
            }

            String[] objectTexture = Directory.GetFiles(HttpRuntime.AppDomainAppPath + "Images\\Decals\\");

            for (int i = 0; i < objectTexture.Length; i++)
            {
                if (objectTexture[i].Contains(".jpg") | objectTexture[i].Contains(".png")) { }
                else
                {
                    var list = new List<string>(objectTexture);
                    list.Remove(objectTexture[i]);
                    objectTexture = list.ToArray();
                }
            }

            for (int i = 0; i < objectTexture.Length; i++) { objectTexture[i] = "/Images/Decals/" + objectTexture[i].Substring(objectTexture[i].LastIndexOf("\\") + 1, objectTexture[i].Length - objectTexture[i].LastIndexOf("\\") - 1); }
            ViewData["ObjectTextures"] = objectTexture;

            if (Session["city"] != null)
            {
                Session["editorLocation"] = (string)Session["city"];
            }

            return View();
        }

        #endregion Constructor Methods

        #region BigConstructor Methods

        public ActionResult Index(String id)
        {
            if(String.IsNullOrEmpty(id) == false) { Session["city"] = id; }
            if(Session["city"] == null) { return View("Intro"); }
            else { return View(); }
        }

        public ActionResult RP()
        {
            return RedirectPermanent("/sayfa-bulunamadi");
        }

        public ActionResult Tasarim(String category, String designName)
        {
            if (string.IsNullOrEmpty(designName) == false)
            {
                string[] cats = new string[] { "dogum-gunu-pastalari", "bebek-pastalari", "1-yas-pastalari", "cizgi-film-pastalari", "sevgili-pastalari", "kutlama-pastalari", "yarisma-pastalari" };
                if (cats.Contains(designName.ToLower()))
                {
                    ViewData["category"] = designName;
                    return View("Tasarimlar");
                }
                else
                {
                    DataTable design_table = dc.DBQueryGetter("select MadeID from MadeCakes where MadeID like '%" + designName.Split('-')[designName.Split('-').Length - 1] + "%'");
                    if (design_table.Rows.Count == 0)
                    {
                        return View("TasarimBulunamadi");
                    }
                    else
                    {
                        string designID = design_table.Rows[0][0].ToString();
                        DataRow design = dc.DBQueryGetter("select * from MadeCakes where MadeID = '" + new Guid(designID) + "'").Rows[0];
                        bool isFaved = false;
                        if (Session["userinfo"] != null) { isFaved = dc.DBQueryGetter("select * from Fav where ProductID = '" + new Guid(designID) + "' and UserID = '" + ((DataRow)Session["userinfo"])["UserID"] + "'").Rows.Count > 0; }

                        ViewData["design"] = design;
                        ViewData["isFaved"] = isFaved;

                        ViewBag.ProductID = design["MadeID"];
                        ViewBag.ProductName = design["Name"];
                        ViewBag.ProductDescription = design["Description"];
                        String[] photos = design["ImagePath"].ToString().Split(new[] { " " }, StringSplitOptions.None);
                        ViewBag.ProductImg = "https://icaked.com/Images/MadeCakes/" + design["MadeID"] + "/" + photos[0];

                        return View();
                    }
                }
            }
            else
            {
                return View("Tasarimlar");
            }
        }

        public ActionResult Editor2(string Id, string designID, string saved)
        {
            if (String.IsNullOrEmpty(Id) == false) { DataTable bakery = dc.DBQueryGetter("select * from Bakeries where ID = '" + Id + "'"); Session["bakery"] = bakery.Rows[0]; ViewData["bakery"] = bakery.Rows[0]; }
            else { Session["bakery"] = null; }
            if (Session != null)
            {
                if (Session["AllObjects"] != null) { Session["AllObjects"] = null; }
                if (Session["Objects"] != null) { Session["Objects"] = null; }
                if (Session["DesignID"] != null) { Session["DesignID"] = null; }
                if (Session["ObjectSizes"] != null) { Session["ObjectSizes"] = null; }
                if (Session["editorMode"] != null) { Session["editorMode"] = null; }
                if (Session["editorType"] != null) { Session["editorType"] = null; }
                if (Session["editorSize"] != null) { Session["editorSize"] = null; }
                if (Session["editorSaveType"] != null) { Session["editorSaveType"] = null; }
            }
            ViewData["ObjectNames"] = oc.GetObjectNames();

            String[] objectTexture = Directory.GetFiles(HttpRuntime.AppDomainAppPath + "Images\\Decals\\");

            for (int i = 0; i < objectTexture.Length; i++)
            {
                if (objectTexture[i].Contains(".jpg") | objectTexture[i].Contains(".png")) { }
                else
                {
                    var list = new List<string>(objectTexture);
                    list.Remove(objectTexture[i]);
                    objectTexture = list.ToArray();
                }
            }

            for (int i = 0; i < objectTexture.Length; i++) { objectTexture[i] = "/Images/Decals/" + objectTexture[i].Substring(objectTexture[i].LastIndexOf("\\") + 1, objectTexture[i].Length - objectTexture[i].LastIndexOf("\\") - 1); }
            ViewData["ObjectTextures"] = objectTexture;

            ViewData["DesignID"] = designID;

            if (String.IsNullOrEmpty(saved) == false) { ViewData["saved"] = true; }
            if (String.IsNullOrEmpty(designID) == false)
            {
                if (designID.ToString().ToLower() == "677B098A-E4E9-411C-A61A-58D29A6A8A0D".ToLower())
                {
                    ViewData["combinator"] = "yes";
                    try
                    {
                        dc.DBQuerySetter("insert into TrackEntries (ID,IP,DateTime) values ('" + Guid.NewGuid() + "','" + Request.UserHostAddress + "','" + DateTime.Now.ToString() + "')");
                    }
                    catch (Exception e) { }
                }
                else { ViewData["combinator"] = ""; }
            }
            else { ViewData["combinator"] = ""; }

            return View();
        }

        public ActionResult EditorENG(string Id, string designID, string saved)
        {
            if (String.IsNullOrEmpty(Id) == false) { DataTable bakery = dc.DBQueryGetter("select * from Bakeries where ID = '" + Id + "'"); Session["bakery"] = bakery.Rows[0]; ViewData["bakery"] = bakery.Rows[0]; }
            else { Session["bakery"] = null; }
            if (Session != null)
            {
                if (Session["AllObjects"] != null) { Session["AllObjects"] = null; }
                if (Session["Objects"] != null) { Session["Objects"] = null; }
                if (Session["DesignID"] != null) { Session["DesignID"] = null; }
                if (Session["ObjectSizes"] != null) { Session["ObjectSizes"] = null; }
                if (Session["editorMode"] != null) { Session["editorMode"] = null; }
                if (Session["editorType"] != null) { Session["editorType"] = null; }
                if (Session["editorSize"] != null) { Session["editorSize"] = null; }
                if (Session["editorSaveType"] != null) { Session["editorSaveType"] = null; }
                if (Session["designNotes"] != null) { Session["designNotes"] = null; }
                if (Session["designImages"] != null) { Session["designImages"] = null; }
                if (Session["editorOutput"] != null) { Session["editorOutput"] = null; }
                if (Session["editorAutoSaveString"] != null) { Session["editorAutoSaveString"] = null; }
                if (Session["editorImg"] != null) { Session["editorImg"] = null; }
                if (Session["editorText"] != null) { Session["editorText"] = null; }
            }
            ViewData["ObjectNames"] = oc.GetObjectNames();

            String[] objectTexture = Directory.GetFiles(HttpRuntime.AppDomainAppPath + "Images\\Decals\\");

            for (int i = 0; i < objectTexture.Length; i++)
            {
                if (objectTexture[i].Contains(".jpg") | objectTexture[i].Contains(".png")) { }
                else
                {
                    var list = new List<string>(objectTexture);
                    list.Remove(objectTexture[i]);
                    objectTexture = list.ToArray();
                }
            }

            for (int i = 0; i < objectTexture.Length; i++) { objectTexture[i] = "/Images/Decals/" + objectTexture[i].Substring(objectTexture[i].LastIndexOf("\\") + 1, objectTexture[i].Length - objectTexture[i].LastIndexOf("\\") - 1); }
            ViewData["ObjectTextures"] = objectTexture;

            ViewData["DesignID"] = designID;

            if (String.IsNullOrEmpty(saved) == false) { ViewData["saved"] = true; }

            if (Session["city"] != null)
            {
                Session["editorLocation"] = (string)Session["city"];
            }

            return View();
        }

        public ActionResult EditorFrame(string Id, string designID, string saved)
        {
            if (String.IsNullOrEmpty(Id) == false) { DataTable bakery = dc.DBQueryGetter("select * from Bakeries where ID = '" + Id + "'"); Session["bakery"] = bakery.Rows[0]; ViewData["bakery"] = bakery.Rows[0]; }
            else { Session["bakery"] = null; }
            if (Session != null)
            {
                if (Session["AllObjects"] != null) { Session["AllObjects"] = null; }
                if (Session["Objects"] != null) { Session["Objects"] = null; }
                if (Session["DesignID"] != null) { Session["DesignID"] = null; }
                if (Session["ObjectSizes"] != null) { Session["ObjectSizes"] = null; }
                if (Session["editorMode"] != null) { Session["editorMode"] = null; }
                if (Session["editorType"] != null) { Session["editorType"] = null; }
                if (Session["editorSize"] != null) { Session["editorSize"] = null; }
                if (Session["editorSaveType"] != null) { Session["editorSaveType"] = null; }
                if (Session["designNotes"] != null) { Session["designNotes"] = null; }
                if (Session["designImages"] != null) { Session["designImages"] = null; }
                if (Session["editorOutput"] != null) { Session["editorOutput"] = null; }
                if (Session["editorAutoSaveString"] != null) { Session["editorAutoSaveString"] = null; }
                if (Session["editorImg"] != null) { Session["editorImg"] = null; }
                if (Session["editorText"] != null) { Session["editorText"] = null; }
            }
            ViewData["ObjectNames"] = oc.GetObjectNames();

            String[] objectTexture = Directory.GetFiles(HttpRuntime.AppDomainAppPath + "Images\\Decals\\");

            for (int i = 0; i < objectTexture.Length; i++)
            {
                if (objectTexture[i].Contains(".jpg") | objectTexture[i].Contains(".png")) { }
                else
                {
                    var list = new List<string>(objectTexture);
                    list.Remove(objectTexture[i]);
                    objectTexture = list.ToArray();
                }
            }

            for (int i = 0; i < objectTexture.Length; i++) { objectTexture[i] = "/Images/Decals/" + objectTexture[i].Substring(objectTexture[i].LastIndexOf("\\") + 1, objectTexture[i].Length - objectTexture[i].LastIndexOf("\\") - 1); }
            ViewData["ObjectTextures"] = objectTexture;

            ViewData["DesignID"] = designID;

            if (String.IsNullOrEmpty(saved) == false) { ViewData["saved"] = true; }

            if (Session["city"] != null)
            {
                Session["editorLocation"] = (string)Session["city"];
            }

            return View();
        }

        public ActionResult Editor(string Id, string designID, string saved, string lang)
        {
            if(String.IsNullOrEmpty(lang) == false) { Session["en"] = true; Session["city"] = "Ankara"; }
            
            if (String.IsNullOrEmpty(Id) == false) { DataTable bakery = dc.DBQueryGetter("select * from Bakeries where ID = '" + Id + "'"); Session["bakery"] = bakery.Rows[0]; ViewData["bakery"] = bakery.Rows[0]; }
            else { Session["bakery"] = null; }
            if (Session != null)
            {
                if (Session["AllObjects"] != null) { Session["AllObjects"] = null; }
                if (Session["Objects"] != null) { Session["Objects"] = null; }
                if (Session["DesignID"] != null) { Session["DesignID"] = null; }
                if (Session["ObjectSizes"] != null) { Session["ObjectSizes"] = null; }
                if (Session["editorMode"] != null) { Session["editorMode"] = null; }
                if (Session["editorType"] != null) { Session["editorType"] = null; }
                if (Session["editorSize"] != null) { Session["editorSize"] = null; }
                if (Session["editorSaveType"] != null) { Session["editorSaveType"] = null; }
                if (Session["designNotes"] != null) { Session["designNotes"] = null; }
                if (Session["designImages"] != null) { Session["designImages"] = null; }
                if (Session["editorOutput"] != null) { Session["editorOutput"] = null; }
                if (Session["editorAutoSaveString"] != null) { Session["editorAutoSaveString"] = null; }
                if (Session["editorImg"] != null) { Session["editorImg"] = null; }
                if (Session["editorText"] != null) { Session["editorText"] = null; }
            }
            ViewData["ObjectNames"] = oc.GetObjectNames();

            String[] objectTexture = Directory.GetFiles(HttpRuntime.AppDomainAppPath + "Images\\Decals\\");

            for (int i = 0; i < objectTexture.Length; i++)
            {
                if (objectTexture[i].Contains(".jpg") | objectTexture[i].Contains(".png")) { }
                else
                {
                    var list = new List<string>(objectTexture);
                    list.Remove(objectTexture[i]);
                    objectTexture = list.ToArray();
                }
            }

            for (int i = 0; i < objectTexture.Length; i++) { objectTexture[i] = "/Images/Decals/" + objectTexture[i].Substring(objectTexture[i].LastIndexOf("\\") + 1, objectTexture[i].Length - objectTexture[i].LastIndexOf("\\") - 1); }
            ViewData["ObjectTextures"] = objectTexture;

            ViewData["DesignID"] = designID;

            if (String.IsNullOrEmpty(saved) == false) { ViewData["saved"] = true; }

            if(Session["city"] != null)
            {
                Session["editorLocation"] = (string)Session["city"];
            }

            return View();
        }

        #endregion BigConstructor Methods


        #region Operational Methods

        public void sendTutorialCookie()
        {
            HttpCookie tutorial_cookie = new HttpCookie("icakedTutorial", "yes"); tutorial_cookie.Expires = DateTime.Now.AddDays(1);
            Response.Cookies.Add(tutorial_cookie);
        }

        [HttpPost]
        public ActionResult FindOrder(FormCollection form)
        {
            DataRow order = dc.DBQueryGetter("select * from OrderLogs where OrderNumber = '" + form["OrderNumber"] + "'").Rows[0];

            return RedirectToAction("BakeryOrderSub", "Bakery", new { orderID = order["OrderID"].ToString() });
        }

        //public void cropImage()
        //{
        //    string[] objPaths = Directory.GetDirectories(HttpRuntime.AppDomainAppPath + "\\Images\\Products").Select(Path.GetFileName).ToArray();

        //    for (int i = 0; i < objPaths.Length; i++)
        //    {
        //        try
        //        {
        //            Bitmap source = new Bitmap(HttpRuntime.AppDomainAppPath + "\\Images\\Products\\" + objPaths[i] + "\\" + objPaths[i] + "_1.jpg");
        //            Rectangle crop = new Rectangle(0, 0, 300, 300);

        //            var bmp = new Bitmap(crop.Width, crop.Height);
        //            using (var gr = Graphics.FromImage(bmp))
        //            {
        //                gr.DrawImage(source, new Rectangle(0, 0, bmp.Width, bmp.Height), crop, GraphicsUnit.Pixel);
        //            }
        //            Directory.CreateDirectory(Server.MapPath("~/tempImages/" + objPaths[i]));
        //            string path = System.IO.Path.Combine(Server.MapPath("~/tempImages/" + objPaths[i] + "/" + objPaths[i] + ".jpg"));

        //            var quantizer = new WuQuantizer();
        //            using (var bitmap = bmp)
        //            {
        //                using (var quantized = quantizer.QuantizeImage(bitmap))
        //                {
        //                    quantized.Save(path, ImageFormat.Jpeg);
        //                }
        //            }

        //            bmp.Dispose();
        //        }
        //        catch (Exception e) { }
        //    }
        //}

        public ActionResult AddPhotosToDesign(IEnumerable<HttpPostedFileBase> designImages, string designId)
        {
            DataRow row = dc.DBQueryGetter("select * from MadeCakes where MadeID = '" + new Guid(designId) + "'").Rows[0];

            string[] photos = row["ImagePath"].ToString().Split(' ');
            int idx = photos.Length;
            List<string> list_photos = new List<string>();

            for (int i = 0; i < ((System.Web.HttpPostedFileBase[])designImages).Length; i++)
            {
                idx++;
                string path = System.IO.Path.Combine(Server.MapPath("~/Images/MadeCakes/" + designId + "/designImg_" + idx.ToString() + ".png"));
                Image sourceimage = Image.FromStream(((HttpPostedFileBase[])designImages)[i].InputStream);
                var newImage = ScaleImage(sourceimage, 300, 300);

                Bitmap bmp = new Bitmap(newImage);
                var quantizer = new WuQuantizer();
                using (var bitmap = bmp)
                {
                    using (var quantized = quantizer.QuantizeImage(bitmap))
                    {
                        quantized.Save(path, ImageFormat.Png); quantized.Save(path, ImageFormat.Png);
                    }
                }
                list_photos.Add("designImg_" + idx.ToString() + ".png");
            }

            dc.DBQuerySetter("update MadeCakes set RealImagePath = '" + String.Join(" ", list_photos.ToArray()) + "' where MadeID = '" + new Guid(designId) + "'");

            TempData["photosTaken"] = true;

            return Redirect(Request.UrlReferrer.ToString());
        }

        public ActionResult ApprovePhotos(string designId)
        {
            if (Session["userinfo"] != null)
            {
                if (((DataRow)Session["userinfo"])["Role"].ToString().Trim().ToLower() == "admin")
                {
                    DataRow row = dc.DBQueryGetter("select * from MadeCakes where MadeID = '" + new Guid(designId) + "'").Rows[0];
                    string newImgStr = row["ImagePath"].ToString() + " " + row["RealImagePath"].ToString();
                    dc.DBQuerySetter("update MadeCakes set ImagePath = '" + newImgStr + "', RealImagePath = '' where MadeID = '" + new Guid(designId) + "'");

                    String designType = uc.cleanseUrlString(row["Type"].ToString().Replace("\r\n", ""));
                    String designName = uc.cleanseUrlString(row["Name"].ToString()) + "-" + row["MadeID"].ToString().Split('-')[0];

                    return RedirectToAction("Tasarim", new { type = designType, designName = designName });
                }
                else
                {
                    return Redirect(Request.UrlReferrer.ToString());
                }
            }
            else
            {
                return Redirect(Request.UrlReferrer.ToString());
            }
        }

        public ActionResult DeletePhotos(string designId)
        {
            DataRow row = dc.DBQueryGetter("select * from MadeCakes where MadeID = '" + new Guid(designId) + "'").Rows[0];

            string[] photos = row["RealImagePath"].ToString().Split(' ');

            for (int i = 0; i < photos.Length; i++)
            {
                string path = System.IO.Path.Combine(Server.MapPath("~/Images/MadeCakes/" + designId + "/" + photos[i]));
                if (System.IO.File.Exists(path)) { System.IO.File.Delete(path); }
            }

            dc.DBQuerySetter("update MadeCakes set RealImagePath = '' where MadeID = '" + new Guid(designId) + "' ");

            return Redirect(Request.UrlReferrer.ToString());
        }

        public JsonResult SetIntroCity(string city)
        {
            Session["city"] = city;
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public JsonResult JoinMail(string mail)
        {
            bool result = true;
            DataTable mails = dc.DBQueryGetter("select * from ContactMails");
            for (int i = 0; i < mails.Rows.Count; i++)
            {
                if (mails.Rows[i]["Mail"].ToString() == mail)
                {
                    result = false; break;
                }
            }
            if (result == true) { dc.DBQuerySetter("insert into ContactMails (ID,Mail,DateTime) values ('" + Guid.NewGuid() + "','" + mail + "','" + DateTime.Now + "')"); }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult MainAutocomplete(string term)
        {
            DataTable bakeries = dc.MemoryCacheByQuery("select * from Bakeries");
            DataTable products = dc.MemoryCacheByQuery("select * from Products where Approved = 'yes'");
            DataTable madeCakes = dc.MemoryCacheByQuery("select * from MadeCakes where Visible = 'yes'");

            List<AutocompleteItem> filtered = new List<AutocompleteItem>();

            for (int i = 0; i < products.Rows.Count; i++)
            {
                string[] nameTemp = products.Rows[i]["Name"].ToString().Split(' ');
                for (int i2 = 0; i2 < nameTemp.Length; i2++)
                {
                    if (nameTemp[i2].ToLower().StartsWith(term.ToLower()))
                    {
                        AutocompleteItem item = new AutocompleteItem();
                        item.name = products.Rows[i]["Name"].ToString();
                        item.bakeryName = getBakeryNameFromID(products.Rows[i]["BakeryID"].ToString());
                        item.img = "Images/Products/" + products.Rows[i]["ProductID"].ToString() + "/" + products.Rows[i]["Thumbnail"].ToString().Split(' ')[0];
                        item.price = products.Rows[i]["Price"].ToString().Split(' ')[0] + " TL";
                        item.link = "pastane/" + item.bakeryName + "/" + uc.cleanseUrlString(products.Rows[i]["Category"].ToString()) + "/" + uc.cleanseUrlString(item.name + "-" + products.Rows[i]["ProductID"].ToString().Split('-')[0]);
                        item.desc = ""; 
                        if(products.Rows[i]["Description"].ToString().Length >= 100) { item.desc = products.Rows[i]["Description"].ToString().Substring(0, 100) + "..."; }
                        else { item.desc = products.Rows[i]["Description"].ToString(); }
                        filtered.Add(item); break;
                    }
                }
            }

            for (int i = 0; i < madeCakes.Rows.Count; i++)
            {
                string[] nameTemp = madeCakes.Rows[i]["Name"].ToString().Split(' ');
                for (int i2 = 0; i2 < nameTemp.Length; i2++)
                {
                    if (nameTemp[i2].ToLower().StartsWith(term.ToLower()))
                    {
                        AutocompleteItem item = new AutocompleteItem();
                        item.name = madeCakes.Rows[i]["Name"].ToString();
                        item.bakeryName = "Tasarım Pasta";
                        item.img = "Images/MadeCakes/" + madeCakes.Rows[i]["MadeID"].ToString() + "/" + madeCakes.Rows[i]["ImagePath"].ToString().Split(' ')[0];
                        item.price = "";
                        item.link = "tasarim-" + uc.cleanseUrlString(madeCakes.Rows[i]["Type"].ToString()) + "/" + uc.cleanseUrlString(madeCakes.Rows[i]["Name"].ToString()) + "-" + uc.cleanseUrlString(madeCakes.Rows[i]["MadeID"].ToString().Split('-')[0]);
                        item.desc = "";
                        if (madeCakes.Rows[i]["Description"].ToString().Length >= 100) { item.desc = madeCakes.Rows[i]["Description"].ToString().Substring(0, 100) + "..."; }
                        else { item.desc = madeCakes.Rows[i]["Description"].ToString(); }
                        filtered.Add(item); break;
                    }
                }
            }

            return Json(filtered, JsonRequestBehavior.AllowGet);
        }

        public string getBakeryNameFromID(string id)
        {
            DataTable bakeries = dc.MemoryCacheByQuery("select * from Bakeries");
            string name = "";
            for (int i = 0; i < bakeries.Rows.Count; i++)
            {
                if (bakeries.Rows[i]["BakeryID"].ToString().ToLower() == id.ToLower()) { name = bakeries.Rows[i]["Name"].ToString(); break; }
            }
            return name;
        }

        public void LogError(string url, string errorStr)
        {
            String userid = "";
            if (Session["userinfo"] != null) { userid = ((DataRow)Session["userinfo"])["UserID"].ToString(); }
            dc.DBQuerySetter("insert into ErrorLogs (ID,DateTime,ErrorID,UserID,URL,Exception) values ('" + Guid.NewGuid() + "','" + DateTime.Now.ToString() + "','" + Guid.NewGuid() + "','" + userid + "','" + url + "','" + errorStr + "')");

            try
            {
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
                                    <label style=""font-size:14px; color:black; float:left; width:100%; text-align:center; padding-bottom:35px;"">Kullanıcı : " + dc.getUsernameByID(new Guid(userid)) + @"</label>
                                    <label style=""font-size:14px; color:black; float:left; width:100%; text-align:center; padding-bottom:35px;"">URL : " + url + @"</label>
                                    <label style=""font-size:14px; color:black; float:left; width:100%; text-align:center; padding-bottom:35px;"">Exception : " + errorStr + @"</label>
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
            catch (Exception e) { }
        }

        public static Image ScaleImage(Image image, int newWidth, int newHeight)
        {
            var newImage = new Bitmap(newWidth, newHeight);

            using (var graphics = Graphics.FromImage(newImage))
                graphics.DrawImage(image, 0, 0, newWidth, newHeight);

            return newImage;
        }

        public ActionResult FeedbackForm(FormCollection form)
        {
            String nameSurname = form["feedbackFormName"];
            String email = form["feedbackFormEmail"];
            String content = form["feedbackFormContent"];
            dc.DBQuerySetter("insert into Feedbacks (ID,FeedbackID,NameSurname,Email,Message) values ('" + Guid.NewGuid() + "','" + Guid.NewGuid() + "','" + nameSurname + "','" + email + "','" + content + "')");

            String userMailBody = @"
                <div style=""width:90%; margin-left:5%; font-family:Helvetica, sans-serif;"">
                    <div style=""margin:auto; min-width:330px; max-width:600px;"">
                        <div style=""padding:40px 0px; width:100%; float:left; background:white;"">
                            <img src=""http://www.icaked.com/Images/Site/main_logo.png"" style=""width:150px; float:left;"" />
                        </div>
                        <div style=""width:100%; float:left; border-top:1px dashed #d6d6d6; border-bottom:1px dashed #d6d6d6;"">
                            <div style=""width:100%; float:left; text-align:justify; padding:40px 0px; background:#f2f2f2; text-align:center;  margin:20px 0px; border-radius:5px; position:relative;"">
                                <div style=""width:100%; float:left;"">
                                    <label style=""font-size:16px; color:black; float:left; width:100%; text-align:center; padding-bottom:30px;"">Merhaba, görüş ve önerilerinizi bize bildirdiğiniz için teşekkür ederiz. En kısa zamanda size geri dönüş yapacağız</label>
                                    <label style=""font-size:14px; color:black; float:left; width:100%; text-align:center; padding-bottom:35px;"">Siteye devam etmek için aşağıdaki linke tıklayabilirsiniz.</label>
                                    <a href=""http://www.icaked.com"" style=""text-decoration:none;"">
                                        <label style=""border-radius:3px; padding:10px 20px; font-size:18px; background:saddlebrown; color:white; box-shadow: 0px 2px 5px -2px gray;"">Pastanızı Tasarlayın</label>
                                    </a>
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

            String siteMailBody = @"
                <div style=""width:90%; margin-left:5%; font-family:Helvetica, sans-serif;"">
                    <div style=""margin:auto; min-width:330px; max-width:600px;"">
                        <div style=""padding:40px 0px; width:100%; float:left; background:white;"">
                            <img src=""http://www.icaked.com/Images/Site/main_logo.png"" style=""width:150px; float:left;"" />
                        </div>
                        <div style=""width:100%; float:left; border-top:1px dashed #d6d6d6; border-bottom:1px dashed #d6d6d6;"">
                            <div style=""width:100%; float:left; text-align:justify; padding:40px 0px; background:#f2f2f2; text-align:center;  margin:20px 0px; border-radius:5px; position:relative;"">
                                <div style=""width:100%; float:left;"">
                                    <label style=""font-size:14px; color:black; float:left; width:100%; text-align:center; padding-bottom:10px;"">İsim Soyisim : " + nameSurname + @"</label>
                                    <label style=""font-size:14px; color:black; float:left; width:100%; text-align:center; padding-bottom:10px;"">E-Posta : " + email + @"</label>
                                    <label style=""font-size:14px; color:black; float:left; width:100%; text-align:center; padding-bottom:30px;"">Tarih : " + DateTime.Now + @"</label>
                                    <label style=""font-size:14px; color:black; float:left; width:100%; text-align:center; padding-bottom:35px;"">" + content + @"</label>
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

            ac.sendMail(email, "iCaked.com - Geri Bildiriminiz Alınmıştır", userMailBody, null, null);
            ac.sendMail("info@icaked.com", "iCaked.com - Geri Bildirim", siteMailBody, null, null);

            TempData["feedbackTaken"] = true;
            return RedirectToAction("Gorusler");
        }

        public ActionResult IstekFigurForm(FormCollection form, HttpPostedFileBase istekFigurImg)
        {
            String nameSurname = form["istekFigurName"];
            String email = form["istekFigurMail"];
            String content = form["istekFigurMessage"];
            Guid requestID = Guid.NewGuid();
            dc.DBQuerySetter("insert into Requests (ID,RequestID,Type,NameSurname,Email,Message) values ('" + Guid.NewGuid() + "','" + requestID + "','Figure',N'" + nameSurname + "','" + email + "',N'" + content + "')");

            String userMailBody = @"
                <div style=""width:90%; margin-left:5%; font-family:Helvetica, sans-serif;"">
                    <div style=""margin:auto; min-width:330px; max-width:600px;"">
                        <div style=""padding:10px 0px; width:100%; float:left; background:white; text-align:center;"">
                            <img src=""http://www.icaked.com/Images/Site/main_logo.png"" style=""width:150px;"" />
                        </div>
                        <div style=""width:100%; float:left; border-top:1px dashed #e6e6e6;"">
                            <div style=""width:100%; float:left; text-align:justify; padding:40px 0px; text-align:center;  margin:20px 0px; border-radius:5px; position:relative;"">
                                <div style=""width:100%; float:left;"">
                                    <label style=""font-size:14px; color:#444; float:left; width:100%; text-align:center; padding-bottom:30px;"">Merhaba, figür isteğiniz alınmıştır. <br /> İstediğiniz 3D figür kısa zamanda editörümüze eklenecektir. <br /> Teşekkür ederiz.</label>
                                    <label style=""font-size:14px; color:#444; float:left; width:100%; text-align:center; padding-bottom:35px;"">Siteye devam etmek için aşağıdaki linke tıklayabilirsiniz.</label>
                                    <a href=""http://www.icaked.com"" style=""text-decoration:none;"">
                                        <label style=""border-radius:3px; padding:10px 20px; font-size:18px; background:#b7613c; color:white; box-shadow: 0px 2px 5px -2px gray;"">Pastanızı Tasarlayın</label>
                                    </a>
                                </div>
                            </div>
                        </div>
                        <div style=""background:#f5f5f5; border-radius:5px; width:100%; margin-bottom:50px; float:left; padding:20px 0px;"">
                            <a style=""text-decoration:none; float:right; width:100%; text-align:center;"">
                                <label style=""font-size:14px; float:right; color:dimgray; width:100%; text-align:center;"">2016 © www.iCaked.com</label>
                            </a>
                        </div>
                    </div>
                </div>
                ";

            String imgStr = "";

            if (istekFigurImg != null)
            {
                String figureImgPath = "https://www.icaked.com/Images/FigureRequest/" + requestID.ToString() + ".png";
                dc.DBQuerySetter("update Requests set ImagePath = '" + requestID.ToString() + ".png' where RequestID = '" + requestID + "'");
                imgStr = @"<img style=""width:300px; margin:20px 0px; background:white; padding:5px; border:1px solid #d6d6d6;"" src=" + figureImgPath + @" />";

                string path = System.IO.Path.Combine(Server.MapPath("~/Images/FigureRequest/" + requestID.ToString() + ".png"));
                Image sourceimage = Image.FromStream(istekFigurImg.InputStream);
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
            }

            String siteMailBody = @"
                <div style=""width:90%; margin-left:5%; font-family:Helvetica, sans-serif;"">
                    <div style=""margin:auto; min-width:330px; max-width:600px;"">
                        <div style=""padding:10px 0px; width:100%; float:left; background:white; text-align:center;"">
                            <img src=""http://www.icaked.com/Images/Site/main_logo.png"" style=""width:150px;"" />
                        </div>
                        <div style=""width:100%; float:left; border-top:1px bottom #e6e6e6;"">
                            <div style=""width:100%; float:left; text-align:justify; padding:50px 0px; text-align:center;  margin:20px 0px; border-radius:5px; position:relative;"">
                                <div style=""width:100%; float:left; text-align:center;"">
                                    <label style=""font-size:18px; color:black; float:left; width:100%; text-align:center; padding-bottom:30px;"">Figür İsteği</label>
                                    <label style=""font-size:14px; color:black; float:left; width:100%; text-align:center; padding-bottom:10px;"">İsim Soyisim : " + nameSurname + @"</label>
                                    <label style=""font-size:14px; color:black; float:left; width:100%; text-align:center; padding-bottom:10px;"">E-Posta : " + email + @"</label>
                                    <label style=""font-size:14px; color:black; float:left; width:100%; text-align:center; padding-bottom:30px;"">Tarih : " + DateTime.Now + @"</label>
                                    <label style=""font-size:14px; color:black; float:left; width:100%; text-align:center; padding-bottom:35px;"">Mesaj : " + content + @"</label>
                                    " + imgStr + @"
                                </div>
                            </div>
                        </div>
                        <div style=""background:#f5f5f5; border-radius:5px; width:100%; margin-bottom:50px; float:left; padding:20px 0px;"">
                            <a style=""text-decoration:none; float:right; width:100%; text-align:center;"">
                                <label style=""font-size:14px; float:right; color:dimgray; width:100%; text-align:center;"">2016 © www.iCaked.com</label>
                            </a>
                        </div>
                    </div>
                </div>
                ";

            ac.sendMail(email, "iCaked.com - Figür İsteğiniz Alınmıştır", userMailBody, null, null);
            ac.sendMail("info@icaked.com", "iCaked.com - Figür Bildirimi", siteMailBody, null, null);

            TempData["istekFigurTaken"] = true;
            return RedirectToAction("FigurIstek");
        }

        public ActionResult IstekTasarimForm(FormCollection form, IEnumerable<HttpPostedFileBase> istekTasarimImg)
        {
            String nameSurname = form["istekTasarimName"];
            String email = form["istekTasarimMail"];
            String tel = form["istekTasarimTel"];
            String sehir = form["istekTasarimSehir"];
            String content = form["istekTasarimMessage"];
            Guid requestID = Guid.NewGuid();
            dc.DBQuerySetter("insert into Requests (ID,RequestID,Type,NameSurname,Email,Tel,Province,Message) values ('" + Guid.NewGuid() + "','" + requestID + "','design',N'" + nameSurname + "','" + email + "','" + tel + "',N'" + sehir + "',N'" + content + "')");

            String userMailBody = @"
                <div style=""width:90%; margin-left:5%; font-family:Helvetica, sans-serif;"">
                    <div style=""margin:auto; min-width:330px; max-width:600px;"">
                        <div style=""padding:10px 0px; width:100%; float:left; margin-top:30px; background:white; text-align:center;"">
                            <img src=""http://www.icaked.com/Images/Site/main_logo.png"" style=""width:150px;"" />
                        </div>
                        <div style=""width:100%; float:left; border-top:1px solid #e6e6e6;"">
                            <div style=""width:100%; float:left; text-align:justify; padding:50px 0px; text-align:center;  margin:20px 0px; border-radius:5px; position:relative;"">
                                <div style=""width:100%; float:left;"">
                                    <label style=""font-size:14px; color:#444; float:left; width:100%; text-align:center; padding-bottom:30px;"">Merhaba, tasarım isteğiniz alınmıştır. <br /> <br /> Tasarımınızı hayata geçirmek için elimizden geleni yapıp en kısa zamanda size geri dönüş yapacağız</label>
                                    <label style=""font-size:14px; color:#444; float:left; width:100%; text-align:center; padding-bottom:35px;"">Siteye devam etmek için aşağıdaki linke tıklayabilirsiniz.</label>
                                    <a href=""http://www.icaked.com"" style=""text-decoration:none;"">
                                        <label style=""border-radius:3px; padding:10px 20px; font-size:16px; background:#b7613c; color:white; box-shadow: 0px 2px 5px -2px gray;"">Pastanızı Tasarlayın</label>
                                    </a>
                                </div>
                            </div>
                        </div>

                        <div style=""background:#f5f5f5; border-radius:5px; width:100%; margin-bottom:50px; float:left; padding:20px 0px;"">
                            <a style=""text-decoration:none; float:right; width:100%; text-align:center;"">
                                <label style=""font-size:14px; float:right; color:dimgray; width:100%; text-align:center;"">2016 © www.iCaked.com</label>
                            </a>
                        </div>
                    </div>
                </div>
                ";

            String imgStr = "";

            if (istekTasarimImg != null & ((System.Web.HttpPostedFileBase[])istekTasarimImg)[0] != null)
            {
                string imgPath = "";
                for (int i = 0; i < ((System.Web.HttpPostedFileBase[])istekTasarimImg).Length; i++)
                {
                    String designImgPath = "https://www.icaked.com/Images/DesignRequest/" + requestID.ToString() + "_" + i + ".png";
                    imgPath = imgPath + requestID.ToString() + "_" + i + ".png" + " ";
                    imgStr = imgStr + @"<img style=""width:200px; margin:20px 0px; background:white; padding:5px; border:1px solid #d6d6d6;"" src=" + designImgPath + @" />";

                    string path = System.IO.Path.Combine(Server.MapPath("~/Images/DesignRequest/" + requestID.ToString() + "_" + i + ".png"));
                    Image sourceimage = Image.FromStream(((System.Web.HttpPostedFileBase[])istekTasarimImg)[i].InputStream);
                    var img = ScaleImage(sourceimage, 300, 300);
                    if (System.IO.File.Exists(path)) { System.IO.File.Delete(path); }

                    Bitmap bmp = new Bitmap(img);
                    var quantizer = new WuQuantizer();
                    using (var bitmap = bmp)
                    {
                        using (var quantized = quantizer.QuantizeImage(bitmap))
                        {
                            quantized.Save(path, ImageFormat.Png); quantized.Save(path, ImageFormat.Jpeg);
                        }
                    }
                }
                imgPath = imgPath.Substring(0, imgPath.Length - 1);
                dc.DBQuerySetter("update Requests set ImagePath = '" + imgPath + "' where RequestID = '" + requestID + "'");
            }

            String siteMailBody = @"
                <div style=""width:90%; margin-left:5%; font-family:Helvetica, sans-serif;"">
                    <div style=""margin:auto; min-width:330px; max-width:600px;"">
                        <div style=""padding:10px 0px; width:100%; float:left; background:white; text-align:center;"">
                            <img src=""http://www.icaked.com/Images/Site/main_logo.png"" style=""width:150px;"" />
                        </div>
                        <div style=""width:100%; float:left; border-top:1px solid #e6e6e6;"">
                            <div style=""width:100%; float:left; text-align:justify; padding:50px 0px; text-align:center;  margin:5px 0px; border-radius:5px; position:relative;"">
                                <div style=""width:100%; float:left; text-align:center;"">
                                    <label style=""font-size:18px; color:black; float:left; width:100%; text-align:center; padding-bottom:30px;"">Tasarım Pasta İsteği</label>
                                    <label style=""font-size:14px; color:black; float:left; width:100%; text-align:center; padding-bottom:10px;"">İsim Soyisim : " + nameSurname + @"</label>
                                    <label style=""font-size:14px; color:black; float:left; width:100%; text-align:center; padding-bottom:10px;"">E-Posta : " + email + @"</label>
                                    <label style=""font-size:14px; color:black; float:left; width:100%; text-align:center; padding-bottom:10px;"">Telefon : " + tel + @"</label>
                                    <label style=""font-size:14px; color:black; float:left; width:100%; text-align:center; padding-bottom:10px;"">Şehir : " + sehir + @"</label>
                                    <label style=""font-size:14px; color:black; float:left; width:100%; text-align:center; padding-bottom:30px;"">Tarih : " + DateTime.Now + @"</label>
                                    <label style=""font-size:14px; color:black; float:left; width:100%; text-align:center; padding-bottom:35px;"">Mesaj : " + content + @"</label>
                                    " + imgStr + @"
                                </div>
                            </div>
                        </div>
                        <div style=""background:#f5f5f5; border-radius:5px; width:100%; margin-bottom:50px; float:left; padding:20px 0px;"">
                            <a style=""text-decoration:none; float:right; width:100%; text-align:center;"">
                                <label style=""font-size:14px; float:right; color:dimgray; width:100%; text-align:center;"">2016 © www.iCaked.com</label>
                            </a>
                        </div>
                    </div>
                </div>
                ";

            ac.sendMail(email, "iCaked.com - Tasarım İsteğiniz Alınmıştır", userMailBody, null, null);
            ac.sendMail("info@icaked.com", "iCaked.com - Tasarım Bildirimi", siteMailBody, null, null);

            TempData["istekTasarimTaken"] = true;
            return RedirectToAction("TasarimIstek");
        }

        #endregion Operational Methods
    }
}