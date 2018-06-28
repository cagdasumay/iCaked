using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using EventApp.Models;
using EventApp.EventApp;
using System.Drawing;


namespace EventApp.Controllers
{
    public class AccountController : Controller
    {
        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["EventAppContext"].ConnectionString);
        private EventAppContext db = new EventAppContext();

        public static Image ScaleImage(Image image, int newWidth, int newHeight)
        {
            var newImage = new Bitmap(newWidth, newHeight);

            using (var graphics = Graphics.FromImage(newImage))
                graphics.DrawImage(image, 0, 0, newWidth, newHeight);

            return newImage;
        }

        public ActionResult Login(string error)
        {
            if (error == "LoginError")
            {
                ViewData["LoginError"] = "LoginError";
                return View();
            }
            else
            {
                ViewData["LoginError"] = "Correct";
                return View();
            }
        }

        public ActionResult LoginTrial(String EMail, String password)
        {
            // aşağıdaki loginControl metodunu kullanıyor
            if (loginControl(EMail, password) == true)
            {
                return RedirectToAction("Home", "Home");
            }
            else
            {
                string LoginError = "LoginError";
                return RedirectToAction("Login", new { error = LoginError });
            }
        }

        public bool loginControl(String EMail, String Password)
        {
            bool result = false;

            // sql içerisinde girilen verileri arıyor
            String query = "select * from [User] where Mail = '" + EMail + "' and Password = '" + Password + "'";

            // connection'ı gerçekleştiriyor galiba
            DataTable table = new DataTable();
            SqlCommand cmd = new SqlCommand(query, con);
            SqlDataAdapter da = new SqlDataAdapter(cmd);

            da.Fill(table);
            con.Close();

            result = (table.Rows.Count > 0);

            if (result == true)
            {
                var list = db.User.ToList();
                var checkUser = list.Where(k => k.Mail == EMail).ToList();

                User user = new Models.User();

                user.Id = checkUser[0].Id;
                user.Name = checkUser[0].Name;
                user.Surname = checkUser[0].Surname;
                user.UserTypeID = checkUser[0].UserTypeID;
                user.Mail = checkUser[0].Mail;
                user.Company = checkUser[0].Company;
                user.PersonelTitle = checkUser[0].PersonelTitle;
                user.Phone = checkUser[0].Phone;
                user.CountryPhoneCode = checkUser[0].CountryPhoneCode;
                user.IsActive = checkUser[0].IsActive;
                user.AccountConfirmation = checkUser[0].AccountConfirmation;

                Session["LoggedIn"] = user;

                var AllEvents = db.Event.ToList();
                var IDofEvent = AllEvents.Where(p => p.EventConfirmation == checkUser[0].AccountConfirmation).ToList();
                Session["EventID"] = IDofEvent[0].Id;
                
                UserType ThisUser = db.UserType.Find(user.UserTypeID);
                Session["UserType"] = ThisUser.Name;

                if (ThisUser.Name == "Katılımcı")
                {
                    int ScheduledAgenda = MyAgendaU(user);
                    Session["AgendaCountU"] = ScheduledAgenda;

                    var allUserOtel = db.UserOtels.ToList();
                    var ThisUserOtel = allUserOtel.Where(k => k.UserID == user.Id).ToList();
                    if (ThisUserOtel.Count > 0 && ThisUserOtel[0].Reservation == true)
                    {
                        var allUserCheckIn = db.UserCheckIn.ToList();
                        var thisUserCheckIn = allUserCheckIn.Where(k => k.UserOtelID == ThisUserOtel[0].Id).ToList();
                        if (thisUserCheckIn.Count == 0)
                        {
                            Session["UserCheckIn"] = ThisUserOtel[0].Id;
                        }
                        else
                        {
                            Session["UserCheckIn"] = Guid.Empty;
                        }
                    }
                    else
                    {
                        Session["UserCheckIn"] = Guid.Empty;
                    }
                }
                else if (ThisUser.Name == "Acente")
                {
                    int ScheduledAgenda = RespAct(user);
                    Session["AgendaCountS"] = ScheduledAgenda;
                }

            }
            return result;
        }

        // Select User Type Sayfası
        public ActionResult SelectUserType(string error)
        {
            List<UserType> UserTypeList = new List<UserType>();
            List<UserType> TypeList = db.UserType.ToList();

            UserTypeList = TypeList.OrderBy(d => (d.Name)).ToList();
            if (error == "Error")
            {
                ViewData["UserTypeError"] = "Error";
            }
            else
            {
                ViewData["UserTypeError"] = "Correct";
            }

            return View(UserTypeList);
        }

        // Select User Type sayfası seçim kontrol
        public ActionResult RegisterForm(Guid usertype)
        {
            List<UserType> TypeList = db.UserType.ToList();
            int count = TypeList.Count;
            for (int i = 0; i < count; i++)
            {
                if (usertype == TypeList[i].Id)
                {
                    UserType registration = new UserType();
                    registration.Id = TypeList[i].Id;
                    registration.Name = TypeList[i].Name;
                    Session["RegistrationUserType"] = registration;
                    return RedirectToAction("RegistrationType", "Account");
                }
            }
            return RedirectToAction("SelectUserType", new { error = "Error" });
        }

        // Registration View'ı burada beirleniyor
        // RegistrSpeaker veya RegisterOrganizationTeam'e giderken araya bir katman confirmation konabilir. Onun için buradaki RedirectToAction'lar değişmeli
        public ActionResult RegistrationType()
        {
            UserType registrationtype = (UserType)Session["RegistrationUserType"];
            if (registrationtype.Name.ToString() == "Katılımcı")
            {
                return RedirectToAction("RegisterAttender", new { registration = registrationtype.Id });
            }
            else if ((string)registrationtype.Name == "Konuşmacı")
            {
                return RedirectToAction("RegisterSpeaker", new { registration = registrationtype.Id });
            }
            else if ((string)registrationtype.Name == "Acente")
            {
                return RedirectToAction("RegisterOrganizationTeam", new { registration = registrationtype.Id });
            }
            return RedirectToAction("SelectUserType", "Account");
        }

        // Attender'lar için Registration
        public ActionResult RegisterAttender(Guid registration)
        {
            Session["UserTypeId"] = registration;
            ViewData["ValidationError"] = "Correct";
            return View();
        }
        [HttpPost]
        public ActionResult RegisterAttender(User usrA, HttpPostedFileBase imageinput)
        {
            if (ModelState.IsValid)
            {
                using (EventAppContext db = new EventAppContext())
                {
                    List<Event> AllEvents = db.Event.ToList();
                    UserEvent AttendedEvent = new UserEvent();
                    for (int i = 0; i < AllEvents.Count; i++)
                    {
                        if (AllEvents[i].EventConfirmation == usrA.AccountConfirmation)
                        {
                            AttendedEvent.EventID = AllEvents[i].Id;
                        }
                    }

                    if (AttendedEvent.EventID.ToString() != "00000000-0000-0000-0000-000000000000")
                    {
                        usrA.UserTypeID = (Guid)Session["UserTypeId"];
                        usrA.ShowMyDetails = true;
                        db.User.Add(usrA);
                        db.SaveChanges();

                        using (EventAppContext dbUE = new EventAppContext())
                        {
                            AttendedEvent.UserID = usrA.Id;
                            AttendedEvent.IsActive = true;
                            dbUE.UserEvent.Add(AttendedEvent);
                            dbUE.SaveChanges();
                        }
                        usrA.PhotoDirectory = ("../ Images /Users/" + usrA.Id.ToString() + ".jpg");
                    }
                    else
                    {
                        usrA.UserTypeID = (Guid)Session["UserTypeId"];
                        ViewData["ValidationError"] = "ValidationError";
                        return View();
                    }

                }

                if (imageinput == null)
                {
                    System.IO.File.Copy(Server.MapPath("/Images/Users/default_thumbnail.png"), Server.MapPath("/Images/Users/" + usrA.Id + ".jpg"));
                    string path = System.IO.Path.Combine(Server.MapPath("~/Images/Users/" + usrA.Id + ".jpg"));
                    Image defImage = Image.FromFile(Server.MapPath("/Images/Users/default_thumbnail.png"));
                    var newImage = ScaleImage(defImage, 100, 100);
                    newImage.Save(path);
                    return RedirectToAction("Login", "Account");
                }
                else
                {
                    System.IO.File.Copy(Server.MapPath("/Images/Users/default_thumbnail.png"), Server.MapPath("/Images/Users/" + usrA.Id + ".jpg"));
                    string path = System.IO.Path.Combine(Server.MapPath("~/Images/Users/" + usrA.Id + ".jpg"));
                    Image sourceimage = Image.FromStream(imageinput.InputStream);
                    var newImage = ScaleImage(sourceimage, 200, 200);
                    newImage.Save(path);
                    return RedirectToAction("Login", "Account");
                }

            }

            return View();
        }

        public ActionResult RegisterSpeaker(Guid registration)
        {
            Session["UserTypeId"] = registration;
            ViewData["SpeakerCompany"] = "Correct";
            ViewData["SpeakerTitle"] = "Correct";
            ViewData["ValidationError"] = "Correct";
            ViewData["SpeakerAbout"] = "Correct";
            return View();
        }
        [HttpPost]
        public ActionResult RegisterSpeaker(User usrS, HttpPostedFileBase imageinput)
        {
            if (ModelState.IsValid)
            {
                using (EventAppContext db = new EventAppContext())
                {
                    List<Event> AllEvents = db.Event.ToList();
                    UserEvent AttendedEvent = new UserEvent();
                    for (int i = 0; i < AllEvents.Count; i++)
                    {
                        if (AllEvents[i].EventConfirmation == usrS.AccountConfirmation)
                        {
                            AttendedEvent.EventID = AllEvents[i].Id;
                        }
                    }

                    if (AttendedEvent.EventID.ToString() != "00000000-0000-0000-0000-000000000000")
                    {
                        if (usrS.Company == null || usrS.PersonelTitle == null || usrS.About == null)
                        {
                            if (usrS.Company == null)
                            {
                                ViewData["SpeakerCompany"] = "CompanyError";
                            }
                            if (usrS.PersonelTitle == null)
                            {
                                ViewData["SpeakerTitle"] = "TitleError";
                            }
                            if (usrS.About == null)
                            {
                                ViewData["SpeakerAbout"] = "AboutError";
                            }
                            usrS.UserTypeID = (Guid)Session["UserTypeId"];
                            ViewData["ValidationError"] = "Correct";
                            return View();
                        }
                        else
                        {
                            usrS.UserTypeID = (Guid)Session["UserTypeId"];
                            usrS.ShowMyDetails = true;
                            db.User.Add(usrS);
                            db.SaveChanges();

                            using (EventAppContext dbUE = new EventAppContext())
                            {
                                AttendedEvent.UserID = usrS.Id;
                                AttendedEvent.IsActive = true;
                                dbUE.UserEvent.Add(AttendedEvent);
                                dbUE.SaveChanges();
                            }
                            usrS.PhotoDirectory = ("../ Images /Users/" + usrS.Id.ToString() + ".jpg");
                        }
                    }
                    else
                    {
                        if (usrS.Company == null || usrS.PersonelTitle == null || usrS.About == null)
                        {
                            if (usrS.Company == null)
                            {
                                ViewData["SpeakerCompany"] = "CompanyError";
                            }
                            if (usrS.PersonelTitle == null)
                            {
                                ViewData["SpeakerTitle"] = "TitleError";
                            }
                            if (usrS.About == null)
                            {
                                ViewData["SpeakerAbout"] = "AboutError";
                            }
                        }
                        usrS.UserTypeID = (Guid)Session["UserTypeId"];
                        ViewData["ValidationError"] = "ValidationError";
                        return View();
                    }

                }

                if (imageinput == null)
                {
                    System.IO.File.Copy(Server.MapPath("/Images/Users/default_thumbnail.png"), Server.MapPath("/Images/Users/" + usrS.Id + ".jpg"));
                    string path = System.IO.Path.Combine(Server.MapPath("~/Images/Users/" + usrS.Id + ".jpg"));
                    Image defImage = Image.FromFile(Server.MapPath("/Images/Users/default_thumbnail.png"));
                    var newImage = ScaleImage(defImage, 100, 100);
                    newImage.Save(path);
                    return RedirectToAction("Login", "Account");
                }
                else
                {
                    System.IO.File.Copy(Server.MapPath("/Images/Users/default_thumbnail.png"), Server.MapPath("/Images/Users/" + usrS.Id + ".jpg"));
                    string path = System.IO.Path.Combine(Server.MapPath("~/Images/Users/" + usrS.Id + ".jpg"));
                    Image sourceimage = Image.FromStream(imageinput.InputStream);
                    var newImage = ScaleImage(sourceimage, 200, 200);
                    newImage.Save(path);
                    return RedirectToAction("Login", "Account");
                }
            }

            return View();
        }

        public ActionResult RegisterOrganizationTeam(Guid registration)
        {
            Session["UserTypeId"] = registration;
            ViewData["ValidationError"] = "Correct";
            return View();
        }

        [HttpPost]
        public ActionResult RegisterOrganizationTeam(User usrO, HttpPostedFileBase imageinput)
        {
            if (ModelState.IsValid)
            {
                using (EventAppContext db = new EventAppContext())
                {
                    List<Event> AllEvents = db.Event.ToList();
                    UserEvent AttendedEvent = new UserEvent();
                    for (int i = 0; i < AllEvents.Count; i++)
                    {
                        if (AllEvents[i].EventConfirmation == usrO.AccountConfirmation)
                        {
                            AttendedEvent.EventID = AllEvents[i].Id;
                        }
                    }

                    if (AttendedEvent.EventID.ToString() != "00000000-0000-0000-0000-000000000000")
                    {
                        usrO.UserTypeID = (Guid)Session["UserTypeId"];
                        usrO.ShowMyDetails = true;
                        db.User.Add(usrO);
                        db.SaveChanges();

                        using (EventAppContext dbUE = new EventAppContext())
                        {
                            AttendedEvent.UserID = usrO.Id;
                            AttendedEvent.IsActive = true;
                            dbUE.UserEvent.Add(AttendedEvent);
                            dbUE.SaveChanges();
                        }
                        usrO.PhotoDirectory = ("../ Images /Users/" + usrO.Id.ToString() + ".jpg");
                    }
                    else
                    {
                        usrO.UserTypeID = (Guid)Session["UserTypeId"];
                        ViewData["ValidationError"] = "ValidationError";
                        return View();
                    }

                }

                if (imageinput == null)
                {
                    System.IO.File.Copy(Server.MapPath("/Images/Users/default_thumbnail.png"), Server.MapPath("/Images/Users/" + usrO.Id + ".jpg"));
                    string path = System.IO.Path.Combine(Server.MapPath("~/Images/Users/" + usrO.Id + ".jpg"));
                    Image defImage = Image.FromFile(Server.MapPath("/Images/Users/default_thumbnail.png"));
                    var newImage = ScaleImage(defImage, 100, 100);
                    newImage.Save(path);
                    return RedirectToAction("Login", "Account");
                }
                else
                {
                    System.IO.File.Copy(Server.MapPath("/Images/Users/default_thumbnail.png"), Server.MapPath("/Images/Users/" + usrO.Id + ".jpg"));
                    string path = System.IO.Path.Combine(Server.MapPath("~/Images/Users/" + usrO.Id + ".jpg"));
                    Image sourceimage = Image.FromStream(imageinput.InputStream);
                    var newImage = ScaleImage(sourceimage, 200, 200);
                    newImage.Save(path);
                    return RedirectToAction("Login", "Account");
                }

            }

            return View();
        }

        public ActionResult NewAttender()
        {
            ViewData["NameError"] = "Correct";
            ViewData["SurnameError"] = "Correct";
            ViewData["MailError"] = "Correct";
            return View();
        }

        public ActionResult NewSpeaker()
        {
            ViewData["NameError"] = "Correct";
            ViewData["SurnameError"] = "Correct";
            ViewData["MailError"] = "Correct";
            ViewData["AboutError"] = "Correct";
            ViewData["TitleError"] = "Correct";
            return View();
        }

        [HttpPost]
        public ActionResult NewAttender(User usrA, HttpPostedFileBase imageinput)
        {

            string NameError = "Correct";
            string SurnameError = "Correct";
            string MailError = "Correct";

            if (usrA.Name == null || usrA.Surname == null || usrA.Mail == null)
            {
                if (usrA.Name == null)
                {
                    NameError = "NameError";
                }
                if (usrA.Surname == null)
                {
                    SurnameError = "SurnameError";
                }
                if (usrA.Mail == null)
                {
                    MailError = "MailError";
                }
                ViewData["NameError"] = NameError;
                ViewData["SurnameError"] = SurnameError;
                ViewData["MailError"] = MailError;
                return View();
            }
            else
            {
                ViewData["NameError"] = NameError;
                ViewData["SurnameError"] = SurnameError;
                ViewData["MailError"] = MailError;
            }

            using (EventAppContext db = new EventAppContext())
            {
                User LoggedUser = (User)Session["LoggedIn"];
                List<Event> AllEvents = db.Event.ToList();
                var thisEvent = AllEvents.Where(q => q.EventConfirmation == LoggedUser.AccountConfirmation).ToList();
                usrA.AccountConfirmation = thisEvent[0].EventConfirmation;

                var alltypes = db.UserType.ToList();
                var attendertype = alltypes.Where(k => k.Name == "Katılımcı").ToList();
                UserType thistype = db.UserType.Find(attendertype[0].Id);
                usrA.UserTypeID = thistype.Id;

                string pass = CreateRandomPassword();
                usrA.Password = pass;
                usrA.ConfirmPassword = pass;

                usrA.ShowMyDetails = true;
                db.User.Add(usrA);
                db.SaveChanges();

                UserEvent AttendedEvent = new UserEvent();
                using (EventAppContext dbUE = new EventAppContext())
                {
                    AttendedEvent.UserID = usrA.Id;
                    AttendedEvent.EventID = thisEvent[0].Id;
                    AttendedEvent.IsActive = true;

                    dbUE.UserEvent.Add(AttendedEvent);
                    dbUE.SaveChanges();
                }
                usrA.PhotoDirectory = ("../ Images /Users/" + usrA.Id.ToString() + ".jpg");
            }

            if (imageinput == null)
            {
                System.IO.File.Copy(Server.MapPath("/Images/Users/default_thumbnail.png"), Server.MapPath("/Images/Users/" + usrA.Id + ".jpg"));
                string path = System.IO.Path.Combine(Server.MapPath("~/Images/Users/" + usrA.Id + ".jpg"));
                Image defImage = Image.FromFile(Server.MapPath("/Images/Users/default_thumbnail.png"));
                var newImage = ScaleImage(defImage, 100, 100);
                newImage.Save(path);
                return RedirectToAction("AttendersOrg", "Users");
            }
            else
            {
                System.IO.File.Copy(Server.MapPath("/Images/Users/default_thumbnail.png"), Server.MapPath("/Images/Users/" + usrA.Id + ".jpg"));
                string path = System.IO.Path.Combine(Server.MapPath("~/Images/Users/" + usrA.Id + ".jpg"));
                Image sourceimage = Image.FromStream(imageinput.InputStream);
                var newImage = ScaleImage(sourceimage, 200, 200);
                newImage.Save(path);
                return RedirectToAction("AttendersOrg", "Users");
            }

        }

        [HttpPost]
        public ActionResult NewSpeaker(User usrA, HttpPostedFileBase imageinput)
        {

            string NameError = "Correct";
            string SurnameError = "Correct";
            string MailError = "Correct";
            string AboutError = "Correct";
            string TitleError = "Correct";

            if (usrA.Name == null || usrA.Surname == null || usrA.Mail == null || usrA.About == null || usrA.PersonelTitle == null)
            {
                if (usrA.Name == null)
                {
                    NameError = "NameError";
                }
                if (usrA.Surname == null)
                {
                    SurnameError = "SurnameError";
                }
                if (usrA.Mail == null)
                {
                    MailError = "MailError";
                }
                if (usrA.About == null)
                {
                    AboutError = "AboutError";
                }
                if (usrA.PersonelTitle == null)
                {
                    TitleError = "TitleError";
                }
                ViewData["NameError"] = NameError;
                ViewData["SurnameError"] = SurnameError;
                ViewData["MailError"] = MailError;
                ViewData["AboutError"] = AboutError;
                ViewData["TitleError"] = TitleError;
                return View();
            }
            else
            {
                ViewData["NameError"] = NameError;
                ViewData["SurnameError"] = SurnameError;
                ViewData["MailError"] = MailError;
                ViewData["AboutError"] = AboutError;
                ViewData["TitleError"] = TitleError;
            }

            using (EventAppContext db = new EventAppContext())
            {
                User LoggedUser = (User)Session["LoggedIn"];
                List<Event> AllEvents = db.Event.ToList();
                var thisEvent = AllEvents.Where(q => q.EventConfirmation == LoggedUser.AccountConfirmation).ToList();
                usrA.AccountConfirmation = thisEvent[0].EventConfirmation;

                var alltypes = db.UserType.ToList();
                var attendertype = alltypes.Where(k => k.Name == "Konuşmacı").ToList();
                UserType thistype = db.UserType.Find(attendertype[0].Id);
                usrA.UserTypeID = thistype.Id;

                string pass = CreateRandomPassword();
                usrA.Password = pass;
                usrA.ConfirmPassword = pass;

                usrA.ShowMyDetails = true;
                db.User.Add(usrA);
                db.SaveChanges();

                UserEvent AttendedEvent = new UserEvent();
                using (EventAppContext dbUE = new EventAppContext())
                {
                    AttendedEvent.UserID = usrA.Id;
                    AttendedEvent.EventID = thisEvent[0].Id;
                    AttendedEvent.IsActive = true;

                    dbUE.UserEvent.Add(AttendedEvent);
                    dbUE.SaveChanges();
                }
                usrA.PhotoDirectory = ("../ Images /Users/" + usrA.Id.ToString() + ".jpg");
            }

            if (imageinput == null)
            {
                System.IO.File.Copy(Server.MapPath("/Images/Users/default_thumbnail.png"), Server.MapPath("/Images/Users/" + usrA.Id + ".jpg"));
                string path = System.IO.Path.Combine(Server.MapPath("~/Images/Users/" + usrA.Id + ".jpg"));
                Image defImage = Image.FromFile(Server.MapPath("/Images/Users/default_thumbnail.png"));
                var newImage = ScaleImage(defImage, 100, 100);
                newImage.Save(path);
                return RedirectToAction("SpeakersOrg", "Users");
            }
            else
            {
                System.IO.File.Copy(Server.MapPath("/Images/Users/default_thumbnail.png"), Server.MapPath("/Images/Users/" + usrA.Id + ".jpg"));
                string path = System.IO.Path.Combine(Server.MapPath("~/Images/Users/" + usrA.Id + ".jpg"));
                Image sourceimage = Image.FromStream(imageinput.InputStream);
                var newImage = ScaleImage(sourceimage, 200, 200);
                newImage.Save(path);
                return RedirectToAction("SpeakersOrg", "Users");
            }

        }

        private static string CreateRandomPassword()
        {
            int passwordLength = 10;
            string allowedChars = "abcdefghijkmnopqrstuvwxyzABCDEFGHJKLMNOPQRSTUVWXYZ0123456789!@$?_-";
            char[] chars = new char[passwordLength];
            Random rd = new Random();

            for (int i = 0; i < passwordLength; i++)
            {
                chars[i] = allowedChars[rd.Next(0, allowedChars.Length)];
            }

            return new string(chars);
        }

        [HttpPost]
        public ActionResult ChangeAttender(User usrA, Guid ThisAttender, HttpPostedFileBase imageinput)
        {
            User ThisUser = db.User.Find(ThisAttender);

            if (usrA.Name != null)
            {
                ThisUser.Name = usrA.Name;
            }

            if (usrA.Surname != null)
            {
                ThisUser.Surname = usrA.Surname;
            }

            if (usrA.Mail != null)
            {
                ThisUser.Mail = usrA.Mail;
            }

            ThisUser.Phone = usrA.Phone;
            ThisUser.Company = usrA.Company;
            ThisUser.PersonelTitle = usrA.PersonelTitle;
            ThisUser.CountryPhoneCode = usrA.CountryPhoneCode;
            ThisUser.City = usrA.City;
            ThisUser.Facebook = usrA.Facebook;
            ThisUser.Instagram = usrA.Instagram;
            ThisUser.Twitter = usrA.Twitter;
            ThisUser.Linkedin = usrA.Linkedin;
            ThisUser.Website = usrA.Website;

            db.SaveChanges();

            if (imageinput != null)
            {
                string path = System.IO.Path.Combine(Server.MapPath("~/Images/Users/" + ThisAttender + ".jpg"));
                Image sourceimage = Image.FromStream(imageinput.InputStream);
                var newImage = ScaleImage(sourceimage, 200, 200);
                newImage.Save(path);
            }

            return RedirectToAction("AttenderDetailsOrg", "Users", new { usrdet = ThisUser.Id });
        }

        public ActionResult DeleteAttender(Guid UserID)
        {
            User DeleteFromUser = db.User.Find(UserID);

            var UserEventList = db.UserEvent.ToList();
            var UE = UserEventList.Where(k => k.UserID == UserID).ToList();
            UserEvent ThisUserEvent = db.UserEvent.Find(UE[0].Id);

            db.User.Remove(DeleteFromUser);
            db.SaveChanges();

            var DelUserEventActivity = db.UserEventActivity.ToList();
            var UEAList = DelUserEventActivity.Where(l => l.UserEventID == ThisUserEvent.Id).ToList();
            for (int i = 0; i < UEAList.Count; i++)
            {
                UserEventActivity deletethis = db.UserEventActivity.Find(UEAList[i].Id);
                db.UserEventActivity.Remove(deletethis);
                db.SaveChanges();
            }

            db.UserEvent.Remove(ThisUserEvent);
            db.SaveChanges();

            return RedirectToAction("AttendersOrg", "Users");
        }

        public ActionResult DeleteSpeaker(Guid UserID)
        {
            User DeleteFromUser = db.User.Find(UserID);

            var UserEventList = db.UserEvent.ToList();
            var UE = UserEventList.Where(k => k.UserID == UserID).ToList();
            UserEvent ThisUserEvent = db.UserEvent.Find(UE[0].Id);

            var allspeakerEA = db.SpeakerEventActivity.ToList();
            var thisSpeakerSessions = allspeakerEA.Where(l => l.SpeakerID == UserID).ToList();
            for (int i = 0; i < thisSpeakerSessions.Count; i++)
            {
                SpeakerEventActivity delete = db.SpeakerEventActivity.Find(thisSpeakerSessions[i].Id);
                db.SpeakerEventActivity.Remove(delete);
                db.SaveChanges();
            }

            db.User.Remove(DeleteFromUser);
            db.SaveChanges();

            var DelUserEventActivity = db.UserEventActivity.ToList();
            var UEAList = DelUserEventActivity.Where(l => l.UserEventID == ThisUserEvent.Id).ToList();
            for (int i = 0; i < UEAList.Count; i++)
            {
                UserEventActivity deletethis = db.UserEventActivity.Find(UEAList[i].Id);
                db.UserEventActivity.Remove(deletethis);
                db.SaveChanges();
            }

            db.UserEvent.Remove(ThisUserEvent);
            db.SaveChanges();

            return RedirectToAction("SpeakersOrg", "Users");
        }

        [HttpPost]
        public ActionResult ChangeSpeaker(User usrA, Guid ThisAttender, HttpPostedFileBase imageinput)
        {
            User ThisUser = db.User.Find(ThisAttender);

            if (usrA.Name != null)
            {
                ThisUser.Name = usrA.Name;
            }

            if (usrA.Surname != null)
            {
                ThisUser.Surname = usrA.Surname;
            }

            if (usrA.Mail != null)
            {
                ThisUser.Mail = usrA.Mail;
            }

            if (usrA.About != null)
            {
                ThisUser.About = usrA.About;
            }

            ThisUser.Phone = usrA.Phone;
            ThisUser.Company = usrA.Company;
            ThisUser.PersonelTitle = usrA.PersonelTitle;
            ThisUser.CountryPhoneCode = usrA.CountryPhoneCode;
            ThisUser.City = usrA.City;
            ThisUser.Facebook = usrA.Facebook;
            ThisUser.Instagram = usrA.Instagram;
            ThisUser.Twitter = usrA.Twitter;
            ThisUser.Linkedin = usrA.Linkedin;
            ThisUser.Website = usrA.Website;

            db.SaveChanges();

            if (imageinput != null)
            {
                string path = System.IO.Path.Combine(Server.MapPath("~/Images/Users/" + ThisAttender + ".jpg"));
                Image sourceimage = Image.FromStream(imageinput.InputStream);
                var newImage = ScaleImage(sourceimage, 200, 200);
                newImage.Save(path);
            }

            return RedirectToAction("SpeakerDetailsOrg", "Users", new { sprdet = ThisUser.Id });
        }

        public ActionResult ChangeMyProfileU(User usrA, Guid ThisAttender, HttpPostedFileBase imageinput)
        {
            User ThisUser = db.User.Find(ThisAttender);

            if (usrA.Name != null)
            {
                ThisUser.Name = usrA.Name;
            }

            if (usrA.Surname != null)
            {
                ThisUser.Surname = usrA.Surname;
            }

            if (usrA.Mail != null)
            {
                ThisUser.Mail = usrA.Mail;
            }

            ThisUser.Phone = usrA.Phone;
            ThisUser.Company = usrA.Company;
            ThisUser.PersonelTitle = usrA.PersonelTitle;
            ThisUser.CountryPhoneCode = usrA.CountryPhoneCode;
            ThisUser.City = usrA.City;
            ThisUser.Facebook = usrA.Facebook;
            ThisUser.Instagram = usrA.Instagram;
            ThisUser.Twitter = usrA.Twitter;
            ThisUser.Linkedin = usrA.Linkedin;
            ThisUser.Website = usrA.Website;

            db.SaveChanges();

            if (imageinput != null)
            {
                string path = System.IO.Path.Combine(Server.MapPath("~/Images/Users/" + ThisAttender + ".jpg"));
                Image sourceimage = Image.FromStream(imageinput.InputStream);
                var newImage = ScaleImage(sourceimage, 200, 200);
                newImage.Save(path);
            }

            return RedirectToAction("MyProfileU", "Users", new { usrdet = ThisUser.Id });
        }

        public dynamic MyAgendaU(User user)
        {
            // UserEventID
            User LoggedUser = user;
            List<Event> AllEvents = db.Event.ToList();
            var thisEvent = AllEvents.Where(q => q.EventConfirmation == LoggedUser.AccountConfirmation).ToList();
            List<UserEvent> usrevnt = db.UserEvent.ToList();
            var thisUserEvent = usrevnt.Where(w => w.UserID == LoggedUser.Id && w.EventID == thisEvent[0].Id).ToList();

            // EventActivityID
            var AllEventActivity = db.EventActivity.ToList();
            List<EventActivity> ThisEventActivities = new List<EventActivity>();
            for (int i = 0; i < AllEventActivity.Count(); i++)
            {
                if (AllEventActivity[i].EventID == thisEvent[0].Id)
                {
                    EventActivity toAgenda2 = db.EventActivity.Find(AllEventActivity[i].Id);
                    ThisEventActivities.Add(toAgenda2);
                }
            }

            // From UserEventActivity
            List<EventActivity> ScheduledEventActivities = new List<EventActivity>();
            var AllScheduledActivities = db.UserEventActivity.ToList();
            for (int j = 0; j < ThisEventActivities.Count; j++)
            {
                var isscheduled = AllScheduledActivities.Where(k => k.EventActivityID == ThisEventActivities[j].Id && k.UserEventID == thisUserEvent[0].Id).ToList();
                if (isscheduled.Count == 1)
                {
                    EventActivity addthis = db.EventActivity.Find(ThisEventActivities[j].Id);
                    ScheduledEventActivities.Add(addthis);
                }
            }

            // ScheduledActivities
            var AllActivities = db.Activity.ToList();
            List<Activity> ActivitiesForAgenda = new List<Activity>();
            for (int t = 0; t < ScheduledEventActivities.Count; t++)
            {
                Activity addActivity = db.Activity.Find(ScheduledEventActivities[t].ActivityID);
                ActivitiesForAgenda.Add(addActivity);
            }

            // Conference Type'ları alıyoruz
            var TypeOfActivity = db.ActivityCategory.ToList();
            var ConferenceType = TypeOfActivity.Where(k => k.Name == "Konferans").ToList();
            var PresentationType = TypeOfActivity.Where(l => l.Name == "Sunum").ToList();
            var PanelType = TypeOfActivity.Where(l => l.Name == "Panel").ToList();
            var CoffeeType = TypeOfActivity.Where(l => l.Name == "Kahve Molası-Yemek-Parti").ToList();

            // Activity Name'ini buluyor
            int countActivityList = ActivitiesForAgenda.Count;
            List<Activity> AgendaList = new List<Activity>();
            for (int i = 0; i < countActivityList; i++)
            {
                if (ActivitiesForAgenda[i].ActivityCategoryID == ConferenceType[0].Id)
                {
                    Activity newconference = db.Activity.Find(ActivitiesForAgenda[i].Id);
                    AgendaList.Add(newconference);
                }
                else if (ActivitiesForAgenda[i].ActivityCategoryID == PresentationType[0].Id)
                {
                    Activity newpresentation = db.Activity.Find(ActivitiesForAgenda[i].Id);
                    AgendaList.Add(newpresentation);
                }
                else if (ActivitiesForAgenda[i].ActivityCategoryID == PanelType[0].Id)
                {
                    Activity newpresentation = db.Activity.Find(ActivitiesForAgenda[i].Id);
                    AgendaList.Add(newpresentation);
                }
                else if (ActivitiesForAgenda[i].ActivityCategoryID == CoffeeType[0].Id)
                {
                    Activity newpresentation = db.Activity.Find(ActivitiesForAgenda[i].Id);
                    AgendaList.Add(newpresentation);
                }
            }

            return (AgendaList.Count);
        }

        public ActionResult LogOut()
        {
            Session["LoggedIn"] = null;
            return RedirectToAction("Login", "Account");
        }

        public dynamic RespAct(User user)
        {
            // UserEventID
            User LoggedUser = user;
            List<Event> AllEvents = db.Event.ToList();
            var thisEvent = AllEvents.Where(q => q.EventConfirmation == LoggedUser.AccountConfirmation).ToList();
            List<UserEvent> usrevnt = db.UserEvent.ToList();
            var thisUserEvent = usrevnt.Where(w => w.UserID == LoggedUser.Id && w.EventID == thisEvent[0].Id).ToList();

            // EventActivityID
            var AllEventActivity = db.EventActivity.ToList();
            List<EventActivity> ThisEventActivities = new List<EventActivity>();
            for (int i = 0; i < AllEventActivity.Count(); i++)
            {
                if (AllEventActivity[i].EventID == thisEvent[0].Id)
                {
                    EventActivity toAgenda2 = db.EventActivity.Find(AllEventActivity[i].Id);
                    ThisEventActivities.Add(toAgenda2);
                }
            }

            var AllRespoEA = db.ResponsibleEventActivity.ToList();
            int countActivities = 0;
            for (int i = 0; i < ThisEventActivities.Count; i++)
            {
                var isResp = AllRespoEA.Where(k => k.EventActivityID == ThisEventActivities[i].Id && k.ResponsibleID == user.Id).ToList();
                if (isResp.Count == 1)
                {
                    countActivities = countActivities + 1;
                }
            }
            
            return (countActivities);
        }

        public ActionResult Register()
        {
            ViewData["NameError"] = "Correct";
            ViewData["SurnameError"] = "Correct";
            ViewData["MailError"] = "Correct";
            ViewData["PhoneError"] = "Correct";
            ViewData["PasswordError"] = "Correct";
            ViewData["PasswordConfirmationError"] = "Correct";
            ViewData["PasswordMatchError"] = "Correct";
            ViewData["ValidationError"] = "Correct";
            return View();
        }
        [HttpPost]
        public ActionResult Register(User usrA, HttpPostedFileBase imageinput)
        {
            var AllTypes = db.UserType.ToList();
            var AttenderType = AllTypes.Where(k => k.Name == "Katılımcı").ToList();

            ViewData["NameError"] = "Correct";
            ViewData["SurnameError"] = "Correct";
            ViewData["MailError"] = "Correct";
            ViewData["PhoneError"] = "Correct";
            ViewData["PasswordError"] = "Correct";
            ViewData["PasswordConfirmationError"] = "Correct";
            ViewData["ValidationError"] = "Correct";
            ViewData["PasswordMatchError"] = "Correct";
            string PasswordMatchError = "Correct";

            if (usrA.Password != null && usrA.ConfirmPassword != null)
            {
                if (usrA.Password != usrA.ConfirmPassword)
                {
                    PasswordMatchError = "PasswordMatchError";
                }
            }

            if (usrA.Name == null || usrA.Surname == null || usrA.Mail == null || usrA.Phone == null || usrA.Password == null || usrA.ConfirmPassword == null || PasswordMatchError == "PasswordMatchError")
            {
                if (usrA.Name == null)
                {
                    ViewData["NameError"] = "NameError";
                }
                if (usrA.Surname == null)
                {
                    ViewData["SurnameError"] = "SurnameError";
                }
                if (usrA.Mail == null)
                {
                    ViewData["MailError"] = "MailError";
                }
                if (usrA.Phone == null)
                {
                    ViewData["PhoneError"] = "PhoneError";
                }
                if (usrA.Password == null)
                {
                    ViewData["PasswordError"] = "PasswordError";
                }
                if (usrA.ConfirmPassword == null)
                {
                    ViewData["PasswordConfirmationError"] = "PasswordConfirmationError";
                }
                if (PasswordMatchError == "PasswordMatchError")
                {
                    ViewData["PasswordMatchError"] = "PasswordMatchError";
                }
                return View(usrA);
            }

            if (ModelState.IsValid)
            {
                using (EventAppContext db = new EventAppContext())
                {
                    List<Event> AllEvents = db.Event.ToList();
                    UserEvent AttendedEvent = new UserEvent();
                    for (int i = 0; i < AllEvents.Count; i++)
                    {
                        if (AllEvents[i].EventConfirmation == usrA.AccountConfirmation)
                        {
                            AttendedEvent.EventID = AllEvents[i].Id;
                        }
                    }

                    if (AttendedEvent.EventID.ToString() != "00000000-0000-0000-0000-000000000000")
                    {
                        usrA.UserTypeID = AttenderType[0].Id;
                        usrA.ShowMyDetails = true;
                        db.User.Add(usrA);
                        db.SaveChanges();

                        using (EventAppContext dbUE = new EventAppContext())
                        {
                            AttendedEvent.UserID = usrA.Id;
                            AttendedEvent.IsActive = true;
                            dbUE.UserEvent.Add(AttendedEvent);
                            dbUE.SaveChanges();
                        }
                        usrA.PhotoDirectory = ("../ Images /Users/" + usrA.Id.ToString() + ".jpg");
                    }
                    else
                    {
                        usrA.UserTypeID = AttenderType[0].Id;
                        ViewData["ValidationError"] = "ValidationError";
                        return View();
                    }

                }

                if (imageinput == null)
                {
                    System.IO.File.Copy(Server.MapPath("/Images/Users/default_thumbnail.png"), Server.MapPath("/Images/Users/" + usrA.Id + ".jpg"));
                    string path = System.IO.Path.Combine(Server.MapPath("~/Images/Users/" + usrA.Id + ".jpg"));
                    Image defImage = Image.FromFile(Server.MapPath("/Images/Users/default_thumbnail.png"));
                    var newImage = ScaleImage(defImage, 100, 100);
                    newImage.Save(path);
                    return RedirectToAction("Login", "Account");
                }
                else
                {
                    System.IO.File.Copy(Server.MapPath("/Images/Users/default_thumbnail.png"), Server.MapPath("/Images/Users/" + usrA.Id + ".jpg"));
                    string path = System.IO.Path.Combine(Server.MapPath("~/Images/Users/" + usrA.Id + ".jpg"));
                    Image sourceimage = Image.FromStream(imageinput.InputStream);
                    var newImage = ScaleImage(sourceimage, 200, 200);
                    newImage.Save(path);
                    return RedirectToAction("Login", "Account");
                }

            }

            return View();
        }
    }
}