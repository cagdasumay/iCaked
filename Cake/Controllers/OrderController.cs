using Cake.Models;
using PdfSharp;
using PdfSharp.Pdf;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using TheArtOfDev.HtmlRenderer.PdfSharp;

namespace Cake.Controllers
{
    internal static class DataRowExtensions
    {
        public static object GetValue(this DataRow row, string column)
        {
            return row.Table.Columns.Contains(column) ? row[column] : null;
        }
    }

    public class OrderController : Controller
    {
        private DatabaseController dc = new DatabaseController();
        private UtilityController uc = new UtilityController();

        public object ClockInfoFromSystem { get; private set; }

        #region Constructor Methods

        public ActionResult Ilan(string redirect)
        {
            DataRow redirectRow = dc.DBQueryGetterGP("select * from Redirect where ID = '" + new Guid(redirect) + "'").Rows[0];

            Session["ilanExists"] = redirectRow["IlanID"].ToString();
            DataRow ilan = dc.DBQueryGetterGP("select * from Ilan where IlanID like '%" + redirectRow["IlanID"].ToString() + "%'").Rows[0];
            String MadeID = ilan["CakeLink"].ToString().Substring(ilan["CakeLink"].ToString().Length - 8, 8);
            DataRow cake = dc.DBQueryGetter("select * from MadeCakes where MadeID like '%" + MadeID + "%'").Rows[0];

            Session["cart"] = null;
            AddDesignToCart(cake["MadeID"].ToString(), ilan["BakeryID"].ToString(), "");
            Session["orderAddress"] = dc.DBQueryGetter("select * from AnonAddresses where ID like '%" + redirectRow["AddressID"].ToString() + "%'").Rows[0];
            Session["orderContent"] = dc.DBQueryGetter("select * from BakeryContents where ID = '" + new Guid(ilan["ContentID"].ToString()) + "'").Rows[0];
            Session["orderBakery"] = dc.DBQueryGetter("select * from ForeignBakeries where BakeryID = '" + new Guid(ilan["BakeryID"].ToString()) + "'").Rows[0];

            FormCollection form = new FormCollection();
            form["deliveryRadio"] = "on";
            DateTime dt = DateTime.Now.AddDays(1);
            form["deliveryDatePicker"] = dt.ToString("dd.MM.yyyy").Replace(".", "/");
            form["deliveryTimePicker"] = "13:00 - 14:00 ";
            form["deliveryTimePickerClean"] = "10:00";
            form["orderNotes"] = "";

            form["contactName"] = redirectRow["ContactName"].ToString();
            form["contactSurname"] = redirectRow["ContactSurname"].ToString();
            form["contactMail"] = redirectRow["ContactMail"].ToString();
            form["contactTel"] = redirectRow["ContactTel"].ToString();

            Session["orderDeliveryInfo"] = form;
            return RedirectToAction("Siparis", "Order");
        }

        public ActionResult Sepetim()
        {
            if (Session["cart"] != null)
            {
                ArrangeCart(); ViewData["empty"] = false;
            }
            else { ViewData["empty"] = true; }

            Session["anonAddress"] = null;
            Session["userAddresses"] = null;
            Session["orderAddress"] = null;
            Session["bakeryDeliver"] = null;
            Session["orderPayment"] = null;
            Session["subeTeslim"] = null;
            Session["pdfID"] = null;
            Session["promotion"] = null;
            Session["PacketWarning"] = null;
            Session["PacketWarningAddress"] = null;
            Session["PartyExists"] = null;
            Session["orderStep"] = "1";

            if (Session["DontTouchBakeryAndContent"] == null)
            {
                Session["orderBakery"] = null;
                Session["orderContent"] = null;
                Session["bakeryRequired"] = null;
                Session["contentRequired"] = null;
            }

            return RedirectToAction("Siparis");
        }

        public ActionResult Pastaneler(string filter, string sehir_pastaneler, string ilce_pastaneler, string semt_pastaneler)
        {
            PastanelerHelper(sehir_pastaneler, ilce_pastaneler, semt_pastaneler);
            return View();
        }

        public ActionResult BakerySelect(String profileId)
        {
            Session["OrderBakery"] = dc.DBQueryGetter("select * from ForeignBakeries where BakeryID = '" + new Guid(profileId) + "'");
            return RedirectToAction("Siparis", new { step = "2" });
        }

        public ActionResult SetDeliveryInfo(FormCollection form)
        {
            Session["orderDeliveryInfo"] = form;
            return RedirectToAction("Siparis");
        }

        public ActionResult FromStep1ToStep2()
        {
            Session["orderStep"] = "2";
            return RedirectToAction("Siparis");
        }

        public ActionResult Siparis()
        {
            List<DataRow> cart = (List<DataRow>)Session["cart"];
            ViewData["orderStarted"] = true;

            if (Session["cart"] != null) { ArrangeCart(); ViewData["empty"] = false; }
            else { ViewData["empty"] = true; }

            bool warnPacket = false; bool warnPacketAddress = false;
            if (Session["PacketWarning"] != null)
            { if ((bool)Session["PacketWarning"] == true) { warnPacket = true; } }

            if (Session["PacketWarningAddress"] != null)
            { if ((bool)Session["PacketWarningAddress"] == true) { warnPacketAddress = true; } }

            String orderStep = (string)Session["orderStep"];

            if (orderStep == "1")
            {
                checkBakery();
                checkContent();
            }
            else
            {
                if (Session["orderAddress"] == null | warnPacketAddress)
                {
                    checkBakery();
                    checkContent();
                    Session["orderStep"] = "2";
                }
                else
                {
                    if (Session["orderBakery"] == null & Session["subeTeslim"] == null)
                    {
                        DataRow address = (DataRow)Session["orderAddress"];
                        PastanelerHelper(address["Province"].ToString(), address["District"].ToString(), address["Neighborhood"].ToString());
                        Session["orderStep"] = "3";
                    }
                    else if ((Session["orderBakery"] == null & Session["subeTeslim"] != null) | warnPacket)
                    {
                        PastanelerHelper(null, null, null);
                        Session["orderStep"] = "3";
                    }
                    else
                    {
                        bool designExists = false;
                        for (int i = 0; i < cart.Count; i++) { if (String.IsNullOrEmpty(cart[i]["isMade"].ToString()) == false) { designExists = true; } }

                        if (designExists)
                        {
                            if (Session["orderContent"] == null)
                            {
                                Session["orderStep"] = "4";
                            }
                            else
                            {
                                if(Session["orderDeliveryInfo"] == null) { Session["orderStep"] = "5"; }
                                else { Session["orderStep"] = "6"; }
                            }
                        }
                        else
                        {
                            if (Session["contentRequired"] != null)
                            {
                                bool contentRequired = (bool)Session["contentRequired"];
                                if (contentRequired) { Session["orderStep"] = "4"; }
                                else
                                {
                                    if (Session["orderDeliveryInfo"] == null) { Session["orderStep"] = "5"; }
                                    else { Session["orderStep"] = "6"; }
                                }
                            }
                            else { Session["orderStep"] = "4"; }
                        }
                    }
                }
            }

            if (Session["contentRequired"] == null & Session["bakeryRequired"] == null)
            {
                bool contentRequired = false; bool bakeryRequired = true;
                for (int i = 0; i < cart.Count; i++)
                {
                    if (String.IsNullOrEmpty(cart[i]["isMade"].ToString()) == false) { contentRequired = true; }
                    if (String.IsNullOrEmpty(cart[i]["BakeryID"].ToString()) == false & cart[i]["Category"].ToString() != "Parti Malzemeleri") { bakeryRequired = false; }
                    try
                    {
                        if (cart[i]["designProduct"] != null)
                        {
                            if (cart[i]["designProduct"].ToString() == "yes") { contentRequired = true; }
                            else { contentRequired = false; }
                        }
                    }
                    catch (Exception e) { contentRequired = false; }
                }
                Session["contentRequired"] = contentRequired; Session["bakeryRequired"] = bakeryRequired;
            }

            return View();
        }

        public void checkBakery()
        {
            List<DataRow> cart = (List<DataRow>)Session["cart"]; bool bakeryFound = false; bool isOnlyParty = true;
            for (int i = 0; i < cart.Count; i++)
            {
                if (!String.IsNullOrEmpty(cart[i]["BakeryID"].ToString()) & cart[i]["Category"].ToString() != "Parti Malzemeleri")
                {
                    Session["orderBakery"] = dc.DBQueryGetter("select * from ForeignBakeries where BakeryID='" + new Guid(cart[i]["BakeryID"].ToString()) + "'").Rows[0];
                    bakeryFound = true;
                }
                if (cart[i]["Category"].ToString() != "Parti Malzemeleri") { isOnlyParty = false; }
            }

            if (isOnlyParty) { Session["orderBakery"] = dc.DBQueryGetter("select * from ForeignBakeries where BakeryID = '98E94E0A-C764-4F99-A2F1-F4915631A891'").Rows[0]; bakeryFound = true; }

            if (bakeryFound == false) { Session["bakeryRequired"] = true; } else { Session["bakeryRequired"] = false; }
        }

        public void checkContent()
        {
            bool contentRequired = false;
            List<DataRow> cart = (List<DataRow>)Session["cart"];
            for (int i = 0; i < cart.Count; i++)
            {
                object isMade = DataRowExtensions.GetValue(cart[i], "MadeID");
                object isDesignProduct = DataRowExtensions.GetValue(cart[i], "DesignProduct");

                if (isMade != null | isDesignProduct != null)
                {
                    if (isDesignProduct != null)
                    {
                        if (isDesignProduct.ToString() == "yes") { contentRequired = true; }
                    }
                    else
                    {
                        if (isMade != null) { contentRequired = true; }
                    }
                }
            }
            Session["contentRequired"] = contentRequired;
        }

        public ActionResult BackToAddress()
        {
            Session["orderAddress"] = null;

            if (Session["orderContent"] != null) { Session["contentRequired"] = true; }

            if (Session["editorOutput"] == null)
            {
                Session["orderBakery"] = null; Session["orderContent"] = null;
            }

            Session["orderPayment"] = null; Session["bakeryDeliver"] = null;

            List<DataRow> cart = (List<DataRow>)Session["cart"];

            bool notMadeExists = false;
            for (int i = 0; i < cart.Count; i++)
            {
                if (String.IsNullOrEmpty(cart[i]["isMade"].ToString())) { notMadeExists = true; }
            }

            if (!notMadeExists)
            {
                if (Session["editorOutput"] == null)
                {
                    for (int i = 0; i < cart.Count; i++) { cart[i]["PagePrice"] = "0.00"; cart[i]["BakeryID"] = DBNull.Value; }
                }
            }

            return RedirectToAction("Siparis");
        }

        public ActionResult BackToBakery()
        {
            Session["orderBakery"] = null; Session["orderContent"] = null; Session["orderPayment"] = null;
            return RedirectToAction("Siparis");
        }

        public ActionResult BackToContent()
        {
            Session["orderContent"] = null; Session["contentRequired"] = true; Session["orderPayment"] = null;
            return RedirectToAction("Siparis");
        }

        public ActionResult BackToInfo()
        {
            Session["orderDeliveryInfo"] = null;
            return RedirectToAction("Siparis");
        }

        public double CalculateBakeryTotal()
        {
            List<DataRow> cart = (List<DataRow>)Session["cart"];

            Double total = 0;
            for (int i = 0; i < cart.Count; i++)
            {
                if (cart[i]["PagePrice"] != null)
                {
                    if (String.IsNullOrEmpty(cart[i]["PagePrice"].ToString()) == false)
                    {
                        total = total + Convert.ToDouble(cart[i]["PagePrice"].ToString().Replace(".", ","));
                    }
                }
            }

            return total;
        }

        public double CalculateCartTotal()
        {
            List<DataRow> cart = (List<DataRow>)Session["cart"];

            Double total = 0;
            for (int i = 0; i < cart.Count; i++)
            {
                if (cart[i]["PagePrice"] != null)
                {
                    if (String.IsNullOrEmpty(cart[i]["PagePrice"].ToString()) == false)
                    {
                        total = total + Convert.ToDouble(cart[i]["PagePrice"].ToString().Replace(".", ","));
                    }
                }
            }

            return total;
        }

        [HttpPost]
        public ActionResult AnonAddress(FormCollection form)
        {
            DataRow address = dc.DBQueryGetter("select top 1 * from UserAddresses").Rows[0];
            Guid anonID = Guid.NewGuid();
            address["ID"] = anonID;
            address["Province"] = form["anonProvinceAddress"];
            address["ProvinceName"] = dc.getLocationNameFromID(form["anonProvinceAddress"], "Province");
            address["District"] = form["anonDistrictAddress"];
            address["DistrictName"] = dc.getLocationNameFromID(form["anonDistrictAddress"], "District");
            address["Neighborhood"] = form["anonNeighborhoodAddress"];
            address["NeighborhoodName"] = dc.getLocationNameFromID(form["anonNeighborhoodAddress"], "Neighborhood");
            address["TownName"] = dc.getLocationNameFromID(form["anonNeighborhoodAddress"], "Town");
            address["SpecificAddress"] = form["anonOpenAddress"];
            address["Description"] = form["anonAddressDescription"];

            dc.DBQuerySetter("insert into AnonAddresses (ID,Province,ProvinceName,District,DistrictName,Neighborhood,NeighborhoodName,TownName,SpecificAddress,Description) values ('" + anonID + "','" + address["Province"] + "','" + address["ProvinceName"] + "','" + address["District"] + "','" + address["DistrictName"] + "','" + address["Neighborhood"] + "','" + address["NeighborhoodName"] + "','" + address["TownName"] + "','" + address["SpecificAddress"] + "','" + address["Description"] + "')");

            if (Session["userinfo"] != null) { dc.DBQuerySetter("update AnonAddresses set UserID = '" + ((DataRow)Session["userinfo"])["UserID"] + "' where ID = '" + anonID + "'"); }

            if (form["registerAnonAddress"] != null & Session["userinfo"] != null)
            {
                if (form["registerAnonAddress"] == "on")
                {
                    dc.DBQuerySetter("insert into UserAddresses (ID,UserID,Province,ProvinceName,District,DistrictName,Neighborhood,NeighborhoodName,TownName,SpecificAddress,Description) values ('" + Guid.NewGuid() + "','" + ((DataRow)Session["userinfo"])["UserID"] + "','" + address["Province"] + "','" + address["ProvinceName"] + "','" + address["District"] + "','" + address["DistrictName"] + "','" + address["Neighborhood"] + "','" + address["NeighborhoodName"] + "','" + address["TownName"] + "','" + address["SpecificAddress"] + "','" + address["Description"] + "')");
                }
            }

            string idx = "0";

            if (Session["userAddresses"] == null)
            {
                DataTable anonTable = dc.DBQueryGetter("select * from AnonAddresses where ID = '" + anonID + "'");
                Session["userAddresses"] = anonTable;
                idx = "0";
            }
            else
            {
                DataTable dt_addresses = (DataTable)Session["userAddresses"];
                dt_addresses.Rows.Add(address.ItemArray);
                Session["userAddresses"] = dt_addresses;
                idx = (dt_addresses.Rows.Count - 1).ToString();
            }

            return RedirectToAction("SelectAddress", new { idx = idx });
        }

        public ActionResult SelectAddress(String idx)
        {
            if (idx != "subeTeslim")
            {
                DataTable dt_address = (DataTable)Session["userAddresses"];

                DataRow address = null;
                address = dt_address.Rows[Convert.ToInt32(idx)];

                if (Session["orderBakery"] == null)
                {
                    Session["orderAddress"] = address;
                    Session["bakeryDeliver"] = true;
                }
                else
                {
                    DataRow bakery = dc.DBQueryGetter("select * from ForeignBakeries where BakeryID = '" + ((DataRow)Session["orderBakery"])["BakeryID"] + "'").Rows[0];
                    bool isOk = false;
                    if (bakery["Province"].ToString() == address["Province"].ToString()) { isOk = true; Session["orderAddress"] = address; }
                    Session["bakeryDeliver"] = isOk;

                    if (Session["orderAddress"] != null)
                    {
                        if (Session["orderAddress"].GetType().Name != "String")
                        {
                            DataTable dt_network = dc.DBQueryGetter("select * from BakeryNetwork where Province = '" + address["Province"] + "' and District = '" + address["District"] + "' and Town = '" + address["Neighborhood"] + "'");
                            Double total = CalculateBakeryTotal();
                            if (Session["orderBakery"] != null)
                            {
                                if (total < Convert.ToDouble(dt_network.Rows[0]["MinPacket"])) { Session["PacketWarningAddress"] = true; } else { Session["PacketWarningAddress"] = false; }
                            }
                        }
                    }
                }
                Session["subeTeslim"] = null;
            }
            else
            {
                Session["orderAddress"] = "subeTeslim";
                Session["subeTeslim"] = true;
            }

            return RedirectToAction("Siparis");
        }

        public ActionResult SelectBakery(String id)
        {
            Session["orderBakery"] = dc.DBQueryGetter("select * from ForeignBakeries where BakeryID = '" + new Guid(id) + "'").Rows[0];
            List<DataRow> cart = (List<DataRow>)Session["cart"];

            Double total = 0;
            for (int i = 0; i < cart.Count; i++)
            {
                if (String.IsNullOrEmpty(cart[i]["BakeryID"].ToString())) { cart[i]["BakeryID"] = new Guid(id); }
                if (String.IsNullOrEmpty(cart[i]["isMade"].ToString()) == false)
                {
                    cart[i]["Price"] = CalculateDesignPrice(cart[i], id);
                    cart[i]["PagePrice"] = cart[i]["Price"];
                }
                if (cart[i]["PagePrice"] != null)
                {
                    if (String.IsNullOrEmpty(cart[i]["PagePrice"].ToString()) == false)
                    {
                        total = total + Convert.ToDouble(cart[i]["PagePrice"].ToString().Replace(".", ","));
                    }
                }
            }

            if (Session["orderAddress"].GetType().Name != "String")
            {
                DataRow address = (DataRow)Session["orderAddress"];
                DataTable dt_network = dc.DBQueryGetter("select * from BakeryNetwork where Province = '" + address["Province"] + "' and District = '" + address["District"] + "' and Town = '" + address["Neighborhood"] + "'");

                if (total < Convert.ToDouble(dt_network.Rows[0]["MinPacket"])) { Session["PacketWarning"] = true; } else { Session["PacketWarning"] = false; }
            }

            return RedirectToAction("Siparis");
        }

        public JsonResult SelectPayment(String type)
        {
            if (type == "1") { Session["orderPayment"] = "POS"; }
            else if (type == "2") { Session["orderPayment"] = "Şubede Ödeme"; }
            else if (type == "3") { Session["orderPayment"] = "Kapıda Nakit Ödeme"; }
            else if (type == "4") { Session["orderPayment"] = "Kapıda Kredi Kartı / Banka Kartı ile Ödeme"; }
            else if (type == "5") { Session["orderPayment"] = "Kapıda Ticket Restaurant Yemek Kartı ile Ödeme"; }
            else if (type == "6") { Session["orderPayment"] = "Kapıda Ticket Restaurant Yemek Çeki  ile Ödeme"; }
            else if (type == "7") { Session["orderPayment"] = "Kapıda Sodexo Yemek Kartı ile Ödeme"; }
            else if (type == "8") { Session["orderPayment"] = "Kapıda Sodexo Yemek Çeki ile Ödeme"; }
            else if (type == "9") { Session["orderPayment"] = "Kapıda Multinet ile Ödeme"; }
            else if (type == "10") { Session["orderPayment"] = "Kapıda SetCard ile Ödeme"; }

            return Json(true, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult SelectContent(FormCollection form)
        {
            Session["orderContent"] = dc.DBQueryGetter("select * from BakeryContents where ID = '" + new Guid(form["designContent"]) + "'").Rows[0]; Session["contentRequired"] = false;
            return RedirectToAction("Siparis");
        }

        public ActionResult PartiSepetleri(String filter)
        {
            String dt = DateTime.Now.ToString();
            DataTable product_table = dc.DBQueryGetter("select * from Products where Category = 'Parti Malzemeleri' and Approved = 'yes'");
            ViewData["categories"] = arrangeCategories(product_table);

            if (String.IsNullOrEmpty(filter) == false) { ViewData["market"] = dc.sortTable(product_table, filter); }
            else { ViewData["market"] = product_table; }

            return View();
        }

        #endregion Constructor Methods

        #region CartMethods

        public string CalculateCartTotal(List<DataRow> cart)
        {
            Double total = 0;
            if (cart != null)
            {
                for (int i = 0; i < cart.Count; i++)
                {
                    if (cart[i]["PagePrice"] != null)
                    {
                        if (String.IsNullOrEmpty(cart[i]["PagePrice"].ToString()) == false)
                        {
                            total = total + Convert.ToDouble(cart[i]["PagePrice"].ToString().Replace(".", ","));
                        }
                    }
                }
            }

            return total.ToString();
        }

        public JsonResult MakeDiscount(string promotionCode)
        {
            Promotion prom = new Promotion();
            List<DataRow> cart = (List<DataRow>)Session["cart"];
            string total = CalculateCartTotal(cart);
            Double totalDouble = Convert.ToDouble(total.Replace(".", ","));

            DataTable dt_promotion = dc.DBQueryGetter("select * from Promotions where PromotionID like '%" + promotionCode + "%' or Name like '%" + promotionCode + "%' and isActive = 'yes' and Count <> '0'");

            if (dt_promotion.Rows.Count > 0)
            {
                prom.isOk = true;
                DataRow row_prom = dt_promotion.Rows[0];

                if (String.IsNullOrEmpty(row_prom["DiscountPercent"].ToString()) == true)
                {
                    if (totalDouble < Convert.ToDouble(row_prom["DiscountTL"].ToString().Replace(".", ",")))   // promosyon fiyattan fazla ise
                    {
                        prom.indirimTutari = total;
                        total = "0.00";
                    }
                    else
                    {
                        total = (totalDouble - Convert.ToDouble(row_prom["DiscountTL"].ToString().Replace(".", ","))).ToString();
                        prom.indirimTutari = Convert.ToDouble(row_prom["DiscountTL"].ToString().Replace(".", ",")).ToString();
                    }
                    prom.newCartPrice = total;
                }
                else if (String.IsNullOrEmpty(row_prom["DiscountPercent"].ToString()) == false)
                {
                    totalDouble = totalDouble - totalDouble * Convert.ToDouble(row_prom["DiscountPercent"].ToString().Replace(".", ",")) / 100;
                    prom.newCartPrice = totalDouble.ToString();
                    prom.indirimTutari = (Convert.ToDouble(total.Replace(".", ",")) - totalDouble).ToString();
                }

                string append = @"
                                  <div style=""width:100%; float:left;"">
                                    <label style=""float:right;  width:100%; font-size:12px; "">İndirim</label>
                                    <label style=""float:right; width:100%; font-size:30px;  font-family:'Open Sans';"">" + prom.indirimTutari + @"<span style=""font-size:16px; float:right; padding-top:15px; padding-left:5px;"">TL</span></label>
                                  </div>
                                    <div style=""width:100%; float:left;"">
                                    <label style=""float:right;  width:100%; font-size:12px; "">Toplam</label>
                                    <label style=""float:right; width:100%; font-size:30px;  font-family:'Open Sans';"">" + prom.newCartPrice + @"<span style=""font-size:16px; float:right; padding-top:15px; padding-left:5px;"">TL</span></label>
                                  </div>";

                prom.append = append;
                prom.code = new Guid(row_prom["PromotionID"].ToString());
                prom.newCount = Convert.ToInt32(row_prom["Count"]);
                Session["promotion"] = prom;
            }
            else { prom.isOk = false; }

            return Json(prom, JsonRequestBehavior.AllowGet);
        }

        public JsonResult AddProductChecker(Guid ProductID, bool addAnyway)
        {
            String result = "ok";
            DataRow product = dc.DBQueryGetter("select * from Products where ProductID = '" + ProductID + "'").Rows[0];
            bool isParty = product["Category"].ToString() == "Parti Malzemeleri";

            if (Session["cart"] == null) { AddProductDirectly(ProductID); }     // ürün ve dizayn yok
            else
            {                                                                   // ürün veya dizayn var
                List<DataRow> products = (List<DataRow>)Session["cart"];
                bool designExists = false; bool partyOnly = true;
                for (int i = 0; i < products.Count; i++)
                {
                    if (String.IsNullOrEmpty(products[i]["isMade"].ToString()) == false) { designExists = true; break; }
                    if (products[i]["BakeryID"].ToString().ToLower() != "98e94e0a-c764-4f99-a2f1-f4915631a891") { partyOnly = false; }
                }

                if (designExists)
                {                                                                // dizayn var
                    if (products.Count == 1)                                     // sadece dizayn var
                    {
                        if (products[0]["Category"].ToString() == "Parti Malzemeleri" | isParty) { AddProductDirectly(ProductID); }
                        else
                        {
                            if (addAnyway == true)
                            {
                                products[0]["BakeryID"] = product["BakeryID"];
                                AddProductDirectly(ProductID);
                                products[0]["PagePrice"] = products[0]["Price"] = CalculateDesignPrice(dc.DBQueryGetter("select * from MadeCakes where MadeID = '" + products[0]["MadeID"] + "'").Rows[0], product["BakeryID"].ToString());
                            }
                            else
                            {
                                result = "productDesignConflict";
                            }
                        }
                    }
                    else                                                        // ürün ve dizayn var
                    {
                        if (products[0]["Category"].ToString() == "Parti Malzemeleri" | isParty) { AddProductDirectly(ProductID); }
                        else
                        {
                            if (addAnyway == true) { Session["cart"] = null; AddProductDirectly(ProductID); }
                            else
                            {
                                bool bakeryConflict = false;
                                for (int i = 0; i < products.Count; i++) { if (products[i]["BakeryID"].ToString().ToLower() != product["BakeryID"].ToString().ToLower()) { bakeryConflict = true; break; } }
                                if (bakeryConflict)
                                {
                                    if (partyOnly == true) { AddProductDirectly(ProductID); }
                                    else { result = "productsConflict"; }
                                }
                                else { AddProductDirectly(ProductID); }
                            }
                        }
                    }
                }
                else
                {                                                               // ürün var dizayn yok
                    if (products[0]["Category"].ToString() == "Parti Malzemeleri" | isParty) { AddProductDirectly(ProductID); }
                    else
                    {
                        if (addAnyway == true) { Session["cart"] = null; AddProductDirectly(ProductID); }
                        else
                        {
                            bool bakeryConflict = false;
                            for (int i = 0; i < products.Count; i++) { if (products[i]["BakeryID"].ToString().ToLower() != product["BakeryID"].ToString().ToLower()) { bakeryConflict = true; break; } }
                            if (bakeryConflict)
                            {
                                if (partyOnly == true) { AddProductDirectly(ProductID); }
                                else { result = "productsConflict"; }
                            }
                            else { AddProductDirectly(ProductID); }
                        }
                    }
                }
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public void AddProductDirectly(Guid ProductID)
        {
            List<DataRow> products = new List<DataRow>();

            DataTable dt = new DataTable();
            DataRow product = null;

            if (Session["currentProduct"] == null)
            {
                dt = dc.DBQueryGetter("select * from Products where ProductID = '" + ProductID + "'");
                dt.Columns.Add("PagePrice", typeof(String)); dt.Rows[0]["PagePrice"] = dt.Rows[0]["Price"].ToString().Split(' ')[0];
                dt.Columns.Add("Count", typeof(String)); dt.Rows[0]["Count"] = "1";
                dt.Columns.Add("Select", typeof(String));

                product = dt.Rows[0];

                if (product["Category"].ToString().ToLower().Contains("Pasta") | product["Category"].ToString() == "Parti Malzemeleri") { product["Select"] = product["SizeOptions"].ToString().Split(' ')[0]; }
                else if (product["Category"].ToString() == "Çikolata" | product["Category"].ToString() == "Tatlı") { product["Select"] = product["Gram"].ToString().Split(' ')[0]; }
                else if (product["Category"].ToString() == "Kek/Kurabiye" | product["Category"].ToString() == "Cupcake") { product["Select"] = product["Number"].ToString().Split(' ')[0]; }
            }
            else {
                product = (DataRow)Session["currentProduct"];
                if(product["ProductID"].ToString().ToLower() != ProductID.ToString().ToLower())
                {
                    product = null;
                    dt = dc.DBQueryGetter("select * from Products where ProductID = '" + ProductID + "'");
                    dt.Columns.Add("PagePrice", typeof(String)); dt.Rows[0]["PagePrice"] = dt.Rows[0]["Price"].ToString().Split(' ')[0];
                    dt.Columns.Add("Count", typeof(String)); dt.Rows[0]["Count"] = "1";
                    dt.Columns.Add("Select", typeof(String));

                    product = dt.Rows[0];

                    if (product["Category"].ToString().ToLower().Contains("Pasta") | product["Category"].ToString() == "Parti Malzemeleri") { product["Select"] = product["SizeOptions"].ToString().Split(' ')[0]; }
                    else if (product["Category"].ToString() == "Çikolata" | product["Category"].ToString() == "Tatlı") { product["Select"] = product["Gram"].ToString().Split(' ')[0]; }
                    else if (product["Category"].ToString() == "Kek/Kurabiye" | product["Category"].ToString() == "Cupcake") { product["Select"] = product["Number"].ToString().Split(' ')[0]; }
                }
            }

            if (Session["cart"] != null)
            {
                products = (List<DataRow>)Session["cart"];
                bool alreadyExists = false;
                for (int i = 0; i < products.Count; i++)
                {
                    if (products[i]["ProductID"].ToString().ToLower() == product["ProductID"].ToString().ToLower())
                    {
                        if (products[i]["Select"].ToString().ToLower() == product["Select"].ToString().ToLower())
                        {
                            alreadyExists = true;
                            int oldCount = Convert.ToInt32(products[i]["Count"]);
                            products[i]["Count"] = (Convert.ToInt32(products[i]["Count"]) + Convert.ToInt32(product["Count"])).ToString();
                            products[i]["PagePrice"] = Convert.ToDouble(products[i]["PagePrice"].ToString().Replace(".", ",")) / oldCount * Convert.ToInt32(products[i]["Count"]);
                        }
                    }
                }
                if (alreadyExists == false)
                {
                    if (product["Category"].ToString() == "Parti Malzemeleri")
                    {
                        product["PagePrice"] = string.Format("{0:0.00}", Convert.ToDouble(product["PagePrice"].ToString().Replace(".", ",")) * 1.18);
                    }
                    products.Add(product);
                }
                Session["cart"] = products;
            }
            else
            {
                if (product["Category"].ToString() == "Parti Malzemeleri")
                {
                    product["PagePrice"] = string.Format("{0:0.00}", Convert.ToDouble(product["PagePrice"].ToString().Replace(".", ",")) * 1.18);
                }
                products.Add(product); Session["cart"] = products;
            }
            Session["cartOpen"] = true;
        }

        public JsonResult AddDesignChecker(Guid MadeID, bool addAnyway, string sizeOption)
        {
            String result = "ok";
            if (Session["cart"] == null) { AddDesignToCart(MadeID.ToString(), null, sizeOption); }
            else
            {
                List<DataRow> products = (List<DataRow>)Session["cart"];
                bool partyOnly = true;
                bool designExists = false; int designIdx = 0;
                for (int i = 0; i < products.Count; i++)
                {
                    if (String.IsNullOrEmpty(products[i]["isMade"].ToString()) == false) { designIdx = i; designExists = true; break; }
                    if (products[i]["BakeryID"].ToString().ToLower() != "98e94e0a-c764-4f99-a2f1-f4915631a891") { partyOnly = false; }
                }

                if (designExists)
                {
                    if (addAnyway == true)
                    {
                        String bakeryID = products[designIdx]["BakeryID"].ToString();
                        products.RemoveAt(designIdx); AddDesignToCart(MadeID.ToString(), bakeryID, sizeOption);
                    }
                    else
                    {
                        result = "designsConflict";
                    }
                }
                else
                {
                    if (addAnyway == true) { AddDesignToCart(MadeID.ToString(), products[0]["BakeryID"].ToString(), sizeOption); }
                    else
                    {
                        if (partyOnly == true) { AddDesignToCart(MadeID.ToString(), "", sizeOption); }
                        else { result = "designProductConflict"; }
                    }
                }
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public void AddDesignToCart(String MadeID, String BakeryID, String sizeOption)
        {
            DataTable design = dc.DBQueryGetter("select * from MadeCakes where MadeID='" + new Guid(MadeID) + "'");

            if (String.IsNullOrEmpty(sizeOption) == false) { design.Rows[0]["SizeOptions"] = sizeOption; }

            design.Columns.Add("isMade", typeof(bool)); design.Rows[0]["isMade"] = true;
            design.Columns.Add("Count", typeof(String)); design.Rows[0]["Count"] = "1";
            design.Columns.Add("Select", typeof(String)); design.Rows[0]["Select"] = design.Rows[0]["SizeOptions"].ToString();
            design.Columns.Add("ProductID", typeof(Guid)); design.Rows[0]["ProductID"] = design.Rows[0]["MadeID"].ToString();

            if (!String.IsNullOrEmpty(BakeryID)) { design.Rows[0]["BakeryID"] = new Guid(BakeryID); design.Rows[0]["Price"] = CalculateDesignPrice(design.Rows[0], BakeryID); }

            design.Columns.Add("PagePrice", typeof(String)); design.Rows[0]["PagePrice"] = design.Rows[0]["Price"].ToString().Split(' ')[0];

            List<DataRow> products = new List<DataRow>();
            if (Session["cart"] != null) { products = (List<DataRow>)Session["cart"]; }
            products.Add(design.Rows[0]);
            Session["cart"] = products;
            Session["cartOpen"] = true;
        }

        public String CalculateDesignPrice(DataRow design, String bakeryID)
        {
            double baseprice = 0; double editorSize = Convert.ToDouble(design["SizeOptions"].ToString().Replace(".", ","));
            DataRow bakery = dc.DBQueryGetter("select * from ForeignBakeries where BakeryID = '" + new Guid(bakeryID) + "'").Rows[0];
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
                    objRow = dc.DBQueryGetter("select * from Objects where ObjectID = '" + new Guid(objstr[1]) + "'").Rows[0];

                    if (objRow["Type"].ToString() == "object" | objRow["Type"].ToString() == "mixed")
                    {
                        string checkSize = objstr[objstr.Length - 1]; string size = "";
                        string[] allowedTypes = new string[] { "b", "o", "k" };

                        if (allowedTypes.Contains(checkSize)) { size = checkSize; } else { size = "o"; }

                        string difficulty = dc.DBQueryGetter("select Difficulty from Objects where ObjectID = '" + new Guid(objstr[1].ToString()) + "'").Rows[0][0].ToString();
                        double dbPrice = Convert.ToDouble(dc.DBQueryGetter("select Price from ObjectPrices where Size = '" + size.ToUpper() + "' and Difficulty = '" + difficulty + "' and BakeryID = '" + new Guid(bakeryID) + "'").Rows[0][0]);
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

            Double calculatedPrice = Math.Round(baseprice + objPrices, 2);

            if(Session["ilanExists"] != null) { calculatedPrice = Math.Round(calculatedPrice / 2,2); }

            return calculatedPrice.ToString();
        }

        //public void MultiDesignPriceCalc()
        //{
        //    DataTable designs = dc.DBQueryGetter("select * from MadeCakes");
        //    //String bakeryID = "CB5EDC14-85FD-40AD-A79A-82D6CA460CD9";

        //    for (int i = 0; i < designs.Rows.Count; i++)
        //    {
        //        DataRow design = designs.Rows[i];

        //        //if (String.IsNullOrEmpty(design["PriceInterval"].ToString()) == true)
        //        //{
        //        //    try
        //        //    {
        //        //        String price = CalculateDesignPrice(designs.Rows[i], bakeryID);
        //        //        dc.DBQuerySetter("update MadeCakes set PriceInterval = '" + price + "' where MadeID = '" + designs.Rows[i]["MadeID"] + "'");
        //        //    }
        //        //    catch (Exception e) { }
        //        //}

        //        String[] descriptions = new string[] {
        //            "" + design["Name"] + " siparişi vererek sevdiklerinize unutamayacağı bir an yaşatın.  Hem kişiye özel pasta yaptırarak sevdiklerinize kendilerini özel hissettirin hem de kutlamanıza lezzet katın.",
        //            "" + design["Category"] + " mı aramıştınız. İşte tam size göre bir " + design["Name"] + ". Şimdi bir " + design["Name"] + " siparişi vererek sevdiklerinizi şımartın.",
        //            "" + design["Category"] + " için en iyi seçeneklerden birisi karşınızda: " + design["Name"] + ". ",
        //            "" + design["Name"] + " siparişi vermek için önce sepete ekleyin ardından pastanenizi seçin ve " + design["Name"] + " istediğiniz tarihte kapınıza gelsin." };

        //        Random rnd = new Random();
        //        int idx = rnd.Next(descriptions.Length);
        //        string description = descriptions[idx];

        //        dc.DBQuerySetter("update MadeCakes set Description = '" + description + "' where MadeID = '" + design["MadeID"] + "'");
        //    }
        //}

        public JsonResult DeleteProductCheck(String idx, bool deleteAnyway)
        {
            String result = "ok";
            List<DataRow> products = (List<DataRow>)Session["cart"];
            bool isMade = !String.IsNullOrEmpty(products[Convert.ToInt32(idx)]["isMade"].ToString());
            bool isParty = products[Convert.ToInt32(idx)]["Category"].ToString() == "Parti Malzemeleri";

            if (isMade | isParty) { DeleteProduct(idx); }
            else
            {
                if (products.Count == 1) { DeleteProduct(idx); }
                else
                {
                    bool allParty = true;
                    for (int i = 0; i < products.Count; i++) { if (products[i]["Category"].ToString() != "Parti Malzemeleri" & i != Convert.ToInt32(idx)) { allParty = false; } }
                    if (allParty) { DeleteProduct(idx); }
                    else
                    {
                        if (deleteAnyway)
                        {
                            for (int i = 0; i < products.Count; i++) { if (String.IsNullOrEmpty(products[i]["isMade"].ToString()) == false) { products[i]["BakeryID"] = DBNull.Value; break; } }
                            DeleteProduct(idx);
                        }
                        else {
                            bool deleteBakeryConflict = false;
                            for (int i = 0; i < products.Count; i++) { if (String.IsNullOrEmpty(products[i]["isMade"].ToString()) == false) { deleteBakeryConflict = true; break; } }
                            if(deleteBakeryConflict) { result = "deleteBakeryConflict"; }
                            else { DeleteProduct(idx); }
                        }
                    }
                }
            }

            List<DataRow> newProducts = (List<DataRow>)Session["cart"];
            if(newProducts == null) { result = "index"; }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public void DeleteProduct(String idx)
        {
            List<DataRow> products = (List<DataRow>)Session["cart"];
            bool allParty = true;

            for (int i = 0; i < products.Count; i++)
            {
                if (String.IsNullOrEmpty(products[0]["isMade"].ToString()) == true)
                {
                    if (products[i]["Category"].ToString() != "Parti Malzemeleri") { allParty = false; }
                }
            }

            if (allParty)
            {
                if (!String.IsNullOrEmpty(products[0]["isMade"].ToString())) { products[0]["Price"] = "0.00"; products[0]["PagePrice"] = "0.00"; }
            }

            if (String.IsNullOrEmpty(products[Convert.ToInt32(idx)]["isMade"].ToString()) == false)
            {
                if (Session["DontTouchBakeryAndContent"] != null) { Session["DontTouchBakeryAndContent"] = null; }
            }

            products.RemoveAt(Convert.ToInt32(idx));

            if (products.Count == 0) { Session["cart"] = null; } else { Session["cart"] = products; }
        }

        public ActionResult AddProductToCart(Guid ProductID)
        {
            List<DataRow> products = new List<DataRow>();

            if (Session["cart"] != null)
            {
                if (Session["currentProduct"] != null)
                {
                    products = (List<DataRow>)Session["cart"];
                }
            }

            if (Session["currentProduct"] != null)
            {
                bool alreadyExists = false;
                for (int i = 0; i < products.Count; i++)
                {
                    if (products[i]["ProductID"].ToString().ToLower() == ((DataRow)Session["currentProduct"])["ProductID"].ToString().ToLower())
                    {
                        if (products[i]["Select"].ToString().ToLower() == ((DataRow)Session["currentProduct"])["Select"].ToString().ToLower())
                        {
                            alreadyExists = true;
                            products[i]["Count"] = (Convert.ToInt32(products[i]["Count"]) + Convert.ToInt32(((DataRow)Session["currentProduct"])["Count"])).ToString();
                            products[i]["PagePrice"] = (Convert.ToDouble(products[i]["PagePrice"].ToString().Replace(".", ",")) + Convert.ToDouble(((DataRow)Session["currentProduct"])["PagePrice"].ToString().Replace(".", ","))).ToString();
                        }
                    }
                }
                if (alreadyExists == false) { products.Add((DataRow)Session["currentProduct"]); }
                Session["cart"] = products; Session["currentProduct"] = null;
            }

            return RedirectToAction("Sepetim");
        }

        public ActionResult UpdateCartProduct(String ProductID, String Type, int Position)
        {
            if (Type == "plus") { }
            List<DataRow> cart = (List<DataRow>)Session["cart"];
            for (int i = 0; i < cart.Count; i++)
            {
                if (i == Position)
                {
                    int newCount = 0; int oldCount = Convert.ToInt32(cart[i]["Count"]);

                    if (Type == "plus") { newCount = Convert.ToInt32(cart[i]["Count"]) + 1; } else if (Type == "minus") { newCount = Convert.ToInt32(cart[i]["Count"]) - 1; }
                    cart[i]["PagePrice"] = Convert.ToDouble(cart[i]["PagePrice"].ToString().Replace(".", ",")) * Convert.ToDouble(newCount) / oldCount;

                    cart[i]["Count"] = newCount;
                    break;
                }
            }

            return RedirectToAction("Sepetim");
        }

        public void ArrangeCart()
        {
            ViewData["cart"] = Session["cart"];
            List<DataRow> cartProducts = (List<DataRow>)ViewData["cart"];
            List<DataRow> bakeries = new List<DataRow>();

            for (int i = 0; i < cartProducts.Count; i++)
            {
                if (cartProducts[i]["BakeryID"] != null & String.IsNullOrEmpty(cartProducts[i]["BakeryID"].ToString()) == false)
                {
                    bakeries.Add(dc.MemoryCacheByQuery("select * from ForeignBakeries where BakeryID = '" + cartProducts[i]["BakeryID"].ToString().ToUpper() + "'").Rows[0]);
                }
            }
            ViewData["bakeries"] = bakeries;
        }

        #endregion CartMethods

        #region Product Methods

        public ActionResult GiveLike(Guid CommentID, string designID)
        {
            bool isTasarim = String.IsNullOrEmpty(designID) == false;

            DataTable likeTable = dc.DBQueryGetter("select * from LikeAction where CommentID = '" + CommentID + "' and UserID = '" + ((DataRow)Session["userinfo"])["UserID"] + "'");

            Guid productID = new Guid(); DataRow comment = null;

            if (isTasarim)
            {
                productID = new Guid(designID);
                comment = dc.DBQueryGetter("select * from DesignComments where ID = '" + CommentID + "'").Rows[0];
            }
            else
            {
                productID = new Guid(((DataRow)Session["currentProduct"])["ProductID"].ToString());
                comment = dc.DBQueryGetter("select * from ProductComments where ID = '" + CommentID + "'").Rows[0];
            }

            int likeCount = Convert.ToInt32(comment["Likes"]) + 1;

            if (likeTable.Rows.Count == 0)
            {
                if (isTasarim) { dc.DBQuerySetter("update DesignComments set Likes = '" + likeCount + "' where ID = '" + CommentID + "'"); }
                else { dc.DBQuerySetter("update ProductComments set Likes = '" + likeCount + "' where ID = '" + CommentID + "'"); }
                dc.DBQuerySetter("insert into LikeAction (ID,CommentID,UserID,ProductID,LikeExists,DislikeExists) values('" + Guid.NewGuid() + "','" + CommentID + "', '" + ((DataRow)Session["userinfo"])["UserID"] + "', '" + productID + "','true','false')");
            }
            else if (likeTable.Rows.Count == 1)
            {
                DataRow likeRow = likeTable.Rows[0]; int dislikeCount = Convert.ToInt32(comment["dislikes"]) - 1;
                if (likeRow["LikeExists"] == null | (bool)likeRow["LikeExists"] == false)
                {
                    if (isTasarim) { dc.DBQuerySetter("update DesignComments set Likes = '" + likeCount + "', Dislikes = '" + dislikeCount + "' where ID = '" + CommentID + "'"); }
                    else { dc.DBQuerySetter("update ProductComments set Likes = '" + likeCount + "', Dislikes = '" + dislikeCount + "' where ID = '" + CommentID + "'"); }
                    dc.DBQuerySetter("update LikeAction set LikeExists = 'true', DislikeExists = 'false' where CommentID = '" + CommentID + "' and UserID = '" + ((DataRow)Session["userinfo"])["UserID"] + "'");
                }
            }

            return Redirect(Request.UrlReferrer.ToString());
        }

        public ActionResult GiveDislike(Guid CommentID, string designID)
        {
            bool isTasarim = String.IsNullOrEmpty(designID) == false;

            DataTable likeTable = dc.DBQueryGetter("select * from LikeAction where CommentID = '" + CommentID + "' and UserID = '" + ((DataRow)Session["userinfo"])["UserID"] + "'");

            Guid productID = new Guid(); DataRow comment = null;

            if (isTasarim)
            {
                productID = new Guid(designID);
                comment = dc.DBQueryGetter("select * from DesignComments where ID = '" + CommentID + "'").Rows[0];
            }
            else
            {
                productID = new Guid(((DataRow)Session["currentProduct"])["ProductID"].ToString());
                comment = dc.DBQueryGetter("select * from ProductComments where ID = '" + CommentID + "'").Rows[0];
            }

            int dislikeCount = Convert.ToInt32(comment["Dislikes"]) + 1;

            if (likeTable.Rows.Count == 0)
            {
                if (isTasarim) { dc.DBQuerySetter("update DesignComments set Dislikes = '" + dislikeCount + "' where ID = '" + CommentID + "'"); }
                else { dc.DBQuerySetter("update ProductComments set Dislikes = '" + dislikeCount + "' where ID = '" + CommentID + "'"); }
                dc.DBQuerySetter("insert into LikeAction (ID,CommentID,UserID,ProductID,LikeExists,DislikeExists) values('" + Guid.NewGuid() + "','" + CommentID + "', '" + ((DataRow)Session["userinfo"])["UserID"] + "', '" + productID + "','false','true')");
            }
            else if (likeTable.Rows.Count == 1)
            {
                DataRow likeRow = likeTable.Rows[0]; int likeCount = Convert.ToInt32(comment["likes"]) - 1;

                if (likeRow["DislikeExists"] == null | (bool)likeRow["DislikeExists"] == false)
                {
                    if (isTasarim) { dc.DBQuerySetter("update DesignComments set Dislikes = '" + dislikeCount + "', Likes = '" + likeCount + "' where ID = '" + CommentID + "'"); }
                    else { dc.DBQuerySetter("update ProductComments set Dislikes = '" + dislikeCount + "', Likes = '" + likeCount + "' where ID = '" + CommentID + "'"); }
                    dc.DBQuerySetter("update LikeAction set DislikeExists = 'true', LikeExists = 'false' where CommentID = '" + CommentID + "' and UserID = '" + ((DataRow)Session["userinfo"])["UserID"] + "'");
                }
            }

            return Redirect(Request.UrlReferrer.ToString());
        }

        public ActionResult AddProductComment(FormCollection form)
        {
            if (form["isDesign"] != null)
            {
                Guid madeID = new Guid(form["MadeID"]);
                DataRow design = dc.DBQueryGetter("select * from MadeCakes where MadeID = '" + madeID + "'").Rows[0];
                Guid userID = new Guid();
                if (Session["userinfo"] != null) { userID = new Guid(((DataRow)Session["userinfo"])["UserID"].ToString()); }
                String nameSurname = form["design-comment-name"];
                Double point = Convert.ToDouble(form["design-comment-rating"].ToString().Replace(".", ","));

                dc.DBQuerySetter("insert into DesignComments (ID,UserID,MadeID,NameSurname,Comment,Likes,Dislikes,DateTime,Approved,Point) values('" + Guid.NewGuid() + "','" + userID + "','" + madeID + "',N'" + nameSurname + "',N'" + form["design-comment-content"] + "','0','0','" + DateTime.Now.ToString() + "','yes'," + point.ToString().Replace(",", ".") + ")");

                ArrangeDesignRating(point.ToString(), madeID.ToString());

                return RedirectToAction("Tasarim", "Home", new { type = uc.cleanseUrlString(design["Type"].ToString()), designName = uc.cleanseUrlString(design["Name"].ToString() + "-" + design["MadeID"].ToString().Split('-')[0]) });
            }
            else
            {
                //Guid productID = new Guid(((DataRow)Session["currentProduct"])["ProductID"].ToString());
                Guid productID = new Guid(form["productID"].ToString());
                Guid userID = new Guid();
                if (Session["userinfo"] != null) { userID = new Guid(((DataRow)Session["userinfo"])["UserID"].ToString()); }
                String nameSurname = form["product-comment-name"];
                DataRow product = dc.DBQueryGetter("select * from Products where ProductID = '" + productID + "'").Rows[0];
                Guid bakeryID = new Guid(product["BakeryID"].ToString());
                DataRow bakery = dc.DBQueryGetter("select * from ForeignBakeries where BakeryID = '" + bakeryID + "'").Rows[0];
                Double point = Convert.ToDouble(form["product-comment-rating"].ToString().Replace(".", ","));

                dc.DBQuerySetter("insert into ProductComments (ID,UserID,ProductID,BakeryID,NameSurname,Comment,Likes,Dislikes,DateTime,Approved,Point) values('" + Guid.NewGuid() + "','" + userID + "','" + productID + "','" + bakeryID + "',N'" + nameSurname + "',N'" + form["product-comment-content"] + "','0','0','" + DateTime.Now.ToString() + "','yes'," + point.ToString().Replace(",", ".") + ")");

                ArrangeProductRating(point.ToString(), productID.ToString());

                return RedirectToAction("Product", "Bakery", new { bakeryName = bakery["Name"], category = uc.cleanseUrlString(product["Category"].ToString()), productName = uc.cleanseUrlString(product["Name"].ToString() + "-" + product["ProductID"].ToString().Split('-')[0]) });
            }
        }

        public void ArrangeDesignRating(string rating, string madeID)
        {
            DataRow product = dc.DBQueryGetter("select * from MadeCakes where MadeID = '" + new Guid(madeID) + "'").Rows[0];
            double oldRating = Convert.ToDouble(product["Rating"].ToString().Replace(".", ","));
            double review = Convert.ToDouble(product["Review"].ToString().Replace(".", ","));
            double newRating = (oldRating * review + Convert.ToDouble(rating)) / (review + 1);

            dc.DBQuerySetter("update MadeCakes set Review = Review + 1, Rating = '" + newRating.ToString().Replace(",", ".") + "' where MadeID = '" + new Guid(madeID) + "'");
        }

        public void ArrangeProductRating(string rating, string productID)
        {
            DataRow product = dc.DBQueryGetter("select * from Products where ProductID = '" + new Guid(productID) + "'").Rows[0];
            double oldRating = Convert.ToDouble(product["Rating"].ToString().Replace(".", ","));
            double review = Convert.ToDouble(product["Review"].ToString().Replace(".", ","));
            double newRating = (oldRating * review + Convert.ToDouble(rating)) / (review + 1);

            dc.DBQuerySetter("update Products set Review = Review + 1, Rating = '" + newRating.ToString().Replace(",", ".") + "' where ProductID = '" + new Guid(productID) + "'");
        }

        public ActionResult FaveProduct(Guid ProductID, Guid BakeryID)
        {
            bool isMade = false;
            if (Session["userinfo"] != null)
            {
                dc.DBQuerySetter("insert into Fav (ID,UserID,ProductID,BakeryID) values ('" + Guid.NewGuid() + "', '" + ((DataRow)Session["userinfo"])["UserID"] + "', '" + ProductID + "', '" + BakeryID + "')");
                DataTable dt = dc.DBQueryGetter("select * from Products where ProductID = '" + ProductID + "'");

                if (dt.Rows.Count == 0) { dt = dc.DBQueryGetter("select * from MadeCakes where MadeID = '" + ProductID + "'"); isMade = true; }
                DataRow dr = dt.Rows[0];

                if (isMade == false)
                {
                    dc.DBQuerySetter("update Products set Fav = '" + (Convert.ToInt32(dr["Fav"]) + 1) + "' where ProductID = '" + ProductID + "'");
                }
                else
                {
                    dc.DBQuerySetter("update MadeCakes set Likes = '" + (Convert.ToInt32(dr["Likes"]) + 1) + "' where MadeID = '" + ProductID + "'");
                }
                if (isMade == true) { return RedirectToAction("Tasarim", "Home", new { type = uc.cleanseUrlString(dr["Type"].ToString()), designName = uc.cleanseUrlString(dr["Name"].ToString() + "-" + dr["MadeID"].ToString().Split('-')[0]) }); }
                else
                {
                    TempData["designID"] = ProductID;
                    DataRow bakery = dc.DBQueryGetter("select * from ForeignBakeries where BakeryID = '" + dr["BakeryID"] + "'").Rows[0];
                    return RedirectToAction("Product", "Bakery", new { bakeryName = bakery["Name"], category = uc.cleanseUrlString(dr["Category"].ToString()), productName = uc.cleanseUrlString(dr["Name"].ToString() + "-" + dr["ProductID"].ToString().Split('-')[0]) });
                }
            }
            else { return RedirectToAction("HataSayfasi", "Home"); }
        }

        public JsonResult FaveProductJson(String ProductID, String BakeryID)
        {
            bool isMade = false;
            if (Session["userinfo"] != null)
            {
                dc.DBQuerySetter("insert into Fav (ID,UserID,ProductID,BakeryID) values ('" + Guid.NewGuid() + "', '" + ((DataRow)Session["userinfo"])["UserID"] + "', '" + new Guid(ProductID) + "', '" + new Guid(BakeryID) + "')");
                DataTable dt = dc.DBQueryGetter("select * from Products where ProductID = '" + new Guid(ProductID) + "'");

                if (dt.Rows.Count == 0) { dt = dc.DBQueryGetter("select * from MadeCakes where MadeID = '" + new Guid(ProductID) + "'"); isMade = true; }
                DataRow dr = dt.Rows[0];

                if (isMade == false)
                {
                    dc.DBQuerySetter("update Products set Fav = '" + (Convert.ToInt32(dr["Fav"]) + 1) + "' where ProductID = '" + new Guid(ProductID) + "'");
                }
                else
                {
                    dc.DBQuerySetter("update MadeCakes set Likes = '" + (Convert.ToInt32(dr["Likes"]) + 1) + "' where MadeID = '" + new Guid(ProductID) + "'");
                }
            }

            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public ActionResult UnfaveProduct(Guid ProductID, Guid BakeryID)
        {
            bool isMade = false;
            if (Session["userinfo"] != null)
            {
                dc.DBQuerySetter("delete from Fav where ProductID = '" + ProductID + "' and UserID = '" + ((DataRow)Session["userinfo"])["UserID"] + "'");
                DataTable dt = dc.DBQueryGetter("select * from Products where ProductID = '" + ProductID + "'");

                if (dt.Rows.Count == 0) { dt = dc.DBQueryGetter("select * from MadeCakes where MadeID = '" + ProductID + "'"); isMade = true; }
                DataRow dr = dt.Rows[0];

                if (isMade == false)
                {
                    dc.DBQuerySetter("update Products set Fav = '" + (Convert.ToInt32(dr["Fav"]) - 1) + "' where ProductID = '" + ProductID + "'");
                }
                else
                {
                    dc.DBQuerySetter("update MadeCakes set Likes = '" + (Convert.ToInt32(dr["Likes"]) - 1) + "' where MadeID = '" + ProductID + "'");
                }
                if (isMade == true) { return RedirectToAction("Tasarim", "Home", new { type = uc.cleanseUrlString(dr["Type"].ToString()), designName = uc.cleanseUrlString(dr["Name"].ToString() + "-" + dr["MadeID"].ToString().Split('-')[0]) }); }
                else
                {
                    TempData["designID"] = ProductID;
                    DataRow bakery = dc.DBQueryGetter("select * from ForeignBakeries where BakeryID = '" + dr["BakeryID"] + "'").Rows[0];
                    return RedirectToAction("Product", "Bakery", new { bakeryName = bakery["Name"], category = uc.cleanseUrlString(dr["Category"].ToString()), productName = uc.cleanseUrlString(dr["Name"].ToString() + "-" + dr["ProductID"].ToString().Split('-')[0]) });
                }
            }
            else { return RedirectToAction("HataSayfasi", "Home"); }
        }

        public JsonResult UnfaveProductJson(String ProductID, String BakeryID)
        {
            bool isMade = false;
            if (Session["userinfo"] != null)
            {
                dc.DBQuerySetter("delete from Fav where ProductID = '" + new Guid(ProductID) + "' and UserID = '" + ((DataRow)Session["userinfo"])["UserID"] + "'");
                DataTable dt = dc.DBQueryGetter("select * from Products where ProductID = '" + new Guid(ProductID) + "'");

                if (dt.Rows.Count == 0) { dt = dc.DBQueryGetter("select * from MadeCakes where MadeID = '" + new Guid(ProductID) + "'"); isMade = true; }
                DataRow dr = dt.Rows[0];

                if (isMade == false)
                {
                    dc.DBQuerySetter("update Products set Fav = '" + (Convert.ToInt32(dr["Fav"]) - 1) + "' where ProductID = '" + new Guid(ProductID) + "'");
                }
                else
                {
                    dc.DBQuerySetter("update MadeCakes set Likes = '" + (Convert.ToInt32(dr["Likes"]) - 1) + "' where MadeID = '" + new Guid(ProductID) + "'");
                }
            }
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        #endregion Product Methods

        #region Operational Methods

        public ActionResult DeleteOrder(string OrderID)
        {
            if (Session["userinfo"] != null)
            {
                if (((DataRow)Session["userinfo"])["UserID"].ToString().ToUpper() == "3BD93549-9C0A-4C9F-A267-EF3521DA42FE")
                {
                    dc.DBQuerySetter("delete from OrderLogs where OrderID = '" + new Guid(OrderID) + "'");
                }
            }
            return Redirect("/Profil");
        }

        public void PastanelerHelper(string sehir_pastaneler, string ilce_pastaneler, string semt_pastaneler)
        {
            bool formFilled = (String.IsNullOrEmpty(sehir_pastaneler) == false & String.IsNullOrEmpty(ilce_pastaneler) == false & String.IsNullOrEmpty(semt_pastaneler) == false);

            if (formFilled)
            {
                DataTable network = dc.DBQueryGetter("select * from BakeryNetwork where Province = '" + sehir_pastaneler + "' and District = '" + ilce_pastaneler + "' and Town = '" + semt_pastaneler + "'");

                string query = "";
                for (int i = 0; i < network.Rows.Count; i++) { query = query + " BakeryID = '" + network.Rows[i]["BakeryID"].ToString() + "' or "; }
                query = query.Substring(0, query.Length - 3);

                DataTable pastane_table = dc.DBQueryGetter("select * from ForeignBakeries where " + query + "");
                ViewData["pastaneler"] = pastane_table;
                ViewData["locationDetected"] = true;
                ViewData["fillSelects"] = new String[] { sehir_pastaneler, ilce_pastaneler, semt_pastaneler };
            }
            else
            {
                if (Session["orderBakery"] != null)
                {
                    DataRow bakery = (DataRow)Session["orderBakery"];
                    DataTable pastane_table = dc.DBQueryGetter("select * from ForeignBakeries where BakeryID = '" + bakery["BakeryID"] + "'");
                    ViewData["pastaneler"] = pastane_table;
                    ViewData["locationDetected"] = true;
                }
                else
                {
                    ViewData["locationDetected"] = false;
                    ViewData["pastaneler"] = dc.DBQueryGetter("select * from ForeignBakeries");
                }

                //if (Session["userinfo"] == null) {  }
                //else
                //{
                //    sehir_pastaneler = ((DataRow)Session["userinfo"])["Province"].ToString();
                //    ilce_pastaneler = ((DataRow)Session["userinfo"])["District"].ToString();
                //    semt_pastaneler = ((DataRow)Session["userinfo"])["Neighborhood"].ToString();

                //    if (!String.IsNullOrEmpty(sehir_pastaneler) & !String.IsNullOrEmpty(ilce_pastaneler) & !String.IsNullOrEmpty(semt_pastaneler))
                //    {
                //        ViewData["locationDetected"] = true;
                //        DataTable pastane_table = dc.DBQueryGetter("select * from ForeignBakeries where (Province = '" + sehir_pastaneler + "' and District = '" + ilce_pastaneler + "')");
                //        ViewData["pastaneler"] = pastane_table;
                //        ViewData["fillSelects"] = new String[] { sehir_pastaneler, ilce_pastaneler, semt_pastaneler };
                //    }
                //    else { ViewData["locationDetected"] = false; }

                //}
            }
        }

        public DataTable ValidateBakeries(DataTable bakeries)
        {
            bakeries.Columns.Add("isOk", typeof(bool));
            bakeries.Columns.Add("validationMessage", typeof(string));

            for (int i = 0; i < bakeries.Rows.Count; i++)
            {
                DataRow bakery = bakeries.Rows[i];

                String overallMessage = "";
                String hourMessage = "Pastanenin gönderim saatleri içerisinde değiliz. \r\n";
                String dayMessage = "Pastanenin bugün gönderim yapmamaktadır. \r\n";
                String openMessage = "Pastane kapalı. \r\n";
                String bdMessage = "Pastane şubede teslim seçeneği sunmuyor. \r\n";
                String hdMessage = "Pastane kapıda ödeme seçeneği sunmuyor. \r\n";

                bool hourValidated = false; bool dayValidated = false; bool isOpenValidated = (bool)bakery["isOpen"]; bool bakeryDeliveryValidated = false; bool homeDeliveryValidated = false;

                DateTime date = DateTime.Now;
                String workHoursTemp = bakery["workHours"].ToString().Replace(":00", "");
                String[] workHours = workHoursTemp.Split('-');
                if (date.Hour >= Convert.ToInt32(workHours[0].Trim()) & date.Hour <= Convert.ToInt32(workHours[1].Trim())) { hourValidated = true; } else { overallMessage = overallMessage + hourMessage + " "; }

                String workDays = bakery["workDays"].ToString();
                int whichDayOfWeek = Convert.ToInt32(date.DayOfWeek) + 1;
                if (workDays[whichDayOfWeek] == '1') { dayValidated = true; } else { overallMessage = overallMessage + dayMessage + " "; }

                if (bakery["BakeryDelivery"].ToString() == "yes") { bakeryDeliveryValidated = true; } else { overallMessage = overallMessage + bdMessage + " "; }

                if (bakery["HomeDelivery"].ToString() == "yes") { homeDeliveryValidated = true; } else { overallMessage = overallMessage + hdMessage + " "; }

                if (isOpenValidated == false) { overallMessage = overallMessage + openMessage; }

                if (hourValidated & dayValidated & isOpenValidated & bakeryDeliveryValidated) { bakeries.Rows[i]["isOk"] = true; bakeries.Rows[i]["validationMessage"] = "Sipariş Verilebilir"; }
                else { bakeries.Rows[i]["isOk"] = false; bakeries.Rows[i]["validationMessage"] = overallMessage; }
            }
            return bakeries;
        }

        public List<string> arrangeCategories(DataTable product_table)
        {
            DateTime dt = DateTime.Now;
            List<string> categories = new List<string>();
            for (int i = 0; i < product_table.Rows.Count; i++) { if (categories.Contains(product_table.Rows[i]["Category"]) == false) { categories.Add(product_table.Rows[i]["Category"].ToString()); } }
            return categories;
        }

        public JsonResult GetAddressMinPacket(string provinceId, string districtId)
        {
            String bakeryID = ((DataRow)Session["orderBakery"])["BakeryID"].ToString();
            DataRow dr = dc.DBQueryGetter("select * from BakeryNetwork where BakeryID = '" + new Guid(bakeryID) + "' and Province = '" + provinceId + "' and District = '" + districtId + "'").Rows[0];
            AddressMinPacket mp = new AddressMinPacket();
            double total = CalculateBakeryTotal();
            if (total < Convert.ToDouble(dr["MinPacket"].ToString().Replace(".", ","))) { mp.isOk = false; mp.color = "red"; mp.message = "Pastanenin adresinize min. paket tutarının ( " + dr["MinPacket"].ToString() + " TL ) altındasınız."; }
            else { mp.isOk = true; mp.color = "green"; mp.message = ""; }
            return Json(mp, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetDistricts(String provinceId)
        {
            DataTable district_table = dc.MemoryCacheByQuery("select * from Ilceler where SehirId = '" + provinceId + "'");
            district_table = dc.ArrangeLocationTable(district_table, "IlceAdi", true, "IlceAdi");
            List<string> append = new List<string>();
            append.Add("<option selected disabled value=''>Seçiniz</option>");
            for (int i = 0; i < district_table.Rows.Count; i++) { append.Add("<option value='" + district_table.Rows[i]["ilceId"] + "'>" + district_table.Rows[i]["IlceAdi"].ToString() + "</option>"); }
            return Json(append, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetNeighborhoods(String districtId)
        {
            DataTable neighborhood_table = dc.MemoryCacheByQuery("select * from SemtMah where ilceId = '" + districtId + "'");
            neighborhood_table = dc.ArrangeLocationTable(neighborhood_table, "MahalleAdi", true, "MahalleAdi");
            List<string> append = new List<string>();
            for (int i = 0; i < neighborhood_table.Rows.Count; i++) { append.Add("<option value='" + neighborhood_table.Rows[i]["SemtMahId"] + "'>" + neighborhood_table.Rows[i]["MahalleAdi"] + "</option>"); }
            return Json(append, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetBakeryDistricts(String provinceId)
        {
            DataTable district_table = dc.MemoryCacheByQuery("select * from Ilceler where SehirId = '" + provinceId + "'");
            district_table = dc.ArrangeLocationTable(district_table, "IlceAdi", true, "IlceAdi");
            List<string> append = new List<string>();
            append.Add("<option value='hepsi'>Hepsi</option>");
            for (int i = 0; i < district_table.Rows.Count; i++) { append.Add("<option value='" + district_table.Rows[i]["ilceId"] + "'>" + district_table.Rows[i]["IlceAdi"].ToString() + "</option>"); }
            return Json(append, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetBakeryTowns(String districtId)
        {
            if (districtId.ToLower() != "hepsi")
            {
                DataTable neighborhood_table = dc.MemoryCacheByQuery("select * from SemtMah where ilceId = '" + districtId + "'");
                neighborhood_table = dc.ArrangeLocationTable(neighborhood_table, "MahalleAdi", true, "MahalleAdi");
                List<string> append = new List<string>();
                append.Add("<option value='hepsi'>Hepsi</option>");
                for (int i = 0; i < neighborhood_table.Rows.Count; i++) { append.Add("<option value='" + neighborhood_table.Rows[i]["SemtMahId"] + "'>" + neighborhood_table.Rows[i]["SemtAdi"] + " - " + neighborhood_table.Rows[i]["MahalleAdi"] + "</option>"); }
                return Json(append, JsonRequestBehavior.AllowGet);
            }
            else { return Json(true, JsonRequestBehavior.AllowGet); }
        }

        [HttpPost]
        public JsonResult Html2Pdf(String html)
        {
            String id = "";
            if (Session["pdfID"] == null)
            {
                Guid pdfGuid = Guid.NewGuid();
                string path = System.IO.Path.Combine(Server.MapPath("~/Bills/"));
                string pdfPath = Path.Combine(path, pdfGuid + ".pdf");

                try
                {
                    PdfDocument pdf = PdfGenerator.GeneratePdf(html, PageSize.A4);
                    pdf.Save(pdfPath);
                }
                catch (Exception e) { }
                Session["pdfID"] = pdfGuid; id = pdfGuid.ToString();
            }
            else
            {
                id = ((Guid)Session["pdfID"]).ToString();
            }

            return Json(id + ".pdf", JsonRequestBehavior.AllowGet);
        }

        #endregion Operational Methods
    }
}