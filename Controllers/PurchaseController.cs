using Cake.Models;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Net;
using System.Web.Mvc;

namespace Cake.Controllers
{
    public class PurchaseController : Controller
    {
        private DatabaseController dc = new DatabaseController();
        private AccountController ac = new AccountController();

        public ActionResult Odeme()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Payment(FormCollection form)
        {
            Session["paymentForm"] = form;

            if ((String)Session["orderPayment"] == "POS" | Session["orderPayment"] == null)
            {
                Session["orderPayment"] = "POS";
                bool secureWanted = true;
                TempData["FormData"] = form;
                // if (form["3dCheck"] != null) { if (form["3dCheck"].ToString() == "on") { secureWanted = true; } }
                if (secureWanted) { return RedirectToAction("SecurePayment"); }
                else { return RedirectToAction("NonSecurePayment"); }
            }
            else
            {
                LogOrder(form);
                return RedirectToAction("odeme");
            }
        }

        public string sumCart()
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

            if (Session["promotion"] != null)
            {
                Promotion prom = (Promotion)Session["promotion"];
                total = Convert.ToDouble(prom.newCartPrice.Replace(".", ","));
            }

            return total.ToString();
        }

        public ActionResult NonSecurePayment()
        {
            FormCollection form = (FormCollection)TempData["FormData"];
            String orderID = LogOrder(form);
            Session["orderID"] = orderID;

            ePayment.cc5payment po = new ePayment.cc5payment();

            //////////////////////////storekey TRPS2658

            po.host = "https://sanalpos.teb.com.tr/fim/api";

            // https://sanalpos.teb.com.tr/fim/est3Dgate  https://entegrasyon.asseco-see.com.tr/fim/est3Dgate
            // https://sanalpos.teb.com.tr/fim/api        https://entegrasyon.asseco-see.com.tr/fim/api

            // SanalPos Arayüz Bilgileri

            // Arayüz Link    : https://sanalpos.teb.com.tr
            // Magaza Numarasi    : 400722658
            // Kullanici Adi      : infiniadmin
            // Güvenlik Kodu
            // (sifre degildir)   : KGJH2JR
            // Para Birimi: TL
            // Sifre              : BOSS1302

            // SanalPos Test Bilgileri

            //po.clientid = "400000100";

            //po.name = "TEBAPI";
            //po.password = "TEBTEB04";
            //po.cardnumber = "4508034508034509";
            //po.expmonth = "12";
            //po.expyear = "16";
            //po.cv2 = "000";
            //po.subtotal = "1";
            //po.currency = "949";
            //po.chargetype = "Auth";

            // SanalPos Prod Bilgileri

            po.clientid = "400722658";

            po.name = "infiniadmin";
            po.password = "BOSS1302";
            po.cardnumber = form["cardNum1"] + form["cardNum2"] + form["cardNum3"] + form["cardNum4"];
            po.expmonth = form["cardMonth"];
            po.expyear = form["cardYear"];
            po.cv2 = form["cardCvc"];
            po.subtotal = sumCart();
            po.currency = "949";
            po.chargetype = "Auth";
            po.bname = form["cardName"] + " " + form["cardSurname"];

            String result = po.processorder();

            if (String.IsNullOrEmpty(po.appr) == false)
            {
                if (po.appr != "Approved") { result = "0"; Session["errorMessage"] = po.errmsg; }
            }

            ViewData["result"] = result;
            Session["secureWanted"] = false;

            if (result == "1")
            {
                dc.DBQuerySetter("update OrderLogs set PaymentApproved = 'yes' where OrderID = '" + new Guid(orderID) + "'");
                sendOrderMails(form, true);
            }

            return RedirectToAction("Odeme");
        }

        public ActionResult SecurePayment()
        {
            FormCollection form = (FormCollection)TempData["FormData"];
            Session["orderID"] = LogOrder(form);

            String clientId = "400722658";  /*String clientid = "400722658";*/
            String amount = sumCart();
            String oid = "";
            String okUrl = "https://www.icaked.com/Odeme";
            String failUrl = "https://www.icaked.com/Odeme";
            String rnd = DateTime.Now.ToString();
            String taksit = "";
            String islemtipi = "Auth";
            String storekey = "TRPS2658"; /*storekey TRPS2658*/
            String hashstr = clientId + oid + amount + okUrl + failUrl + islemtipi + taksit + rnd + storekey;

            System.Security.Cryptography.SHA1 sha = new System.Security.Cryptography.SHA1CryptoServiceProvider();
            byte[] hashbytes = System.Text.Encoding.GetEncoding("ISO-8859-9").GetBytes(hashstr);
            byte[] inputbytes = sha.ComputeHash(hashbytes);
            String hash = Convert.ToBase64String(inputbytes);

            using (WebClient client = new WebClient())
            {
                byte[] response =
                client.UploadValues("https://sanalpos.teb.com.tr/fim/est3Dgate", new NameValueCollection()
                {
                    { "pan", form["cardNum1"] + form["cardNum2"] + form["cardNum3"] + form["cardNum4"] },
                    { "cv2", form["cardCvc"] },
                    { "Ecom_Payment_Card_ExpDate_Year", form["cardYear"] },
                    { "Ecom_Payment_Card_ExpDate_Month", form["cardMonth"] },
                    { "clientid", "400722658" },
                    { "amount", sumCart() },
                    { "oid", "" },
                    { "okUrl", "https://www.icaked.com/Odeme" },
                    { "failUrl", "https://www.icaked.com/Odeme" },
                    { "rnd", rnd },
                    { "hash", hash },
                    { "islemtipi", islemtipi },
                    { "taksit", taksit },
                    { "storetype", "3d" },
                    { "currency", "949" }
                });

                ViewData["result"] = System.Text.Encoding.UTF8.GetString(response);
            }

            Session["secureWanted"] = true;

            return View();
        }

        public String LogOrder(FormCollection form)
        {
            FormCollection infoForm = (FormCollection)Session["orderDeliveryInfo"];
            String totalPrice = "0"; String inside = null; String madeID = ""; String discount = "0"; Guid promotionCode = new Guid();
            List<DataRow> cart = (List<DataRow>)Session["cart"];

            int orderNumber = Convert.ToInt32(dc.DBQueryGetter("select Num from OrderNum").Rows[0][0]);
            Session["orderNumber"] = orderNumber;
            int newNum = orderNumber + 1;
            dc.DBQuerySetter("update OrderNum set Num = '" + newNum + "' where ID = '446c0f8a-144c-4618-a33d-d363726797d5'");

            String paymentType = "";
            if (Session["orderPayment"] == null | string.IsNullOrEmpty((String)Session["orderPayment"])) { paymentType = "POS"; } else { paymentType = (String)Session["orderPayment"]; }

            if (Session["orderContent"] != null) { inside = ((DataRow)Session["orderContent"])["Inside"].ToString(); }

            String[] productIDList = new String[cart.Count];
            String[] productCountList = new String[cart.Count];
            String[] productPriceList = new String[cart.Count];

            for (int i = 0; i < cart.Count; i++)
            {
                productIDList[i] = cart[i]["ProductID"].ToString();
                productCountList[i] = cart[i]["Count"].ToString();
                productPriceList[i] = cart[i]["PagePrice"].ToString();
                totalPrice = (Convert.ToDouble(totalPrice.Replace(".", ",")) + Convert.ToDouble(cart[i]["PagePrice"].ToString().Replace(".", ","))).ToString();

                if (Session["promotion"] != null)
                {
                    Promotion prom = (Promotion)Session["promotion"];
                    totalPrice = Convert.ToDouble(prom.newCartPrice.Replace(".", ",")).ToString();
                    discount = prom.indirimTutari;
                    promotionCode = prom.code;
                }

                if (String.IsNullOrEmpty(cart[i]["isMade"].ToString()) == false) { madeID = cart[i]["MadeID"].ToString(); }
            }

            String productIDs = String.Join(" ", productIDList);
            String productCounts = String.Join(" ", productCountList);
            String productPrices = String.Join(" ", productPriceList);

            Guid orderID = Guid.NewGuid(); Guid billID = Guid.NewGuid();
            String bakeryID = ((DataRow)Session["orderBakery"])["BakeryID"].ToString();

            string carrySecondHour = "";
            if (infoForm["bakerySecondHourCopy"] != null) { carrySecondHour = infoForm["bakerySecondHourCopy"]; }

            string deliveryDateTime = "";
            if (infoForm["selectedHour"] == "normal") { deliveryDateTime = infoForm["deliveryDatePicker"] + " " + infoForm["deliveryTimePicker"] + " " + carrySecondHour; }
            else { deliveryDateTime = infoForm["deliveryDatePicker"] + " " + infoForm["deliveryTimePickerClean"] + " " + carrySecondHour; }

            dc.DBQuerySetter("insert into OrderLogs (ID,OrderID,OrderNumber,BakeryID,BillID,DateTime,ContactName,ContactSurname,ContactEmail,ContactPhone,ProductIDs,ProductNumbers,ProductPrices,Inside,TotalPrice,PaymentType,PaymentApproved,DeliveryDatetime,Status,Promotion,PromotionCode) values ('" + Guid.NewGuid() + "','" + orderID + "','" + orderNumber + "','" + new Guid(bakeryID) + "','" + billID + "','" + DateTime.Now.ToString() + "',N'" + infoForm["contactName"] + "',N'" + infoForm["contactSurname"] + "',N'" + infoForm["contactMail"] + "','" + infoForm["contactTel"] + "','" + productIDs + "','" + productCounts + "','" + productPrices + "',N'" + inside + "','" + totalPrice + "',N'" + paymentType + "','no','" + deliveryDateTime + "',N'Bekliyor','" + discount + "','" + promotionCode + "')");

            if (Session["userinfo"] != null) { dc.DBQuerySetter("update OrderLogs set UserID = '" + ((DataRow)Session["userinfo"])["UserID"] + "' where OrderID = '" + orderID + "'"); }
            if (Session["orderAddress"] != null)
            {
                if (Session["orderAddress"].GetType().Name != "String")
                {
                    dc.DBQuerySetter("update OrderLogs set AddressID = '" + ((DataRow)Session["orderAddress"])["ID"] + "' where OrderID = '" + orderID + "'");
                }
            }
            if (String.IsNullOrEmpty(madeID) == false) { dc.DBQuerySetter("update OrderLogs set MadeID = '" + new Guid(madeID) + "' where OrderID = '" + orderID + "'"); }
            if (infoForm["orderNotes"] != null) { if (String.IsNullOrEmpty(infoForm["orderNotes"].ToString()) == false) { dc.DBQuerySetter("update OrderLogs set Note = N'" + infoForm["orderNotes"].ToString() + "' where OrderID = '" + orderID + "'"); } }

            if (paymentType == "POS") { LogBill(billID, orderID, form); }

            LogBakeriesToOrder(orderID);

            Session["orderMain"] = dc.DBQueryGetter("select * from OrderLogs where OrderID = '" + orderID + "'").Rows[0];

            return orderID.ToString();
        }

        public void LogBakeriesToOrder(Guid OrderID)
        {
            try
            {
                String bakeryIDStr = ""; String bakeryNameStr = "";
                List<DataRow> cart = (List<DataRow>)Session["cart"];
                for(int i = 0; i < cart.Count; i++)
                {
                    bakeryIDStr = bakeryIDStr + cart[i]["BakeryID"].ToString() + " "; bakeryNameStr = bakeryNameStr + dc.getBakeryNameByID(new Guid(cart[i]["BakeryID"].ToString())) + " ";
                }
                bakeryIDStr = bakeryIDStr.Substring(0,bakeryIDStr.Length-1); bakeryNameStr = bakeryNameStr.Substring(0, bakeryNameStr.Length - 1);
                dc.DBQuerySetter("update OrderLogs set Bakeries = '" + bakeryIDStr + "', BakeryNames = N'" + bakeryNameStr + "' where OrderID = '" + OrderID + "'");
            }
            catch(Exception e) { }
        }

        public void LogBill(Guid billID, Guid orderID, FormCollection form)
        {
            String cardnum = form["cardNum1"] + form["cardNum2"].ToString().Substring(0, 2) + "****" + form["cardNum3"].ToString().Substring(2, 2) + form["cardNum4"];
            String cardexpire = form["cardMonth"] + "/" + form["cardYear"];

            String paymentType = "";
            if (Session["orderPayment"] == null | string.IsNullOrEmpty((String)Session["orderPayment"])) { paymentType = "POS"; } else { paymentType = (String)Session["orderPayment"]; }

            dc.DBQuerySetter("insert into BillingLogs (ID,BillID,OrderID,PaymentApproved,PaymentType,CardOwnerName,CardOwnerSurname,CardNum,CardExpire,CardCvc) values ('" + Guid.NewGuid() + "','" + billID + "','" + orderID + "','no',N'" + paymentType + "',N'" + form["cardName"] + "',N'" + form["cardSurname"] + "','" + cardnum + "','" + cardexpire + "','" + form["cardCvc"] + "')");

            if (Session["pdfID"] != null)
            {
                dc.DBQuerySetter("update BillingLogs set PdfID = '" + (Guid)Session["pdfID"] + "' where BillID = '" + billID + "'");
            }

            if (form["faturaName"] != null & form["faturaAdres"] != null)
            {
                if (String.IsNullOrEmpty(form["faturaName"].ToString()) == false & String.IsNullOrEmpty(form["faturaAdres"].ToString()) == false)
                {
                    dc.DBQuerySetter("update BillingLogs set BillNameSurname = N'" + form["faturaName"] + "', BillAddress = N'" + form["faturaAdres"] + "' where BillID = '" + billID + "'");
                }
            }

            if (form["faturaDaire"] != null & form["faturaID"] != null)
            {
                if (String.IsNullOrEmpty(form["faturaDaire"].ToString()) == false & String.IsNullOrEmpty(form["faturaID"].ToString()) == false)
                {
                    dc.DBQuerySetter("update BillingLogs set BillIdentity = '" + form["faturaID"] + "', BillDaire = N'" + form["faturaDaire"] + "' where BillID = '" + billID + "'");
                }
            }
        }

        public void sendOrderMails(FormCollection form, bool isPaymentSuccessful)
        {
            DataRow bakery = (DataRow)Session["orderBakery"];

            string orderID = (string)Session["orderID"];
            string orderNumber = (string)Session["OrderNumber"];
            DataRow order = dc.DBQueryGetter("select * from OrderLogs where OrderID = '" + new Guid(orderID) + "'").Rows[0];

            List<DataRow> cart = (List<DataRow>)Session["cart"];

            string carrySecondHour = "";
            if (form["bakerySecondHourCopy"] != null) { carrySecondHour = form["bakerySecondHourCopy"]; }

            string deliveryDateTime = "";
            if (form["selectedHour"] == "normal") { deliveryDateTime = form["deliveryDatePicker"] + " " + form["deliveryTimePicker"] + " " + carrySecondHour; }
            else { deliveryDateTime = form["deliveryDatePicker"] + " " + form["deliveryTimePickerClean"] + " " + carrySecondHour; }

            string totalPrice = "0"; string madeID = ""; string paymentType = ""; string adresStr = ""; string adresLabel = ""; string madeProductStr = ""; string productsStr = "";
            DataRow address = null; DataRow madeProduct = null;
            int madeIDx = 0;

            if (Session["orderPayment"] == null | string.IsNullOrEmpty((String)Session["orderPayment"])) { paymentType = "POS"; } else { paymentType = (String)Session["orderPayment"]; }

            for (int i = 0; i < cart.Count; i++)
            {
                totalPrice = (Convert.ToDouble(totalPrice.Replace(".", ",")) + Convert.ToDouble(cart[i]["PagePrice"].ToString().Replace(".", ","))).ToString();
                if (String.IsNullOrEmpty(cart[i]["isMade"].ToString()) == false) { madeID = cart[i]["MadeID"].ToString(); madeProduct = cart[i]; }
            }

            if (String.IsNullOrEmpty(order["MadeID"].ToString()) == false)
            {
                madeProduct = dc.DBQueryGetter("select * from MadeCakes where MadeID = '" + order["MadeID"] + "'").Rows[0];
                for (int i = 0; i < order["ProductIDs"].ToString().Split(' ').Length; i++) { if (order["MadeID"].ToString().ToLower() == order["ProductIDs"].ToString().Split(' ')[i].ToString().ToLower()) { madeIDx = i; break; } }
            }

            if (Session["orderAddress"] != null) { if (Session["orderAddress"].GetType().Name != "String") { address = (DataRow)Session["orderAddress"]; } }
            if (address != null)
            {
                adresStr = @"
                    <label style=""float:left; width:100%; padding-bottom:15px;"">" + address["ProvinceName"] + ", " + address["DistrictName"] + ", " + address["TownName"] + ", " + address["NeighborhoodName"] + @"</label>
                    <label style=""float:left; width:100%; padding-bottom:15px;"">" + address["SpecificAddress"] + @"</label>
                    <label style=""float:left; width:100%; padding-bottom:15px;"">" + address["Description"] + @"</label>";

                adresLabel = @"
                    <label style=""font-weight:bold; width:100%; float:left; padding-bottom:15px;"">Sipariş Adresi : </label>
                    ";
            }

            List<DataRow> products = new List<DataRow>();
            for (int i = 0; i < order["ProductIDs"].ToString().Split(' ').Length; i++)
            {
                try
                {
                    products.Add(dc.DBQueryGetter("select * from Products where ProductID = '" + order["ProductIDs"].ToString().Split(' ')[i] + "'").Rows[0]);
                }
                catch (Exception e) { }
            }

            if (madeProduct != null)
            {
                string madeInner = "";

                for (int i2 = 0; i2 < madeProduct["ImagePath"].ToString().Split(' ').Length; i2++)
                {
                    string productImg = "https://www.icaked.com/Images/MadeCakes/" + madeProduct["MadeID"] + "/" + madeProduct["ImagePath"].ToString().Split(' ')[i2];
                    madeInner = madeInner + @"<img src=" + productImg + @" style = ""width:150px; cursor:pointer; float:left; margin-right:10px; margin-left:10px;"" />";
                }

                madeProductStr = @"
                        <div style=""width:100%; float:left; border-bottom:1px solid #cccccc; margin-bottom:10px; padding-bottom:20px;"">
                            <div style=""width:100%; float:left; margin-bottom:10px; float:left;"">
                                <div style=""width:100%; float:left; margin-bottom:10px;"">
                                    <label style=""float:left; margin-left:10px; font-size:14px; color:#9a4622; margin-right:10px;"">Adet : </label>
                                    <label style=""float:left; margin-left:10px; font-size:14px;"">" + order["ProductNumbers"].ToString().Split(' ')[madeIDx] + @" adet</label>
                                </div>
                                <div style=""width:100%; float:left; margin-bottom:10px;"">
                                    <label style=""float:left; margin-left:10px; font-size:14px; color:#9a4622; margin-right:10px;"">Ürün İsmi : </label>
                                    <label style=""float:left; margin-left:10px; font-size:14px;"">" + madeProduct["Name"] + @"</label>
                                </div>
                                <div style=""width:100%; float:left; margin-bottom:10px;"">
                                    <label style=""float:left; margin-left:10px; font-size:14px; color:#9a4622; margin-right:10px;"">Ürün İçeriği : </label>
                                    <label style=""float:left; margin-left:10px; font-size:14px;"">" + order["Inside"] + @"</label>
                                </div>
                                <div style=""width:100%; float:left; margin-bottom:10px;"">
                                    <label style=""float:left; margin-left:10px; font-size:14px; color:#9a4622; margin-right:10px;"">Ürün Fiyatı : </label>
                                    <label style=""float:left; margin-left:10px; font-size:14px;"">" + order["ProductPrices"].ToString().Split(' ')[madeIDx] + @" TL</label>
                                </div>
                            </div>
                            <div style=""width:100%; float:left; float:left;"">" + madeInner + @"</div>
                        </div>
                ";
            }

            for (int i = 0; i < products.Count; i++)
            {
                string productsInner = "";

                for (int i2 = 0; i2 < products[i]["Thumbnail"].ToString().Split(' ').Length; i2++)
                {
                    string productImg = "https://www.icaked.com/Images/Products/" + products[i]["ProductID"] + "/" + products[i]["Thumbnail"].ToString().Split(' ')[i2];
                    if (String.IsNullOrEmpty(products[i]["Thumbnail"].ToString().Split(' ')[i2]) == false)
                    {
                        productsInner = productsInner + @"<img src=" + productImg + @" style=""width:150px; cursor:pointer; float:left; margin-right:10px; margin-left:10px;"" />";
                    }
                }

                productsStr = productsStr +
                @"<div style=""width:100%; float:left; border-bottom:1px solid #cccccc; margin-bottom:10px; padding-bottom:20px;"">
                    <div style = ""width:100%; float:left; margin-bottom:10px; float:left;"">
                        <div style=""width:100%; float:left; margin-bottom:10px;"">
                            <label style=""float:left; margin-left:10px; font-size:14px; color:#9a4622; margin-right:10px;"">Adet : </label>
                            <label style=""float:left; margin-left:10px; font-size:14px;"">" + order["ProductNumbers"].ToString().Split(' ')[i] + @" adet</label>
                        </div>
                        <div style=""width:100%; float:left; margin-bottom:10px;"">
                            <label style=""float:left; margin-left:10px; font-size:14px; color:#9a4622; margin-right:10px;"">Ürün İsmi : </label>
                            <label style=""float:left; margin-left:10px; font-size:14px;"">" + products[i]["Name"] + @"</label>
                        </div>
                        <div style=""width:100%; float:left; margin-bottom:10px;"">
                            <label style=""float:left; margin-left:10px; font-size:14px; color:#9a4622; margin-right:10px;"">Ürün Fiyatı : </label>
                            <label style=""float:left; margin-left:10px; font-size:14px;"">" + order["ProductPrices"].ToString().Split(' ')[i] + @" TL</label>
                        </div>
                    </div>
                    <div style=""width:100%; float:left; float:left;"">" + productsInner + @"</div>
                </div>";
            }

            string orderlink = "https://www.icaked.com/Bakery/BakeryOrderSub?orderID=" + orderID;

            string customerDeclaration = @"
                        <div style =""width:100%; float:left; border-top:1px dashed #3f2518;"">
                            <div style=""width:100%; float:left; text-align:justify; padding:40px 0px; text-align:center; margin:20px 0px; border-radius:5px; position:relative; background:#f2f2f2;"">
                                <div style=""width:100%; float:left;"">
                                    <label style=""font-size:20px; color:#9a4622; float:left; width:100%; text-align:center; padding-bottom:20px;"">Siparişiniz bize ulaşmıştır</label>
                                    <label style=""font-size:14px; color:#9a4622; float:left; width:100%; text-align:center; padding-bottom:20px;"">iCaked’i seçtiğiniz teşekkür ederiz. En tatlı anlarınızda yanınızda olmaktan büyük mutluluk duyuyoruz</label>
                                    <label style=""font-size:12px; color:#9a4622; float:left; width:100%; text-align:center; padding-bottom:20px;"">Siparişinizin detayları aşağıda verilmiştir. </label>
                                    <a href=" + orderlink + @" style=""font-size:12px; float:left; width:100%; text-align:center; padding-bottom:20px; color:black;"">Sipariş durumunuzu buradan takip edebilirsiniz. </a>
                                </div>
                            </div>
                        </div>";

            string bakeryDeclaration = @"
                        <div style =""width:100%; float:left; border-top:1px dashed #3f2518;"">
                            <div style=""width:100%; float:left; text-align:justify; padding:40px 0px; text-align:center; margin:20px 0px; border-radius:5px; position:relative; background:#f2f2f2;"">
                                <div style=""width:100%; float:left;"">
                                    <label style=""font-size:20px; color:#9a4622; float:left; width:100%; text-align:center; padding-bottom:20px;"">Merhaba " + bakery["Name"] + @"</label>
                                    <label style=""font-size:14px; color:#9a4622; float:left; width:100%; text-align:center; padding-bottom:20px;"">iCaked.com üzerinden pastanenizden sipariş oluşturuldu.</label>
                                    <label style=""font-size:12px; color:#9a4622; float:left; width:100%; text-align:center; padding-bottom:20px;"">Siparişinizin detayları aşağıda verilmiştir. </label>
                                    <a href=" + orderlink + @" style=""font-size:12px; float:left; width:100%; text-align:center; padding-bottom:20px; color:black;"">Sipariş durumunuzu buradan takip edebilirsiniz. </a>
                                </div>
                            </div>
                        </div>";

            String customerMailBody = @"
                 <div style=""width:90%; margin-left:5%; font-family:Helvetica, sans-serif;"">
                    <div style=""margin:auto; min-width:330px; max-width:800px;"">

                        <div style=""padding:40px 0px; width:100%; float:left; background:white;"">
                            <img src=""https://www.icaked.com/Images/Site/main_logo.png"" style=""width:150px; float:left;"" />
                        </div>"
                    + customerDeclaration +
                        @"<label style=""float:left; font-size:18px; padding:15px 0px; padding-top:0px; width:100%; border-bottom:1px dashed #3f2518; margin-top:30px; text-align:left; color:#9a4622; line-height:initial;"">Sipariş Bilgileri</label>
                        <div style=""width:100%; float:left; text-align:left; padding:15px 0px;"">
                            <div style=""width:20%; float:left; min-width:130px; color:#9a4622;"">
                                <label style=""font-weight:bold; width:100%; float:left; padding-bottom:15px;"">Pastane : </label>
                                <label style=""font-weight:bold; width:100%; float:left; padding-bottom:15px;"">Sipariş Numarası : </label>
                                <label style=""font-weight:bold; width:100%; float:left; padding-bottom:15px;"">Sipariş Tarihi : </label>
                                <label style=""font-weight:bold; width:100%; float:left; padding-bottom:15px;"">Teslimat Tarihi : </label>
                                <label style=""font-weight:bold; width:100%; float:left; padding-bottom:15px;"">Toplam Fiyat : </label>
                                <label style=""font-weight:bold; width:100%; float:left; padding-bottom:15px;"">Ödeme Şekli : </label>
                                <label style=""font-weight:bold; width:100%; float:left; padding-bottom:15px;"">Sipariş Notları : </label>
                                " + adresLabel +
                          @"</div>
                            <div style=""width:60%; float:left;"">
                                <label style=""float:left; width:100%; padding-bottom:15px;"">" + bakery["Name"] + @"</label>
                                <label style=""float:left; width:100%; padding-bottom:15px;"">" + orderNumber + @"</label>
                                <label style=""float:left; width:100%; padding-bottom:15px;"">" + DateTime.Now + @"</label>
                                <label style=""float:left; width:100%; padding-bottom:15px; font-weight:bold;"">" + deliveryDateTime.Replace("/", ".") + @"</label>
                                <label style=""float:left; width:100%; padding-bottom:15px;"">" + totalPrice + @" TL </label>
                                <label style=""float:left; width:100%; padding-bottom:15px;"">" + paymentType + @"</label>
                                <label style=""float:left; width:100%; padding-bottom:15px;"">" + form["orderNotes"] + @"</label>
                                " + adresStr +
                          @"</div>
                        </div>

                        <label style=""float:left; font-size:18px; padding:15px 0px; padding-top:0px; width:100%; border-bottom:1px dashed #3f2518; margin-top:30px; text-align:left; color:#9a4622; line-height:initial;"">İletişim Bilgileri</label>
                        <div style=""width:100%; float:left; text-align:left; padding:15px 0px;"">
                            <div style=""width:20%; min-width:130px; float:left; color:#9a4622;"">
                                <label style=""font-weight:bold; width:100%; padding-bottom:15px; float:left;"">İsim - Soyisim : </label>
                                <label style=""font-weight:bold; width:100%; padding-bottom:15px; float:left;"">E-Posta : </label>
                                <label style=""font-weight:bold; width:100%; padding-bottom:15px; float:left;"">Telefon : </label>
                            </div>
                            <div style=""width:60%; float:left;"">
                                <label style=""float:left; width:100%; padding-bottom:15px;"">" + form["contactName"] + " " + form["contactSurname"] + @"</label>
                                <label style=""float:left; width:100%; padding-bottom:15px;"">" + form["contactMail"] + @"</label>
                                <label style=""float:left; width:100%; padding-bottom:15px;"">" + form["contactTel"] + @"</label>
                            </div>
                        </div>

                        <label style=""float:left; font-size:18px; padding:15px 0px; padding-top:0px; width:100%; border-bottom:1px dashed #3f2518; margin-top:30px; text-align:left; color:#9a4622; line-height:initial;"">Ürün Bilgileri</label>
                        <div style=""width:100%; float:left; text-align:left; padding:15px 0px;"">
                            <div style=""width:99%; padding:20px 0px; float:left;"">" + madeProductStr + productsStr + @"
                            </div>
                        </div>
                        <div style=""background:#f0f0f0; width:100%; float:left; padding:20px 0px;"">
                            <a href=""http://www.facebook.com/icaked""><img src=""https://www.icaked.com/Images/Site/fb_icon.png"" style=""float:left; margin:3px; width:25px; margin-top:0px; margin-left:15px;"" /></a>
                            <a href=""http://www.twitter.com/icakedcom""><img src=""https://www.icaked.com/Images/Site/tw_icon.png"" style=""float:left; margin:3px; width:25px; margin-top:0px;"" /></a>
                            <a href=""http://www.instagram.com/icakedcom""><img src=""https://www.icaked.com/Images/Site/in_icon.png"" style=""float:left; margin:3px; width:25px; margin-top:0px;"" /></a>
                            <a href=""http://www.facebook.com/icakedcom""><img src=""https://www.icaked.com/Images/Site/pi_icon.png"" style=""float:left; margin:3px; width:25px; margin-top:0px;"" /></a>
                            <a style=""text-decoration:none; float:right;"">
                                <label style=""font-size:14px; line-height:30px; float:right; color:black; margin-right:20px;"">2016 © www.iCaked.com</label>
                            </a>
                        </div>
                    </div>
                </div>
            ";

            String bakeryMailBody = @"
                 <div style=""width:90%; margin-left:5%; font-family:Helvetica, sans-serif;"">
                    <div style=""margin:auto; min-width:330px; max-width:800px;"">

                        <div style=""padding:40px 0px; width:100%; float:left; background:white;"">
                            <img src=""https://www.icaked.com/Images/Site/main_logo.png"" style=""width:150px; float:left;"" />
                        </div>"
                    + bakeryDeclaration +
                        @"<label style=""float:left; font-size:18px; padding:15px 0px; padding-top:0px; width:100%; border-bottom:1px dashed #3f2518; margin-top:30px; text-align:left; color:#9a4622; line-height:initial;"">Sipariş Bilgileri</label>
                        <div style=""width:100%; float:left; text-align:left; padding:15px 0px;"">
                            <div style=""width:20%; float:left; min-width:130px; color:#9a4622;"">
                                <label style=""font-weight:bold; width:100%; float:left; padding-bottom:15px;"">Pastane : </label>
                                <label style=""font-weight:bold; width:100%; float:left; padding-bottom:15px;"">Sipariş Numarası : </label>
                                <label style=""font-weight:bold; width:100%; float:left; padding-bottom:15px;"">Sipariş Tarihi : </label>
                                <label style=""font-weight:bold; width:100%; float:left; padding-bottom:15px;"">Teslimat Tarihi : </label>
                                <label style=""font-weight:bold; width:100%; float:left; padding-bottom:15px;"">Toplam Fiyat : </label>
                                <label style=""font-weight:bold; width:100%; float:left; padding-bottom:15px;"">Ödeme Şekli : </label>
                                <label style=""font-weight:bold; width:100%; float:left; padding-bottom:15px;"">Sipariş Notları : </label>
                                " + adresLabel +
                          @"</div>
                            <div style=""width:60%; float:left;"">
                                <label style=""float:left; width:100%; padding-bottom:15px;"">" + bakery["Name"] + @"</label>
                                <label style=""float:left; width:100%; padding-bottom:15px;"">" + orderNumber + @"</label>
                                <label style=""float:left; width:100%; padding-bottom:15px;"">" + DateTime.Now + @"</label>
                                <label style=""float:left; width:100%; padding-bottom:15px; font-weight:bold;"">" + deliveryDateTime.Replace("/", ".") + @"</label>
                                <label style=""float:left; width:100%; padding-bottom:15px;"">" + totalPrice + @" TL </label>
                                <label style=""float:left; width:100%; padding-bottom:15px;"">" + paymentType + @"</label>
                                <label style=""float:left; width:100%; padding-bottom:15px;"">" + form["orderNotes"] + @"</label>
                                " + adresStr +
                          @"</div>
                        </div>

                        <label style=""float:left; font-size:18px; padding:15px 0px; padding-top:0px; width:100%; border-bottom:1px dashed #3f2518; margin-top:30px; text-align:left; color:#9a4622; line-height:initial;"">İletişim Bilgileri</label>
                        <div style=""width:100%; float:left; text-align:left; padding:15px 0px;"">
                            <div style=""width:20%; min-width:130px; float:left; color:#9a4622;"">
                                <label style=""font-weight:bold; width:100%; padding-bottom:15px; float:left;"">İsim - Soyisim : </label>
                                <label style=""font-weight:bold; width:100%; padding-bottom:15px; float:left;"">E-Posta : </label>
                                <label style=""font-weight:bold; width:100%; padding-bottom:15px; float:left;"">Telefon : </label>
                            </div>
                            <div style=""width:60%; float:left;"">
                                <label style=""float:left; width:100%; padding-bottom:15px;"">" + form["contactName"] + " " + form["contactSurname"] + @"</label>
                                <label style=""float:left; width:100%; padding-bottom:15px;"">" + form["contactMail"] + @"</label>
                                <label style=""float:left; width:100%; padding-bottom:15px;"">" + form["contactTel"] + @"</label>
                            </div>
                        </div>

                        <label style=""float:left; font-size:18px; padding:15px 0px; padding-top:0px; width:100%; border-bottom:1px dashed #3f2518; margin-top:30px; text-align:left; color:#9a4622; line-height:initial;"">Ürün Bilgileri</label>
                        <div style=""width:100%; float:left; text-align:left; padding:15px 0px;"">
                            <div style=""width:99%; padding:20px 0px; float:left;"">" + madeProductStr + productsStr + @"
                            </div>
                        </div>
                        <div style=""background:#f0f0f0; width:100%; float:left; padding:20px 0px;"">
                            <a href=""http://www.facebook.com/icaked""><img src=""https://www.icaked.com/Images/Site/fb_icon.png"" style=""float:left; margin:3px; width:25px; margin-top:0px; margin-left:15px;"" /></a>
                            <a href=""http://www.twitter.com/icakedcom""><img src=""https://www.icaked.com/Images/Site/tw_icon.png"" style=""float:left; margin:3px; width:25px; margin-top:0px;"" /></a>
                            <a href=""http://www.instagram.com/icakedcom""><img src=""https://www.icaked.com/Images/Site/in_icon.png"" style=""float:left; margin:3px; width:25px; margin-top:0px;"" /></a>
                            <a href=""http://www.facebook.com/icakedcom""><img src=""https://www.icaked.com/Images/Site/pi_icon.png"" style=""float:left; margin:3px; width:25px; margin-top:0px;"" /></a>
                            <a style=""text-decoration:none; float:right;"">
                                <label style=""font-size:14px; line-height:30px; float:right; color:black; margin-right:20px;"">2016 © www.iCaked.com</label>
                            </a>
                        </div>
                    </div>
                </div>
            ";

            // bakery Mail

            String bakeryMail = bakery["Email"].ToString();
            String bakerySubject = "iCaked - Sipariş Bildirimi";

            // customer Mail

            String customerMail = form["contactMail"];
            String customerSubject = "iCaked - Sipariş Bildirimi";

            ac.sendMail("info@icaked.com", customerSubject, customerMailBody, null, null);
            ac.sendMail(customerMail, customerSubject, customerMailBody, null, null);
            ac.sendMail(bakeryMail, bakerySubject, bakeryMailBody, null, null);
        }
    }
}