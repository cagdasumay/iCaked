using Cake.Models;
using nQuant;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.Caching;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Xml.Serialization;

namespace Cake.Controllers
{
    public class DatabaseController : Controller
    {
        private SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["CakeDB"].ConnectionString);
        private SqlConnection con2 = new SqlConnection(ConfigurationManager.ConnectionStrings["GPDB"].ConnectionString);

        public void DisposeCache()
        {
            MemoryCache.Default.Dispose();
        }

        public DataTable MemoryCacheByQuery(string query)
        {
            DataTable dt = (DataTable)MemoryCache.Default[query];
            CacheItemPolicy policy = new CacheItemPolicy();
            policy.AbsoluteExpiration = new DateTimeOffset(DateTime.Now.AddHours(1));

            if (dt == null)
            {
                dt = DBQueryGetter(query);
                MemoryCache.Default.Add(query, dt, policy);
            }

            return dt;
        }

        public JsonResult GetDataSet(string bakery, string orderStart, string orderEnd, string deliveryStart, string deliveryEnd, string priceStart, string priceEnd, string reportPaymentType, string reportOrderStatus, string reportPaymentStatus)
        {
            DataTable tempdt = new DataTable();
            if (Session["bakeryinfo"] == null)
            {
                if (Session["userinfo"] != null)
                {
                    if (((DataRow)Session["userinfo"])["UserID"].ToString().ToUpper() == "3BD93549-9C0A-4C9F-A267-EF3521DA42FE")
                    {
                        if(bakery == "all") { tempdt = DBQueryGetter("select * from OrderLogs"); }
                        else { tempdt = DBQueryGetter("select * from OrderLogs where Bakeries like '%" + new Guid(bakery) + "%'"); }
                    }
                }
            }
            else
            {
                tempdt = DBQueryGetter("select * from OrderLogs where BakeryID = '" + ((DataRow)Session["bakeryinfo"])["BakeryID"] + "'");
            }

            DataTable dt = tempdt.Clone();
            List<string[]> dataset = new List<string[]>();

            string upperPrice = ""; string lowerPrice = "";
            if (Convert.ToDouble(priceEnd) > Convert.ToDouble(priceStart)) { upperPrice = priceEnd; lowerPrice = priceStart; } else { upperPrice = priceStart; lowerPrice = priceEnd; }

            try
            {
                for (int i = 0; i < tempdt.Rows.Count; i++)
                {
                    bool orderApproved = false;
                    bool deliveryApproved = false;
                    bool priceApproved = false;
                    bool paymentTypeApproved = false;
                    bool paymentStatusApproved = false;
                    bool orderStatusApproved = false;
                    DataRow row = tempdt.Rows[i];
                    if (Convert.ToDouble(row["TotalPrice"]) <= Convert.ToDouble(priceEnd.Replace(".", ",")) & Convert.ToDouble(row["TotalPrice"]) >= Convert.ToDouble(priceStart.Replace(".", ","))) { priceApproved = true; }
                    if (DateTime.Compare(Convert.ToDateTime(row["DateTime"]), Convert.ToDateTime(orderStart)) >= 0 & DateTime.Compare(Convert.ToDateTime(row["DateTime"]), Convert.ToDateTime(orderEnd)) <= 0) { orderApproved = true; }
                    DateTime deliveryDate = Convert.ToDateTime(row["DeliveryDateTime"].ToString().Split(' ')[0]);
                    if (DateTime.Compare(deliveryDate, Convert.ToDateTime(deliveryStart)) >= 0 & DateTime.Compare(deliveryDate, Convert.ToDateTime(deliveryEnd)) <= 0) { deliveryApproved = true; }
                    if (reportPaymentType == "all") { paymentTypeApproved = true; } else { if (row["PaymentType"].ToString() == reportPaymentType) { paymentTypeApproved = true; } }
                    if (reportOrderStatus == "all") { orderStatusApproved = true; } else { if (row["Status"].ToString() == reportOrderStatus) { orderStatusApproved = true; } }
                    if (reportPaymentStatus == "all") { paymentStatusApproved = true; } else { if (row["PaymentApproved"].ToString() == reportPaymentStatus) { paymentStatusApproved = true; } }
                    if (orderApproved & deliveryApproved & priceApproved & paymentTypeApproved & paymentStatusApproved & orderStatusApproved) { dt.Rows.Add(row.ItemArray); }
                }
            }
            catch (Exception exp) { }

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                DataRow row = dt.Rows[i];
                string[] array = new string[7];
                array[0] = row["DateTime"].ToString();
                array[1] = row["DeliveryDateTime"].ToString();
                array[2] = row["OrderNumber"].ToString();
                array[3] = row["Status"].ToString();

                if(bakery != "all")
                {
                    String totalPrice = "0";
                    String[] bakeries = row["Bakeries"].ToString().Split(' ');
                    String[] productPrices = row["ProductPrices"].ToString().Split(' ');
                    for (int x = 0; x < bakeries.Length; x++)
                    {
                        if (bakeries[x].ToString().ToLower() == bakery.ToLower())
                        {
                            totalPrice = (Convert.ToDouble(totalPrice.Replace(".", ",")) + Convert.ToDouble(productPrices[x].Replace(".", ","))).ToString();
                        }
                    }
                    array[4] = totalPrice + " TL";
                }
                else { array[4] = row["TotalPrice"].ToString() + " TL"; }             

                string temp = ""; string approved = "ÖDENMEDİ";
                if (row["PaymentType"].ToString() == "POS") { temp = "İnternetten kredi kartı ile ödeme"; } else { temp = row["PaymentType"].ToString(); }
                if (row["PaymentApproved"].ToString() == "yes") { approved = "ÖDENDİ"; }

                array[5] = temp;
                array[6] = approved;
                dataset.Add(array);
            }

            return Json(dataset, JsonRequestBehavior.AllowGet);
        }

        //public void DBCorrector()
        //{
        //    DataTable objects = DBQueryGetter("select * from Objects");

        //    for (int i = 0; i < objects.Rows.Count; i++)
        //    {
        //        DataRow row = objects.Rows[i];
        //        if (row["ImagePath"] != null)
        //        {
        //            if (String.IsNullOrEmpty(row["ImagePath"].ToString()) == false)
        //            {
        //                String newImagePath = row["ObjectName"].ToString() + "_image.png";
        //                if (row["ImagePath"].ToString() != newImagePath)
        //                {
        //                    DBQuerySetter("update Objects set ImagePath = '" + newImagePath + "' where ObjectName = '" + row["ObjectName"] + "' and Type = '" + row["Type"] + "'");
        //                }
        //            }
        //        }

        //        if (row["TexturePath"] != null)
        //        {
        //            if (String.IsNullOrEmpty(row["TexturePath"].ToString()) == false)
        //            {
        //                String newTexturePath = row["ObjectName"].ToString() + "_texture.png";
        //                if (row["TexturePath"].ToString() != newTexturePath)
        //                {
        //                    DBQuerySetter("update Objects set TexturePath = '" + newTexturePath + "' where ObjectName = '" + row["ObjectName"] + "' and Type = '" + row["Type"] + "'");
        //                }
        //            }
        //        }

        //        if (row["AlphaPath"] != null)
        //        {
        //            if (String.IsNullOrEmpty(row["AlphaPath"].ToString()) == false)
        //            {
        //                String newAlphaPath = row["ObjectName"].ToString() + "_alpha.png";
        //                if (row["AlphaPath"].ToString() != newAlphaPath)
        //                {
        //                    DBQuerySetter("update Objects set AlphaPath = '" + newAlphaPath + "' where ObjectName = '" + row["ObjectName"] + "' and Type = '" + row["Type"] + "'");
        //                }
        //            }
        //        }
        //    }
        //}

        //public void productDateCorrector()
        //{
        //    DataTable products = DBQueryGetter("select * from Products");
        //    for(int i = 0; i < products.Rows.Count; i++)
        //    {
        //        DataRow product = products.Rows[i];
        //        String addDate = product["AddDate"].ToString();
        //        String date = addDate.Split(' ')[0];
        //        String day = date.Split('/')[0]; String month = date.Split('/')[1]; String year = date.Split('/')[2];
        //        if (day.Length == 1) { day = "0" + day; }
        //        if (month.Length == 1) { month = "0" + month; }
        //        String newDate = month + "/" + day + "/" + year + " " + addDate.Split(' ')[1];
        //        DBQuerySetter("update Products set AddDate = '" + newDate + "' where ProductID = '" + new Guid(product["ProductID"].ToString()) + "'");
        //    }
        //}

        //public void ratingCorrector()
        //{
        //    DataTable products = DBQueryGetter("select * from Products where Rating = '0' and Priority = '1'");
        //    for (int i = 0; i < products.Rows.Count; i++)
        //    {
        //        Random rnd = new Random();
        //        double temp = rnd.NextDouble() * 1.5 + 3.5;
        //        string newRating = string.Format("{0:0.0}", Math.Truncate(temp * 10) / 10).Replace(",", ".");
        //        DBQuerySetter("update Products set Rating = '" + newRating + "' where ProductID = '" + new Guid(products.Rows[i]["ProductID"].ToString()) + "'");
        //    }
        //}

        //public void reviewCorrector()
        //{
        //    DataTable products = DBQueryGetter("select * from Products where Review is NULL and Priority = '1'");
        //    for (int i = 0; i < products.Rows.Count; i++)
        //    {
        //        Random rnd = new Random();
        //        double temp = rnd.Next(10, 50);
        //        string newReview = temp.ToString();
        //        DBQuerySetter("update Products set Review = '" + newReview + "' where ProductID = '" + new Guid(products.Rows[i]["ProductID"].ToString()) + "'");
        //    }
        //}

        //public void orderBakeryCorrector()
        //{
        //    DataTable orders = DBQueryGetter("select * from OrderLogs");
        //    for (int i = 0; i < orders.Rows.Count; i++)
        //    {
        //        Guid orderID = new Guid(orders.Rows[i]["OrderID"].ToString());
        //        string[] productPrices = orders.Rows[i]["ProductPrices"].ToString().Split(' ');
        //        string bakeryID = ""; string bakeryName = "";
        //        for(int i2 = 0; i2 < productPrices.Length; i2++)
        //        {
        //            bakeryID = bakeryID + orders.Rows[i]["BakeryID"].ToString() + " "; bakeryName = bakeryName + getBakeryNameByID(new Guid(orders.Rows[i]["BakeryID"].ToString())) + " ";
        //        }
        //        bakeryID = bakeryID.Substring(0,bakeryID.Length - 1); bakeryName = bakeryName.Substring(0, bakeryName.Length - 1);
        //        DBQuerySetter("update OrderLogs set Bakeries = '" + bakeryID.ToUpper() + "',BakeryNames = N'" + bakeryName + "' where OrderID = '" + orderID + "'");
        //    }
        //}

        //public void DBTextureCorrector()
        //{
        //    string[] objPaths = Directory.GetFiles(HttpRuntime.AppDomainAppPath + "\\Objects\\Sources", "*.obj").Select(Path.GetFileName).ToArray();
        //    string[] texturePaths = Directory.GetFiles(HttpRuntime.AppDomainAppPath + "\\Objects\\Textures", "*.png").Select(Path.GetFileName).ToArray();
        //    for (int i = 0; i < objPaths.Length; i++)
        //    {
        //        string objname = objPaths[i].Replace(".obj", "");
        //        if (texturePaths.Contains(objname + "_texture.png") == false)
        //        {
        //            DBQuerySetter("update Objects set TexturePath = '' where ObjectName = '" + objname + "'");
        //        }
        //    }
        //    String xx = "";
        //}

        //public void ReadExcel()
        //{
        //    DataTable objects = DBQueryGetter("select * from Objects");

        //    String ecxelPath = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\Users\Cagdas Umay\Desktop\categories.xlsx;Extended Properties=Excel 12.0 Xml";
        //    using (OleDbConnection connection = new OleDbConnection(ecxelPath))
        //    {
        //        connection.Open();
        //        OleDbCommand command = new OleDbCommand("select * from [Sheet1$]", connection);
        //        using (OleDbDataReader dr = command.ExecuteReader())
        //        {
        //            while (dr.Read())
        //            {
        //                String objname = dr["ObjectName"].ToString();
        //                String type = dr["Type"].ToString();
        //                String keywords = objname.Replace("_", " ") + "," + dr["Keywords"].ToString().Replace("_", " ");
        //                String categories = dr["Category"].ToString();
        //                if (type == "cake" | type == "object")
        //                {
        //                    DBQuerySetter("update Objects set keywords = N'" + keywords + "',category = N'" + categories + "' where ObjectName = '" + objname + "' and Type = '" + type + "'");
        //                }
        //                else
        //                {
        //                    DBQuerySetter("update Objects set keywords = N'" + DBNull.Value + "',category = N'" + DBNull.Value + "' where ObjectName = '" + objname + "' and Type = '" + type + "'");
        //                }

        //            }
        //        }
        //    }

        //    for (int i = 0; i < objects.Rows.Count; i++)
        //    {
        //        DataRow obj = objects.Rows[i];
        //        if (obj["Type"].ToString() == "random" | obj["Type"].ToString() == "side" | obj["Type"].ToString() == "object2D")
        //        {
        //            DBQuerySetter("update Objects set keywords = '" + DBNull.Value + "', category = '" + DBNull.Value + "' where ObjectName = '" + obj["ObjectName"] + "' and Type = '" + obj["Type"] + "'");
        //        }
        //    }
        //}

        //public void ReadExcel2()
        //{
        //    DataTable objects = DBQueryGetter("select * from Objects");

        //    String ecxelPath = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\Users\Cagdas Umay\Desktop\object2D_categories.xlsx;Extended Properties=Excel 12.0 Xml";
        //    using (OleDbConnection connection = new OleDbConnection(ecxelPath))
        //    {
        //        connection.Open();
        //        OleDbCommand command = new OleDbCommand("select * from [Sheet1$]", connection);
        //        using (OleDbDataReader dr = command.ExecuteReader())
        //        {
        //            while (dr.Read())
        //            {
        //                String objname = dr["ObjectName"].ToString();
        //                String keywords = dr["Keywords"].ToString();

        //                for (int i = 0; i < objects.Rows.Count; i++)
        //                {
        //                    if (objects.Rows[i]["ObjectName"].ToString() == objname)
        //                    {
        //                        string[] temp = objects.Rows[i]["Keywords"].ToString().Split(',');
        //                        keywords = temp[0] + "," + keywords;
        //                        DBQuerySetter("update Objects set keywords = N'" + keywords + "' where ObjectName = '" + objname + "' and Type = 'object2D'");
        //                    }
        //                }
        //            }
        //        }
        //    }
        //}

        //public void ReadExcel3()
        //{
        //    DataTable objects = DBQueryGetter("select * from Objects");

        //    String ecxelPath = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\Users\Cagdas Umay\Desktop\obj_price_table2.xlsx;Extended Properties=Excel 12.0 Xml";
        //    using (OleDbConnection connection = new OleDbConnection(ecxelPath))
        //    {
        //        connection.Open();
        //        OleDbCommand command = new OleDbCommand("select * from [Sayfa1$]", connection);
        //        using (OleDbDataReader dr = command.ExecuteReader())
        //        {
        //            while (dr.Read())
        //            {
        //                String objname = dr["ObjectName"].ToString();
        //                String type = dr["Type"].ToString();
        //                String minSize = dr["MinSize"].ToString().Replace(",", ".");
        //                String maxSize = dr["MaxSize"].ToString().Replace(",", ".");
        //                String difficulty = dr["Difficulty"].ToString();
        //                String stepSize = dr["StepSize"].ToString().Replace(",", ".");

        //                DBQuerySetter("update Objects set MinSize = '" + minSize + "', MaxSize = '" + maxSize + "', Difficulty = '" + difficulty + "', StepSize = '" + stepSize + "' where ObjectName = '" + objname + "' and Type = '" + type + "'");
        //            }
        //        }
        //    }
        //}

        //public void ReadExcel4()
        //{
        //    DataTable objects = DBQueryGetter("select * from Products where Category like '%Pasta%'");

        //    String ecxelPath = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\Users\Cagdas Umay\Desktop\categories.xlsx;Extended Properties=Excel 12.0 Xml";
        //    using (OleDbConnection connection = new OleDbConnection(ecxelPath))
        //    {
        //        connection.Open();
        //        OleDbCommand command = new OleDbCommand("select * from [Sayfa1$]", connection);
        //        using (OleDbDataReader dr = command.ExecuteReader())
        //        {
        //            while (dr.Read())
        //            {
        //                String name = dr["Name"].ToString();
        //                String subcat = dr["SubCategory"].ToString();

        //                DBQuerySetter("update Products set Category = N'" + subcat + "' where Name = N'" + name + "'");
        //            }
        //        }
        //    }
        //}

        public JsonResult GetStatistics()
        {
            int eDGP = 0; int eSP = 0; int eKP = 0; int eCFP = 0; int e1YP = 0; int eBP = 0;
            int hDGP = 0; int hSP = 0; int hKP = 0; int hCFP = 0; int h1YP = 0; int hBP = 0; int hYAS = 0; int hP = 0; int hKK = 0; int hC = 0;
            int liva = 0; int ferlife = 0; int karameli = 0; int pastasanati = 0; int parti = 0;

            int tDGP = 0; int tSP = 0; int tKP = 0; int tCFP = 0; int t1YP = 0; int tBP = 0;
            int hhDGP = 0; int hhSP = 0; int hhKP = 0; int hhCFP = 0; int hh1YP = 0; int hhBP = 0; int hhYAS = 0; int hhP = 0; int hhKK = 0; int hhC = 0;

            DataTable orders = DBQueryGetter("select * from OrderLogs");
            DataTable madeCakes = DBQueryGetter("select * from MadeCakes");
            DataTable products = DBQueryGetter("select * from Products");

            int totalDBProduct = products.Rows.Count + madeCakes.Rows.Count;
            int totalOrder = orders.Rows.Count;
            int totalMember = DBQueryGetter("select * from Users").Rows.Count;
            int totalComment = DBQueryGetter("select * from ProductComments").Rows.Count + DBQueryGetter("select * from DesignComments").Rows.Count;
            int hazirUrun = 0;
            int tasarimPastaHazir = 0;
            int tasarimPastaEditor = madeCakes.Rows.Count;
            int livaProduct = 0; int karameliProduct = 0; int ferlifeProduct = 0; int pastasanatiProduct = 0; int partiProduct = 0;

            for (int i = 0; i < products.Rows.Count; i++)
            {
                DataRow product = products.Rows[i];
                if(product["DesignProduct"].ToString() == "yes") { tasarimPastaHazir++; } else { hazirUrun++; }
                String bakeryID = product["BakeryID"].ToString().ToUpper();
                if (bakeryID == "CB5EDC14-85FD-40AD-A79A-82D6CA460CD9") { livaProduct++; }
                else if (bakeryID == "8A40EEF5-8D42-4D32-A8A8-64F9F4E24A1B") { karameliProduct++; }
                else if (bakeryID == "2B63C55F-5B84-488C-A54A-FA2F0EE565F3") { ferlifeProduct++; }
                else if (bakeryID == "488BD6DC-555D-4933-A3EA-9F20E44E05D5") { pastasanatiProduct++; }
                else if (bakeryID == "98E94E0A-C764-4F99-A2F1-F4915631A891") { partiProduct++; }

                String category = product["Category"].ToString();
                if (category == "Doğum Günü Pastaları") { hhDGP++; }
                else if (category == "Sevgili Pastaları") { hhSP++; }
                else if (category == "Kutlama Pastaları") { hhKP++; }
                else if (category == "Çizgi Film Pastaları") { hhCFP++; }
                else if (category == "1 Yaş Pastaları") { hh1YP++; }
                else if (category == "Bebek Pastaları") { hhBP++; }
                else if (category == "Yaş Pasta") { hhYAS++; }
                else if (category == "Parti Malzemeleri") { hhP++; }
                else if (category == "Kek/Kurabiye") { hhKK++; }
                else if (category == "Çikolata") { hhC++; }
            }

            for(int i = 0; i < madeCakes.Rows.Count; i++)
            {
                DataRow madeCake = madeCakes.Rows[i];
                String category = madeCake["Category"].ToString();
                if (category == "Doğum Günü Pastaları") { tDGP++; }
                else if (category == "Sevgili Pastaları") { tSP++; }
                else if (category == "Kutlama Pastaları") { tKP++; }
                else if (category == "Çizgi Film Pastaları") { tCFP++; }
                else if (category == "1 Yaş Pastaları") { t1YP++; }
                else if (category == "Bebek Pastaları") { tBP++; }
            }

            for (int i = 0; i < orders.Rows.Count; i++)
            {
                DataRow order = orders.Rows[i];
                if(String.IsNullOrEmpty(order["MadeID"].ToString()) == false)
                {
                    for(int i2 = 0; i2 < madeCakes.Rows.Count; i2++)
                    {
                        DataRow madeCake = madeCakes.Rows[i2];
                        if(madeCake["MadeID"].ToString().ToLower() == order["MadeID"].ToString().ToLower())
                        {
                            String category = madeCake["Category"].ToString();
                            if(category == "Doğum Günü Pastaları") { eDGP++; }
                            else if (category == "Sevgili Pastaları") { eSP++; }
                            else if (category == "Kutlama Pastaları") { eKP++; }
                            else if (category == "Çizgi Film Pastaları") { eCFP++; }
                            else if (category == "1 Yaş Pastaları") { e1YP++; }
                            else if (category == "Bebek Pastaları") { eBP++; }
                            break;
                        }
                    }
                }

                String[] productIDs = order["ProductIDs"].ToString().Split(' ');
                for(int i3 = 0; i3 < productIDs.Length; i3++)
                {
                    for (int i2 = 0; i2 < products.Rows.Count; i2++)
                    {
                        DataRow product = products.Rows[i2];
                        if (product["ProductID"].ToString().ToLower() == productIDs[i3].ToString().ToLower())
                        {
                            String category = product["Category"].ToString();
                            if (category == "Doğum Günü Pastaları") { hDGP++; }
                            else if (category == "Sevgili Pastaları") { hSP++; }
                            else if (category == "Kutlama Pastaları") { hKP++; }
                            else if (category == "Çizgi Film Pastaları") { hCFP++; }
                            else if (category == "1 Yaş Pastaları") { h1YP++; }
                            else if (category == "Bebek Pastaları") { hBP++; }
                            else if (category == "Yaş Pasta") { hYAS++; }
                            else if (category == "Parti Malzemeleri") { hP++; }
                            else if (category == "Kek/Kurabiye") { hKK++; }
                            else if (category == "Çikolata") { hC++; }
                            break;
                        }
                    }
                }

                String bakeryID = order["BakeryID"].ToString().ToUpper();
                if(bakeryID == "CB5EDC14-85FD-40AD-A79A-82D6CA460CD9") { liva = liva + productIDs.Length; }
                else if (bakeryID == "8A40EEF5-8D42-4D32-A8A8-64F9F4E24A1B") { karameli = karameli + productIDs.Length; }
                else if (bakeryID == "2B63C55F-5B84-488C-A54A-FA2F0EE565F3") { ferlife = ferlife + productIDs.Length; }
                else if (bakeryID == "488BD6DC-555D-4933-A3EA-9F20E44E05D5") { pastasanati = pastasanati + productIDs.Length; }
                else if (bakeryID == "98E94E0A-C764-4F99-A2F1-F4915631A891") { parti = parti + productIDs.Length; }
            }

            int totalProduct = liva + karameli + ferlife + pastasanati + parti;

            return Json(new { eDGP = eDGP, eSP = eSP, eKP = eKP, eCFP = eCFP, e1YP = e1YP, eBP = eBP, tDGP = tDGP, tSP = tSP, tKP = tKP, tCFP = tCFP, t1YP = t1YP, tBP = tBP, hDGP = hDGP, hSP = hSP, hKP = hKP, hCFP = hCFP, h1YP = h1YP, hBP = hBP, hYAS = hYAS, hP = hP, hKK = hKK, hC = hC, totalOrder = totalOrder, totalProduct = totalProduct, liva = liva, karameli = karameli, ferlife = ferlife, pastasanati = pastasanati, parti = parti, totalDBProduct = totalDBProduct, totalMember = totalMember, totalComment = totalComment, hazirUrun = hazirUrun, tasarimPastaEditor = tasarimPastaEditor, tasarimPastaHazir = tasarimPastaHazir, livaProduct = livaProduct, karameliProduct = karameliProduct, ferlifeProduct = ferlifeProduct, pastasanatiProduct = pastasanatiProduct, partiProduct = partiProduct, hhDGP = hhDGP, hhSP = hhSP, hhKP = hhKP, hhCFP = hhCFP, hh1YP = hh1YP, hhBP = hhBP, hhYAS = hhYAS, hhP = hhP, hhKK = hhKK, hhC = hhC }, JsonRequestBehavior.AllowGet);
        }

        public DataTable DBQueryGetter(String query)
        {
            DataTable table = new DataTable();
            SqlCommand cmd = new SqlCommand(query, con);
            SqlDataAdapter da = new SqlDataAdapter(cmd);

            try
            {
                if (injectionFilter(query) == false)
                {
                    con.Open();
                    da.Fill(table);
                    con.Close();
                }
            }
            catch (Exception e) { con.Close(); }

            return table;
        }

        public bool DBQuerySetter(String query)
        {
            try
            {
                if (injectionFilter(query) == false)
                {
                    SqlCommand cmd = new SqlCommand(query, con);
                    con.Open();
                    int i = cmd.ExecuteNonQuery();
                    con.Close();
                    if (i == 0) { return false; } else { return true; }
                }
                else { return false; }
            }
            catch (Exception e) { con.Close(); return false; }
        }

        public DataTable DBQueryGetterGP(String query)
        {
            DataTable table = new DataTable();
            SqlCommand cmd = new SqlCommand(query, con2);
            SqlDataAdapter da = new SqlDataAdapter(cmd);

            try
            {
                if (injectionFilter(query) == false)
                {
                    con2.Open();
                    da.Fill(table);
                    con2.Close();
                }
            }
            catch (Exception e) { con2.Close(); }

            return table;
        }

        public bool DBQuerySetterGP(String query)
        {
            try
            {
                if (injectionFilter(query) == false)
                {
                    SqlCommand cmd = new SqlCommand(query, con2);
                    con2.Open();
                    int i = cmd.ExecuteNonQuery();
                    con2.Close();
                    if (i == 0) { return false; } else { return true; }
                }
                else { return false; }
            }
            catch (Exception e) { con2.Close(); return false; }
        }

        public bool injectionFilter(String query)
        {
            string[] filter = new string[] { "where", "select", "from", "delete", "drop", "alter table", "table", "insert into", "update", "set", "join", "script", "body", "alert", "insert" };
            bool injectionExists = false;

            Match match = Regex.Match(query, @"'([^']*)");
            if (match.Success)
            {
                string matchedValue = match.Groups[1].Value;
                if (filter.Contains(matchedValue)) { injectionExists = true; }
            }

            if (injectionExists) { LogAttack(); return true; } else { return false; }
        }

        public void LogAttack()
        {
            string apiUrl = "http://freegeoip.net/xml/92.45.178.57"/* + Request.UserHostAddress*/;

            HttpClient client = new HttpClient();

            var response = client.GetAsync(apiUrl).Result;

            if (response != null && response.ReasonPhrase != "Unauthorized")
            {
                var resp = response.Content.ReadAsStringAsync();
                GeoLocation geo = new GeoLocation();
                geo.Response = new Response();

                XmlSerializer serializer = new XmlSerializer(typeof(Response));
                using (TextReader reader = new StringReader(resp.Result))
                {
                    try
                    {
                        geo.Response = (Response)serializer.Deserialize(reader);

                        string query = "insert into AttackLogs (ID,AttackerIP,LogReason,CountryCode,CountryName,RegionCode,RegionName,City,ZipCode,TimeZone,Latitude,Longitude,MetroCode) values ('" + Guid.NewGuid() + "','" + geo.Response.IP + "','SQL Injection','" + geo.Response.CountryCode + "','" + geo.Response.CountryName + "','" + geo.Response.RegionCode + "','" + geo.Response.RegionName + "','" + geo.Response.City + "','" + geo.Response.ZipCode + "','" + geo.Response.TimeZone + "','" + geo.Response.Latitude + "','" + geo.Response.Longitude + "','" + geo.Response.MetroCode + "')";
                        SqlCommand cmd = new SqlCommand(query, con);
                        con.Open();
                        int i = cmd.ExecuteNonQuery();
                        con.Close();
                    }
                    catch (Exception e) { }
                }
            }
        }

        public String getBakeryNameFromProduct(Guid ProductID)
        {
            Guid bakeryID = new Guid(DBQueryGetter("select BakeryID from Products where ProductID = '" + ProductID + "'").Rows[0][0].ToString());
            return MemoryCacheByQuery("select Name from Bakeries where BakeryID = '" + bakeryID + "'").Rows[0][0].ToString();
        }

        public String getBakeryNameByID(Guid BakeryID)
        {
            return MemoryCacheByQuery("select Name from Bakeries where BakeryID = '" + BakeryID + "'").Rows[0][0].ToString();
        }

        public String getProductNameByID(Guid ProductID)
        {
            return MemoryCacheByQuery("select Name from Products where ProductID = '" + ProductID + "'").Rows[0][0].ToString();
        }

        public String getDesignNameByID(Guid MadeID)
        {
            return MemoryCacheByQuery("select Name from MadeCakes where MadeID = '" + MadeID + "'").Rows[0][0].ToString();
        }

        public String getUsernameByID(Guid UserID)
        {
            DataTable dt = MemoryCacheByQuery("select Name,Surname from Users where UserID = '" + UserID + "'");
            return dt.Rows[0][0] + " " + dt.Rows[0][1];
        }

        public DataTable ArrangeLocationTable(DataTable dt, string sortCol, bool isLowerCaseWanted, string lowerCol)
        {
            if (String.IsNullOrEmpty(sortCol) == false) { DataView view = dt.DefaultView; view.Sort = sortCol + " asc"; dt = view.ToTable(); }
            if (isLowerCaseWanted) { for (int i = 0; i < dt.Rows.Count; i++) { dt.Rows[i][lowerCol] = dt.Rows[i][lowerCol].ToString().Substring(0, 1) + dt.Rows[i][lowerCol].ToString().Substring(1, dt.Rows[i][lowerCol].ToString().Length - 1).ToLower(); } }
            return dt;
        }

        public DataTable sortByDate(DataTable dt)
        {
            try { dt.Columns.Add("sortDate", typeof(DateTime)); } catch (Exception e) { }

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                dt.Rows[i]["sortDate"] = Convert.ToDateTime(dt.Rows[i]["Datetime"]);
            }

            DataView view = dt.DefaultView;
            view.Sort = "sortDate desc";
            return view.ToTable();
        }

        public DataTable sortByDateCol(DataTable dt,string colName,string ascdesc)
        {
            try { dt.Columns.Add("sortDate", typeof(DateTime)); } catch (Exception e) { }

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                dt.Rows[i]["sortDate"] = Convert.ToDateTime(dt.Rows[i][colName]);
            }

            DataView view = dt.DefaultView;
            view.Sort = "sortDate " + ascdesc;
            return view.ToTable();
        }

        public DataTable sortTable(DataTable product_table, String type)
        {
            DataView view = product_table.DefaultView;
            view.Sort = type;
            return view.ToTable();
        }

        public String getIdFromName(string name, string type)
        {
            DataTable dt = new DataTable();
            if (type == "District") { dt = MemoryCacheByQuery("select ilceId from Ilceler where IlceAdi like N'" + name + "'"); }
            else if (type == "Province") { dt = MemoryCacheByQuery("select SehirId from Sehirler where SehirAdi like N'" + name + "'"); }
            else { dt = MemoryCacheByQuery("select SemtMahId from SemtMah where MahalleAdi like N'" + name + "'"); }
            String res = dt.Rows[0][0].ToString();
            return res;
        }

        public String getNameFromId(string id, string type)
        {
            DataTable dt = new DataTable();
            if (type == "District") { dt = MemoryCacheByQuery("select IlceAdi from Ilceler where ilceId = '" + Convert.ToInt32(id) + "'"); }
            else if (type == "Province") { dt = MemoryCacheByQuery("select SehirAdi from Sehirler where SehirId = '" + Convert.ToInt32(id) + "'"); }
            else { dt = MemoryCacheByQuery("select MahalleAdi from SemtMah where SemtMahId = '" + Convert.ToInt32(id) + "'"); }
            String res = dt.Rows[0][0].ToString();
            return res.Substring(0, 1) + res.Substring(1, res.Length - 1).ToLower();
        }

        public DataTable getDistrictsOfProvince(string id)
        {
            DataTable dt = new DataTable();
            dt = MemoryCacheByQuery("select * from Ilceler where SehirId = '" + id + "'");
            for (int i = 0; i < dt.Rows.Count; i++) { String temp = dt.Rows[i]["IlceAdi"].ToString(); dt.Rows[i]["IlceAdi"] = temp[0] + temp.Substring(1, temp.Length - 1).ToLower(); }
            return dt;
        }

        public DataTable getNeighborhoodsOfDistrict(string id)
        {
            DataTable dt = new DataTable();
            dt = MemoryCacheByQuery("select * from SemtMah where ilceId = '" + id + "'");
            for (int i = 0; i < dt.Rows.Count; i++) { String temp = dt.Rows[i]["MahalleAdi"].ToString(); dt.Rows[i]["MahalleAdi"] = temp[0] + temp.Substring(1, temp.Length - 1).ToLower(); }
            return dt;
        }

        public DataRow getUserByID(string id)
        {
            return MemoryCacheByQuery("select * from Users where UserID = '" + new Guid(id) + "'").Rows[0];
        }

        public String getLocationNameFromID(String id, String type)
        {
            String result = "";
            if (type == "Province") { result = MemoryCacheByQuery("select SehirAdi from Sehirler where  SehirId = '" + Convert.ToInt32(id) + "'").Rows[0][0].ToString(); }
            else if (type == "District") { result = MemoryCacheByQuery("select IlceAdi from Ilceler where  ilceId = '" + Convert.ToInt32(id) + "'").Rows[0][0].ToString(); }
            else if (type == "Neighborhood") { result = MemoryCacheByQuery("select MahalleAdi from SemtMah where  SemtMahId = '" + Convert.ToInt32(id) + "'").Rows[0][0].ToString(); }
            else if (type == "Town") { result = MemoryCacheByQuery("select SemtAdi from SemtMah where  SemtMahId = '" + Convert.ToInt32(id) + "' ").Rows[0][0].ToString(); }
            return result;
        }

        public List<DataRow> ArrangeProductsByBakery(DataTable dt)
        {
            List<DataRow> toplanmisRowlar = new List<DataRow>();
            DataTable result = new DataTable();
            List<string> bakeryIds = new List<string>();
            List<List<DataRow>> productByBakery = new List<List<DataRow>>();

            for(int i = 0; i < dt.Rows.Count; i++)
            {
                DataRow row = dt.Rows[i];
                if (!bakeryIds.Contains(row["BakeryID"].ToString())) { bakeryIds.Add(row["BakeryID"].ToString()); productByBakery.Add(new List<DataRow>()); }
                int index = bakeryIds.FindIndex(x => x.StartsWith(row["BakeryID"].ToString()));
                productByBakery[index].Add(row);
            }

            int maxLength = 0;
            for(int i = 0; i < productByBakery.Count; i++)
            {
                if(productByBakery[i].Count > maxLength) { maxLength = productByBakery[i].Count; }
            }

            int idx = 0;
            for (int i2 = 0; i2 < maxLength; i2++)
            {
                for (int i = 0; i < productByBakery.Count; i++)
                {
                    try { toplanmisRowlar.Add(productByBakery[i][idx]); }catch(Exception e) { }            
                }
                idx++;
            }

            return toplanmisRowlar;
        }

        //public void copyProducts()
        //{
        //    //DataTable bakeryProducts = DBQueryGetter("select * from Products where BakeryID in ('CB5EDC14-85FD-40AD-A79A-82D6CA460CD9','488BD6DC-555D-4933-A3EA-9F20E44E05D5','2B63C55F-5B84-488C-A54A-FA2F0EE565F3','8A40EEF5-8D42-4D32-A8A8-64F9F4E24A1B')");
        //    DataTable bakeryProducts = DBQueryGetter("select * from Products where BakeryID in ('CB5EDC14-85FD-40AD-A79A-82D6CA460CD9')");

        //    for (int i = 0; i < bakeryProducts.Rows.Count; i++)
        //    {
        //        try
        //        {
        //            String source = HttpRuntime.AppDomainAppPath + "Images\\Products\\" + bakeryProducts.Rows[i]["ProductID"].ToString();
        //            String[] files = Directory.GetFiles(source);

        //            for (int i2 = 0; i2 < files.Length; i2++)
        //            {
        //                String destination = "C:\\Users\\Cagdas Umay\\Desktop\\products\\" + Path.GetFileName(files[i2]);
        //                ImageFormat format = null;
        //                if (destination.Contains(".png")) { format = ImageFormat.Png; } else { format = ImageFormat.Jpeg; }
        //                Image img = Image.FromFile(files[i2]);
        //                Image scaledimg = ScaleImage(img, 256, 256);
        //                scaledimg.Save(destination);
        //                scaledimg.Dispose();
        //            }
        //        }
        //        catch(Exception e) { }
        //    }
        //}

        //public static Image ScaleImage(Image image, int newWidth, int newHeight)
        //{
        //    var newImage = new Bitmap(newWidth, newHeight);

        //    using (var graphics = Graphics.FromImage(newImage))
        //    {
        //        graphics.DrawImage(image, 0, 0, newWidth, newHeight);
        //    }

        //    image.Dispose();
        //    return newImage;
        //}

        //public void InsertImages()
        //{
        //    String[] files = Directory.GetFiles("C:\\Users\\Cagdas Umay\\Desktop\\compressed");

        //    for (int i = 0; i < files.Length; i++)
        //    {
        //        String uid = Path.GetFileName(files[i]).Replace(".jpg","");
        //        uid = uid.Substring(0,uid.Length-2);

        //        String dirPath = "C:\\Users\\Cagdas Umay\\Desktop\\imgs\\" + uid;
        //        String filePath = dirPath + "\\" + Path.GetFileName(files[i]);
        //        if (Directory.Exists(dirPath) == false) { Directory.CreateDirectory(dirPath); }
        //        System.IO.File.Copy(files[i], filePath);           
        //    }

        //}

    }
}