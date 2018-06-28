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
using System.Xml;
using System.Xml.Serialization;

namespace Cake.Controllers
{
    public class ObjectController : Controller
    {
        private DatabaseController dc = new DatabaseController();
        private OrderController oc = new OrderController();
        private UtilityController uc = new UtilityController();

        #region Design Methods

        public JsonResult EditorLocationSelect(String location)
        {
            Session["editorLocation"] = location;
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public JsonResult EditorModeSelect(String type)
        {
            Session["editorMode"] = type;
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public JsonResult EditorDesignSelect(String type)
        {
            Session["editorType"] = type;
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public JsonResult EditorCakeSelect(String size)
        {
            Session["editorSize"] = size;
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public JsonResult EditorCookieSelect(String size)
        {
            Session["editorSize"] = size;
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public JsonResult EditorCupcakeSelect(String size)
        {
            Session["editorSize"] = size;
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetDesignID()
        {
            Guid id = Guid.NewGuid();
            if (Session["DesignID"] == null) { Session["DesignID"] = id; }
            else { id = (Guid)Session["DesignID"]; }
            return Json(id.ToString(), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult UserDecal(FormCollection data)
        {
            if (Request.Files["decalCopy"] != null)
            {
                using (var binaryReader = new BinaryReader(Request.Files["decalCopy"].InputStream))
                {
                    String path = Path.Combine(Server.MapPath("~/Images/TempDecals"), Request.Files["decalCopy"].FileName);
                    Request.Files["decalCopy"].SaveAs(path);
                }
            }
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public JsonResult LoadDesign(String id)
        {
            Session["Objects"] = null;
            List<CakeObject> result = new List<CakeObject>();
            DataRow row = dc.DBQueryGetter("select * from MadeCakes where MadeID = '" + id + "'").Rows[0];
            Session["DesignID"] = new Guid(id);
            if (row["Mode"] != null) { Session["editorMode"] = row["Mode"].ToString(); }
            if (row["Type"] != null) { Session["editorType"] = row["Type"].ToString(); }
            if (row["SizeOptions"] != null) { Session["editorSize"] = row["SizeOptions"].ToString(); }
            String[] objstr = row["ObjString"].ToString().Split('~');

            for (int i = 0; i < objstr.Length; i++)
            {
                try
                {
                    CakeObject obj = new CakeObject();
                    if (!objstr[i].Contains("00000000-0000-0000-0000-000000000000")) { obj = LoadObjectFromPath(dc.MemoryCacheByQuery("select * from Objects where ObjectID = '" + objstr[i].ToString().Split(':')[1] + "'")); }
                    string[] objArray = objstr[i].ToString().Split(':');
                    obj.EditorName = objArray[0];
                    obj.TexturePath = objArray[2];
                    obj.JsonStr = row["JsonString"].ToString();

                    string[] allowedTypes = new string[] { "c", "b", "o", "k", "2d", "n" };

                    if (obj.Type != "cake")
                    {
                        if (allowedTypes.Contains(objArray[objArray.Length - 1]))
                        {
                            obj.SavedSize = objArray[objArray.Length - 1];
                            if (obj.SavedSize == "c") { obj.SavedSize = "o"; }
                        }
                    }
                    else
                    {
                        obj.SavedSize = objArray[objArray.Length - 1];
                        obj.CakeSize = objArray[objArray.Length - 1];
                    }

                    if (String.IsNullOrEmpty(obj.SavedSize))
                    {
                        if (allowedTypes.Contains(obj.SavedSize) == false) { obj.SavedSize = "n"; }
                    }

                    result.Add(obj);
                }
                catch (Exception e) { }
            }
            Session["Objects"] = result;
            var jsonResult = Json(result, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = 50000000;
            return jsonResult;
        }

        public ActionResult DeleteDesign(String designID)
        {
            String userid = ((DataRow)Session["userinfo"])["UserID"].ToString();
            dc.DBQuerySetter("delete from MadeCakes where MadeID = '" + new Guid(designID) + "' and UserID = '" + new Guid(userid) + "'");
            String path = HttpRuntime.AppDomainAppPath + "Images\\MadeCakes\\" + designID;
            if (Directory.Exists(path)) { Directory.Delete(path, true); }
            dc.DBQuerySetter("delete from DesignComments where MadeID = '" + new Guid(designID) + "'");
            return Redirect("/Profil");
        }

        public JsonResult ChangeDesignName(String designID, String newName)
        {
            dc.DBQuerySetter("update MadeCakes set Name = '" + newName + "' where MadeID = '" + new Guid(designID) + "'");
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetTexturePath(String TextureName, String DesignID)
        {
            return Json("/Images/PlaneTextures/picnic_texture.jpg", JsonRequestBehavior.AllowGet);
        }

        public JsonResult SaveType(String type)
        {
            Session["editorSaveType"] = type;
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        //public void SaveAllSmallImgs()
        //{
        //    string[] directories = Directory.GetDirectories(HttpRuntime.AppDomainAppPath + "Images\\MadeCakes\\");
        //    for(int i = 0; i < directories.Length; i++)
        //    {
        //        string folderName = Path.GetFileName(directories[i]);
        //        SaveSmallCakeImg(folderName);
        //    }
        //}

        public void SaveSmallCakeImg(string designID)
        {
            try
            {
                string loadpath = System.IO.Path.Combine(Server.MapPath("~/Images/MadeCakes/" + designID + "/designImg_1.png"));
                string savepath = System.IO.Path.Combine(Server.MapPath("~/Images/MadeCakes/" + designID + "/designImg-275x275.png"));

                var srcImage = Image.FromFile(loadpath);

                var newWidth = (int)(275);
                var newHeight = (int)(275);
                var newImage = new Bitmap(newWidth, newHeight);
                using (var graphics = Graphics.FromImage(newImage))
                {
                    graphics.DrawImage(srcImage, new Rectangle(0, 0, newWidth, newHeight));
                }

                var bitmap = new Bitmap(newImage);

                var quantizer = new WuQuantizer();
                using (var quantized = quantizer.QuantizeImage(bitmap))
                {
                    if (System.IO.File.Exists(savepath)) { System.IO.File.Delete(savepath); }
                    quantized.Save(savepath, ImageFormat.Png);
                }
            }
            catch (Exception e) { }
        }

        public JsonResult AutoSaveCake(string content)
        {
            string[] datas = content.Split(new string[] { "??" }, StringSplitOptions.None); string editorSize = "0"; int cakeCounter = 0;

            List<CakeObject> objs = (List<CakeObject>)Session["AllObjects"];
            String objStr = "";
            String formObjNames = datas[3];
            String[] formObjArray = formObjNames.Split(':');

            try
            {
                for (int i = 0; i < objs.Count; i++)
                {                                                                                                                  // tüm objeler
                    for (int i2 = 0; i2 < formObjArray.Length; i2++)                                                                                                    // json'daki tüm objeler
                    {
                        if (String.IsNullOrEmpty(formObjArray[i2])) { continue; }
                        string[] currentObj = formObjArray[i2].Split(',');                                                                                               // objeyi aç

                        if (currentObj[0].Split('-')[0] == objs[i].EditorName.Split('-')[0] & currentObj[0].Split('-')[1] == objs[i].EditorName.Split('-')[1])                                                                                                         // eşleşti mi
                        {
                            string size = "";
                            if (currentObj[0].Contains("side") | currentObj[0].Contains("rand") | currentObj[0].Contains("text") | currentObj[0].Contains("2D") | currentObj[0].Contains("plate") | currentObj[0].Contains("decal")) { }    // hesaplanmayacaklar
                            else    // cake ve 3d objler
                            {
                                DataRow obj = dc.MemoryCacheByQuery("select * from Objects where ObjectID = '" + objs[i].ID + "'").Rows[0];                                  // db'den objeyi çek
                                double scale = Convert.ToDouble(currentObj[1].Replace(".", ","));                                                                        // jsondan scale al, nokta yerine virgül lazım double'da

                                if (obj["Type"].ToString() == "cake")
                                {
                                    size = getCakeSize(scale).ToString();                                                                                               // cake size çek
                                    editorSize = (Convert.ToInt32(datas[1].Split(',')[cakeCounter]) + Convert.ToInt32(size)).ToString();
                                    cakeCounter++;
                                }
                                else if (obj["Type"].ToString() == "object" | obj["Type"].ToString() == "mixed")
                                {
                                    double minSize = Convert.ToDouble(obj["MinSize"].ToString().Replace(".", ","));
                                    double maxSize = Convert.ToDouble(obj["MaxSize"].ToString().Replace(".", ","));

                                    double sizetemp = (maxSize - minSize) / 3;

                                    if (scale <= (minSize + sizetemp)) { size = "k"; }
                                    else if (scale >= (minSize + sizetemp) & scale <= (minSize + sizetemp * 2)) { size = "o"; }
                                    else if (scale >= (minSize + sizetemp * 2)) { size = "b"; }
                                }
                                else
                                {
                                    size = "n";
                                }
                            }
                            increaseObjUsage(objs[i].ID.ToString());
                            objStr = objStr + objs[i].EditorName + ":" + objs[i].ID.ToString() + ":" + objs[i].TexturePath + ":" + size + "~";                      // obj stringini oluştur.
                        }
                    }
                    cakeCounter = 0;
                }
            }
            catch (Exception e) { }

            objStr = objStr.Substring(0, objStr.Length - 1);    // artan tildayı siliyoz.

            Session["editorAutoSaveString"] = objStr;

            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetDesignPrice(string bakeryID)
        {
            String objStr = (String)Session["editorAutoSaveString"];
            String price = CalculateDesignPriceJson(bakeryID, objStr);

            return Json(price, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SaveCake(FormCollection form)
        {
            String designName = ""; String editorMode = ""; String editorType = ""; String editorSize = "0"; String visible = "no";
            String price = "0.00";
            String designID = ((Guid)Session["DesignID"]).ToString();
            DataTable oldDesign = dc.DBQueryGetter("select * from MadeCakes where MadeID = '" + new Guid(designID) + "'");
            string[] cakeSizes = new string[0];

            if (Session["editorMode"] != null) { editorMode = (string)Session["editorMode"]; }
            if (Session["editorType"] != null) { editorType = (string)Session["editorType"]; }
            if (form["design_visible"] != null) { visible = form["design_visible"].ToString(); }
            if (form["cakeSizes"] != null) { cakeSizes = form["cakeSizes"].ToString().Split(','); }

            Guid userID = new Guid();

            if (Session["userinfo"] != null)
            {
                userID = new Guid(((DataRow)Session["userinfo"])["UserID"].ToString());
            }

            DataTable designTable = dc.DBQueryGetter("select * from MadeCakes where MadeID = '" + new Guid(designID) + "' and UserID = '" + userID + "'");
            bool isDesignExists = designTable.Rows.Count > 0;

            if (isDesignExists)
            {
                if (Session["editorSaveType"] != null)
                {
                    if ((string)Session["editorSaveType"] == "saveas") { designID = Guid.NewGuid().ToString(); designName = form["designName"].ToString(); }
                    else { designName = oldDesign.Rows[0]["Name"].ToString(); }
                }
                else { designName = oldDesign.Rows[0]["Name"].ToString(); }
            }
            else { designID = Guid.NewGuid().ToString(); designName = form["designName"].ToString(); }

            Directory.CreateDirectory(Server.MapPath("~/Images/MadeCakes/" + designID)); Directory.CreateDirectory(Server.MapPath("~/Images/Products/" + designID));
            String imagePath = "";

            for (int k = 0; k < 8; k++)
            {
                string index = (k + 1).ToString();
                string dataStr = form["CanvasImgStr" + index];

                Image sourceimage = LoadImage(dataStr);
                int xOffset = (int)((sourceimage.Width - sourceimage.Height) / 2.0);
                Rectangle crop = new Rectangle(xOffset, 0, sourceimage.Height, sourceimage.Height);
                var bmp = new Bitmap(crop.Width, crop.Height);
                using (var gr = Graphics.FromImage(bmp))
                {
                    gr.DrawImage(sourceimage, new Rectangle(0, 0, bmp.Width, bmp.Height), crop, GraphicsUnit.Pixel);
                }

                string path = System.IO.Path.Combine(Server.MapPath("~/Images/MadeCakes/" + designID + "/designImg_" + index + ".png"));
                string path2 = System.IO.Path.Combine(Server.MapPath("~/Images/Products/" + designID + "/" + designID + "_" + index + ".jpg"));
                imagePath = imagePath + "designImg_" + index + ".png ";

                var quantizer = new WuQuantizer();
                using (var bitmap = bmp)
                {
                    using (var quantized = quantizer.QuantizeImage(bitmap))
                    {
                        if (System.IO.File.Exists(path)) { System.IO.File.Delete(path); }
                        quantized.Save(path, ImageFormat.Png); quantized.Save(path2, ImageFormat.Png);
                    }
                }
                // saving image
            }

            SaveSmallCakeImg(designID);

            imagePath = imagePath.Substring(0, imagePath.Length - 1);

            //String[] objectSizes = (String[])Session["objectSizes"];

            List<CakeObject> objs = (List<CakeObject>)Session["AllObjects"];
            String objStr = "";
            String formObjNames = form["GlobalObjNames"];
            String[] formObjArray = formObjNames.Split(':');

            List<string> editorText = new List<string>();
            List<string> editorImg = new List<string>();

            for (int i = 0; i < formObjArray.Length; i++)
            {
                string[] elems = formObjArray[i].Split(',');
                if (elems.Length >= 3)
                {
                    if (elems[0].Contains("3dtext")) { editorText.Add(elems[2]); }
                    if (elems[0].Contains("decal")) { editorImg.Add(elems[2]); }
                }
            }

            if (editorText.Count != 0) { Session["editorText"] = editorText; }
            if (editorImg.Count != 0) { Session["editorImg"] = editorImg; }

            for (int i = 0; i < objs.Count; i++)
            {                                                                                                                  // tüm objeler
                for (int i2 = 0; i2 < formObjArray.Length; i2++)                                                                                                    // json'daki tüm objeler
                {
                    if (String.IsNullOrEmpty(formObjArray[i2])) { continue; }
                    string[] currentObj = formObjArray[i2].Split(',');                                                                                               // objeyi aç

                    if (currentObj[0].Split('-')[0] == objs[i].EditorName.Split('-')[0] & currentObj[0].Split('-')[1] == objs[i].EditorName.Split('-')[1])                                                                                                         // eşleşti mi
                    {
                        string size = "";
                        if (currentObj[0].Contains("side") | currentObj[0].Contains("rand") | currentObj[0].Contains("text") | currentObj[0].Contains("2D") | currentObj[0].Contains("plate") | currentObj[0].Contains("decal")) { }    // hesaplanmayacaklar
                        else    // cake ve 3d objler
                        {
                            DataRow obj = dc.MemoryCacheByQuery("select * from Objects where ObjectID = '" + objs[i].ID + "'").Rows[0];                                  // db'den objeyi çek
                            double scale = Convert.ToDouble(currentObj[1].Replace(".", ","));                                                                        // jsondan scale al, nokta yerine virgül lazım double'da

                            if (obj["Type"].ToString() == "cake")
                            {
                                size = getCakeSize(scale).ToString();                                                                                               // cake size çek
                                editorSize = (Convert.ToInt32(editorSize) + Convert.ToInt32(size)).ToString();
                            }
                            else if (obj["Type"].ToString() == "object" | obj["Type"].ToString() == "mixed")
                            {
                                double minSize = Convert.ToDouble(obj["MinSize"].ToString().Replace(".", ","));
                                double maxSize = Convert.ToDouble(obj["MaxSize"].ToString().Replace(".", ","));

                                double sizetemp = (maxSize - minSize) / 3;

                                if (scale <= (minSize + sizetemp)) { size = "k"; }
                                else if (scale >= (minSize + sizetemp) & scale <= (minSize + sizetemp * 2)) { size = "o"; }
                                else if (scale >= (minSize + sizetemp * 2)) { size = "b"; }
                            }
                            else
                            {
                                size = "n";
                            }
                        }
                        increaseObjUsage(objs[i].ID.ToString());
                        objStr = objStr + objs[i].EditorName + ":" + objs[i].ID.ToString() + ":" + objs[i].TexturePath + ":" + size + "~";                      // obj stringini oluştur.
                    }
                }
            }

            objStr = objStr.Substring(0, objStr.Length - 1);    // artan tildayı siliyoz.

            string category = "";
            if (form["designCategory"] != null) { if (String.IsNullOrEmpty(form["designCategory"].ToString()) == false) { category = form["designCategory"]; } }

            if (isDesignExists) // design db kayıt
            {
                if (Session["editorSaveType"] != null)
                {
                    if ((string)Session["editorSaveType"] == "saveas")
                    {
                        dc.DBQuerySetter("insert into MadeCakes (ID,MadeID,UserID,JsonString,ObjString,ImagePath,DateTime,Name,Likes,Price,Type,Mode,SizeOptions,Visible,Category) values ('" + Guid.NewGuid() + "','" + new Guid(designID) + "','" + userID + "','" + form["JsonStr"] + "','" + objStr + "','" + imagePath + "','" + DateTime.Now + "',N'" + designName + "','0','" + price + "','" + editorType + "','" + editorMode + "','" + editorSize + "','" + visible + "',N'" + category + "')");
                    }
                    else { dc.DBQuerySetter("update MadeCakes set UserID = '" + userID + "', JsonString = '" + form["JsonStr"] + "', ObjString = '" + objStr + "',ImagePath = '" + imagePath + "', DateTime = '" + DateTime.Now + "',Price = '" + price + "',Name = N'" + designName + "',Visible = '" + visible + "',SizeOptions = '" + editorSize + "' where MadeID = '" + new Guid(designID) + "'"); }
                }
                else { dc.DBQuerySetter("update MadeCakes set UserID = '" + userID + "', JsonString = '" + form["JsonStr"] + "', ObjString = '" + objStr + "',ImagePath = '" + imagePath + "', DateTime = '" + DateTime.Now + "',Price = '" + price + "',Name = N'" + designName + "',Visible = '" + visible + "',SizeOptions = '" + editorSize + "' where MadeID = '" + new Guid(designID) + "'"); }
            }
            else
            {
                if (category == "Yarışma Pastaları") { visible = "yes"; }
                visible = "yes";
                dc.DBQuerySetter("insert into MadeCakes (ID,MadeID,UserID,JsonString,ObjString,ImagePath,DateTime,Name,Likes,Price,Type,Mode,SizeOptions,Visible,Category) values ('" + Guid.NewGuid() + "','" + new Guid(designID) + "','" + userID + "','" + form["JsonStr"] + "','" + objStr + "','" + imagePath + "','" + DateTime.Now + "',N'" + designName + "','0','" + price + "','" + editorType + "','" + editorMode + "','" + editorSize + "','" + visible + "',N'" + category + "')");
            }

            try
            {
                DataRow design = dc.DBQueryGetter("select * from MadeCakes where MadeID = '" + new Guid(designID) + "'").Rows[0];

                String[] descriptions = new string[] {
                    "" + design["Name"] + " siparişi vererek sevdiklerinize unutamayacağı bir an yaşatın.  Hem kişiye özel pasta yaptırarak sevdiklerinize kendilerini özel hissettirin hem de kutlamanıza lezzet katın.",
                    "" + design["Category"] + " mı aramıştınız. İşte tam size göre bir " + design["Name"] + ". Şimdi bir " + design["Name"] + " siparişi vererek sevdiklerinizi şımartın.",
                    "" + design["Category"] + " için en iyi seçeneklerden birisi karşınızda: " + design["Name"] + ". ",
                    "" + design["Name"] + " siparişi vermek için önce sepete ekleyin ardından pastanenizi seçin ve " + design["Name"] + " istediğiniz tarihte kapınıza gelsin.",
                    "Sizin için değerli olan kişinin özel bir günü ve siz de ona " + design["Category"] + " almayı mı düşünüyorsunuz? " + design["Name"] + " en doğru tercih olacaktır.",
                    "" + design["Category"] + " severler için en güzel sürprizlerden biri " + design["Name"] + "",
                    "" + design["Name"] + " partinizi unutulmaz yapmak için sizi bekliyor",
                    "" + design["Category"] + " arasında kararsız mı kaldınız? Partinin en önemli parçasının aynı zamanda en iyisi olmasını mı istiyorsunuz? " + design["Name"] + " sizi asla hayal kırıklığına uğratmayacak.",
                    "" + design["Category"] + " nı çok beğeniyorsunuz ama en iyisini sipariş etmek mi istiyorsunuz? " + design["Category"] + " nın lideri: " + design["Name"] + "",
                    "" + design["Name"] + " sını çok beğendiniz ama daha fazlasını mı görmek istiyorsunuz? Ziyaret etmeniz gereken adres: " + design["Category"] + "",
                    "Seçim yaparken zorluk mu yaşıyorsunuz? Kafa karışıklıklarına son vermeye geldi: " + design["Name"] + "",
                    "Partiler " + design["Name"] + " olmadan asla gerçek parti olamaz.Hemen sipariş ver " + design["Name"] + " kapına gelsin.",
                    "Diyettesiniz ama küçük bir kaçamaktan zarar gelmez mi diyorsunuz ? " + design["Name"] + " en tatlı suç ortağınız olmak için bekliyor.",
                    "Sevdiğinize belki özel bir sebepten, belki de içinizden geldiği için bir pasta hediye etmek istiyorsunuz ama " + design["Category"] + " kategorisi içindeki seçenekler arasında kayıp mı oldunuz? Doğru tercih için bir ipucu: " + design["Name"] + "",
                    "" + design["Category"] + " sevdiğiniz için doğru seçimse, " + design["Name"] + " kesinlikle doğrunun da doğrusu olacak.",
                    "" + design["Category"] + " arasında en sevilenlerden birisi " + design["Name"] + " hemen sipariş verin kapınıza gelsin",
                    "Bir kutlama varsa " + design["Name"] + " da oradadır.",
                    "Sade görünümlü yaş pastalardan sıkıldınız mı? " + design["Name"] + " göz atmanız için sizi bekliyor",
                    "Zamanınız dar ve  " + design["Category"] + " kategorisi içinde size en uygunu hangisi merak mı ediyorsunuz ? " + design["Name"] + " imdadınıza yetişti.",
                    "Aklınızdaki pasta için " + design["Category"] + " kategorisine baktınız ve en iyinin de en iyisini buldunuz: " + design["Name"] + "",
                    "Çeşitliliği olan bir kategorinin önemli bir üyesi: " + design["Name"] + "",
                    "" + design["Category"] + " severlerin çoğunluğunun tercihi: " + design["Name"] + ""
                };

                Random rnd = new Random();
                int idx = rnd.Next(descriptions.Length);
                string description = descriptions[idx];

                string priceInterval = CalculateDesignPrice(design, "CB5EDC14-85FD-40AD-A79A-82D6CA460CD9");
                dc.DBQuerySetter("update MadeCakes set PriceInterval = '" + priceInterval.Replace(",", ".") + "', Description = N'" + description + "', Rating = '0', Review = 0 where MadeID = '" + new Guid(designID) + "'");
            }
            catch (Exception e) { }

            if (form["designNotes"] != null) { Session["designNotes"] = form["designNotes"].ToString(); }

            if (form["addToCart"].ToString() == "yes")
            {
                Session["editorOutput"] = "yes";
                string bakeryID = null;
                if (Session["cart"] != null)
                {
                    List<DataRow> cart = (List<DataRow>)Session["cart"];

                    if (form["whichEditor"] != null)
                    {
                        AssignBakeryAndContent(form["BakeryID"].ToString(), form["ContentID"].ToString());
                        bakeryID = ((DataRow)Session["orderBakery"])["BakeryID"].ToString();
                    }
                    else
                    {
                        if (String.IsNullOrEmpty(cart[0]["BakeryID"].ToString()) == false)
                        {
                            bakeryID = cart[0]["BakeryID"].ToString();
                        }
                    }
                }
                else
                {
                    if (form["whichEditor"] != null)
                    {
                        AssignBakeryAndContent(form["BakeryID"].ToString(), form["ContentID"].ToString());
                        bakeryID = ((DataRow)Session["orderBakery"])["BakeryID"].ToString();
                    }
                }

                AddDesignToCart(designID, bakeryID);
                return RedirectToAction("Sepetim", "Order");
            }
            else
            {
                //return RedirectToAction("Editor", "Home", new { saved = "yes", designID = designID });
                if (form["designCategory"].ToString() == "Yarışma Pastaları") { return Redirect("/tasarim-pasta/yarisma-pastalari"); }
                else { return Redirect("/tasarim-pasta"); }
            }
        }

        public JsonResult SavePastaResmi()
        {
            if (Request.Files.Count != 0)
            {
                DateTime date = DateTime.Now;
                double seconds = (date - new DateTime(1970, 1, 1)).TotalSeconds;
                String directory_path = Server.MapPath("~/Images/DesignImages/" + seconds.ToString().Split(',')[0]);
                if (Directory.Exists(directory_path) == false) { Directory.CreateDirectory(directory_path); }
                List<string> img_paths = new List<string>();

                for (int i = 0; i < Request.Files.Count; i++)
                {
                    var file = Request.Files[i];
                    string img_path = Server.MapPath("~/Images/DesignImages/" + seconds.ToString().Split(',')[0] + "/requested" + (i + 1) + ".jpg");
                    Image sourceimage = Image.FromStream(file.InputStream);
                    var newImage = ScaleImage(sourceimage, 300, 300);

                    Bitmap bmp = new Bitmap(newImage);
                    var quantizer = new WuQuantizer();
                    using (var bitmap = bmp)
                    {
                        using (var quantized = quantizer.QuantizeImage(bitmap))
                        {
                            quantized.Save(img_path, ImageFormat.Png);
                        }
                    }
                    img_paths.Add("Images/DesignImages/" + seconds.ToString().Split(',')[0] + "/requested" + (i + 1) + ".jpg");
                }

                Session["designImages"] = img_paths;
            }
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public static Image ScaleImage(Image image, int newWidth, int newHeight)
        {
            var newImage = new Bitmap(newWidth, newHeight);

            using (var graphics = Graphics.FromImage(newImage))
                graphics.DrawImage(image, 0, 0, newWidth, newHeight);

            return newImage;
        }

        public void increaseObjUsage(string objID)
        {
            try
            {
                DataRow obj = dc.DBQueryGetter("select * from Objects where ObjectID = '" + new Guid(objID) + "'").Rows[0];
                int newCount = Convert.ToInt32(obj["Usage"]) + 1;
                dc.DBQuerySetter("update Objects set Usage = '" + newCount + "' where ObjectID = '" + new Guid(objID) + "'");
            }
            catch (Exception e) { }
        }

        public void AddDesignToCart(String MadeID, String BakeryID)
        {
            DataTable design = dc.MemoryCacheByQuery("select * from MadeCakes where MadeID = '" + new Guid(MadeID) + "'");
            design.Columns.Add("isMade", typeof(bool)); design.Rows[0]["isMade"] = true;
            design.Columns.Add("Count", typeof(String)); design.Rows[0]["Count"] = "1";
            design.Columns.Add("Select", typeof(String)); design.Rows[0]["Select"] = design.Rows[0]["SizeOptions"].ToString();
            design.Columns.Add("ProductID", typeof(Guid)); design.Rows[0]["ProductID"] = design.Rows[0]["MadeID"].ToString();

            if (!String.IsNullOrEmpty(BakeryID)) { design.Rows[0]["BakeryID"] = new Guid(BakeryID); design.Rows[0]["Price"] = CalculateDesignPrice(design.Rows[0], BakeryID); }

            design.Columns.Add("PagePrice", typeof(String)); design.Rows[0]["PagePrice"] = design.Rows[0]["Price"].ToString().Split(' ')[0];

            List<DataRow> products = new List<DataRow>();
            if (Session["cart"] != null) { products = (List<DataRow>)Session["cart"]; }

            for (int i = 0; i < products.Count; i++)
            {
                if (String.IsNullOrEmpty(products[i]["isMade"].ToString()) == false) { products.RemoveAt(i); break; }
            }

            products.Add(design.Rows[0]);
            Session["cart"] = products;
        }

        public String CalculateDesignPriceJson(String bakeryID, String objStr)
        {
            double baseprice = 0; int editorSize = 0;

            String[] objs = objStr.Split('~');

            for (int i = 0; i < objs.Length; i++)
            {
                if (objs[i].Contains("cake")) { editorSize = editorSize + Convert.ToInt32(objs[i].Split(':')[3]); }
            }

            DataRow bakery = dc.MemoryCacheByQuery("select * from ForeignBakeries where BakeryID = '" + new Guid(bakeryID) + "'").Rows[0];
            String basePrice = "";
            basePrice = bakery["CakePrice"].ToString();
            baseprice = Convert.ToDouble(editorSize) * Convert.ToDouble(basePrice.Replace(".", ","));

            double objPrices = 0;
            for (int i = 0; i < objStr.Split('~').Length; i++)
            {
                string[] objstr = objStr.Split('~')[i].Split(':');

                DataRow objRow = null;

                if (objstr[1].ToString() != "00000000-0000-0000-0000-000000000000")
                {
                    objRow = dc.MemoryCacheByQuery("select * from Objects where ObjectID = '" + new Guid(objstr[1]) + "'").Rows[0];

                    if (objRow["Type"].ToString() == "object" | objRow["Type"].ToString() == "mixed")
                    {
                        string checkSize = objstr[objstr.Length - 1]; string size = "";
                        string[] allowedTypes = new string[] { "b", "o", "k" };

                        if (allowedTypes.Contains(checkSize)) { size = checkSize; } else { size = "o"; }

                        string difficulty = dc.MemoryCacheByQuery("select Difficulty from Objects where ObjectID = '" + new Guid(objstr[1].ToString()) + "'").Rows[0][0].ToString();

                        double dbPrice = 0;
                        dbPrice = Convert.ToDouble(dc.MemoryCacheByQuery("select Price from ObjectPrices where Size = '" + size.ToUpper() + "' and Difficulty = '" + difficulty + "' and BakeryID = '" + new Guid(bakeryID) + "'").Rows[0][0]);

                        if (Convert.ToDouble(editorSize) == 2) { objPrices = objPrices + dbPrice + 5; }
                        else { objPrices = objPrices + dbPrice; }
                    }
                    else if (objRow["Difficulty"].ToString() == "11") {
                        double price = Convert.ToDouble(dc.MemoryCacheByQuery("select Price from ObjectPrices where Difficulty = '11' and BakeryID = '" + new Guid(bakeryID) + "'").Rows[0][0].ToString().Replace(".",","));
                        double dbPrice = dbPrice = editorSize * price + 5;
                        objPrices = objPrices + dbPrice;
                    }
                }
            }

            return Math.Round(baseprice + objPrices, 2).ToString();
        }

        public void PriceIntervalCorrector(string MadeID)
        {
            DataRow design = dc.DBQueryGetter("select * from MadeCakes where MadeID = '" + new Guid(MadeID) + "'").Rows[0];
            String price = CalculateDesignPrice(design, "CB5EDC14-85FD-40AD-A79A-82D6CA460CD9");
            dc.DBQuerySetter("update MadeCakes set PriceInterval = '" + price + "' where MadeID = '" + new Guid(MadeID) + "'");
        }

        public String CalculateDesignPrice(DataRow design, String bakeryID)
        {
            double baseprice = 0; double editorSize = Convert.ToDouble(design["SizeOptions"].ToString().Replace(".", ","));
            DataRow bakery = dc.MemoryCacheByQuery("select * from ForeignBakeries where BakeryID = '" + new Guid(bakeryID) + "'").Rows[0];
            String basePrice = "";
            if (design["Type"].ToString() == "cake") { basePrice = bakery["CakePrice"].ToString(); }
            else if (design["Type"].ToString() == "cupcake") { basePrice = bakery["CupcakePrice"].ToString(); }
            else if (design["Type"].ToString() == "cookie") { basePrice = bakery["CookiePrice"].ToString(); }
            baseprice = editorSize * Convert.ToDouble(basePrice.Replace(".", ","));

            double objPrices = 0;
            for (int i = 0; i < design["ObjString"].ToString().Split('~').Length; i++)
            {
                string[] objstr = design["ObjString"].ToString().Split('~')[i].Split(':');

                DataRow objRow = null;

                if (objstr[1].ToString() != "00000000-0000-0000-0000-000000000000")
                {
                    objRow = dc.MemoryCacheByQuery("select * from Objects where ObjectID = '" + new Guid(objstr[1]) + "'").Rows[0];

                    if (objRow["Type"].ToString() == "object" | objRow["Type"].ToString() == "mixed")
                    {
                        string checkSize = objstr[objstr.Length - 1]; string size = "";
                        string[] allowedTypes = new string[] { "b", "o", "k" };

                        if (allowedTypes.Contains(checkSize)) { size = checkSize; } else { size = "o"; }

                        string difficulty = dc.MemoryCacheByQuery("select Difficulty from Objects where ObjectID = '" + new Guid(objstr[1].ToString()) + "'").Rows[0][0].ToString();
                        double dbPrice = Convert.ToDouble(dc.MemoryCacheByQuery("select Price from ObjectPrices where Size = '" + size.ToUpper() + "' and Difficulty = '" + difficulty + "' and BakeryID = '" + new Guid(bakeryID) + "'").Rows[0][0]);
                        if (Convert.ToDouble(design["SizeOptions"].ToString().Replace(".", ",")) == 2) { objPrices = objPrices + dbPrice + 5; }
                        else { objPrices = objPrices + dbPrice; }
                    }
                    else if (objRow["Difficulty"].ToString() == "11")
                    {
                        double price = Convert.ToDouble(dc.MemoryCacheByQuery("select Price from ObjectPrices where Difficulty = '11' and BakeryID = '" + new Guid(bakeryID) + "'").Rows[0][0].ToString().Replace(".", ","));
                        double dbPrice = dbPrice = editorSize * price + 5;
                        objPrices = objPrices + dbPrice;
                    }
                }
            }

            return Math.Round(baseprice + objPrices, 2).ToString();
        }

        public Image LoadImage(String str)
        {
            byte[] bytes = Convert.FromBase64String(str.Replace("data:image/jpeg;base64,", ""));
            Image image;
            using (MemoryStream ms = new MemoryStream(bytes)) { image = Image.FromStream(ms); }
            return image;
        }

        public JsonResult DontShowTutorial()
        {
            Session["tutorial"] = false;
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        #endregion Design Methods

        #region Object Methods

        public JsonResult AddObject(String ObjectName, int count)
        {
            JsonResult jsn = new JsonResult();
            if (String.IsNullOrEmpty(ObjectName) == false)
            {
                DataTable obj = dc.DBQueryGetter("select * from Objects where ObjectName = '" + ObjectName + "'");
                if (count == 0) { jsn = Json(LoadObjectFromPath(obj), JsonRequestBehavior.AllowGet); }
                else
                {
                    for (int i = 0; i < count; i++)
                    {
                        if (i == 0) { jsn = Json(LoadObjectFromPath(obj), JsonRequestBehavior.AllowGet); }
                        else { Json(LoadObjectFromPath(obj), JsonRequestBehavior.AllowGet); }
                    }
                }
            }
            else
            {
                CakeObject obj = new CakeObject();
                obj.EditorName = NameObject("object");
                obj.ID = new Guid("00000000-0000-0000-0000-000000000000");
                obj.TexturePath = "";
                obj.Type = "object";
                List<CakeObject> objs = new List<CakeObject>(); List<CakeObject> allobjs = new List<CakeObject>();
                if (Session["Objects"] != null) { objs = (List<CakeObject>)Session["Objects"]; }
                objs.Add(obj); Session["Objects"] = objs;
                if (Session["AllObjects"] != null) { allobjs = (List<CakeObject>)Session["AllObjects"]; }
                allobjs.Add(obj); Session["AllObjects"] = allobjs;

                jsn = Json(obj, JsonRequestBehavior.AllowGet);
            }

            jsn.MaxJsonLength = 50000000;
            return jsn;
        }

        public JsonResult GetRandomObject(String type, int count)
        {
            JsonResult jsn = new JsonResult();
            String _type = "";
            if (type == "cake")
            {
                _type = "cake";
            }
            else if (type == "object")
            {
                _type = "object";
            }
            else if (type == "side")
            {
                _type = "side";
            }
            else if (type == "random")
            {
                _type = "random";
            }
            if (String.IsNullOrEmpty(type) == false)
            {
                DataTable obj = dc.DBQueryGetter("select top 1 * from Objects where Type = '" + _type + "' order by newid()");
                if (count == 0) { jsn = Json(LoadObjectFromPath(obj), JsonRequestBehavior.AllowGet); }
                else
                {
                    for (int i = 0; i < count; i++)
                    {
                        if (i == 0) { jsn = Json(LoadObjectFromPath(obj), JsonRequestBehavior.AllowGet); }
                        else { Json(LoadObjectFromPath(obj), JsonRequestBehavior.AllowGet); }
                    }
                }
            }
            else
            {
                CakeObject obj = new CakeObject();
                obj.EditorName = NameObject("object");
                obj.ID = new Guid("00000000-0000-0000-0000-000000000000");
                obj.TexturePath = "";
                obj.Type = "object";
                List<CakeObject> objs = new List<CakeObject>(); List<CakeObject> allobjs = new List<CakeObject>();
                if (Session["Objects"] != null) { objs = (List<CakeObject>)Session["Objects"]; }
                objs.Add(obj); Session["Objects"] = objs;
                if (Session["AllObjects"] != null) { allobjs = (List<CakeObject>)Session["AllObjects"]; }
                allobjs.Add(obj); Session["AllObjects"] = allobjs;

                jsn = Json(obj, JsonRequestBehavior.AllowGet);
            }
            jsn.MaxJsonLength = 50000000;

            return jsn;
        }

        public JsonResult CopyObject(String ObjectName)
        {
            CakeObject newObj = new CakeObject();
            List<CakeObject> objs = (List<CakeObject>)Session["Objects"];
            for (int i = 0; i < objs.Count; i++)
            {
                if (objs[i].EditorName == ObjectName)
                {
                    DataTable obj = dc.MemoryCacheByQuery("select * from Objects where ObjectName = '" + objs[i].Name + "'");
                    newObj = LoadObjectFromPath(obj);
                    break;
                }
            }
            return Json(newObj, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteObject(String ObjectName)
        {
            List<CakeObject> objs = (List<CakeObject>)Session["Objects"];
            CakeObject objToBeDeleted = new CakeObject();
            for (int i = 0; i < objs.Count; i++) { if (objs[i].EditorName == ObjectName) { objToBeDeleted = objs[i]; objs.RemoveAt(i); Session["Objects"] = objs; break; } }

            return Json(objToBeDeleted, JsonRequestBehavior.AllowGet);
        }

        public List<DataTable> GetObjectNames()
        {
            DataTable cakes = dc.MemoryCacheByQuery("select * from Objects where Type = 'Cake' and Approved = 'yes'");
            DataTable objects = dc.MemoryCacheByQuery("select * from Objects where Type = 'Object' and Approved = 'yes'");
            DataTable texts = dc.MemoryCacheByQuery("select * from Objects where Type = 'Text' and Approved = 'yes'");
            DataTable sides = dc.MemoryCacheByQuery("select * from Objects where Type = 'Side' and Approved = 'yes'");
            DataTable random = dc.MemoryCacheByQuery("select * from Objects where Type = 'Random' and Approved = 'yes'");
            DataTable objects2 = dc.MemoryCacheByQuery("select * from Objects where Type = 'object2D' and Approved = 'yes'");
            DataTable mixed = dc.MemoryCacheByQuery("select * from Objects where Type = 'mixed' and Approved = 'yes'");

            DataView cakesView = cakes.DefaultView;
            DataView objectsView = objects.DefaultView;
            DataView textsView = texts.DefaultView;
            DataView sidesView = sides.DefaultView;
            DataView randomView = random.DefaultView;
            DataView objects2View = objects2.DefaultView;
            DataView mixedView = mixed.DefaultView;

            cakesView.Sort = objectsView.Sort = textsView.Sort = sidesView.Sort = randomView.Sort = objects2View.Sort = mixedView.Sort = "Usage desc";
            cakes = cakesView.ToTable(); objects = objectsView.ToTable(); texts = textsView.ToTable(); sides = sidesView.ToTable(); random = randomView.ToTable(); objects2 = objects2View.ToTable(); mixed = mixedView.ToTable();

            List<DataTable> dt_list = new List<DataTable>();
            dt_list.Add(cakes); dt_list.Add(objects); dt_list.Add(texts); dt_list.Add(sides); dt_list.Add(random); dt_list.Add(objects2); dt_list.Add(mixed);

            return dt_list;
        }

        public String NameObject(String ObjectType)
        {
            bool isSide = ObjectType.Contains("side"); bool isRandom = ObjectType.Contains("random"); bool is2D = ObjectType.Contains("2D"); bool isMixed = ObjectType.Contains("mixed");
            String newObjName = "";
            List<CakeObject> objs = new List<CakeObject>();
            List<CakeObject> allobjs = new List<CakeObject>();

            if (isSide | isRandom | is2D | isMixed) { ObjectType = "object"; }

            if (Session["Objects"] != null) { objs = (List<CakeObject>)Session["Objects"]; }
            if (Session["AllObjects"] != null) { allobjs = (List<CakeObject>)Session["AllObjects"]; }

            if (allobjs.Count == 0) { newObjName = ObjectType + "-0"; }
            else
            {
                int lastidx = 0;
                for (int i = 0; i < allobjs.Count; i++) { if (allobjs[i].Type == ObjectType) { string en = allobjs[i].EditorName.Replace("-side", "").Replace("-randart", ""); ; lastidx = Convert.ToInt32(en.Substring(en.IndexOf("-") + 1, en.Length - en.IndexOf("-") - 1)); } }
                newObjName = ObjectType + "-" + (lastidx + 1).ToString();
            }

            if (isSide) { newObjName = newObjName + "-side"; }
            if (isRandom) { newObjName = newObjName + "-randart"; }

            return newObjName;
        }

        public JsonResult ChangeObjectTexture(String ObjName, String source)
        {
            if (source.Contains("/Objects/Textures/")) { source = source.Replace("/Objects/Textures/", ""); }
            List<CakeObject> objs = (List<CakeObject>)Session["objects"];
            for (int i = 0; i < objs.Count; i++)
            {
                if (objs[i].EditorName == ObjName) { objs[i].TexturePath = source; }
            }
            Session["objects"] = objs;
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public string getBakeryIdFromLocation(string location)
        {
            string id = "";
            if (location == "76") { id = "CB5EDC14-85FD-40AD-A79A-82D6CA460CD9"; }
            else if (location == "48") { id = "488BD6DC-555D-4933-A3EA-9F20E44E05D5"; }
            return id;
        }

        public CakeObject LoadObjectFromPath(DataTable table)
        {
            DataRow row = table.Rows[0];
            CakeObject obj = new CakeObject();

            //int idx = 0;
            //string location = (string)Session["editorLocation"];
            //string id = getBakeryIdFromLocation(location);
            //string[] bakeryIds = row["BakeryID"].ToString().Split(' ');
            //for(int i = 0; i < bakeryIds.Length; i++) { if(bakeryIds[i] == id) { idx = i; } }

            obj.ID = new Guid(row["ObjectID"].ToString());
            obj.Name = row["ObjectName"].ToString();
            obj.EditorName = NameObject(row["Type"].ToString());
            obj.Type = row["Type"].ToString();
            if (obj.Type == "side" | obj.Type == "random" | obj.Type == "object2D" | obj.Type == "mixed") { obj.Type = "object"; }
            obj.ObjectPath = row["ObjectPath"].ToString();
            obj.Source = String.Join("\n", System.IO.File.ReadAllLines(HttpRuntime.AppDomainAppPath + "Objects\\Sources\\" + obj.ObjectPath));
            obj.MinSize = row["MinSize"].ToString();
            obj.MaxSize = row["MaxSize"].ToString();
            obj.StepSize = row["StepSize"].ToString();
            obj.NormalScale = Convert.ToDouble(row["NormalScale"]);

            if (String.IsNullOrEmpty(row["ImagePath"].ToString()) == false) { obj.ImagePath = row["ImagePath"].ToString(); } else { obj.ImagePath = ""; }
            if (String.IsNullOrEmpty(row["TexturePath"].ToString()) == false) { obj.TexturePath = row["TexturePath"].ToString(); } else { obj.TexturePath = ""; }
            if (String.IsNullOrEmpty(row["Properties"].ToString()) == false)
            {
                XmlSerializer serializer = new XmlSerializer(typeof(ObjectProperties));
                using (TextReader reader = new StringReader(row["Properties"].ToString()))
                {
                    try
                    {
                        obj.Properties = (ObjectProperties)serializer.Deserialize(reader);
                    }
                    catch (Exception e) { }
                }
            }
            if (String.IsNullOrEmpty(obj.Properties.styles.color) == true) { obj.Properties.styles.color = "FFFFFF"; }
            if (String.IsNullOrEmpty(obj.Properties.styles.coords) == true) { obj.Properties.styles.coords = ""; }

            if (obj.Type.ToString() == "cake")
            {
                if (Session["editorSize"] != null)
                {
                    obj.Properties.styles.scale = getCakeDimension(Convert.ToInt32((string)Session["editorSize"])); obj.CakeSize = (string)Session["editorSize"];
                }
            }

            List<CakeObject> objects = new List<CakeObject>(); List<CakeObject> allobjects = new List<CakeObject>();
            if (Session["Objects"] != null) { objects = (List<CakeObject>)Session["Objects"]; }
            if (Session["AllObjects"] != null) { allobjects = (List<CakeObject>)Session["AllObjects"]; }
            objects.Add(obj); Session["Objects"] = objects;
            allobjects.Add(obj); Session["AllObjects"] = allobjects;
            return obj;
        }

        #endregion Object Methods

        public JsonResult getDesignCategories()
        {
            string type = (string)Session["editorType"];
            List<string> categories = new List<string>();
            DataTable dt = dc.MemoryCacheByQuery("select * from DesignCategories where Type = '" + type + "' and Category <> N'Yarışma Pastaları'");
            for (int i = 0; i < dt.Rows.Count; i++) { categories.Add(dt.Rows[i]["Category"].ToString().Replace("\r\n", "")); }
            return Json(categories, JsonRequestBehavior.AllowGet);
        }

        public int getCakeSize(double scale)
        {       // interval veriyoz çünkü kafasına göre 0.86666667 'yı 0.8667 'ya dönüştürüyor.
            int kackisilik = 0;
            if (scale >= 0.3333 & scale < 0.5333) { kackisilik = 2; }
            else if (scale >= 0.5333 & scale < 0.5833) { kackisilik = 4; }
            else if (scale >= 0.5833 & scale < 0.6333) { kackisilik = 5; }
            else if (scale >= 0.6333 & scale < 0.8) { kackisilik = 6; }
            else if (scale >= 0.8 & scale < 0.8666) { kackisilik = 8; }
            else if (scale >= 0.8666 & scale < 0.9333) { kackisilik = 10; }
            else if (scale >= 0.9333 & scale < 1) { kackisilik = 12; }
            else if (scale >= 1 & scale < 1.0333) { kackisilik = 14; }
            else if (scale >= 1.0333 & scale < 1.0666) { kackisilik = 15; }
            else if (scale >= 1.0666 & scale < 1.1333) { kackisilik = 16; }
            else if (scale >= 1.1333 & scale < 1.2) { kackisilik = 18; }
            else if (scale >= 1.2 & scale < 1.2666) { kackisilik = 20; }
            else if (scale >= 1.2666 & scale < 1.3333) { kackisilik = 22; }
            else if (scale >= 1.3333 & scale < 1.3777) { kackisilik = 24; }
            else if (scale >= 1.3777 & scale < 1.4) { kackisilik = 25; }
            else if (scale >= 1.4 & scale < 1.4666) { kackisilik = 26; }
            else if (scale >= 1.4666 & scale < 1.5333) { kackisilik = 28; }
            else if (scale >= 1.5333 & scale < 1.6) { kackisilik = 30; }
            else if (scale >= 1.6 & scale < 1.6666) { kackisilik = 32; }
            else if (scale >= 1.6666 & scale < 1.7) { kackisilik = 34; }
            else if (scale >= 1.7 & scale < 1.7333) { kackisilik = 35; }
            else if (scale >= 1.7333 & scale < 1.8) { kackisilik = 36; }
            else if (scale >= 1.8 & scale < 1.8666) { kackisilik = 38; }
            else if (scale >= 1.8666 & scale < 1.9333) { kackisilik = 40; }
            else if (scale >= 1.9333 & scale < 2) { kackisilik = 42; }
            else if (scale >= 2 & scale < 2.0333) { kackisilik = 44; }
            else if (scale >= 2.0333 & scale < 2.0666) { kackisilik = 45; }
            else if (scale >= 2.0666 & scale < 2.1333) { kackisilik = 46; }
            else if (scale >= 2.1333 & scale < 2.2) { kackisilik = 48; }
            else if (scale >= 2.2 & scale < 2.2666) { kackisilik = 50; }
            else if (scale >= 2.2666 & scale < 2.3333) { kackisilik = 52; }
            else if (scale >= 2.3333 & scale < 2.3777) { kackisilik = 54; }
            else if (scale >= 2.3777 & scale < 2.4) { kackisilik = 55; }
            else if (scale >= 2.4 & scale < 2.4666) { kackisilik = 56; }
            else if (scale >= 2.4666 & scale < 2.5333) { kackisilik = 58; }
            else if (scale >= 2.5333 & scale < 2.6) { kackisilik = 60; }
            else if (scale >= 2.6) { kackisilik = 62; }

            return kackisilik;
        }

        public decimal getCakeDimension(int kackisilik)
        {
            double newScale = 0;
            if (kackisilik == 2) { newScale = 0.3333; }
            else if (kackisilik == 4) { newScale = 0.5333; }
            else if (kackisilik == 5) { newScale = 0.5833; }
            else if (kackisilik == 6) { newScale = 0.6333; }
            else if (kackisilik == 8) { newScale = 0.8; }
            else if (kackisilik == 10) { newScale = 0.8666; }
            else if (kackisilik == 12) { newScale = 0.9333; }
            else if (kackisilik == 14) { newScale = 1; }
            else if (kackisilik == 15) { newScale = 1.0333; }
            else if (kackisilik == 16) { newScale = 1.0666; }
            else if (kackisilik == 18) { newScale = 1.1333; }
            else if (kackisilik == 20) { newScale = 1.2; }
            else if (kackisilik == 22) { newScale = 1.2666; }
            else if (kackisilik == 24) { newScale = 1.3333; }
            else if (kackisilik == 25) { newScale = 1.3777; }
            else if (kackisilik == 26) { newScale = 1.4; }
            else if (kackisilik == 28) { newScale = 1.4666; }
            else if (kackisilik == 30) { newScale = 1.5333; }
            else if (kackisilik == 32) { newScale = 1.6; }
            else if (kackisilik == 34) { newScale = 1.6666; }
            else if (kackisilik == 35) { newScale = 1.7; }
            else if (kackisilik == 36) { newScale = 1.7333; }
            else if (kackisilik == 38) { newScale = 1.8; }
            else if (kackisilik == 40) { newScale = 1.8666; }
            else if (kackisilik == 42) { newScale = 1.9333; }
            else if (kackisilik == 44) { newScale = 2; }
            else if (kackisilik == 45) { newScale = 2.0333; }
            else if (kackisilik == 46) { newScale = 2.0666; }
            else if (kackisilik == 48) { newScale = 2.1333; }
            else if (kackisilik == 50) { newScale = 2.2; }
            else if (kackisilik == 52) { newScale = 2.2666; }
            else if (kackisilik == 54) { newScale = 2.3333; }
            else if (kackisilik == 55) { newScale = 2.3777; }
            else if (kackisilik == 56) { newScale = 2.4; }
            else if (kackisilik == 58) { newScale = 2.4666; }
            else if (kackisilik == 60) { newScale = 2.5333; }
            else if (kackisilik == 62) { newScale = 2.6; }
            return Convert.ToDecimal(newScale);
        }

        #region Operational Methods

        [ValidateInput(false)]
        public ActionResult AdminAddObj(FormCollection form, HttpPostedFileBase objectFile, HttpPostedFileBase objectImage, HttpPostedFileBase objectTexture, HttpPostedFileBase objectAlpha)
        {
            String approved = "no"; String keywords = ""; String properties = "";
            if (form["admin-object-approve"] != null) { if (form["admin-object-approve"] == "on") { approved = "yes"; } }
            if (form["admin-object-keywords"] != null) { keywords = form["admin-object-keywords"].ToString(); }
            if (form["admin-object-properties"] != null) { if (String.IsNullOrEmpty(form["admin-object-properties"].ToString()) == false) { properties = form["admin-object-properties"].ToString(); } }

            String adminCategories = "";
            if (form["adminCategory1"] != null) { adminCategories = adminCategories + form["adminCategory1"] + ","; }
            if (form["adminCategory2"] != null) { adminCategories = adminCategories + form["adminCategory2"] + ","; }
            if (form["adminCategory3"] != null) { adminCategories = adminCategories + form["adminCategory3"] + ","; }
            if (form["adminCategory4"] != null) { adminCategories = adminCategories + form["adminCategory4"] + ","; }
            if (form["adminCategory5"] != null) { adminCategories = adminCategories + form["adminCategory5"] + ","; }
            if (form["adminCategory6"] != null) { adminCategories = adminCategories + form["adminCategory6"] + ","; }

            if (string.IsNullOrEmpty(adminCategories) == false)
            {
                adminCategories = adminCategories.Substring(0, adminCategories.Length - 1);
            }

            CakeObject co = new CakeObject();
            co.Type = form["admin-object-type"];
            co.Properties = new ObjectProperties(); co.Properties.styles = new styles();
            co.Properties.styles.scale = 1;
            if (String.IsNullOrEmpty(form["admin-object-color"]) == false) { co.Properties.styles.color = form["admin-object-color"]; } else { co.Properties.styles.color = "FFFFFF"; }
            String xml = SerializeObject(co);                                                                                                           // propertyleri update et

            Guid id = Guid.NewGuid(); Guid objectId = Guid.NewGuid();
            var fileName = Path.GetFileName(objectFile.FileName);
            bool alreadyExists = (dc.DBQueryGetter("select * from Objects where ObjectPath = '" + fileName + "'").Rows.Count > 0);                  // obje varmı check et

            if (fileName.ToLower().Contains(".obj") & alreadyExists == false)
            {
                var path = Path.Combine(Server.MapPath("~/Objects/Sources"), fileName);
                objectFile.SaveAs(path);                                                                                                            // obje kaydı

                dc.DBQuerySetter("insert into Objects (ID, ObjectID, ObjectName, Type, ObjectPath, Properties, Approved, Keywords, Currency, Category, MinSize, MaxSize, StepSize, Difficulty, Usage, NormalPath, NormalScale) values ('" + id + "','" + objectId + "','" + fileName.Replace(".obj", "") + "','" + form["admin-object-type"] + "','" + fileName + "','" + xml + "', '" + approved + "', N'" + keywords + "','TL',N'" + adminCategories + "','" + form["admin-object-minsize"].ToString().Replace(',', '.') + "','" + form["admin-object-maxsize"].ToString().Replace(',', '.') + "','" + form["admin-object-stepsize"].ToString().Replace(',', '.') + "','" + form["admin-object-difficulty"] + "','0','','0.25')");
                // db obje kaydı

                if (objectImage != null)
                {
                    var imgFileName = fileName.Replace(".obj", "_image.png");
                    var imgPath = Path.Combine(Server.MapPath("~/Objects/Images"), imgFileName);
                    objectImage.SaveAs(imgPath);                                                                                                    // image kaydı
                    dc.DBQuerySetter("update Objects set ImagePath = '" + imgFileName + "' where ObjectPath = '" + fileName + "'");                 // db image kaydı
                }

                if (objectTexture != null)
                {
                    var textureFileName = fileName.Replace(".obj", "_texture.png");
                    var texturePath = Path.Combine(Server.MapPath("~/Objects/Textures"), textureFileName);
                    objectTexture.SaveAs(texturePath);                                                                                              // texture kaydı
                    dc.DBQuerySetter("update Objects set TexturePath = '" + textureFileName + "' where ObjectPath = '" + fileName + "'");           // db texture kaydı
                }

                if (objectAlpha != null)
                {
                    var alphaFileName = fileName.Replace(".obj", "_texture_alpha.png");
                    var alphaPath = Path.Combine(Server.MapPath("~/Objects/Textures"), alphaFileName);
                    objectAlpha.SaveAs(alphaPath);                                                                                                  // alpha kaydı
                    dc.DBQuerySetter("update Objects set AlphaPath = '" + alphaFileName + "' where ObjectPath = '" + fileName + "'");               // db alpha kaydı
                }

                if (String.IsNullOrEmpty(properties) == false)
                {
                    dc.DBQuerySetter("update Objects set Properties = '" + properties + "' where ObjectID = '" + objectId + "'");
                }
            }

            dc.DisposeCache();

            return RedirectToAction("UserProfile", "Account");
        }

        public ActionResult AdminDeleteObj(String ObjectID)
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

            if (dr["AlphaPath"] != null)
            {
                String path = HttpRuntime.AppDomainAppPath + "Objects\\Textures\\" + dr["AlphaPath"];
                FileInfo imgFile = new FileInfo(path); if (imgFile.Exists) { imgFile.Delete(); }                                                            // texture silme
            }

            dc.DisposeCache();
            return RedirectToAction("UserProfile", "Account");
        }

        [ValidateInput(false)]
        public ActionResult AdminEditObj(String ObjectID, FormCollection form, HttpPostedFileBase objectFile, HttpPostedFileBase objectImage, HttpPostedFileBase objectTexture, HttpPostedFileBase objectAlpha)
        {
            String approved = "no"; String keywords = ""; String price = ""; String properties = "";
            if (form["admin-object-approve"] != null) { if (form["admin-object-approve"] == "on") { approved = "yes"; } }
            if (form["admin-object-keywords"] != null) { keywords = form["admin-object-keywords"].ToString(); }
            if (form["admin-object-price"] != null) { price = form["admin-object-price"].ToString().Replace(" TL", "").Replace(",", "."); }
            if (form["admin-object-properties"] != null) { if (String.IsNullOrEmpty(form["admin-object-properties"].ToString()) == false) { properties = form["admin-object-properties"].ToString(); } }

            String adminCategories = "";
            if (form["adminCategory1"] != null) { adminCategories = adminCategories + form["adminCategory1"] + ","; }
            if (form["adminCategory2"] != null) { adminCategories = adminCategories + form["adminCategory2"] + ","; }
            if (form["adminCategory3"] != null) { adminCategories = adminCategories + form["adminCategory3"] + ","; }
            if (form["adminCategory4"] != null) { adminCategories = adminCategories + form["adminCategory4"] + ","; }
            if (form["adminCategory5"] != null) { adminCategories = adminCategories + form["adminCategory5"] + ","; }
            if (form["adminCategory6"] != null) { adminCategories = adminCategories + form["adminCategory6"] + ","; }
            adminCategories = adminCategories.Substring(0, adminCategories.Length - 1);

            dc.DBQuerySetter("update Objects set Properties = '" + properties + "' where ObjectID = '" + new Guid(ObjectID) + "'");

            DataRow dr = dc.DBQueryGetter("select * from Objects where ObjectID = '" + ObjectID + "'").Rows[0];

            dc.DBQuerySetter("update Objects set Type = '" + form["admin-object-type"] + "',Approved = '" + approved + "',Keywords = N'" + keywords + "',Currency = 'TL',Category = N'" + adminCategories + "',MinSize = '" + form["admin-object-minsize"].ToString().Replace(',', '.') + "',MaxSize = '" + form["admin-object-maxsize"].ToString().Replace(',', '.') + "',StepSize = '" + form["admin-object-stepsize"].ToString().Replace(',', '.') + "',Difficulty = '" + form["admin-object-difficulty"] + "',NormalPath = '' where ObjectID = '" + new Guid(ObjectID) + "'");         // db kategori update

            if (String.IsNullOrEmpty(form["admin-object-color"]) == false)
            {
                CakeObject co = new CakeObject();
                XmlSerializer serializer = new XmlSerializer(typeof(ObjectProperties));
                using (TextReader reader = new StringReader(dr["Properties"].ToString()))
                {
                    co.Properties = (ObjectProperties)serializer.Deserialize(reader);
                }
                co.Type = form["admin-object-category"];
                if (String.IsNullOrEmpty(form["admin-object-color"]) == false) { co.Properties.styles.color = form["admin-object-color"]; } else { co.Properties.styles.color = "FFFFFF"; }
                String xml = SerializeObject(co);
                dc.DBQuerySetter("update Objects set Properties = '" + xml + "' where ObjectID = '" + ObjectID + "'");                         // db properties update
            }

            if (objectFile != null)
            {
                var fileName = Path.GetFileName(dr["ObjectPath"].ToString());
                var path = Path.Combine(Server.MapPath("~/Objects/Sources"), fileName);
                objectFile.SaveAs(path);
                dc.DBQuerySetter("update Objects set ObjectPath = '" + fileName + "' where ObjectID = '" + ObjectID + "'");                    // db obje update
            }

            if (objectImage != null)
            {
                var imgFileName = dr["ImagePath"].ToString();
                var imgPath = Path.Combine(Server.MapPath("~/Objects/Images"), imgFileName);
                objectImage.SaveAs(imgPath);                                                                                                            // db image update
                dc.DBQuerySetter("update Objects set Type = '" + form["admin-object-category"] + "', ImagePath = '" + imgFileName + "' where ObjectID = '" + ObjectID + "'");
            }

            if (objectTexture != null)
            {
                var fileName = dr["TexturePath"].ToString();
                var path = Path.Combine(Server.MapPath("~/Objects/Textures"), fileName);
                objectTexture.SaveAs(path);
                dc.DBQuerySetter("update Objects set TexturePath = '" + fileName + "' where ObjectID = '" + ObjectID + "'");                   // db texture update
            }

            if (objectAlpha != null)
            {
                var fileName = dr["AlphaPath"].ToString();
                var path = Path.Combine(Server.MapPath("~/Objects/Textures"), fileName);
                objectAlpha.SaveAs(path);
                dc.DBQuerySetter("update Objects set AlphaPath = '" + fileName + "' where ObjectID = '" + ObjectID + "'");                   // db alpha update
            }
            dc.DisposeCache();
            return RedirectToAction("UserProfile", "Account");
        }

        public string SerializeObject(CakeObject obj)
        {
            String result = "";
            XmlSerializer xsSubmit = new XmlSerializer(typeof(ObjectProperties));
            using (StringWriter sww = new StringWriter())
            using (XmlWriter writer = XmlWriter.Create(sww))
            {
                xsSubmit.Serialize(writer, obj.Properties);
                result = sww.ToString();
            }
            return result;
        }

        public JsonResult EditorBakeryAppend(string ProvinceCode)
        {
            if (ProvinceCode == "null")
            {
                ProvinceCode = (string)Session["city"];
            }
            DataTable dt_bakeries = dc.MemoryCacheByQuery("select * from ForeignBakeries where Province like '%" + ProvinceCode + "%'");
            return Json(uc.SerializeDatatable(dt_bakeries), JsonRequestBehavior.AllowGet);
        }

        public JsonResult EditorContentAppend(string BakeryID)
        {
            DataTable dt_contents = dc.MemoryCacheByQuery("select * from BakeryContents where BakeryID = '" + new Guid(BakeryID) + "' and BakeryID <> '98E94E0A-C764-4F99-A2F1-F4915631A891' and Type = 'cake'");
            return Json(uc.SerializeDatatable(dt_contents), JsonRequestBehavior.AllowGet);
        }

        public void AssignBakeryAndContent(string BakeryID, string ContentID)
        {
            DataRow bakery = dc.MemoryCacheByQuery("select * from ForeignBakeries where BakeryID = '" + new Guid(BakeryID) + "'").Rows[0];
            DataRow content = dc.MemoryCacheByQuery("select * from BakeryContents where ID = '" + new Guid(ContentID) + "'").Rows[0];

            Session["orderBakery"] = bakery;
            Session["orderContent"] = content;
            Session["contentRequired"] = false;
            Session["bakeryRequired"] = false;

            Session["DontTouchBakeryAndContent"] = true;
        }

        #endregion Operational Methods
    }
}