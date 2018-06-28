using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using EventApp.EventApp;
using EventApp.Models;
using System.Drawing;
using System.Globalization;

namespace EventApp.Controllers
{
    public class EventsController : Controller
    {
        private EventAppContext db = new EventAppContext();

        public ActionResult SocialMedia()
        {
            User LoggedUser = (User)Session["LoggedIn"];
            List<Event> AllEvents = db.Event.ToList();
            var TE = AllEvents.Where(q => q.EventConfirmation == LoggedUser.AccountConfirmation).ToList();
            Event ThisEvent = db.Event.Find(TE[0].Id);

            ViewData["ThisEvent"] = ThisEvent;
            return View();
        }

        public ActionResult SocialMediaOrg()
        {
            User LoggedUser = (User)Session["LoggedIn"];
            List<Event> AllEvents = db.Event.ToList();
            var TE = AllEvents.Where(q => q.EventConfirmation == LoggedUser.AccountConfirmation).ToList();
            Event ThisEvent = db.Event.Find(TE[0].Id);

            ViewData["ThisEvent"] = ThisEvent;
            return View();
        }

        public ActionResult EditSocialMedia()
        {
            User LoggedUser = (User)Session["LoggedIn"];
            List<Event> AllEvents = db.Event.ToList();
            var TE = AllEvents.Where(q => q.EventConfirmation == LoggedUser.AccountConfirmation).ToList();
            Event ThisEvent = db.Event.Find(TE[0].Id);
            ViewData["EventName"] = ThisEvent.Name;
            ViewData["EventID"] = ThisEvent.Id;

            return View(ThisEvent);
        }

        public ActionResult CompleteSocialMedia(Event evnt, Guid EventID)
        {
            Event ThisEvent = db.Event.Find(EventID);

            ThisEvent.Website = evnt.Website;
            ThisEvent.Facebook = evnt.Facebook;
            ThisEvent.Twitter = evnt.Twitter;
            ThisEvent.Instagram = evnt.Instagram;
            ThisEvent.Linkedin = evnt.Linkedin;
            ThisEvent.Youtube = evnt.Youtube;
            ThisEvent.Pinterest = evnt.Pinterest;
            ThisEvent.Tumblr = evnt.Tumblr;
            ThisEvent.GooglePlus = evnt.GooglePlus;

            db.SaveChanges();

            return RedirectToAction("SocialMediaOrg", "Events");
        }

        public ActionResult OtelsEditable()
        {
            User LoggedUser = (User)Session["LoggedIn"];
            List<Event> AllEvents = db.Event.ToList();
            var TE = AllEvents.Where(q => q.EventConfirmation == LoggedUser.AccountConfirmation).ToList();
            Event ThisEvent = db.Event.Find(TE[0].Id);

            var AllOtels = db.Otels.ToList();
            var ThisOtels = AllOtels.Where(k => k.EventID == ThisEvent.Id).ToList();
            List<Otels> SavedOtels = new List<Otels>();
            for (int i = 0; i < ThisOtels.Count; i++)
            {
                Otels AddThis = db.Otels.Find(ThisOtels[i].Id);
                SavedOtels.Add(AddThis);
            }

            var SortedOrder = SavedOtels.OrderByDescending(q => q.OnePersonPrice).ToList();
            List<Otels> SavedOrderedOtels = new List<Otels>();
            for (int i = 0; i < SortedOrder.Count; i++)
            {
                Otels AddThis = db.Otels.Find(SortedOrder[i].Id);
                SavedOrderedOtels.Add(AddThis);
            }
            ViewData["OrderedOtels"] = SavedOrderedOtels;

            return View();
        }

        public ActionResult Otels()
        {
            User LoggedUser = (User)Session["LoggedIn"];
            List<Event> AllEvents = db.Event.ToList();
            var TE = AllEvents.Where(q => q.EventConfirmation == LoggedUser.AccountConfirmation).ToList();
            Event ThisEvent = db.Event.Find(TE[0].Id);

            var AllOtels = db.Otels.ToList();
            var ThisOtels = AllOtels.Where(k => k.EventID == ThisEvent.Id).ToList();
            List<Otels> SavedOtels = new List<Otels>();
            for (int i = 0; i < ThisOtels.Count; i++)
            {
                Otels AddThis = db.Otels.Find(ThisOtels[i].Id);
                SavedOtels.Add(AddThis);
            }

            var SortedOrder = SavedOtels.OrderByDescending(q => q.OnePersonPrice).ToList();
            List<Otels> SavedOrderedOtels = new List<Otels>();
            for (int i = 0; i < SortedOrder.Count; i++)
            {
                Otels AddThis = db.Otels.Find(SortedOrder[i].Id);
                SavedOrderedOtels.Add(AddThis);
            }
            ViewData["OrderedOtels"] = SavedOrderedOtels;

            var AllUserOtel = db.UserOtels.ToList();
            var IsChecked = AllUserOtel.Where(k => k.UserID == LoggedUser.Id).ToList();
            if (IsChecked.Count > 0)
            {
                ViewData["UserOtelID"] = IsChecked[0].Id;
            }
            else
            {
                ViewData["UserOtelID"] = Guid.Empty;
            }

            return View();
        }

        public ActionResult NewOtel(string Error)
        {
            if (Error == "Error")
            {
                ViewData["NameError"] = TempData["NameError"];
                ViewData["ImageError"] = TempData["ImageError"];
                ViewData["LatError"] = TempData["LatError"];
                ViewData["LonError"] = TempData["LonError"];
                Otels otl = (Otels)TempData["Model"];
                return View(otl);
            }
            else
            {
                ViewData["NameError"] = "Correct";
                ViewData["ImageError"] = "Correct";
                ViewData["LatError"] = "Correct";
                ViewData["LonError"] = "Correct";
                return View();
            }
        }

        [HttpPost]
        public ActionResult NewOtel(Otels otl, HttpPostedFileBase imageinput, HttpPostedFileBase imageinput2, HttpPostedFileBase imageinput3, HttpPostedFileBase imageinput4, HttpPostedFileBase imageinput5)
        {
            string NameError = "Correct";
            string ImageError = "Correct";
            string LatError = "Correct";
            string LonError = "Correct";

            if (otl.Name == null)
            {
                NameError = "NameError";
            }

            if (imageinput == null)
            {
                ImageError = "ImageError";
            }

            if (otl.Lattitude == null)
            {
                LatError = "LatError";
            }

            if (otl.Longitude == null)
            {
                LonError = "LonError";
            }
             if (NameError == "NameError" || ImageError == "ImageError" || LatError == "LatError" || LonError == "LonError")
            {
                TempData["NameError"] = NameError;
                TempData["ImageError"] = ImageError;
                TempData["LatError"] = LatError;
                TempData["LonError"] = LonError;
                TempData["Model"] = otl;
                return RedirectToAction("NewOtel", "Events", new { Error = "Error" });
            }

            // Save etme işlemi
            User LoggedUser = (User)Session["LoggedIn"];
            List<Event> AllEvents = db.Event.ToList();
            var TE = AllEvents.Where(q => q.EventConfirmation == LoggedUser.AccountConfirmation).ToList();
            Event ThisEvent = db.Event.Find(TE[0].Id);

            int PhotoNumber = 1;
            if (imageinput2 != null)
            {
                PhotoNumber = PhotoNumber + 1;
            }
            if (imageinput3 != null)
            {
                PhotoNumber = PhotoNumber + 1;
            }
            if (imageinput4 != null)
            {
                PhotoNumber = PhotoNumber + 1;
            }
            if (imageinput5 != null)
            {
                PhotoNumber = PhotoNumber + 1;
            }

            otl.EventID = ThisEvent.Id;
            otl.PhotoNumber = PhotoNumber;
            db.Otels.Add(otl);
            db.SaveChanges();

            if (imageinput != null)
            {
                System.IO.File.Copy(Server.MapPath("/Images/Otels/hotel.png"), Server.MapPath("/Images/Otels/" + otl.Id + "1.jpg"));
                string path = System.IO.Path.Combine(Server.MapPath("~/Images/Otels/" + otl.Id + "1.jpg"));
                Image sourceimage = Image.FromStream(imageinput.InputStream);
                var newImage = ScaleImage(sourceimage, 800, 600);
                newImage.Save(path);
            }

            int Order = 2;
            if (imageinput2 != null)
            {
                System.IO.File.Copy(Server.MapPath("/Images/Otels/hotel.png"), Server.MapPath("/Images/Otels/" + otl.Id + Order + ".jpg"));
                string path = System.IO.Path.Combine(Server.MapPath("~/Images/Otels/" + otl.Id + Order + ".jpg"));
                Image sourceimage = Image.FromStream(imageinput2.InputStream);
                var newImage = ScaleImage(sourceimage, 800, 600);
                newImage.Save(path);
                Order = Order + 1;
            }
            if (imageinput3 != null)
            {
                System.IO.File.Copy(Server.MapPath("/Images/Otels/hotel.png"), Server.MapPath("/Images/Otels/" + otl.Id + Order + ".jpg"));
                string path = System.IO.Path.Combine(Server.MapPath("~/Images/Otels/" + otl.Id + Order + ".jpg"));
                Image sourceimage = Image.FromStream(imageinput3.InputStream);
                var newImage = ScaleImage(sourceimage, 800, 600);
                newImage.Save(path);
                Order = Order + 1;
            }
            if (imageinput4 != null)
            {
                System.IO.File.Copy(Server.MapPath("/Images/Otels/hotel.png"), Server.MapPath("/Images/Otels/" + otl.Id + Order + ".jpg"));
                string path = System.IO.Path.Combine(Server.MapPath("~/Images/Otels/" + otl.Id + Order + ".jpg"));
                Image sourceimage = Image.FromStream(imageinput4.InputStream);
                var newImage = ScaleImage(sourceimage, 800, 600);
                newImage.Save(path);
                Order = Order + 1;
            }
            if (imageinput5 != null)
            {
                System.IO.File.Copy(Server.MapPath("/Images/Otels/hotel.png"), Server.MapPath("/Images/Otels/" + otl.Id + Order + ".jpg"));
                string path = System.IO.Path.Combine(Server.MapPath("~/Images/Otels/" + otl.Id + Order + ".jpg"));
                Image sourceimage = Image.FromStream(imageinput5.InputStream);
                var newImage = ScaleImage(sourceimage, 800, 600);
                newImage.Save(path);
                Order = Order + 1;
            }

            return RedirectToAction("OtelsEditable", "Events");
        }

        public static Image ScaleImage(Image image, int newWidth, int newHeight)
        {
            var newImage = new Bitmap(newWidth, newHeight);

            using (var graphics = Graphics.FromImage(newImage))
                graphics.DrawImage(image, 0, 0, newWidth, newHeight);

            return newImage;
        }

        public ActionResult OtelDetails(Guid OtelID)
        {
            User LoggedUser = (User)Session["LoggedIn"];
            var AllUserOtel = db.UserOtels.ToList();
            var IsChecked = AllUserOtel.Where(k => k.UserID == LoggedUser.Id).ToList();
            string reservation = "NoReservation";
            if (IsChecked.Count > 0 && IsChecked[0].OtelID == OtelID)
            {
                reservation = "ReservationThisOtel";
            }
            else if (IsChecked.Count > 0 && IsChecked[0].OtelID != OtelID)
            {
                Otels aaa = db.Otels.Find(IsChecked[0].OtelID);
                reservation = aaa.Name + " için rezervasyon gerçekleştirdiniz.";
            }

            ViewData["Reservation"] = reservation;

            Otels ThisOtel = db.Otels.Find(OtelID);
            ViewData["OtelDetails"] = ThisOtel; 
            return View();
        }

        public ActionResult OtelDetailsOrg (Guid OtelID)
        {
            Otels ThisOtel = db.Otels.Find(OtelID);
            ViewData["OtelDetails"] = ThisOtel;
            return View();
        }

        public ActionResult EditOtel(Guid OtelID)
        {
            Otels ThisOtel = db.Otels.Find(OtelID);
            ViewData["OtelName"] = ThisOtel.Name;
            ViewData["EditOtelID"] = ThisOtel.Id;
            ViewData["PhotoNumber"] = ThisOtel.PhotoNumber;
            return View(ThisOtel);
        }

        [HttpPost]
        public ActionResult UpdateOtel(Otels otl, Guid ThisOtel, HttpPostedFileBase imageinput, HttpPostedFileBase imageinput2, HttpPostedFileBase imageinput3, HttpPostedFileBase imageinput4, HttpPostedFileBase imageinput5)
        {
            Otels UpdatedOtel = db.Otels.Find(ThisOtel);

            if (otl.Name != null)
            {
                UpdatedOtel.Name = otl.Name;
            }

            if (otl.Lattitude != null)
            {
                UpdatedOtel.Lattitude = otl.Lattitude;
            }

            if (otl.Longitude != null)
            {
                UpdatedOtel.Longitude = otl.Longitude;
            }

            UpdatedOtel.OtelContent = otl.OtelContent;
            UpdatedOtel.StarNumber = otl.StarNumber;
            UpdatedOtel.Website = otl.Website;
            UpdatedOtel.Currency = otl.Currency;
            UpdatedOtel.OnePersonPrice = otl.OnePersonPrice;
            UpdatedOtel.TwoPersonPrice = otl.TwoPersonPrice;
            UpdatedOtel.ThreePersonPrice = otl.ThreePersonPrice;

            db.SaveChanges();

            if (imageinput != null)
            {
                string path = System.IO.Path.Combine(Server.MapPath("~/Images/Otels/" + ThisOtel + "1.jpg"));
                Image sourceimage = Image.FromStream(imageinput.InputStream);
                var newImage = ScaleImage(sourceimage, 800, 600);
                newImage.Save(path);
            }
            if (imageinput2 != null)
            {
                string path = System.IO.Path.Combine(Server.MapPath("~/Images/Otels/" + ThisOtel + "2.jpg"));
                Image sourceimage = Image.FromStream(imageinput2.InputStream);
                var newImage = ScaleImage(sourceimage, 800, 600);
                newImage.Save(path);
            }
            if (imageinput3 != null)
            {
                string path = System.IO.Path.Combine(Server.MapPath("~/Images/Otels/" + ThisOtel + "3.jpg"));
                Image sourceimage = Image.FromStream(imageinput3.InputStream);
                var newImage = ScaleImage(sourceimage, 800, 600);
                newImage.Save(path);
            }
            if (imageinput4 != null)
            {
                string path = System.IO.Path.Combine(Server.MapPath("~/Images/Otels/" + ThisOtel + "4.jpg"));
                Image sourceimage = Image.FromStream(imageinput4.InputStream);
                var newImage = ScaleImage(sourceimage, 800, 600);
                newImage.Save(path);
            }
            if (imageinput5 != null)
            {
                string path = System.IO.Path.Combine(Server.MapPath("~/Images/Otels/" + ThisOtel + "5.jpg"));
                Image sourceimage = Image.FromStream(imageinput5.InputStream);
                var newImage = ScaleImage(sourceimage, 800, 600);
                newImage.Save(path);
            }

            return RedirectToAction("OtelsEditable", "Events");
        }

        public ActionResult OtelReservation(Guid OtelID)
        {
            ViewData["CheckOutError"] = "Correct";
            ViewData["CheckInError"] = "Correct";
            ViewData["GuestNumber"] = 1;
            ViewData["Name1Error"] = "Correct";
            ViewData["Name2Error"] = "Correct";
            ViewData["Surname1Error"] = "Correct";
            ViewData["Surname2Error"] = "Correct";
            ViewData["Mail1Error"] = "Correct";
            ViewData["Mail2Error"] = "Correct";
            ViewData["Phone1Error"] = "Correct";
            ViewData["Phone2Error"] = "Correct";
            ViewData["OtelID"] = OtelID;
            ViewData["Error"] = "Correct";
            Otels ThisOtel = db.Otels.Find(OtelID);
            ViewData["ThisOtel"] = ThisOtel;
            return View();
        }

        [HttpPost]
        public ActionResult OtelReservation (UserOtel usrotl, int GuestNumber, string Guest1Name, string Guest1Surname, string Guest1PhoneCode, string Guest1Phone, string Guest1Mail, string Guest2Name, string Guest2Surname, string Guest2PhoneCode, string Guest2Phone, string Guest2Mail)
        {
            Guid OtelID = (Guid)TempData["OtelID"];
            Otels ThisOtel = db.Otels.Find(OtelID);
            ViewData["ThisOtel"] = ThisOtel;
            ViewData["OtelID"] = OtelID;
            User LoggedUser = (User)Session["LoggedIn"];
            List<Event> AllEvents = db.Event.ToList();
            var TE = AllEvents.Where(q => q.EventConfirmation == LoggedUser.AccountConfirmation).ToList();
            Event ThisEvent = db.Event.Find(TE[0].Id);

            string Name1Error = "Correct";
            string Name2Error = "Correct";
            string Surname1Error = "Correct";
            string Surname2Error = "Correct";
            string Mail1Error = "Correct";
            string Mail2Error = "Correct";
            string Phone1Error = "Correct";
            string Phone2Error = "Correct";


            ViewData["GuestNumber"] = GuestNumber;
            if (GuestNumber == 1)
            {
                if (usrotl.CheckInStr == null || usrotl.CheckOutStr == null)
                {
                    if (usrotl.CheckInStr == null)
                    {
                        ViewData["CheckInError"] = "CheckInError";
                    }
                    if (usrotl.CheckOutStr == null)
                    {
                        ViewData["CheckOutError"] = "CheckOutError";
                    }

                    ViewData["Name1Error"] = Name1Error;
                    ViewData["Name2Error"] = Name2Error;
                    ViewData["Surname1Error"] = Surname1Error;
                    ViewData["Surname2Error"] = Surname2Error;
                    ViewData["Mail1Error"] = Mail1Error;
                    ViewData["Mail2Error"] = Mail2Error;
                    ViewData["Error"] = "Error";
                    ViewData["Phone1Error"] = Phone1Error;
                    ViewData["Phone2Error"] = Phone2Error;
                    return View(usrotl);
                }
                
                DateTime CheckIn = DateTime.ParseExact(usrotl.CheckInStr, "dd/MM/yyyy", new CultureInfo("tr"));
                DateTime CheckOut = DateTime.ParseExact(usrotl.CheckOutStr, "dd/MM/yyyy", new CultureInfo("tr"));

                usrotl.UserID = LoggedUser.Id;
                usrotl.OtelID = OtelID;
                usrotl.CheckInDate = CheckIn;
                usrotl.CheckOutDate = CheckOut;
                usrotl.NumberOfGuest = GuestNumber;
                usrotl.CheckIn = false;
                usrotl.Reservation = false;
                usrotl.ActivationCode = CreateRandomActivationCode() + "0";
                db.UserOtels.Add(usrotl);
                db.SaveChanges();

            }
            else if (GuestNumber == 2)
            {
                if (usrotl.CheckInStr == null || usrotl.CheckOutStr == null || Guest1Name == "" || Guest1Surname == "" || !Guest1Mail.Contains("@") || !Guest1Mail.Contains(".") || Guest1Phone.Contains("_") || Guest1Phone == "")
                {
                    if (usrotl.CheckInStr == null)
                    {
                        ViewData["CheckInError"] = "CheckInError";
                    }
                    if (usrotl.CheckOutStr == null)
                    {
                        ViewData["CheckOutError"] = "CheckOutError";
                    }
                    if (Guest1Name == "")
                    {
                        ViewData["Name1Error"] = "Name1Error";
                    }
                    if (Guest1Surname == "")
                    {
                        ViewData["Surname1Error"] = "Surname1Error";
                    }
                    if (!Guest1Mail.Contains("@") || !Guest1Mail.Contains("."))
                    {
                        ViewData["Mail1Error"] = "Mail1Error";
                    }
                    if (Guest1Phone.Contains("_") || Guest1Phone == "")
                    {
                        ViewData["Phone1Error"] = "Phone1Error";
                    }

                    ViewData["Name2Error"] = Name2Error;
                    ViewData["Surname2Error"] = Surname2Error;
                    ViewData["Mail2Error"] = Mail2Error;
                    ViewData["Error"] = "Error";
                    return View(usrotl);
                }

                DateTime CheckIn = DateTime.ParseExact(usrotl.CheckInStr, "dd/MM/yyyy", new CultureInfo("tr"));
                DateTime CheckOut = DateTime.ParseExact(usrotl.CheckOutStr, "dd/MM/yyyy", new CultureInfo("tr"));

                usrotl.UserID = LoggedUser.Id;
                usrotl.OtelID = OtelID;
                usrotl.CheckInDate = CheckIn;
                usrotl.CheckOutDate = CheckOut;
                usrotl.NumberOfGuest = GuestNumber;
                usrotl.CheckIn = false;
                usrotl.Reservation = false;
                string ActCode = CreateRandomActivationCode();
                usrotl.ActivationCode = ActCode + "0";
                db.UserOtels.Add(usrotl);
                db.SaveChanges();

                OtelGuest Guest1 = new OtelGuest();
                Guest1.Name = Guest1Name;
                Guest1.Surname = Guest1Surname;
                Guest1.Mail = Guest1Mail;
                Guest1.UserOtelID = usrotl.Id;
                Guest1.ActivationCode = ActCode + "1";
                Guest1.CheckIn = false;
                Guest1.Reservation = false;
                Guest1.CountryPhoneCode = Int32.Parse(Guest1PhoneCode);
                Guest1.Phone = Guest1Phone;
                db.OtelGuests.Add(Guest1);
                db.SaveChanges();

            }
            else if (GuestNumber == 3)
            {
                if (usrotl.CheckInStr == null || usrotl.CheckOutStr == null || Guest1Name == "" || Guest1Surname == "" || !Guest1Mail.Contains("@") || !Guest1Mail.Contains(".") || Guest2Name == "" || Guest2Surname == "" || !Guest2Mail.Contains("@") || !Guest2Mail.Contains(".") || Guest1Phone.Contains("_") || Guest2Phone.Contains("_") || Guest1Phone == "" || Guest2Phone == "")
                {
                    if (usrotl.CheckInStr == null)
                    {
                        ViewData["CheckInError"] = "CheckInError";
                    }
                    if (usrotl.CheckOutStr == null)
                    {
                        ViewData["CheckOutError"] = "CheckOutError";
                    }
                    if (Guest1Name == "")
                    {
                        ViewData["Name1Error"] = "Name1Error";
                    }
                    if (Guest1Surname == "")
                    {
                        ViewData["Surname1Error"] = "Surname1Error";
                    }
                    if (!Guest1Mail.Contains("@") || !Guest1Mail.Contains("."))
                    {
                        ViewData["Mail1Error"] = "Mail1Error";
                    }
                    if (Guest2Name == "")
                    {
                        ViewData["Name2Error"] = "Name2Error";
                    }
                    if (Guest2Surname == "")
                    {
                        ViewData["Surname2Error"] = "Surname2Error";
                    }
                    if (!Guest2Mail.Contains("@") || !Guest2Mail.Contains("."))
                    {
                        ViewData["Mail2Error"] = "Mail2Error";
                    }
                    if (Guest1Phone.Contains("_") || Guest1Phone == "")
                    {
                        ViewData["Phone1Error"] = "Phone1Error";
                    }
                    if (Guest2Phone.Contains("_") || Guest2Phone == "")
                    {
                        ViewData["Phone2Error"] = "Phone2Error";
                    }

                    ViewData["Error"] = "Error";
                    return View(usrotl);
                }

                DateTime CheckIn = DateTime.ParseExact(usrotl.CheckInStr, "dd/MM/yyyy", new CultureInfo("tr"));
                DateTime CheckOut = DateTime.ParseExact(usrotl.CheckOutStr, "dd/MM/yyyy", new CultureInfo("tr"));

                usrotl.UserID = LoggedUser.Id;
                usrotl.OtelID = OtelID;
                usrotl.CheckInDate = CheckIn;
                usrotl.CheckOutDate = CheckOut;
                usrotl.NumberOfGuest = GuestNumber;
                usrotl.CheckIn = false;
                usrotl.Reservation = false;
                string ActCode = CreateRandomActivationCode();
                usrotl.ActivationCode = ActCode + "0";
                db.UserOtels.Add(usrotl);
                db.SaveChanges();

                OtelGuest Guest1 = new OtelGuest();
                Guest1.Name = Guest1Name;
                Guest1.Surname = Guest1Surname;
                Guest1.Mail = Guest1Mail;
                Guest1.UserOtelID = usrotl.Id;
                Guest1.ActivationCode = ActCode + "1";
                Guest1.CheckIn = false;
                Guest1.Reservation = false;
                Guest1.CountryPhoneCode = Int32.Parse(Guest1PhoneCode);
                Guest1.Phone = Guest1Phone;
                db.OtelGuests.Add(Guest1);
                db.SaveChanges();

                OtelGuest Guest2 = new OtelGuest();
                Guest2.Name = Guest2Name;
                Guest2.Surname = Guest2Surname;
                Guest2.Mail = Guest2Mail;
                Guest2.UserOtelID = usrotl.Id;
                Guest2.ActivationCode = ActCode + "2";
                Guest2.CheckIn = false;
                Guest2.Reservation = false;
                Guest2.CountryPhoneCode = Int32.Parse(Guest2PhoneCode);
                Guest2.Phone = Guest2Phone;
                db.OtelGuests.Add(Guest2);
                db.SaveChanges();
            }

            return RedirectToAction("Otels", "Events");
        }

        public JsonResult SetPassengerNum(string num)
        {
            TempData["num"] = Convert.ToInt32(num);
            return Json(num, JsonRequestBehavior.AllowGet);
        }

        private static string CreateRandomActivationCode()
        {
            int passwordLength = 8;
            string allowedChars = "ABCDEFGHJKLMNOPQRSTUVWXYZ0123456789!@$?_-";
            char[] chars = new char[passwordLength];
            Random rd = new Random();

            for (int i = 0; i < passwordLength; i++)
            {
                chars[i] = allowedChars[rd.Next(0, allowedChars.Length)];
            }

            return new string(chars);
        }

        public ActionResult OtelRegistrationList()
        {
            User LoggedUser = (User)Session["LoggedIn"];
            List<Event> AllEvents = db.Event.ToList();
            var TE = AllEvents.Where(q => q.EventConfirmation == LoggedUser.AccountConfirmation).ToList();
            Event ThisEvent = db.Event.Find(TE[0].Id);

            var AllOtels = db.Otels.ToList();
            var ThisOtels = AllOtels.Where(k => k.EventID == ThisEvent.Id).ToList();
            List<Otels> SavedOtels = new List<Otels>();
            for (int i = 0; i < ThisOtels.Count; i++)
            {
                Otels AddThis = db.Otels.Find(ThisOtels[i].Id);
                SavedOtels.Add(AddThis);
            }
            ViewData["Otels"] = SavedOtels;

            var AllUserOtels = db.UserOtels.ToList();
            List<UserOtel> ThisUserOtel = new List<UserOtel>();
            for (int i = 0; i < ThisOtels.Count; i++)
            {
                var AddThisList = AllUserOtels.Where(k => k.OtelID == ThisOtels[i].Id).ToList();
                if (AddThisList.Count > 0)
                {
                    for (int j = 0; j < AddThisList.Count; j++)
                    {
                        UserOtel AddThis = db.UserOtels.Find(AddThisList[j].Id);
                        ThisUserOtel.Add(AddThis);
                    }
                }
            }
            ViewData["UserOtels"] = ThisUserOtel;

            List<User> UserInfo = new List<User>();
            for (int t = 0; t < ThisUserOtel.Count; t++)
            {
                User AddThis = db.User.Find(ThisUserOtel[t].UserID);
                UserInfo.Add(AddThis);
            }
            ViewData["UserInfo"] = UserInfo;
            List<bool> Notifications = Notify(ThisUserOtel);
            ViewData["Notifications"] = Notifications;

            return View();
        }

        public dynamic Notify (List<UserOtel> UserOtelList)
        {
            // check-in tablosunda olup check-in=true değilse (UserOtel)
            // check-in tablosunda olup check-in=true değilse (OtelGuest)
            List<bool> Notification = new List<bool>();
            var AllUserCheckIn = db.UserCheckIn.ToList();
            for (int i = 0; i < UserOtelList.Count; i++)
            {
                var isChecked = AllUserCheckIn.Where(k => k.UserOtelID == UserOtelList[i].Id).ToList();
                if (isChecked.Count > 0 && UserOtelList[i].CheckIn == false)
                {
                    Notification.Add(false);
                }
                else
                {
                    Notification.Add(true);
                }
            }

            var AllOtelGuest = db.OtelGuests.ToList();
            for (int i = 0; i < UserOtelList.Count; i++)
            {
                List<OtelGuest> ThisOtelGuests = new List<OtelGuest>();
                var AddThem = AllOtelGuest.Where(k => k.UserOtelID == UserOtelList[i].Id).ToList();
                if (AddThem.Count > 0)
                {
                    for (int j = 0; j < AddThem.Count; j++)
                    {
                        OtelGuest AddThis = db.OtelGuests.Find(AddThem[j].Id);
                        ThisOtelGuests.Add(AddThis);
                    }
                }
                if (ThisOtelGuests.Count > 0)
                {
                    for (int t = 0; t < ThisOtelGuests.Count; t++)
                    {
                        var isChecked = AllUserCheckIn.Where(k => k.UserOtelID == ThisOtelGuests[t].Id).ToList();
                        if (isChecked.Count > 0 && ThisOtelGuests[t].CheckIn == false)
                        {
                            Notification[i] = false;
                        }
                    }
                }
            }

            return Notification;
        }

        public ActionResult ApproveOtelApplication(Guid UserOtelID)
        {
            UserOtel ThisUserOtel = db.UserOtels.Find(UserOtelID);
            List<OtelGuest> ThisOtelGuest = new List<OtelGuest>();

            var AllOtelGuests = db.OtelGuests.ToList();
            if (ThisUserOtel.NumberOfGuest > 1)
            {
                var AddTheseGuests = AllOtelGuests.Where(k => k.UserOtelID == ThisUserOtel.Id).ToList();
                for (int i = 0; i < AddTheseGuests.Count; i++)
                {
                    OtelGuest AddThis = db.OtelGuests.Find(AddTheseGuests[i].Id);
                    ThisOtelGuest.Add(AddThis);
                }
            }
            ViewData["GuestList"] = ThisOtelGuest;
            ViewData["UserOtel"] = ThisUserOtel;

            UserCheckIn CheckedUser = new UserCheckIn();
            var AllUserCheckInList = db.UserCheckIn.ToList();
            var ThisUserCheckIn = AllUserCheckInList.Where(k => k.UserOtelID == ThisUserOtel.Id).ToList();

            string status = "stage1";
            if (ThisUserOtel.Reservation == true && ThisUserOtel.CheckIn == false && ThisUserCheckIn.Count == 0)
            {
                status = "stage2";
            }
            else if (ThisUserOtel.Reservation == true && ThisUserOtel.CheckIn == false && ThisUserCheckIn.Count == 1)
            {
                CheckedUser = db.UserCheckIn.Find(ThisUserCheckIn[0].Id);
                status = "stage3";
            }
            else if (ThisUserOtel.Reservation == true && ThisUserOtel.CheckIn == true && ThisUserCheckIn.Count == 1)
            {
                CheckedUser = db.UserCheckIn.Find(ThisUserCheckIn[0].Id);
                status = "stage4";
            }
            ViewData["Status"] = status;
            ViewData["CheckedUser"] = CheckedUser;

            Guid OtelID = ThisUserOtel.OtelID;
            Otels ThisOtel = db.Otels.Find(OtelID);
            ViewData["OtelName"] = ThisOtel.Name;

            Guid UserID = ThisUserOtel.UserID;
            User ThisUser = db.User.Find(UserID);
            ViewData["UserName"] = ThisUser;

            List<bool> CheckedInGuest = new List<bool>();
            List<UserCheckIn> GuestCheckIn = new List<UserCheckIn>();
            for (int i = 0; i < ThisOtelGuest.Count; i++)
            {
                var isChecked = AllUserCheckInList.Where(k => k.UserOtelID == ThisOtelGuest[i].Id).ToList();
                if (isChecked.Count == 1)
                {
                    Guid CheckInID = isChecked[0].Id;
                    UserCheckIn Addthis = db.UserCheckIn.Find(CheckInID);
                    GuestCheckIn.Add(Addthis);
                    CheckedInGuest.Add(true);
                }
                else
                {
                    UserCheckIn empty = new UserCheckIn();
                    GuestCheckIn.Add(empty);
                    CheckedInGuest.Add(false);
                }
            }
            ViewData["CheckedInGuest"] = CheckedInGuest;
            ViewData["GuestCheckInList"] = GuestCheckIn;

            return View();
        }
        
        public ActionResult ChangetoReservation(Guid UserOtelID)
        {
            UserOtel ThisUserOtel = db.UserOtels.Find(UserOtelID);
            ThisUserOtel.Reservation = true;
            db.Entry(ThisUserOtel).State = EntityState.Modified;
            db.SaveChanges();

            if (ThisUserOtel.NumberOfGuest > 1)
            {
                var allGuests = db.OtelGuests.ToList();
                var ThisGuests = allGuests.Where(k => k.UserOtelID == ThisUserOtel.Id).ToList();
                for (int i = 0; i < ThisGuests.Count; i++)
                {
                    OtelGuest ChangeThis = db.OtelGuests.Find(ThisGuests[i].Id);
                    ChangeThis.Reservation = true;
                    db.Entry(ChangeThis).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }

            return RedirectToAction("ApproveOtelApplication", "Events", new { UserOtelID = UserOtelID});
        }

        public ActionResult UserCheckInForm(Guid UserOtelID)
        {
            ViewData["UserOtelID"] = UserOtelID;
            UserOtel ThisUserOtel = db.UserOtels.Find(UserOtelID);
            Guid UserID = ThisUserOtel.UserID;
            User ThisUser = db.User.Find(UserID);
            Guid OtelID = ThisUserOtel.OtelID;
            Otels ThisOtel = db.Otels.Find(OtelID);
            ViewData["OtelName"] = ThisOtel.Name;

            ViewData["CheckIn"] = ThisUserOtel.CheckInStr;
            ViewData["CheckOut"] = ThisUserOtel.CheckOutStr;

            ViewData["NameError"] = "Correct";
            ViewData["SurnameError"] = "Correct";
            ViewData["MailError"] = "Correct";
            ViewData["PhoneError"] = "Correct";
            ViewData["Error"] = "Correct";

            var AllUserCheckIn = db.UserCheckIn.ToList();
            var isChecked = AllUserCheckIn.Where(k => k.UserOtelID == UserOtelID).ToList();
            if (isChecked.Count > 0)
            {
                ViewData["isChecked"] = "Checked";
                return View();
            }
            else
            {
                UserCheckIn CheckIn = new UserCheckIn();
                CheckIn.UserOtelID = UserOtelID;
                CheckIn.ActivationCode = ThisUserOtel.ActivationCode;
                CheckIn.Name = ThisUser.Name;
                CheckIn.Surname = ThisUser.Surname;
                CheckIn.CheckInStr = ThisUserOtel.CheckInStr;
                CheckIn.CheckInDate = ThisUserOtel.CheckInDate;
                CheckIn.CheckOutStr = ThisUserOtel.CheckOutStr;
                CheckIn.CheckOutDate = ThisUserOtel.CheckOutDate;
                CheckIn.Mail = ThisUser.Mail;
                CheckIn.Phone = ThisUser.Phone;
                CheckIn.CountryPhoneCode = ThisUser.CountryPhoneCode;
                return View(CheckIn);
            }
        }

        [HttpPost]
        public ActionResult UserCheckInForm(UserCheckIn user, HttpPostedFileBase backend, HttpPostedFileBase frontend)
        {
            Guid UserOtelID = (Guid)TempData["UserOtelID"];
            string NameError = "Correct";
            string SurnameError = "Correct";
            string MailError = "Correct";
            string PhoneError = "Correct";

            if (user.Name == null || user.Surname == null || user.Mail == null || user.Phone == null || frontend == null)
            {
                if (user.Name == null)
                {
                    NameError = "NameError";
                }
                if (user.Surname == null)
                {
                    SurnameError = "SurnameError";
                }
                if (user.Mail == null)
                {
                    MailError = "MailError";
                }
                if (user.Phone == null)
                {
                    PhoneError = "PhoneError";
                }

                ViewData["NameError"] = NameError;
                ViewData["SurnameError"] = SurnameError;
                ViewData["MailError"] = MailError;
                ViewData["PhoneError"] = PhoneError;
                ViewData["Error"] = "Error";

                ViewData["UserOtelID"] = UserOtelID;
                UserOtel ThisUserOtel = db.UserOtels.Find(UserOtelID);
                Guid UserID = ThisUserOtel.UserID;
                User ThisUser = db.User.Find(UserID);
                Guid OtelID = ThisUserOtel.OtelID;
                Otels ThisOtel = db.Otels.Find(OtelID);
                ViewData["OtelName"] = ThisOtel.Name;

                UserCheckIn CheckIn = new UserCheckIn();
                CheckIn.UserOtelID = UserOtelID;
                CheckIn.ActivationCode = ThisUserOtel.ActivationCode;
                CheckIn.Name = ThisUser.Name;
                CheckIn.Surname = ThisUser.Surname;
                CheckIn.CheckInStr = ThisUserOtel.CheckInStr;
                CheckIn.CheckInDate = ThisUserOtel.CheckInDate;
                CheckIn.CheckOutStr = ThisUserOtel.CheckOutStr;
                CheckIn.CheckOutDate = ThisUserOtel.CheckOutDate;
                CheckIn.Mail = ThisUser.Mail;
                CheckIn.Phone = ThisUser.Phone;
                CheckIn.CountryPhoneCode = ThisUser.CountryPhoneCode;

                ViewData["CheckIn"] = ThisUserOtel.CheckInStr;
                ViewData["CheckOut"] = ThisUserOtel.CheckOutStr;

                return View(CheckIn);
            }
            else
            {
                UserCheckIn NewCheckIn = new UserCheckIn();
                NewCheckIn.Name = user.Name;
                NewCheckIn.Surname = user.Surname;
                NewCheckIn.CountryPhoneCode = user.CountryPhoneCode;
                NewCheckIn.Phone = user.Phone;
                NewCheckIn.Mail = user.Mail;
                NewCheckIn.UserOtelID = UserOtelID;
                NewCheckIn.CheckInStr = user.CheckInStr;
                NewCheckIn.CheckInDate = user.CheckInDate;
                NewCheckIn.CheckOutStr = user.CheckOutStr;
                NewCheckIn.CheckOutDate = user.CheckOutDate;

                if (backend != null)
                {
                    NewCheckIn.IDCardBackEnd = "Valid";
                }
                if (frontend != null)
                {
                    NewCheckIn.IDCardFrontEnd = "Valid";
                }

                db.UserCheckIn.Add(NewCheckIn);
                db.SaveChanges();

                if(frontend != null && backend != null)
                {
                    System.IO.File.Copy(Server.MapPath("/Images/CheckIn/NationalID.png"), Server.MapPath("/Images/CheckIn/" + NewCheckIn.Id + "0.jpg"));
                    string path = System.IO.Path.Combine(Server.MapPath("~/Images/CheckIn/" + NewCheckIn.Id + "0.jpg"));
                    Image sourceimage = Image.FromStream(frontend.InputStream);
                    var newImage = ScaleImage(sourceimage, 800, 1000);
                    newImage.Save(path);

                    System.IO.File.Copy(Server.MapPath("/Images/CheckIn/NationalID.png"), Server.MapPath("/Images/CheckIn/" + NewCheckIn.Id + "1.jpg"));
                    string path2 = System.IO.Path.Combine(Server.MapPath("~/Images/CheckIn/" + NewCheckIn.Id + "1.jpg"));
                    Image sourceimage2 = Image.FromStream(backend.InputStream);
                    var newImage2 = ScaleImage(sourceimage2, 800, 1000);
                    newImage2.Save(path2);
                }
                else if (frontend != null && backend == null)
                {
                    System.IO.File.Copy(Server.MapPath("/Images/CheckIn/NationalID.png"), Server.MapPath("/Images/CheckIn/" + NewCheckIn.Id + "0.jpg"));
                    string path = System.IO.Path.Combine(Server.MapPath("~/Images/CheckIn/" + NewCheckIn.Id + "0.jpg"));
                    Image sourceimage = Image.FromStream(frontend.InputStream);
                    var newImage = ScaleImage(sourceimage, 1600, 1000);
                    newImage.Save(path);
                }
                
                Session["UserCheckIn"] = Guid.Empty;
                return RedirectToAction("CheckInConfirm", "Events", new { GuestID = UserOtelID, Guest = "UserOtel" });
            }
        }

        public ActionResult GuestCheckInForm(Guid GuestID)
        {
            OtelGuest ThisGuest = db.OtelGuests.Find(GuestID);
            Guid UserOtelID = ThisGuest.UserOtelID;

            ViewData["GuestID"] = GuestID;
            UserOtel ThisUserOtel = db.UserOtels.Find(UserOtelID);
            Guid UserID = ThisUserOtel.UserID;
            User ThisUser = db.User.Find(UserID);
            Guid OtelID = ThisUserOtel.OtelID;
            Otels ThisOtel = db.Otels.Find(OtelID);
            ViewData["OtelName"] = ThisOtel.Name;

            ViewData["CheckIn"] = ThisUserOtel.CheckInStr;
            ViewData["CheckOut"] = ThisUserOtel.CheckOutStr;

            ViewData["NameError"] = "Correct";
            ViewData["SurnameError"] = "Correct";
            ViewData["MailError"] = "Correct";
            ViewData["PhoneError"] = "Correct";
            ViewData["Error"] = "Correct";

            var AllUserCheckIn = db.UserCheckIn.ToList();
            var isChecked = AllUserCheckIn.Where(k => k.UserOtelID == GuestID).ToList();
            if (isChecked.Count > 0)
            {
                ViewData["isChecked"] = "Checked";
                return View();
            }
            else
            {
                UserCheckIn CheckIn = new UserCheckIn();
                CheckIn.UserOtelID = UserOtelID;
                CheckIn.ActivationCode = ThisGuest.ActivationCode;
                CheckIn.Name = ThisGuest.Name;
                CheckIn.Surname = ThisGuest.Surname;
                CheckIn.CheckInStr = ThisUserOtel.CheckInStr;
                CheckIn.CheckInDate = ThisUserOtel.CheckInDate;
                CheckIn.CheckOutStr = ThisUserOtel.CheckOutStr;
                CheckIn.CheckOutDate = ThisUserOtel.CheckOutDate;
                CheckIn.Mail = ThisGuest.Mail;
                CheckIn.Phone = ThisGuest.Phone;
                CheckIn.CountryPhoneCode = ThisGuest.CountryPhoneCode;

                return View(CheckIn);
            }
            
        }

        [HttpPost]
        public ActionResult GuestCheckInForm(UserCheckIn user, HttpPostedFileBase backend, HttpPostedFileBase frontend)
        {
            Guid GuestID = (Guid)TempData["GuestID"];
            OtelGuest ThisGuest = db.OtelGuests.Find(GuestID);
            Guid UserOtelID = ThisGuest.UserOtelID;

            string NameError = "Correct";
            string SurnameError = "Correct";
            string MailError = "Correct";
            string PhoneError = "Correct";

            if (user.Name == null || user.Surname == null || user.Mail == null || user.Phone == null || frontend == null)
            {
                if (user.Name == null)
                {
                    NameError = "NameError";
                }
                if (user.Surname == null)
                {
                    SurnameError = "SurnameError";
                }
                if (user.Mail == null)
                {
                    MailError = "MailError";
                }
                if (user.Phone == null)
                {
                    PhoneError = "PhoneError";
                }

                ViewData["NameError"] = NameError;
                ViewData["SurnameError"] = SurnameError;
                ViewData["MailError"] = MailError;
                ViewData["PhoneError"] = PhoneError;
                ViewData["Error"] = "Error";

                ViewData["GuestID"] = GuestID;

                UserOtel ThisUserOtel = db.UserOtels.Find(UserOtelID);
                Guid UserID = ThisUserOtel.UserID;
                User ThisUser = db.User.Find(UserID);
                Guid OtelID = ThisUserOtel.OtelID;
                Otels ThisOtel = db.Otels.Find(OtelID);
                ViewData["OtelName"] = ThisOtel.Name;

                UserCheckIn CheckIn = new UserCheckIn();
                CheckIn.UserOtelID = UserOtelID;
                CheckIn.ActivationCode = ThisGuest.ActivationCode;
                CheckIn.Name = ThisGuest.Name;
                CheckIn.Surname = ThisGuest.Surname;
                CheckIn.CheckInStr = ThisUserOtel.CheckInStr;
                CheckIn.CheckInDate = ThisUserOtel.CheckInDate;
                CheckIn.CheckOutStr = ThisUserOtel.CheckOutStr;
                CheckIn.CheckOutDate = ThisUserOtel.CheckOutDate;
                CheckIn.Mail = ThisGuest.Mail;
                CheckIn.Phone = ThisGuest.Phone;
                CheckIn.CountryPhoneCode = ThisGuest.CountryPhoneCode;

                ViewData["CheckIn"] = ThisUserOtel.CheckInStr;
                ViewData["CheckOut"] = ThisUserOtel.CheckOutStr;

                return View(CheckIn);
            }
            else
            {
                UserCheckIn NewCheckIn = new UserCheckIn();
                NewCheckIn.Name = user.Name;
                NewCheckIn.Surname = user.Surname;
                NewCheckIn.CountryPhoneCode = user.CountryPhoneCode;
                NewCheckIn.Phone = user.Phone;
                NewCheckIn.Mail = user.Mail;
                NewCheckIn.UserOtelID = GuestID;
                NewCheckIn.CheckInStr = user.CheckInStr;
                NewCheckIn.CheckInDate = user.CheckInDate;
                NewCheckIn.CheckOutStr = user.CheckOutStr;
                NewCheckIn.CheckOutDate = user.CheckOutDate;

                if (backend != null)
                {
                    NewCheckIn.IDCardBackEnd = "Valid";
                }
                if (frontend != null)
                {
                    NewCheckIn.IDCardFrontEnd = "Valid";
                }

                db.UserCheckIn.Add(NewCheckIn);
                db.SaveChanges();

                if (backend != null && frontend != null)
                {
                    System.IO.File.Copy(Server.MapPath("/Images/CheckIn/NationalID.png"), Server.MapPath("/Images/CheckIn/" + NewCheckIn.Id + "0.jpg"));
                    string path = System.IO.Path.Combine(Server.MapPath("~/Images/CheckIn/" + NewCheckIn.Id + "0.jpg"));
                    Image sourceimage = Image.FromStream(frontend.InputStream);
                    var newImage = ScaleImage(sourceimage, 800, 1000);
                    newImage.Save(path);

                    System.IO.File.Copy(Server.MapPath("/Images/CheckIn/NationalID.png"), Server.MapPath("/Images/CheckIn/" + NewCheckIn.Id + "1.jpg"));
                    string path2 = System.IO.Path.Combine(Server.MapPath("~/Images/CheckIn/" + NewCheckIn.Id + "1.jpg"));
                    Image sourceimage2 = Image.FromStream(backend.InputStream);
                    var newImage2 = ScaleImage(sourceimage2, 800, 1000);
                    newImage2.Save(path2);
                }
                else if (backend == null && frontend != null)
                {
                    System.IO.File.Copy(Server.MapPath("/Images/CheckIn/NationalID.png"), Server.MapPath("/Images/CheckIn/" + NewCheckIn.Id + "0.jpg"));
                    string path = System.IO.Path.Combine(Server.MapPath("~/Images/CheckIn/" + NewCheckIn.Id + "0.jpg"));
                    Image sourceimage = Image.FromStream(frontend.InputStream);
                    var newImage = ScaleImage(sourceimage, 1600, 1000);
                    newImage.Save(path);
                }

                return RedirectToAction("CheckInConfirm", "Events", new { GuestID = GuestID, Guest = "OtelGuest"});
            }
        }

        public ActionResult ChangeToCheckIn (Guid CheckInID, Guid UserOtelID, string Guest, string User)
        {
            if (Guest == "GuestOtel")
            {
                OtelGuest ThisGuest = db.OtelGuests.Find(CheckInID);
                ThisGuest.CheckIn = true;
                db.Entry(ThisGuest).State = EntityState.Modified;
                db.SaveChanges();
            }
            else if (User == "UserOtel")
            {
                UserOtel ThisUserOtel = db.UserOtels.Find(CheckInID);
                ThisUserOtel.CheckIn = true;
                db.Entry(ThisUserOtel).State = EntityState.Modified;
                db.SaveChanges();
            }
            return RedirectToAction("ApproveOtelApplication", "Events", new { UserOtelID = UserOtelID });
        }

        public ActionResult DeleteOtel (Guid OtelID)
        {
            // Otel
            Otels ThisOtel = db.Otels.Find(OtelID);

            // UserOtel
            var AllUserOtel = db.UserOtels.ToList();
            var thisUserOtel = AllUserOtel.Where(k => k.OtelID == ThisOtel.Id).ToList();
            List<UserOtel> UserOtelList = new List<UserOtel>();
            for (int i = 0; i < thisUserOtel.Count; i++)
            {
                UserOtel AddThis = db.UserOtels.Find(thisUserOtel[i].Id);
                UserOtelList.Add(AddThis);
            }

            //OtelGuest
            var AllOtelGuest = db.OtelGuests.ToList();
            List<OtelGuest> OtelGuestList = new List<OtelGuest>(); 
            for (int j = 0; j < UserOtelList.Count; j++)
            {
                var AddThese = AllOtelGuest.Where(l => l.UserOtelID == UserOtelList[j].Id).ToList();
                if (AddThese.Count > 0)
                {
                    for (int i = 0; i < AddThese.Count; i++)
                    {
                        OtelGuest AddThis = db.OtelGuests.Find(AddThese[i].Id);
                        OtelGuestList.Add(AddThis);
                    }
                }
            }

            // UserCheckIn
            var AllUserCheckIn = db.UserCheckIn.ToList();
            List<UserCheckIn> CheckInList = new List<UserCheckIn>();
            for (int t = 0; t < UserOtelList.Count; t++)
            {
                var isChecked = AllUserCheckIn.Where(k => k.UserOtelID == UserOtelList[t].Id).ToList();
                if (isChecked.Count > 0)
                {
                    UserCheckIn AddThis = db.UserCheckIn.Find(isChecked[0].Id);
                    CheckInList.Add(AddThis);
                }
            }

            for (int i = 0; i < OtelGuestList.Count; i++)
            {
                var isChecked = AllUserCheckIn.Where(k => k.UserOtelID == OtelGuestList[i].Id).ToList();
                if (isChecked.Count > 0)
                {
                    UserCheckIn AddThis = db.UserCheckIn.Find(isChecked[0].Id);
                    CheckInList.Add(AddThis);
                }
            }

            // Silme İşlemleri
            db.Otels.Remove(ThisOtel);
            db.SaveChanges();

            for (int i = 0; i < UserOtelList.Count; i++)
            {
                db.UserOtels.Remove(UserOtelList[i]);
                db.SaveChanges();
            }

            for (int i = 0; i < OtelGuestList.Count; i++)
            {
                db.OtelGuests.Remove(OtelGuestList[i]);
                db.SaveChanges();
            }

            for (int i = 0; i < CheckInList.Count; i++)
            {
                db.UserCheckIn.Remove(CheckInList[i]);
                db.SaveChanges();
            }

            return RedirectToAction("OtelsEditable", "Events");
        }

        public ActionResult CancelReservationUser(Guid OtelID)
        {
            User LoggedUser = (User)Session["LoggedIn"];
            var AllUserOtels = db.UserOtels.ToList();
            var ReservedUserOtel = AllUserOtels.Where(k => k.UserID == LoggedUser.Id && k.OtelID == OtelID).ToList();
            Guid UserOtelID = ReservedUserOtel[0].Id;

            // UserOtel, OtelGuest, OtelCheckIn
            UserOtel ThisUserOtel = db.UserOtels.Find(UserOtelID);
            List<UserCheckIn> CheckInList = new List<UserCheckIn>();
            List<OtelGuest> OtelGuestList = new List<OtelGuest>();
            var AllUserCheckIn = db.UserCheckIn.ToList();

            if (ThisUserOtel.NumberOfGuest > 1)
            {
                var AllOtelGuest = db.OtelGuests.ToList();
                var ThisOtelGuest = AllOtelGuest.Where(l => l.UserOtelID == ThisUserOtel.Id).ToList();
                for (int i = 0; i < ThisOtelGuest.Count; i++)
                {
                    OtelGuest AddThis = db.OtelGuests.Find(ThisOtelGuest[i].Id);
                    OtelGuestList.Add(AddThis);
                }

                for (int i = 0; i < OtelGuestList.Count; i++)
                {
                    var isChecked = AllUserCheckIn.Where(k => k.UserOtelID == OtelGuestList[i].Id).ToList();
                    if (isChecked.Count > 0)
                    {
                        UserCheckIn AddThis = db.UserCheckIn.Find(isChecked[0].Id);
                        CheckInList.Add(AddThis);
                    }
                }
            }

            var ThisUserCheckIn = AllUserCheckIn.Where(k => k.UserOtelID == ThisUserOtel.Id).ToList();
            if (ThisUserCheckIn.Count > 0)
            {
                UserCheckIn AddThis = db.UserCheckIn.Find(ThisUserCheckIn[0].Id);
                CheckInList.Add(AddThis);
            }

            // Silme İşlemi
            db.UserOtels.Remove(ThisUserOtel);
            db.SaveChanges();

            if (OtelGuestList.Count > 0)
            {
                for (int i = 0; i < OtelGuestList.Count; i++)
                {
                    db.OtelGuests.Remove(OtelGuestList[i]);
                    db.SaveChanges();
                }
            }

            if (CheckInList.Count > 0)
            {
                for (int i = 0; i < CheckInList.Count; i++)
                {
                    db.UserCheckIn.Remove(CheckInList[i]);
                    db.SaveChanges();
                }
            }
            Session["UserCheckIn"] = Guid.Empty;
            return RedirectToAction("Otels", "Events");
        }

        public ActionResult CancelReservationOrg(Guid UserOtelID)
        {
            UserOtel ThisUserOtel = db.UserOtels.Find(UserOtelID);
            List<UserCheckIn> CheckInList = new List<UserCheckIn>();
            List<OtelGuest> OtelGuestList = new List<OtelGuest>();
            var AllUserCheckIn = db.UserCheckIn.ToList();

            if (ThisUserOtel.NumberOfGuest > 1)
            {
                var AllOtelGuest = db.OtelGuests.ToList();
                var ThisOtelGuest = AllOtelGuest.Where(l => l.UserOtelID == ThisUserOtel.Id).ToList();
                for (int i = 0; i < ThisOtelGuest.Count; i++)
                {
                    OtelGuest AddThis = db.OtelGuests.Find(ThisOtelGuest[i].Id);
                    OtelGuestList.Add(AddThis);
                }

                for (int i = 0; i < OtelGuestList.Count; i++)
                {
                    var isChecked = AllUserCheckIn.Where(k => k.UserOtelID == OtelGuestList[i].Id).ToList();
                    if (isChecked.Count > 0)
                    {
                        UserCheckIn AddThis = db.UserCheckIn.Find(isChecked[0].Id);
                        CheckInList.Add(AddThis);
                    }
                }
            }

            var ThisUserCheckIn = AllUserCheckIn.Where(k => k.UserOtelID == ThisUserOtel.Id).ToList();
            if (ThisUserCheckIn.Count > 0)
            {
                UserCheckIn AddThis = db.UserCheckIn.Find(ThisUserCheckIn[0].Id);
                CheckInList.Add(AddThis);
            }

            // Silme İşlemi
            db.UserOtels.Remove(ThisUserOtel);
            db.SaveChanges();

            if (OtelGuestList.Count > 0)
            {
                for (int i = 0; i < OtelGuestList.Count; i++)
                {
                    db.OtelGuests.Remove(OtelGuestList[i]);
                    db.SaveChanges();
                }
            }

            if (CheckInList.Count > 0)
            {
                for (int i = 0; i < CheckInList.Count; i++)
                {
                    db.UserCheckIn.Remove(CheckInList[i]);
                    db.SaveChanges();
                }
            }

            return RedirectToAction("OtelRegistrationList", "Events");
        }

        public ActionResult ReTypeCheckInOrg(Guid UserID, Guid UserOtelID, string user, string guest)
        {
            var allUserCheckIn = db.UserCheckIn.ToList();
            var ThisUserCheckIn = allUserCheckIn.Where(k => k.UserOtelID == UserID).ToList();
            UserCheckIn ThisUser = db.UserCheckIn.Find(ThisUserCheckIn[0].Id);

            if (user == "UserOtel")
            {
                UserOtel ChangeThis = db.UserOtels.Find(UserID);
                var AllCheckIn = db.UserCheckIn.ToList();
                var ThisCheckIn = AllCheckIn.Where(k => k.UserOtelID == ChangeThis.Id).ToList();
                UserCheckIn DeleteThis = db.UserCheckIn.Find(ThisCheckIn[0].Id);
                db.UserCheckIn.Remove(DeleteThis);
                db.SaveChanges();
            }
            else if (guest == "OtelGuest")
            {
                OtelGuest ChangeThis = db.OtelGuests.Find(UserID);
                var AllCheckIn = db.UserCheckIn.ToList();
                var ThisCheckIn = AllCheckIn.Where(k => k.UserOtelID == ChangeThis.Id).ToList();
                UserCheckIn DeleteThis = db.UserCheckIn.Find(ThisCheckIn[0].Id);
                db.UserCheckIn.Remove(DeleteThis);
                db.SaveChanges();
            }

            return RedirectToAction("ApproveOtelApplication", "Events", new { UserOtelID = UserOtelID });
        }

        public ActionResult CheckInConfirm(Guid GuestID, string Guest)
        {
            if (Guest == "OtelGuest")
            {
                OtelGuest ThisGuest = db.OtelGuests.Find(GuestID);
                ViewData["Name"] = ThisGuest.Name + " " + ThisGuest.Surname;
                ViewData["Mail"] = ThisGuest.Mail;

                UserOtel ThisUserOtel = db.UserOtels.Find(ThisGuest.UserOtelID);
                ViewData["CheckIn"] = ThisUserOtel.CheckInStr;
                ViewData["CheckOut"] = ThisUserOtel.CheckOutStr;

                Otels ThisOtel = db.Otels.Find(ThisUserOtel.OtelID);
                ViewData["OtelName"] = ThisOtel.Name;
                ViewData["OtelID"] = ThisOtel.Id;
            }
            else
            {
                UserOtel ThisUserOtel = db.UserOtels.Find(GuestID);
                ViewData["CheckIn"] = ThisUserOtel.CheckInStr;
                ViewData["CheckOut"] = ThisUserOtel.CheckOutStr;

                User ThisUser = db.User.Find(ThisUserOtel.UserID);
                ViewData["Name"] = ThisUser.Name + " " + ThisUser.Surname;
                ViewData["Mail"] = ThisUser.Mail;

                Otels ThisOtel = db.Otels.Find(ThisUserOtel.OtelID);
                ViewData["OtelName"] = ThisOtel.Name;
                ViewData["OtelID"] = ThisOtel.Id;
            }

            return View();
        }

        public ActionResult UserAccomodation(Guid UserOtelID)
        {
            UserOtel ThisUserOtel = db.UserOtels.Find(UserOtelID);
            List<OtelGuest> ThisOtelGuest = new List<OtelGuest>();

            var AllOtelGuests = db.OtelGuests.ToList();
            if (ThisUserOtel.NumberOfGuest > 1)
            {
                var AddTheseGuests = AllOtelGuests.Where(k => k.UserOtelID == ThisUserOtel.Id).ToList();
                for (int i = 0; i < AddTheseGuests.Count; i++)
                {
                    OtelGuest AddThis = db.OtelGuests.Find(AddTheseGuests[i].Id);
                    ThisOtelGuest.Add(AddThis);
                }
            }
            ViewData["GuestList"] = ThisOtelGuest;
            ViewData["UserOtel"] = ThisUserOtel;

            UserCheckIn CheckedUser = new UserCheckIn();
            var AllUserCheckInList = db.UserCheckIn.ToList();
            var ThisUserCheckIn = AllUserCheckInList.Where(k => k.UserOtelID == ThisUserOtel.Id).ToList();

            string status = "stage1";
            if (ThisUserOtel.Reservation == true && ThisUserOtel.CheckIn == false && ThisUserCheckIn.Count == 0)
            {
                status = "stage2";
            }
            else if (ThisUserOtel.Reservation == true && ThisUserOtel.CheckIn == false && ThisUserCheckIn.Count == 1)
            {
                CheckedUser = db.UserCheckIn.Find(ThisUserCheckIn[0].Id);
                status = "stage3";
            }
            else if (ThisUserOtel.Reservation == true && ThisUserOtel.CheckIn == true && ThisUserCheckIn.Count == 1)
            {
                CheckedUser = db.UserCheckIn.Find(ThisUserCheckIn[0].Id);
                status = "stage4";
            }
            ViewData["Status"] = status;
            ViewData["CheckedUser"] = CheckedUser;

            Guid OtelID = ThisUserOtel.OtelID;
            Otels ThisOtel = db.Otels.Find(OtelID);
            ViewData["OtelName"] = ThisOtel.Name;
            ViewData["OtelID"] = ThisOtel.Id;

            Guid UserID = ThisUserOtel.UserID;
            User ThisUser = db.User.Find(UserID);
            ViewData["UserName"] = ThisUser;

            List<bool> CheckedInGuest = new List<bool>();
            List<UserCheckIn> GuestCheckIn = new List<UserCheckIn>();
            for (int i = 0; i < ThisOtelGuest.Count; i++)
            {
                var isChecked = AllUserCheckInList.Where(k => k.UserOtelID == ThisOtelGuest[i].Id).ToList();
                if (isChecked.Count == 1)
                {
                    Guid CheckInID = isChecked[0].Id;
                    UserCheckIn Addthis = db.UserCheckIn.Find(CheckInID);
                    GuestCheckIn.Add(Addthis);
                    CheckedInGuest.Add(true);
                }
                else
                {
                    UserCheckIn empty = new UserCheckIn();
                    GuestCheckIn.Add(empty);
                    CheckedInGuest.Add(false);
                }
            }
            ViewData["CheckedInGuest"] = CheckedInGuest;
            ViewData["GuestCheckInList"] = GuestCheckIn;

            User LoggedUser = (User)Session["LoggedIn"];
            List<Event> AllEvents = db.Event.ToList();
            var TE = AllEvents.Where(q => q.EventConfirmation == LoggedUser.AccountConfirmation).ToList();
            Event ThisEvent = db.Event.Find(TE[0].Id);
            var AllOtels = db.Otels.ToList();
            var ThisOtels = AllOtels.Where(k => k.EventID == ThisEvent.Id).ToList();
            List<Otels> OtelList = new List<Otels>();
            for (int i = 0; i < ThisOtels.Count; i++)
            {
                Otels AddThis = db.Otels.Find(ThisOtels[i].Id);
                OtelList.Add(AddThis);
            }
            ViewData["OtelList"] = OtelList;

            return View(ThisUserOtel);
        }

        public ActionResult UpdateAccomodationUser(UserOtel usrotel, Guid ReserveOtel)
        {
            Guid UserOtelID = (Guid)TempData["IDofUserOtel"];

            UserOtel ThisUserOtel = db.UserOtels.Find(UserOtelID);
            ThisUserOtel.CheckInStr = usrotel.CheckInStr;
            ThisUserOtel.CheckOutStr = usrotel.CheckOutStr;
            ThisUserOtel.OtelID = ReserveOtel;

            DateTime CheckIn = DateTime.ParseExact(usrotel.CheckInStr, "dd/MM/yyyy", new CultureInfo("tr"));
            DateTime CheckOut = DateTime.ParseExact(usrotel.CheckOutStr, "dd/MM/yyyy", new CultureInfo("tr"));

            ThisUserOtel.CheckInDate = CheckIn;
            ThisUserOtel.CheckOutDate = CheckOut;

            db.Entry(ThisUserOtel).State = EntityState.Modified;
            db.SaveChanges();



            return RedirectToAction("UserAccomodation", "Events", new { UserOtelID = UserOtelID });
        }

        public ActionResult DeleteGuest(Guid GuestID)
        {
            OtelGuest ThisGuest = db.OtelGuests.Find(GuestID);
            Guid UserOtelID = ThisGuest.UserOtelID;
            db.OtelGuests.Remove(ThisGuest);
            db.SaveChanges();

            UserOtel ThisUserOtel = db.UserOtels.Find(UserOtelID);
            ThisUserOtel.NumberOfGuest = ThisUserOtel.NumberOfGuest - 1;
            db.Entry(ThisUserOtel).State = EntityState.Modified;
            db.SaveChanges();

            return RedirectToAction("UserAccomodation", "Events", new { UserOtelID = UserOtelID });
        }

        [HttpPost]
        public ActionResult NewGuest(string Guest1Name, string Guest1Surname, string Guest1PhoneCode, string Guest1Phone, string Guest1Mail)
        {
            Guid UserOtelID = (Guid)TempData["IDofUserOtel"];
            UserOtel ThisUserOtel = db.UserOtels.Find(UserOtelID);

            OtelGuest Guest1 = new OtelGuest();
            Guest1.Name = Guest1Name;
            Guest1.Surname = Guest1Surname;
            Guest1.Mail = Guest1Mail;
            Guest1.UserOtelID = UserOtelID;
            Guest1.ActivationCode = ThisUserOtel.ActivationCode + "5";
            Guest1.CheckIn = false;
            Guest1.Reservation = false;
            Guest1.CountryPhoneCode = Int32.Parse(Guest1PhoneCode);
            Guest1.Phone = Guest1Phone;
            db.OtelGuests.Add(Guest1);
            db.SaveChanges();

            ThisUserOtel.NumberOfGuest = ThisUserOtel.NumberOfGuest + 1;
            db.Entry(ThisUserOtel).State = EntityState.Modified;
            db.SaveChanges();

            return RedirectToAction("UserAccomodation", "Events", new { UserOtelID = UserOtelID });
        }

        public ActionResult NewEvent()
        {
            return View();
        }

        [HttpPost]
        public ActionResult NewEvent(Event evnt, HttpPostedFileBase imageinputsmall, HttpPostedFileBase imageinputlarge)
        {
            db.Event.Add(evnt);
            db.SaveChanges();

            if(imageinputsmall == null)
            {
                System.IO.File.Copy(Server.MapPath("/Images/Logo/small.png"), Server.MapPath("/Images/Logo/" + evnt.Id + "s.png"));
                string path = System.IO.Path.Combine(Server.MapPath("~/Images/Logo/" + evnt.Id + "s.png"));
                Image defImage = Image.FromFile(Server.MapPath("/Images/Logo/small.png"));
                var newImage = ScaleImage(defImage, 86, 14);
                newImage.Save(path);
            }
            else
            {
                System.IO.File.Copy(Server.MapPath("/Images/Logo/small.png"), Server.MapPath("/Images/Logo/" + evnt.Id + "s.png"));
                string path = System.IO.Path.Combine(Server.MapPath("~/Images/Logo/" + evnt.Id + "s.png"));
                Image sourceimage = Image.FromStream(imageinputsmall.InputStream);
                var newImage = ScaleImage(sourceimage, 86, 14);
                newImage.Save(path);
            }

            if (imageinputlarge == null)
            {
                System.IO.File.Copy(Server.MapPath("/Images/Logo/big.png"), Server.MapPath("/Images/Logo/" + evnt.Id + "b.png"));
                string path = System.IO.Path.Combine(Server.MapPath("~/Images/Logo/" + evnt.Id + "b.png"));
                Image defImage = Image.FromFile(Server.MapPath("/Images/Logo/big.png"));
                var newImage = ScaleImage(defImage, 104, 17);
                newImage.Save(path);
            }
            else
            {
                System.IO.File.Copy(Server.MapPath("/Images/Logo/big.png"), Server.MapPath("/Images/Logo/" + evnt.Id + "b.png"));
                string path = System.IO.Path.Combine(Server.MapPath("~/Images/Logo/" + evnt.Id + "b.png"));
                Image sourceimage = Image.FromStream(imageinputlarge.InputStream);
                var newImage = ScaleImage(sourceimage, 104, 17);
                newImage.Save(path);
            }

            return RedirectToAction("Home", "Home");
        }
        public ActionResult Gallery()
        {
            return View();
        }

        public ActionResult CommentWall()
        {
            return View();
        }


















































































































































































































































































































































































































































































































































        public ActionResult AAAA()
        {
            return View();
        }

        // GET: Events
        public ActionResult Index()
        {
            return View(db.Event.ToList());
        }

        // GET: Events/Details/5
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Event @event = db.Event.Find(id);
            if (@event == null)
            {
                return HttpNotFound();
            }
            return View(@event);
        }

        // GET: Events/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Events/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,IsActive,EventConfirmation,EventLogo,Facebook,Twitter,Instagram,Linkedin,Website,Youtube,Pinterest")] Event @event)
        {
            if (ModelState.IsValid)
            {
                @event.Id = Guid.NewGuid();
                db.Event.Add(@event);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(@event);
        }

        // GET: Events/Edit/5
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Event @event = db.Event.Find(id);
            if (@event == null)
            {
                return HttpNotFound();
            }
            return View(@event);
        }

        // POST: Events/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,IsActive,EventConfirmation,EventLogo,Facebook,Twitter,Instagram,Linkedin,Website,Youtube,Pinterest")] Event @event)
        {
            if (ModelState.IsValid)
            {
                db.Entry(@event).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(@event);
        }

        // GET: Events/Delete/5
        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Event @event = db.Event.Find(id);
            if (@event == null)
            {
                return HttpNotFound();
            }
            return View(@event);
        }

        // POST: Events/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            Event @event = db.Event.Find(id);
            db.Event.Remove(@event);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
