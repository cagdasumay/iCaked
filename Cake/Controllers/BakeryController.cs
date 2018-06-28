using Cake.Models;
using nQuant;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Web;
using System.Web.Mvc;
using System.Xml.Serialization;

namespace Cake.Controllers
{
    public class BakeryController : Controller
    {
        private DatabaseController dc = new DatabaseController();
        private ObjectController oc = new ObjectController();
        private UtilityController uc = new UtilityController();

        #region Constructor Methods

        public ActionResult Product(String ProductSelect, String ProductCount, String bakeryName, String category, String productName)
        {
            if (String.IsNullOrEmpty(productName) == false)
            {
                DataTable product = dc.MemoryCacheByQuery("select * from Products where ProductID like '%" + productName.Split('-')[productName.Split('-').Length - 1] + "%'");
                Guid ProductID = new Guid(product.Rows[0]["ProductID"].ToString());
                ProductHelper(product, ProductID, ProductSelect, ProductCount);
                return View();
            }
            else
            {
                //if (string.IsNullOrEmpty(category) == false) { return View(); }
                //else
                //{
                if (string.IsNullOrEmpty(bakeryName) == false)
                {
                    String whoseProfileIsThis = "bakery"; String whatIsThePermission = "";
                    if (bakeryName == "pasta-sanati") { bakeryName = "Pasta Sanatı"; }
                    DataTable bakeryinfo = dc.DBQueryGetter("select * from ForeignBakeries where Name like N'%" + bakeryName + "%'");

                    if (bakeryinfo.Rows.Count > 0)
                    {
                        String profileId = bakeryinfo.Rows[0]["BakeryID"].ToString();

                        ViewData["bakeryProducts"] = dc.DBQueryGetter("select * from Products where BakeryID = '" + profileId + "' and Approved = 'yes' order by newid()");
                        ViewData["bakeryinfo"] = bakeryinfo.Rows[0];

                        if (Session["userinfo"] != null)
                        {
                            if (((DataRow)Session["userinfo"])["Role"].ToString() == "admin") { whatIsThePermission = "admin"; }
                            else { whatIsThePermission = "user"; }
                        }
                        else if (Session["bakeryinfo"] != null)
                        {
                            if (((DataRow)Session["bakeryinfo"])["BakeryID"].ToString().ToLower() == profileId.ToLower())
                            {
                                whatIsThePermission = "own";
                                ViewData["bakeryOrders"] = dc.DBQueryGetter("select * from OrderLogs where BakeryID = '" + profileId + "'");
                            }
                            else { whatIsThePermission = "user"; }
                        }
                        else { whatIsThePermission = "user"; }

                        ViewData["whoseProfileIsThis"] = whoseProfileIsThis;
                        ViewData["whatIsThePermission"] = whatIsThePermission;
                    }
                    else
                    {
                        ViewData["bakeryFound"] = false;
                    }

                    if (String.IsNullOrEmpty(category) == false) { ViewData["category"] = category; }

                    return View("PastaneSelected");
                }
                else
                {
                    Header h = new Header();
                    h.title = "Pasta Siparişi - Pasta Siparişi Verebileceğiniz Pastaneler";
                    h.keywords = "liva, online pasta siparişi, butik pasta siparişi, yaş pasta siparişi, ankara pasta siparişi, ankara pastaneler";
                    h.description = "iCaked'in anlaşmalı olduğu pastaneler sizlere sayısız ürün arasından seçim imkanı sunuyor. Tasarım pasta, butik pasta seçenekleri sizleri bekliyor.";
                    ViewData["header"] = h;
                    return View("Pastane");
                }
                //}
            }
        }

        #endregion Constructor Methods

        #region Operational Methods

        public ActionResult AddBakery(FormCollection form, HttpPostedFileBase addBakeryPhoto)
        {
            String imgName = form["addBakeryName"].ToString().Trim().ToLower().Replace("ş", "s").Replace("ı", "i").Replace("ğ", "g").Replace("ç", "c").Replace("ö", "o").Replace("ü", "u") + "_thumbnail.png";
            String path = Path.Combine(Server.MapPath("~/Images/Bakery"), imgName);
            addBakeryPhoto.SaveAs(path);
            String BD = "no"; String HD = "no";
            if (form["addBakeryBD"] != null) { if (form["addBakeryBD"] == "on") { BD = "yes"; } }
            if (form["addBakeryHD"] != null) { if (form["addBakeryHD"] == "on") { HD = "yes"; } }

            dc.DBQuerySetter("insert into ForeignBakeries (ID,BrandID,BakeryID,Username,Password,Email,Name,Province,District,Address,Phone,isOpen,workHours,designWorkHours,workDays,earliestDelivery,earliestDeliveryDesign,Thumbnail,Description,Ranking,Taste,Editor,Role,CakePrice,CupcakePrice,CookiePrice,SpecificAddress,MinPacket,BakeryDelivery,HomeDelivery,isButik,BillWanted,Keywords) values ('" + Guid.NewGuid() + "','" + Guid.NewGuid() + "','" + Guid.NewGuid() + "',N'" + form["addBakeryUsername"].ToString().Trim() + "',N'" + form["addBakeryPassword"].ToString().Trim() + "',N'" + form["addBakeryMail"].ToString().Trim() + "',N'" + form["addBakeryName"].ToString().Trim() + "','" + form["addBakeryZip"].ToString().Trim() + "','867','42792','" + form["addBakeryTel"].ToString() +"','1','" + form["addBakeryWorkhours"].ToString().Trim() + "','" + form["addBakeryDesignWorkhours"].ToString().Trim() + "','1111111','" + form["addBakeryEarliest"].ToString().Trim() + "','" + form["addBakeryEarliestDesign"].ToString().Trim() + "',N'" + imgName +"',N'" + form["addBakeryDesc"].ToString().Trim() + "','9','9','9','Bakery','" + form["addBakeryCakePrice"].ToString().Trim() + "','" + form["addBakeryCupcakePrice"].ToString().Trim() + "','" + form["addBakeryCookiePrice"].ToString().Trim() + "',N'" + form["addBakeryAddress"].ToString().Trim() + "','15.00','" + BD + "','" + HD + "','no','0',N'" + form["addBakeryKeywords"].ToString().Trim() + "')");

            return Redirect(Request.UrlReferrer.ToString());
        }

        [HttpPost]
        public ActionResult AddEditLocation(FormCollection form)
        {
            String provinceId = form["bakeryProvinceLocation"];
            String provinceName = dc.DBQueryGetter("select SehirAdi from Sehirler where SehirId = '" + Convert.ToInt32(provinceId) + "'").Rows[0][0].ToString();

            String districtId = form["bakeryDistrictLocation"];
            String townId = form["bakeryTownLocation"];

            List<DataRow> districtNames = new List<DataRow>();
            List<DataRow> townNames = new List<DataRow>();

            if (districtId.ToLower() == "hepsi")
            {
                DataTable districts = dc.DBQueryGetter("select * from Ilceler where SehirId = '" + Convert.ToInt32(provinceId) + "'");
                for (int i = 0; i < districts.Rows.Count; i++) { districtNames.Add(districts.Rows[i]); }
            }
            else
            {
                DataTable districts = dc.DBQueryGetter("select * from Ilceler where ilceId = '" + Convert.ToInt32(districtId) + "'");
                districtNames.Add(districts.Rows[0]);
            }

            if (townId.ToLower() == "hepsi")
            {
                DataTable towns = dc.DBQueryGetter("select * from SemtMah where ilceId = '" + Convert.ToInt32(districtId) + "'");
                for (int i = 0; i < towns.Rows.Count; i++) { townNames.Add(towns.Rows[i]); }
            }
            else
            {
                DataTable towns = dc.DBQueryGetter("select * from SemtMah where SemtMahId = '" + Convert.ToInt32(townId) + "'");
                townNames.Add(towns.Rows[0]);
            }

            for (int i = 0; i < districtNames.Count; i++)
            {
                String district = districtNames[i]["ilceId"].ToString();
                String districtName = districtNames[i]["IlceAdi"].ToString();
                for (int i2 = 0; i2 < townNames.Count; i2++)
                {
                    String town = townNames[i2]["SemtMahId"].ToString();
                    String townName = townNames[i2]["SemtAdi"].ToString();
                    String neighborhoodName = townNames[i2]["MahalleAdi"].ToString();
                    bool locationExists = dc.DBQueryGetter("select * from BakeryNetwork where BakeryID = '" + ((DataRow)Session["bakeryinfo"])["BakeryID"] + "' and Province = '" + provinceId + "' and District = '" + district + "' and Town = '" + town + "'").Rows.Count > 0;
                    if (locationExists == false)
                    {
                        dc.DBQuerySetter("insert into BakeryNetwork (ID,NetworkID,BakeryID,Province,ProvinceName,District,DistrictName,Town,TownName,NeighborhoodName,MinPacket,DeliveryPrice) values ('" + Guid.NewGuid() + "','" + Guid.NewGuid() + "','" + ((DataRow)Session["bakeryinfo"])["BakeryID"] + "','" + provinceId + "',N'" + provinceName + "','" + district + "',N'" + districtName + "','" + town + "',N'" + townName + "',N'" + neighborhoodName + "','" + form["bakeryMinPacket"] + "','" + form["bakeryDeliveryPrice"] + "')");
                    }
                }
            }

            return Redirect(Request.UrlReferrer.ToString());
        }

        [HttpPost]
        public ActionResult EditLocation(FormCollection form)
        {
            string town = form["town"].ToString();
            dc.DBQuerySetter("update BakeryNetwork set MinPacket = '" + form["editMinPacket"] + "', DeliveryPrice = '" + form["editDeliveryPrice"] + "' where Town = '" + town + "'");
            return Redirect(Request.UrlReferrer.ToString());
        }

        public ActionResult DeleteLocation(String id)
        {
            dc.DBQuerySetter("delete from BakeryNetwork where BakeryID = '" + ((DataRow)Session["bakeryinfo"])["BakeryID"] + "' and NetworkID = '" + new Guid(id) + "'");
            return Redirect(Request.UrlReferrer.ToString());
        }

        public ActionResult BakeryOrderSub(String orderID)
        {
            ViewData["bakeryCurrentOrder"] = dc.DBQueryGetter("select * from OrderLogs where OrderID = '" + new Guid(orderID) + "'").Rows[0];
            return View();
        }

        public JsonResult ChangeOrderStatus(String status, String orderID)
        {
            dc.DBQuerySetter("update OrderLogs set Status = N'" + status + "' where OrderID = '" + new Guid(orderID) + "'");

            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AddEditBakeryPrice(FormCollection form)
        {
            String price = form["price"].Replace(',', '.').Replace("TL", "");

            if (form["what"].ToString() == "add")
            {
                dc.DBQuerySetter("insert into BakeryPrices (ID,BakeryID,Type,Number,Price,Currency) values ('" + Guid.NewGuid() + "', '" + new Guid(form["bakeryID"].ToString()) + "','" + form["type"] + "','" + form["size"] + "', '" + price + "','TL')");
            }
            else if (form["what"].ToString() == "edit")
            {
                dc.DBQuerySetter("update BakeryPrices set Number = '" + form["size"] + "', Price = '" + price + "' where ID = '" + form["ID"] + "'");
            }

            return Redirect(Request.UrlReferrer.ToString());
        }

        public ActionResult DeleteBakeryPrice(String priceID)
        {
            dc.DBQuerySetter("delete from BakeryPrices where ID = '" + new Guid(priceID) + "'");
            return Redirect(Request.UrlReferrer.ToString());
        }

        public ActionResult AddEditBakeryContent(FormCollection form)
        {
            if (form["what"].ToString() == "add")
            {
                dc.DBQuerySetter("insert into BakeryContents (ID,BakeryID,Type,Inside) values ('" + Guid.NewGuid() + "', '" + new Guid(form["bakeryID"].ToString()) + "','" + form["type"] + "',N'" + form["Content"] + "')");
            }
            else if (form["what"].ToString() == "edit")
            {
                dc.DBQuerySetter("update BakeryContents set Inside = N'" + form["Content"] + "' where ID = '" + form["ID"] + "'");
            }

            return Redirect(Request.UrlReferrer.ToString());
        }

        public ActionResult DeleteBakeryContent(String contentID)
        {
            dc.DBQuerySetter("delete from BakeryContents where ID = '" + new Guid(contentID) + "'");
            return Redirect(Request.UrlReferrer.ToString());
        }

        public JsonResult approveComment(String CommentID)
        {
            String yesno = dc.DBQueryGetter("select Approved from ProductComments where ID = '" + CommentID + "'").Rows[0][0].ToString();
            if (yesno == "yes") { yesno = "no"; }
            else if (yesno == "no") { yesno = "yes"; }
            dc.DBQuerySetter("update ProductComments set Approved = '" + yesno + "' where ID = '" + new Guid(CommentID) + "'");
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public string getProductSize(string productID, string productPrice)
        {
            DataRow product = dc.DBQueryGetter("select * from Products where ProductID = '" + new Guid(productID) + "'").Rows[0];
            int idx = 0;
            String[] prices = product["Price"].ToString().Split(' ');
            String[] sizes = product["SizeOptions"].ToString().Split(' ');

            try
            {
                for (int i = 0; i < prices.Length; i++) { if (prices[i] == productPrice) { idx = i; } }
            }
            catch(Exception e) { }

            return sizes[idx];
        }

        public void ProductHelper(DataTable dt, Guid ProductID, String ProductSelect, String ProductCount)
        {
            Double price = 0;
            DataColumnCollection columns = dt.Columns;
            if (columns.Contains("PagePrice") == false)
            {
                dt.Columns.Add("PagePrice", typeof(String)); dt.Columns.Add("Count", typeof(String)); dt.Columns.Add("Select", typeof(String));
            }

            if (String.IsNullOrEmpty(ProductCount)) { dt.Rows[0]["Count"] = "1"; } else { dt.Rows[0]["Count"] = ProductCount; }

            String category = dt.Rows[0]["Category"].ToString();

            if (String.IsNullOrEmpty(ProductSelect))
            {
                if (category.Contains("Pasta") | category == "Parti Malzemeleri") { dt.Rows[0]["Select"] = ((dt.Rows[0])["SizeOptions"].ToString().Split(' '))[0]; }
                else if (category == "Çikolata" | category == "Tatlı")
                {
                    if (String.IsNullOrEmpty((dt.Rows[0])["Gram"].ToString()) == false) { dt.Rows[0]["Select"] = ((dt.Rows[0])["Gram"].ToString().Split(' '))[0]; }
                    else { dt.Rows[0]["Select"] = ((dt.Rows[0])["Number"].ToString().Split(' '))[0]; }
                }
                else if (category == "Cupcake" | category == "Kek/Kurabiye") { dt.Rows[0]["Select"] = ((dt.Rows[0])["Number"].ToString().Split(' '))[0]; }

                price = Convert.ToDouble(dt.Rows[0]["Price"].ToString().Split(' ')[0].Replace(".", ","));
            }
            else
            {
                dt.Rows[0]["Select"] = ProductSelect; String[] select = new String[0];
                if (category.Contains("Pasta") | category == "Parti Malzemeleri") { select = ((dt.Rows[0])["SizeOptions"].ToString().Split(' ')); }
                else if (category == "Çikolata" | category == "Tatlı")
                {
                    if (String.IsNullOrEmpty((dt.Rows[0])["Gram"].ToString()) == false) { select = ((dt.Rows[0])["Gram"].ToString().Split(' ')); }
                    else { dt.Rows[0]["Select"] = ((dt.Rows[0])["Number"].ToString().Split(' ')); }
                }
                else if (category == "Cupcake" | category == "Kek/Kurabiye") { select = ((dt.Rows[0])["Number"].ToString().Split(' ')); }

                for (int i = 0; i < select.Length; i++)
                {
                    if (select[i] == ProductSelect)
                    {
                        price = Convert.ToDouble(dt.Rows[0]["Price"].ToString().Split(' ')[i].Replace(".", ","));
                    }
                }
            }

            dt.Rows[0]["PagePrice"] = price * Convert.ToDouble(dt.Rows[0]["Count"]);

            ViewData["ProductInfo"] = dt.Rows[0];
            ViewData["ProductBakery"] = dc.DBQueryGetter("select * from ForeignBakeries where BakeryID = '" + ((DataRow)ViewData["ProductInfo"])["BakeryID"].ToString().ToUpper() + "'").Rows[0];

            bool isFaved = false;
            if (Session["userinfo"] != null) { isFaved = dc.DBQueryGetter("select * from Fav where ProductID = '" + ProductID + "' and UserID = '" + ((DataRow)Session["userinfo"])["UserID"] + "'").Rows.Count > 0; }
            ViewData["isFaved"] = isFaved;

            Session["currentProduct"] = ViewData["ProductInfo"];

            ViewBag.ProductID = dt.Rows[0]["ProductID"];
            ViewBag.ProductName = dt.Rows[0]["Name"];
            ViewBag.ProductDescription = dt.Rows[0]["Description"];
            String[] photos = dt.Rows[0]["Thumbnail"].ToString().Split(new[] { "  " }, StringSplitOptions.None);
            ViewBag.ProductImg = "https://icaked.com/Images/Products/" + dt.Rows[0]["ProductID"] + "/" + photos[0];
        }

        public String GetBackgroundByPoint(decimal point)
        {
            String color = "";

            if (point <= 3) { color = "#FF0000"; }
            else if (point > 3 & point <= 6) { color = "#FF9100"; }
            else if (point > 6 & point <= 8) { color = "#C8E600"; }
            else if (point > 8) { color = "#01b506"; }

            return color;
        }

        [HttpPost]
        public ActionResult AddBakeryObject(FormCollection form, IEnumerable<HttpPostedFileBase> objectFile, HttpPostedFileBase objectImage, HttpPostedFileBase objectTexture)
        {
            DataRow bakery = (DataRow)Session["bakeryinfo"];
            String approved = "no"; String keywords = ""; String price = "";
            if (form["bakery-object-keywords"] != null) { keywords = form["bakery-object-keywords"].ToString(); }
            if (form["bakery-object-price"] != null) { price = form["bakery-object-price"].ToString().Replace(" TL", "").Replace(",", "."); }

            if (((System.Web.HttpPostedFileBase[])objectFile).Length > 1) { objectImage = null; objectTexture = null; }                                 // birden fazla dosyada image ve texture'ı nulla

            CakeObject co = new CakeObject();
            co.Type = form["bakery-object-category"];
            co.Properties = new ObjectProperties(); co.Properties.styles = new styles();
            co.Properties.styles.scale = 1;
            if (String.IsNullOrEmpty(form["bakery-object-color"]) == false) { co.Properties.styles.color = form["bakery-object-color"]; } else { co.Properties.styles.color = "FFFFFF"; }
            String xml = oc.SerializeObject(co);                                                                                                           // propertyleri update et

            for (int i = 0; i < ((System.Web.HttpPostedFileBase[])objectFile).Length; i++)
            {
                Guid id = Guid.NewGuid(); Guid objectId = Guid.NewGuid();
                var fileName = Path.GetFileName(((System.Web.HttpPostedFileBase[])objectFile)[i].FileName);
                bool alreadyExists = (dc.DBQueryGetter("select * from Objects where ObjectPath = '" + fileName + "'").Rows.Count > 0);         // obje varmı check et

                if (fileName.ToLower().Contains(".obj") & alreadyExists == false)
                {
                    Random rnd = new Random();
                    int num = rnd.Next(100000); string copyFileName = fileName.Replace(".obj", "") + "_" + num.ToString();
                    var path2 = Path.Combine(Server.MapPath("~/objects2"), copyFileName + ".obj");
                    ((System.Web.HttpPostedFileBase[])objectFile)[i].SaveAs(path2);

                    var path = Path.Combine(Server.MapPath("~/Objects"), fileName);
                    ((System.Web.HttpPostedFileBase[])objectFile)[i].SaveAs(path);                                                                      // obje kaydı

                    dc.DBQuerySetter("insert into Objects (ID, ObjectID, BakeryID, ObjectName, Type, ObjectPath, Properties, Approved, Keywords, Price, Currency) values ('" + id + "','" + objectId + "','" + bakery["BakeryID"] + "','" + fileName.Replace(".obj", "") + "','" + form["bakery-object-category"] + "','" + fileName + "','" + xml + "', '" + approved + "', '" + keywords + "', '" + price + "','TL')");
                    // db obje kaydı

                    if (objectImage != null)
                    {
                        var imgFileName = fileName.Replace(".obj", "_image.png");
                        var imgPath = Path.Combine(Server.MapPath("~/Objects/Images"), imgFileName);
                        objectImage.SaveAs(imgPath);                                                                                                    // image kaydı
                        dc.DBQuerySetter("update Objects set ImagePath = '" + imgFileName + "' where ObjectPath = '" + fileName + "'");        // db image kaydı
                    }

                    if (objectTexture != null)
                    {
                        var textureFileName = fileName.Replace(".obj", "_texture.png");
                        var texturePath = Path.Combine(Server.MapPath("~/Objects/Textures"), textureFileName);
                        objectTexture.SaveAs(texturePath);                                                                                              // texture kaydı
                        dc.DBQuerySetter("update Objects set TexturePath = '" + textureFileName + "' where ObjectPath = '" + fileName + "'");  // db texture kaydı
                    }
                }
            }
            return Redirect(Request.UrlReferrer.ToString());
        }

        public ActionResult DeleteBakeryObject(String ObjectID)
        {
            Guid objid = new Guid(ObjectID);
            DataRow dr = dc.DBQueryGetter("select * from Objects where ObjectID = '" + ObjectID + "'").Rows[0];                                    // db obje silme
            dc.DBQuerySetter("delete from Objects where ObjectID = '" + objid + "'");                                                              // obje silme

            FileInfo objFile = new FileInfo(HttpRuntime.AppDomainAppPath + "Objects\\Sources\\" + dr["ObjectPath"]); if (objFile.Exists) { objFile.Delete(); }

            if (dr["ImagePath"] != null)
            {
                String path = HttpRuntime.AppDomainAppPath + "Objects\\Images\\" + dr["ImagePath"];
                FileInfo imgFile = new FileInfo(path); if (imgFile.Exists) { imgFile.Delete(); }                                                            // image silme
            }

            if (dr["TexturePath"] != null)
            {
                String path = HttpRuntime.AppDomainAppPath + "Objects\\Textures\\" + dr["TexturePath"];
                FileInfo imgFile = new FileInfo(path); if (imgFile.Exists) { imgFile.Delete(); }                                                            // texture silme
            }

            return Redirect(Request.UrlReferrer.ToString());
        }

        public ActionResult EditBakeryObject(String ObjectID, FormCollection form, IEnumerable<HttpPostedFileBase> objectFile, HttpPostedFileBase objectImage, HttpPostedFileBase objectTexture)
        {
            String approved = "no"; String keywords = ""; String price = "";
            if (form["bakery-object-keywords"] != null) { keywords = form["bakery-object-keywords"].ToString(); }
            if (form["bakery-object-price"] != null) { price = form["bakery-object-price"].ToString().Replace(" TL", "").Replace(",", "."); }

            DataRow dr = dc.DBQueryGetter("select * from Objects where ObjectID = '" + ObjectID + "'").Rows[0];
            dc.DBQuerySetter("update Objects set Type = '" + form["bakery-object-category"] + "',approved = '" + approved + "',keywords = '" + keywords + "',price = '" + price + "',currency = 'TL' where ObjectID = '" + ObjectID + "'");         // db kategori update

            if (String.IsNullOrEmpty(form["bakery-object-color"]) == false)
            {
                CakeObject co = new CakeObject();
                XmlSerializer serializer = new XmlSerializer(typeof(ObjectProperties));
                using (TextReader reader = new StringReader(dr["Properties"].ToString()))
                {
                    co.Properties = (ObjectProperties)serializer.Deserialize(reader);
                }
                co.Type = form["bakery-object-category"];
                if (String.IsNullOrEmpty(form["bakery-object-color"]) == false) { co.Properties.styles.color = form["bakery-object-color"]; } else { co.Properties.styles.color = "FFFFFF"; }
                String xml = oc.SerializeObject(co);
                dc.DBQuerySetter("update Objects set Properties = '" + xml + "' where ObjectID = '" + ObjectID + "'");                         // db properties update
            }

            if (objectFile != null & ((System.Web.HttpPostedFileBase[])objectFile)[0] != null)
            {
                var fileName = Path.GetFileName(dr["ObjectPath"].ToString());
                var path = Path.Combine(Server.MapPath("~/Objects/Sources"), fileName);
                ((System.Web.HttpPostedFileBase[])objectFile)[0].SaveAs(path);
                dc.DBQuerySetter("update Objects set ObjectPath = '" + fileName + "' where ObjectID = '" + ObjectID + "'");                    // db obje update
            }

            if (objectImage != null)
            {
                var imgFileName = dr["ObjectPath"].ToString().Replace(".obj", "_images.png");
                var imgPath = Path.Combine(Server.MapPath("~/Objects/Images"), imgFileName);
                objectImage.SaveAs(imgPath);                                                                                                            // db image update
                dc.DBQuerySetter("update Objects set Type = '" + form["bakery-object-category"] + "', ImagePath = '" + imgFileName + "' where ObjectID = '" + ObjectID + "'");
            }

            if (objectTexture != null)
            {
                var fileName = Path.GetFileName(dr["ObjectPath"].ToString().Replace(".obj", "_texture.png"));
                var path = Path.Combine(Server.MapPath("~/Objects/Textures"), fileName);
                objectTexture.SaveAs(path);
                dc.DBQuerySetter("update Objects set TexturePath = '" + fileName + "' where ObjectID = '" + ObjectID + "'");                   // db texture update
            }

            return Redirect(Request.UrlReferrer.ToString());
        }

        [HttpPost]
        public ActionResult AddBakeryProduct(FormCollection form, IEnumerable<HttpPostedFileBase> bakeryProductImages)
        {
            Guid productID = Guid.NewGuid(); String imgPaths = ""; String approved = "no"; int stock = 0; string designProduct = "no";
            if (form["bakery_product_approved"] != null) { if (form["bakery_product_approved"] == "on") { approved = "yes"; } }
            if (form["bakery_product_designProduct"] != null) { if (form["bakery_product_designProduct"] == "on") { designProduct = "yes"; } }
            if (Convert.ToInt32(form["bakery_product_stockAvailable"]) >= 0) { stock = Convert.ToInt32(form["bakery_product_stockAvailable"]); }

            Directory.CreateDirectory(Server.MapPath("~/Images/Products/" + productID));

            for (int i = 0; i < ((System.Web.HttpPostedFileBase[])bakeryProductImages).Length; i++)
            {
                System.IO.File.Copy(Server.MapPath("/Images/Site/no_image.png"), Server.MapPath("/Images/Products/" + productID + "/" + productID + "_" + (i + 1) + ".jpg"));
                string path = System.IO.Path.Combine(Server.MapPath("~/Images/Products/" + productID + "/" + productID + "_" + (i + 1) + ".jpg"));
                Image sourceimage = Image.FromStream(((HttpPostedFileBase[])bakeryProductImages)[i].InputStream);
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

                imgPaths = imgPaths + productID + "_" + (i + 1) + ".jpg  ";
            }

            DataRow bakeryinfo = (DataRow)Session["bakeryinfo"]; Guid bakeryID = new Guid();
            if (bakeryinfo == null) { bakeryID = new Guid(form["bakery_name"]); } else { bakeryID = new Guid(bakeryinfo["BakeryID"].ToString()); }

            String query = "insert into Products (ID,ProductID,BakeryID,Name,Category,StockAvailable,Rating,Thumbnail,Description,Currency,AddDate,Fav,Approved,DesignProduct) values ('" + Guid.NewGuid() + "','" + productID + "','" + bakeryID + "',N'" + form["bakery_product_name"].Replace("'", "''") + "',N'" + form["bakery_product_category"] + "','" + stock + "','0','" + imgPaths + "',N'" + form["bakery_product_description"].Replace("'", "''") + "','TL','" + DateTime.Now + "','0','" + approved + "','" + designProduct + "')";
            bool result = dc.DBQuerySetter(query);

            string cat = form["bakery_product_category"];

            if (cat.Contains("Pasta"))
            {
                String sizeOptions = "";
                String sizePrice = "";
                if (form["cakeSize1"] != null) { string[] str = form["cakeSize1"].Split(' '); sizeOptions = sizeOptions + str[0] + " "; sizePrice = sizePrice + str[1].Replace(",", ".") + " "; }
                if (form["cakeSize2"] != null) { string[] str = form["cakeSize2"].Split(' '); sizeOptions = sizeOptions + str[0] + " "; sizePrice = sizePrice + str[1].Replace(",", ".") + " "; }
                if (form["cakeSize3"] != null) { string[] str = form["cakeSize3"].Split(' '); sizeOptions = sizeOptions + str[0] + " "; sizePrice = sizePrice + str[1].Replace(",", ".") + " "; }
                if (form["cakeSize4"] != null) { string[] str = form["cakeSize4"].Split(' '); sizeOptions = sizeOptions + str[0] + " "; sizePrice = sizePrice + str[1].Replace(",", ".") + " "; }
                if (form["cakeSize5"] != null) { string[] str = form["cakeSize5"].Split(' '); sizeOptions = sizeOptions + str[0] + " "; sizePrice = sizePrice + str[1].Replace(",", ".") + " "; }
                if (form["cakeSize6"] != null) { string[] str = form["cakeSize6"].Split(' '); sizeOptions = sizeOptions + str[0] + " "; sizePrice = sizePrice + str[1].Replace(",", ".") + " "; }
                sizeOptions = sizeOptions.Substring(0, sizeOptions.Length - 1); sizePrice = sizePrice.Substring(0, sizePrice.Length - 1);
                String[] prices = sizePrice.Split(' ');
                for (int i = 0; i < prices.Length; i++) { if (!prices[i].Contains(".")) { prices[i] = prices[i] + ".00"; } }

                dc.DBQuerySetter("update Products set SizeOptions = '" + sizeOptions + "', Price = '" + sizePrice + "' where ProductID = '" + productID + "'");
            }
            else if (cat == "Çikolata" | cat == "Tatlı")
            {
                String gramOptions = "";
                String gramPrice = "";
                if (form["gram1"] != null) { string[] str = form["gram1"].Split(' '); gramOptions = gramOptions + str[0] + " "; gramPrice = gramPrice + str[1].Replace(",", ".") + " "; }
                if (form["gram2"] != null) { string[] str = form["gram2"].Split(' '); gramOptions = gramOptions + str[0] + " "; gramPrice = gramPrice + str[1].Replace(",", ".") + " "; }
                if (form["gram3"] != null) { string[] str = form["gram3"].Split(' '); gramOptions = gramOptions + str[0] + " "; gramPrice = gramPrice + str[1].Replace(",", ".") + " "; }
                if (form["gram4"] != null) { string[] str = form["gram4"].Split(' '); gramOptions = gramOptions + str[0] + " "; gramPrice = gramPrice + str[1].Replace(",", ".") + " "; }
                if (form["gram5"] != null) { string[] str = form["gram5"].Split(' '); gramOptions = gramOptions + str[0] + " "; gramPrice = gramPrice + str[1].Replace(",", ".") + " "; }
                if (form["gram6"] != null) { string[] str = form["gram6"].Split(' '); gramOptions = gramOptions + str[0] + " "; gramPrice = gramPrice + str[1].Replace(",", ".") + " "; }

                if (String.IsNullOrEmpty(gramOptions) == false)
                {
                    gramOptions = gramOptions.Substring(0, gramOptions.Length - 1); gramPrice = gramPrice.Substring(0, gramPrice.Length - 1);
                    String[] prices = gramPrice.Split(' ');
                    for (int i = 0; i < prices.Length; i++) { if (!prices[i].Contains(".")) { prices[i] = prices[i] + ".00"; } }
                    dc.DBQuerySetter("update Products set Gram = '" + gramOptions + "', Price = '" + gramPrice + "' where ProductID = '" + productID + "'");
                }
                else if (String.IsNullOrEmpty(form["bakery_product_number"].ToString()) == false)
                {
                    dc.DBQuerySetter("update Products set Number = '" + form["bakery_product_number"].ToString() + "', Price = '" + form["bakery_product_price"].ToString() + "', Approved = '" + approved + "', DesignProduct = '" + designProduct + "' where ProductID = '" + productID + "'");
                }
            }
            else if (cat == "Cupcake" | cat == "Kek/Kurabiye")
            {
                decimal price = Convert.ToDecimal(form["bakery_product_price"].Replace(".", ",").Replace("TL", "").Replace(" ", ""));
                dc.DBQuerySetter("update Products set Number = '" + form["bakery_product_number"] + "', Price = '" + price.ToString().Replace(",", ".") + "' where ProductID = '" + productID + "'");
            }
            else if (cat == "Parti Malzemeleri")
            {
                String description = "";
                String sizeOptions = "";
                String sizePrice = "";
                if (form["partySize1"] != null) { string[] str = form["partySize1"].Split(' '); sizeOptions = sizeOptions + str[0] + " "; sizePrice = sizePrice + str[2].Replace(",", ".") + " "; description = description + form["partyDescription1"] + "~"; }
                if (form["partySize2"] != null) { string[] str = form["partySize2"].Split(' '); sizeOptions = sizeOptions + str[0] + " "; sizePrice = sizePrice + str[2].Replace(",", ".") + " "; description = description + form["partyDescription2"] + "~"; }
                if (form["partySize3"] != null) { string[] str = form["partySize3"].Split(' '); sizeOptions = sizeOptions + str[0] + " "; sizePrice = sizePrice + str[2].Replace(",", ".") + " "; description = description + form["partyDescription3"] + "~"; }
                if (form["partySize4"] != null) { string[] str = form["partySize4"].Split(' '); sizeOptions = sizeOptions + str[0] + " "; sizePrice = sizePrice + str[2].Replace(",", ".") + " "; description = description + form["partyDescription4"] + "~"; }
                if (form["partySize5"] != null) { string[] str = form["partySize5"].Split(' '); sizeOptions = sizeOptions + str[0] + " "; sizePrice = sizePrice + str[2].Replace(",", ".") + " "; description = description + form["partyDescription5"] + "~"; }
                if (form["partySize6"] != null) { string[] str = form["partySize6"].Split(' '); sizeOptions = sizeOptions + str[0] + " "; sizePrice = sizePrice + str[2].Replace(",", ".") + " "; description = description + form["partyDescription6"] + "~"; }
                sizeOptions = sizeOptions.Substring(0, sizeOptions.Length - 1); sizePrice = sizePrice.Substring(0, sizePrice.Length - 1); description = description.Substring(0, description.Length - 1);
                String[] prices = sizePrice.Split(' ');
                for (int i = 0; i < prices.Length; i++) { if (!prices[i].Contains(".")) { prices[i] = prices[i] + ".00"; } }
                description = description.Replace("\r\n", "<br />");
                dc.DBQuerySetter("update Products set SizeOptions = '" + sizeOptions + "',Description = N'" + description + "', Price = '" + sizePrice + "' where ProductID = '" + productID + "'");
            }

            if (form["bakery_name"] == null) { dc.DBQuerySetter("update Products set BakeryID = '" + form["bakery_name"] + "' where ProductID = '" + productID + "'"); }

            return Redirect(Request.UrlReferrer.ToString());
        }

        [HttpPost]
        public ActionResult EditBakeryProduct(FormCollection form, IEnumerable<HttpPostedFileBase> bakeryProductImages)
        {
            Guid productID = new Guid(form["product-id-hidden"]); String imgPaths = ""; String query = ""; String approved = "no"; int stock = 0; String designProduct = "no";

            DataRow bakeryinfo = (DataRow)Session["bakeryinfo"];
            if (Convert.ToInt32(form["bakery_product_stockAvailable"]) >= 0) { stock = Convert.ToInt32(form["bakery_product_stockAvailable"]); }

            if (form["bakery_product_approved"] != null) { if (form["bakery_product_approved"] == "on") { approved = "yes"; } }
            else
            {
                DataRow product = dc.DBQueryGetter("select * from Products where ProductID = '" + productID + "'").Rows[0];
                if (product["Approved"].ToString() == "yes") { approved = "yes"; }
            }

            if (form["bakery_product_designProduct"] != null) { if (form["bakery_product_designProduct"] == "on") { designProduct = "yes"; } }

            if (bakeryProductImages != null & ((HttpPostedFileBase[])bakeryProductImages)[0] != null)
            {
                if (Directory.Exists(HttpRuntime.AppDomainAppPath + "Images\\Products\\" + productID + "\\"))
                {
                    string[] files = Directory.GetFiles(HttpRuntime.AppDomainAppPath + "Images\\Products\\" + productID + "\\");
                    for (int i = 0; i < files.Length; i++)
                    {
                        if (System.IO.File.Exists(HttpRuntime.AppDomainAppPath + "Images\\Products\\" + productID + "\\" + files[i])) { System.IO.File.Delete(HttpRuntime.AppDomainAppPath + "Images\\Products\\" + productID + "\\" + files[i]); }
                    }
                }
                else
                {
                    Directory.CreateDirectory(Server.MapPath("~/Images/Products/" + productID));
                }

                for (int i = 0; i < ((System.Web.HttpPostedFileBase[])bakeryProductImages).Length; i++)
                {
                    string path = System.IO.Path.Combine(Server.MapPath("~/Images/Products/" + productID + "/" + productID + "_" + (i + 1) + ".jpg"));
                    Image sourceimage = Image.FromStream(((HttpPostedFileBase[])bakeryProductImages)[i].InputStream);
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

                    imgPaths = imgPaths + productID + "_" + (i + 1) + ".jpg  ";
                }
                query = "update Products set Name=N'" + form["bakery_product_name"] + "',Category='" + form["bakery_product_category"] + "',Thumbnail='" + imgPaths + "',Description=N'" + form["bakery_product_description"] + "',AddDate='" + DateTime.Now + "', StockAvailable = '" + stock + "' where ProductID = '" + productID + "'";
            }
            else
            {
                query = "update Products set Name=N'" + form["bakery_product_name"] + "',Category='" + form["bakery_product_category"] + "',Description=N'" + form["bakery_product_description"] + "',AddDate='" + DateTime.Now + "', StockAvailable = '" + stock + "' where ProductID = '" + productID + "'";
            }

            bool result = dc.DBQuerySetter(query);

            string cat = form["bakery_product_category"];
            if (cat.Contains("Pasta"))
            {
                String sizeOptions = "";
                String sizePrice = "";
                if (form["cakeSize1"] != null) { string[] str = form["cakeSize1"].Split(' '); sizeOptions = sizeOptions + str[0] + " "; sizePrice = sizePrice + str[1].Replace(",", ".") + " "; }
                if (form["cakeSize2"] != null) { string[] str = form["cakeSize2"].Split(' '); sizeOptions = sizeOptions + str[0] + " "; sizePrice = sizePrice + str[1].Replace(",", ".") + " "; }
                if (form["cakeSize3"] != null) { string[] str = form["cakeSize3"].Split(' '); sizeOptions = sizeOptions + str[0] + " "; sizePrice = sizePrice + str[1].Replace(",", ".") + " "; }
                if (form["cakeSize4"] != null) { string[] str = form["cakeSize4"].Split(' '); sizeOptions = sizeOptions + str[0] + " "; sizePrice = sizePrice + str[1].Replace(",", ".") + " "; }
                if (form["cakeSize5"] != null) { string[] str = form["cakeSize5"].Split(' '); sizeOptions = sizeOptions + str[0] + " "; sizePrice = sizePrice + str[1].Replace(",", ".") + " "; }
                if (form["cakeSize6"] != null) { string[] str = form["cakeSize6"].Split(' '); sizeOptions = sizeOptions + str[0] + " "; sizePrice = sizePrice + str[1].Replace(",", ".") + " "; }
                sizeOptions = sizeOptions.Substring(0, sizeOptions.Length - 1); sizePrice = sizePrice.Substring(0, sizePrice.Length - 1);

                dc.DBQuerySetter("update Products set SizeOptions = '" + sizeOptions + "', Price = '" + sizePrice + "', Approved = '" + approved + "', DesignProduct = '" + designProduct + "' where ProductID = '" + productID + "'");
            }
            else if (cat == "Çikolata" | cat == "Tatlı")
            {
                String gramOptions = "";
                String gramPrice = "";
                if (form["gram1"] != null) { string[] str = form["gram1"].Split(' '); gramOptions = gramOptions + str[0] + " "; gramPrice = gramPrice + str[1].Replace(",", ".") + " "; }
                if (form["gram2"] != null) { string[] str = form["gram2"].Split(' '); gramOptions = gramOptions + str[0] + " "; gramPrice = gramPrice + str[1].Replace(",", ".") + " "; }
                if (form["gram3"] != null) { string[] str = form["gram3"].Split(' '); gramOptions = gramOptions + str[0] + " "; gramPrice = gramPrice + str[1].Replace(",", ".") + " "; }
                if (form["gram4"] != null) { string[] str = form["gram4"].Split(' '); gramOptions = gramOptions + str[0] + " "; gramPrice = gramPrice + str[1].Replace(",", ".") + " "; }
                if (form["gram5"] != null) { string[] str = form["gram5"].Split(' '); gramOptions = gramOptions + str[0] + " "; gramPrice = gramPrice + str[1].Replace(",", ".") + " "; }
                if (form["gram6"] != null) { string[] str = form["gram6"].Split(' '); gramOptions = gramOptions + str[0] + " "; gramPrice = gramPrice + str[1].Replace(",", ".") + " "; }
                if (String.IsNullOrEmpty(gramOptions) == false)
                {
                    gramOptions = gramOptions.Substring(0, gramOptions.Length - 1); gramPrice = gramPrice.Substring(0, gramPrice.Length - 1);
                    dc.DBQuerySetter("update Products set Gram = '" + gramOptions + "', Price = '" + gramPrice + "', Approved = '" + approved + "', DesignProduct = '" + designProduct + "' where ProductID = '" + productID + "'");
                }
                else if (String.IsNullOrEmpty(form["bakery_product_number"].ToString()) == false)
                {
                    dc.DBQuerySetter("update Products set Number = '" + form["bakery_product_number"].ToString() + "', Price = '" + form["bakery_product_price"].ToString() + "', Approved = '" + approved + "', DesignProduct = '" + designProduct + "' where ProductID = '" + productID + "'");
                }
            }
            else if (cat == "Cupcake" | cat == "Kek/Kurabiye")
            {
                decimal price = Convert.ToDecimal(form["bakery_product_price"].Replace(".", ",").Replace("TL", "").Replace(" ", ""));
                dc.DBQuerySetter("update Products set Number = '" + form["bakery_product_number"] + "', Price = '" + price.ToString().Replace(",", ".") + "', Approved = '" + approved + "', DesignProduct = '" + designProduct + "' where ProductID = '" + productID + "'");
            }
            else if (cat == "Parti Malzemeleri")
            {
                String description = "";
                String sizeOptions = "";
                String sizePrice = "";
                if (form["partySize1"] != null) { string[] str = form["partySize1"].Split(' '); sizeOptions = sizeOptions + str[0] + " "; sizePrice = sizePrice + str[2].Replace(",", ".") + " "; description = description + form["partyDescription1"] + "~"; }
                if (form["partySize2"] != null) { string[] str = form["partySize2"].Split(' '); sizeOptions = sizeOptions + str[0] + " "; sizePrice = sizePrice + str[2].Replace(",", ".") + " "; description = description + form["partyDescription2"] + "~"; }
                if (form["partySize3"] != null) { string[] str = form["partySize3"].Split(' '); sizeOptions = sizeOptions + str[0] + " "; sizePrice = sizePrice + str[2].Replace(",", ".") + " "; description = description + form["partyDescription3"] + "~"; }
                if (form["partySize4"] != null) { string[] str = form["partySize4"].Split(' '); sizeOptions = sizeOptions + str[0] + " "; sizePrice = sizePrice + str[2].Replace(",", ".") + " "; description = description + form["partyDescription4"] + "~"; }
                if (form["partySize5"] != null) { string[] str = form["partySize5"].Split(' '); sizeOptions = sizeOptions + str[0] + " "; sizePrice = sizePrice + str[2].Replace(",", ".") + " "; description = description + form["partyDescription5"] + "~"; }
                if (form["partySize6"] != null) { string[] str = form["partySize6"].Split(' '); sizeOptions = sizeOptions + str[0] + " "; sizePrice = sizePrice + str[2].Replace(",", ".") + " "; description = description + form["partyDescription6"] + "~"; }
                sizeOptions = sizeOptions.Substring(0, sizeOptions.Length - 1); sizePrice = sizePrice.Substring(0, sizePrice.Length - 1); description = description.Substring(0, description.Length - 1);
                String[] prices = sizePrice.Split(' ');
                for (int i = 0; i < prices.Length; i++) { if (!prices[i].Contains(".")) { prices[i] = prices[i] + ".00"; } }
                description = description.Replace("\r\n", "<br />");
                dc.DBQuerySetter("update Products set SizeOptions = '" + sizeOptions + "',Description = N'" + description + "', Price = '" + sizePrice + "' where ProductID = '" + productID + "'");
            }

            if (form["bakery_name"] == null) { dc.DBQuerySetter("update Products set BakeryID = '" + form["bakery_name"] + "' where ProductID = '" + productID + "'"); }

            return Redirect(Request.UrlReferrer.ToString());
        }

        public ActionResult DeleteBakeryProduct(String productID)
        {
            String path = HttpRuntime.AppDomainAppPath + "Images\\Products\\" + productID + "\\";
            DirectoryInfo folder = new DirectoryInfo(path); if (folder.Exists) { folder.Delete(true); }

            bool result = dc.DBQuerySetter("delete from Products where ProductID = '" + productID + "'");
            dc.DBQuerySetter("delete from ProductComments where ProductID = '" + productID + "'");
            dc.DBQuerySetter("delete from Fav where ProductID = '" + productID + "'");

            return Redirect(Request.UrlReferrer.ToString());
        }

        public static Image ScaleImage(Image image, int newWidth, int newHeight)
        {
            var newImage = new Bitmap(newWidth, newHeight);

            using (var graphics = Graphics.FromImage(newImage))
                graphics.DrawImage(image, 0, 0, newWidth, newHeight);

            return newImage;
        }

        #endregion Operational Methods
    }
}