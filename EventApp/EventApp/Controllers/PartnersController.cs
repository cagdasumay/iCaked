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

namespace EventApp.Controllers
{
    public class PartnersController : Controller
    {
        private EventAppContext db = new EventAppContext();

        public ActionResult AddCompany(string CompanyError)
        {
            List<Partner> AllCompanies = db.Partner.ToList();
            List<Partner> SortedCompanies = new List<Partner>();
            SortedCompanies = AllCompanies.OrderBy(d => d.Name).ToList();
            ViewData["CompanyList"] = SortedCompanies;

            if (CompanyError == null)
            {
                List<string> Errors = new List<string>();
                string NameError = "Correct";
                string LogoError = "Correct";
                string WebsiteError = "Correct";
                string IntroductionError = "Correct";
                ViewData["SelectCategory"] = "Correct";
                Errors.Add(NameError);
                Errors.Add(LogoError);
                Errors.Add(WebsiteError);
                Errors.Add(IntroductionError);
                ViewData["CompanyErrors"] = Errors;

                return View();
            }
            else
            {
                ViewData["SelectCategory"] = TempData["SelectCategory"];
                ViewData["CompanyErrors"] = TempData["CompanyErrors"];
                Partner CompanyInfo = (Partner)TempData["Partner"];
                return View(CompanyInfo);
            }


        }

        public static Image ScaleImage(Image image, int newWidth, int newHeight)
        {
            var newImage = new Bitmap(newWidth, newHeight);

            using (var graphics = Graphics.FromImage(newImage))
                graphics.DrawImage(image, 0, 0, newWidth, newHeight);

            return newImage;
        }

        [HttpPost]
        public ActionResult CompanyDetails(Partner newpartner, HttpPostedFileBase imageinput, Guid? CompanySelect, string Continue)
        {
            if (CompanySelect.ToString() != "00000000-0000-0000-0000-000000000000")
            {
                Session["PartnerID"] = CompanySelect;
                return RedirectToAction("SponsorshipDetails", "Partners", new { PartnerID = CompanySelect });
            }
            else if (CompanySelect.ToString() == "00000000-0000-0000-0000-000000000000" && Continue == null)
            {
                List<string> Errors = new List<string>();
                string NameError = "Correct";
                string LogoError = "Correct";
                string WebsiteError = "Correct";
                string IntroductionError = "Correct";
                Errors.Add(NameError);
                Errors.Add(LogoError);
                Errors.Add(WebsiteError);
                Errors.Add(IntroductionError);
                TempData["CompanyErrors"] = Errors;
                TempData["Partner"] = newpartner;
                TempData["SelectCategory"] = "SelectCategory";
                return RedirectToAction("AddCompany", "Partners", new { CompanyError = "CompanyError" });
            }
            else if (ModelState.IsValid)
            {
                if (newpartner.Name == null || newpartner.Website == null || newpartner.Introduction == null || imageinput == null)
                {
                    List<string> Errors = new List<string>();
                    string NameError = "Correct";
                    string LogoError = "Correct";
                    string WebsiteError = "Correct";
                    string IntroductionError = "Correct";
                    if (newpartner.Name == null)
                    {
                        NameError = "NameError";
                    }
                    if (newpartner.Website == null)
                    {
                        WebsiteError = "WebsiteError";
                    }
                    if (newpartner.Introduction == null)
                    {
                        IntroductionError = "IntroductionError";
                    }
                    if (imageinput == null)
                    {
                        LogoError = "LogoError";
                    }
                    Errors.Add(NameError);
                    Errors.Add(LogoError);
                    Errors.Add(WebsiteError);
                    Errors.Add(IntroductionError);
                    TempData["CompanyErrors"] = Errors;
                    TempData["Partner"] = newpartner;
                    TempData["SelectCategory"] = "Correct";
                    return RedirectToAction("AddCompany", "Partners", new { CompanyError = "CompanyError" });

                }

                using (EventAppContext db = new EventAppContext())
                {
                    db.Partner.Add(newpartner);
                    db.SaveChanges();
                    Session["PartnerID"] = newpartner.Id;
                }


                System.IO.File.Copy(Server.MapPath("/PartnerLogo/companies.png"), Server.MapPath("/PartnerLogo/" + newpartner.Id + ".jpg"));
                string path = System.IO.Path.Combine(Server.MapPath("~/PartnerLogo/" + newpartner.Id + ".jpg"));
                Image sourceimage = Image.FromStream(imageinput.InputStream);
                var newImage = ScaleImage(sourceimage, 250, 250);
                newImage.Save(path);
                return RedirectToAction("SponsorshipDetails", "Partners", new { PartnerID = newpartner.Id });
            }
            else
            {
                return RedirectToAction("AddCompany", "Partners");
            }
        }

        public ActionResult SponsorshipDetails(Guid? PartnerID)
        {
            string CatErrors = (string)TempData["CatErrors"];
            if (CatErrors == null)
            {
                CatErrors = "Correct";
            }
            ViewData["CatErrors"] = CatErrors;

            string Error = (string)TempData["Error"];
            if (Error == "Error")
            {
                ViewData["Error"] = "Error";
            }
            else
            {
                ViewData["Error"] = "Correct";
            }

            List<PartnerEventCategory> AllCategories = db.PartnerEventCategory.ToList();
            List<PartnerEventCategory> SortedCategories = new List<PartnerEventCategory>();
            SortedCategories = AllCategories.OrderBy(d => d.Rating).ToList();
            ViewData["Categories"] = SortedCategories;

            // Partner bilgilerini tutmak lazım bundan aşağıda bunu yap
            Partner ThisPartner = db.Partner.Find(PartnerID);
            ViewData["Partner"] = ThisPartner;
            Session["SavePartner"] = ThisPartner.Id;
            return View();
        }

        public ActionResult SubmitSponsorship(Guid CategorySelect)
        {
            if (CategorySelect.ToString() != "00000000-0000-0000-0000-000000000000")
            {
                User LoggedUser = (User)Session["LoggedIn"];
                List<Event> AllEvents = db.Event.ToList();
                var thisEvent = AllEvents.Where(q => q.EventConfirmation == LoggedUser.AccountConfirmation).ToList();
                Guid PartnerID = (Guid)Session["SavePartner"];
                PartnerEvent PE = new PartnerEvent();
                PE.EventID = thisEvent[0].Id;
                PE.PartnerEventCategoryID = CategorySelect;
                PE.PartnerID = PartnerID;
                db.PartnerEvent.Add(PE);
                db.SaveChanges();

                return RedirectToAction("PartnerListEditable", "Partners");
            }
            else
            {
                Guid PartnerID = (Guid)Session["SavePartner"];
                TempData["Error"] = "Error";
                return RedirectToAction("SponsorshipDetails", "Partners", new { PartnerID = PartnerID});
            }
        }

        public ActionResult PartnerListEditable()
        {
            User LoggedUser = (User)Session["LoggedIn"];
            List<Event> AllEvents = db.Event.ToList();
            var thisEvent = AllEvents.Where(q => q.EventConfirmation == LoggedUser.AccountConfirmation).ToList();

            List<PartnerEventCategory> AllCategories = db.PartnerEventCategory.ToList();
            List<PartnerEventCategory> ThisCategories = new List<PartnerEventCategory>();
            for (int i = 0; i < AllCategories.Count; i++)
            {
                if (AllCategories[i].EventID == thisEvent[0].Id)
                {
                    ThisCategories.Add(AllCategories[i]);
                }
            }
            List<PartnerEventCategory> SortedEventCategories = new List<PartnerEventCategory>();
            SortedEventCategories = ThisCategories.OrderBy(d => d.Rating).ToList();
            // Yukarıda bu Event için oluşturulan Category'leri sırasıyla aldık

            List<PartnerEvent> AllPartnerEvents = db.PartnerEvent.ToList();
            List<Partner> ThisPartners = new List<Partner>();
            List<PartnerEvent> ThisPartnerEventList = new List<PartnerEvent>();
            List<PartnerEventCategory> SortedFullList = new List<PartnerEventCategory>();
            for (int j = 0; j < SortedEventCategories.Count; j++)
            {
                for (int k = 0; k < AllPartnerEvents.Count; k++)
                {
                    if (AllPartnerEvents[k].PartnerEventCategoryID == SortedEventCategories[j].Id)
                    {
                        ThisPartnerEventList.Add(AllPartnerEvents[k]);
                        Partner add = db.Partner.Find(AllPartnerEvents[k].PartnerID);
                        ThisPartners.Add(add);
                        SortedFullList.Add(SortedEventCategories[j]);
                    }
                }
            }
            // Yukarıda, sıralı olarak kayıtlı PartnerEvent'leri ve Partner bilgilerini aldık
            ViewData["PartnerList"] = ThisPartners;
            ViewData["PartnerCategoryList"] = SortedFullList;
            ViewData["PartnerEventList"] = ThisPartnerEventList;

            return View();
        }

        public ActionResult DeletePartner(Guid IDofPartner)
        {
            PartnerEvent PE = db.PartnerEvent.Find(IDofPartner);
            db.PartnerEvent.Remove(PE);
            db.SaveChanges();

            return RedirectToAction("PartnerListEditable", "Partners");
        }

        // Bunu kullanmıyoruz
        public ActionResult PartnerEdit(Guid IDofPartnerEvent, Guid IDofPartner)
        {
            PartnerEvent EditPartnerEvent = db.PartnerEvent.Find(IDofPartnerEvent);

            Partner thispartner = db.Partner.Find(IDofPartner);
            ViewData["Partner"] = thispartner;
            ViewData["ratingerror"] = "Correct";
            ViewData["categoryerror"] = "Correct";
            Session["EditPartnerEventID"] = IDofPartnerEvent;
            Session["EditPartnerID"] = IDofPartner;
            return View(EditPartnerEvent);
        }

        public ActionResult EditSponsorship(PartnerEvent partnerevent)
        {
            User LoggedUser = (User)Session["LoggedIn"];
            List<Event> AllEvents = db.Event.ToList();
            var thisEvent = AllEvents.Where(q => q.EventConfirmation == LoggedUser.AccountConfirmation).ToList();
            partnerevent.PartnerID = (Guid)Session["EditPartnerID"];
            partnerevent.EventID = thisEvent[0].Id;
            db.PartnerEvent.Add(partnerevent);
            db.SaveChanges();

            Guid deletefromdb = (Guid)Session["EditPartnerEventID"];
            PartnerEvent partner = db.PartnerEvent.Find(deletefromdb);
            db.PartnerEvent.Remove(partner);
            db.SaveChanges();

            return RedirectToAction("PartnerListEditable", "Partners");
        }

        public ActionResult List()
        {
            User LoggedUser = (User)Session["LoggedIn"];
            List<Event> AllEvents = db.Event.ToList();
            var thisEvent = AllEvents.Where(q => q.EventConfirmation == LoggedUser.AccountConfirmation).ToList();

            List<PartnerEventCategory> AllCategories = db.PartnerEventCategory.ToList();
            List<PartnerEventCategory> ThisCategories = new List<PartnerEventCategory>();
            for (int i = 0; i < AllCategories.Count; i++)
            {
                if (AllCategories[i].EventID == thisEvent[0].Id)
                {
                    ThisCategories.Add(AllCategories[i]);
                }
            }
            List<PartnerEventCategory> SortedEventCategories = new List<PartnerEventCategory>();
            SortedEventCategories = ThisCategories.OrderBy(d => d.Rating).ToList();
            // Yukarıda bu Event için oluşturulan Category'leri sırasıyla aldık

            List<PartnerEvent> AllPartnerEvents = db.PartnerEvent.ToList();
            List<Partner> ThisPartners = new List<Partner>();
            List<PartnerEvent> ThisPartnerEventList = new List<PartnerEvent>();
            List<PartnerEventCategory> SortedFullList = new List<PartnerEventCategory>();
            for (int j = 0; j < SortedEventCategories.Count; j++)
            {
                for (int k = 0; k < AllPartnerEvents.Count; k++)
                {
                    if (AllPartnerEvents[k].PartnerEventCategoryID == SortedEventCategories[j].Id)
                    {
                        ThisPartnerEventList.Add(AllPartnerEvents[k]);
                        Partner add = db.Partner.Find(AllPartnerEvents[k].PartnerID);
                        ThisPartners.Add(add);
                        SortedFullList.Add(SortedEventCategories[j]);
                    }
                }
            }
            // Yukarıda, sıralı olarak kayıtlı PartnerEvent'leri ve Partner bilgilerini aldık
            List<bool> isvalid = new List<bool>();
            for (int i = 0; i < SortedEventCategories.Count; i++)
            {
                var check = SortedFullList.Where(k => k.Id == SortedEventCategories[i].Id).ToList();
                if (check.Count > 0)
                {
                    isvalid.Add(true);
                }
                else
                {
                    isvalid.Add(false);
                }
            }

            ViewData["IsValid"] = isvalid;
            ViewData["CategoryTypes"] = SortedEventCategories;
            ViewData["PartnerList"] = ThisPartners;
            ViewData["PartnerCategoryList"] = SortedFullList;
            ViewData["PartnerEventList"] = ThisPartnerEventList;
            ViewData["EventName"] = thisEvent[0].Name;

            return View();
        }

        public ActionResult TypeCategories()
        {
            string fromAddCotegory = (string)TempData["fromAddCotegory"];
            // Şu ana kadar save edilenlerin listesini aldık
            User LoggedUser = (User)Session["LoggedIn"];
            List<Event> AllEvents = db.Event.ToList();
            var thisEvent = AllEvents.Where(q => q.EventConfirmation == LoggedUser.AccountConfirmation).ToList();

            List<PartnerEventCategory> AllCategories = db.PartnerEventCategory.ToList();
            List<PartnerEventCategory> ThisCategories = new List<PartnerEventCategory>();
            for (int i = 0; i < AllCategories.Count; i++)
            {
                PartnerEventCategory addthis = db.PartnerEventCategory.Find(AllCategories[i].Id);
                ThisCategories.Add(addthis);
            }

            List<PartnerEventCategory> SortedCategories = new List<PartnerEventCategory>();
            SortedCategories = ThisCategories.OrderBy(d => d.Rating).ToList();

            ViewData["SavedCategories"] = SortedCategories;

            // Yeni eklenenleri list içerisine alıyoruz
            if (fromAddCotegory == "Correct")
            {
                ViewData["newcategories"] = TempData["newcategories"];
                ViewData["Error"] = TempData["Error"];
            }
            else
            {
                return RedirectToAction("AddCategory", "Partners");
            }

            return View();
        }

        public dynamic AddCategory(PartnerEventCategory newcategory, string AddCategory, string Delete)
        {
            if (AddCategory == "Complete")
            {
                if (newcategory.Name != null && newcategory.Rating != 0)
                {
                    List<PartnerEventCategory> complete = (List<PartnerEventCategory>)Session["Categories"];
                    complete.Add(newcategory);
                    TempData["CompleteCategories"] = complete;
                    return RedirectToAction("CompleteCategories", "Partners");
                }
                else if (newcategory.Name == null && newcategory.Rating == 0)
                {
                    List<PartnerEventCategory> complete = (List<PartnerEventCategory>)Session["Categories"];
                    TempData["CompleteCategories"] = complete;
                    return RedirectToAction("CompleteCategories", "Partners");
                }
            }
            else if (Delete != null)
            {
                List<PartnerEventCategory> complete = (List<PartnerEventCategory>)Session["Categories"];
                for (int i = 0; i < complete.Count; i++)
                {
                    if (complete[i].Name == Delete)
                    {
                        PartnerEventCategory delthis = complete[i];
                        complete.Remove(delthis);
                        Session["Categories"] = complete;
                    }
                }
            }

            List<PartnerEventCategory> addedchoices = new List<PartnerEventCategory>();
            List<PartnerEventCategory> categories = (List<PartnerEventCategory>)Session["Categories"];

            if (newcategory.Name != null || newcategory.Rating != 0)
            {
                if (categories == null)
                {
                    if (newcategory.Name != null && newcategory.Rating != 0)
                    {
                        TempData["Error"] = "Correct";
                        addedchoices.Add(newcategory);
                    }
                    else if (newcategory.Name == null && newcategory.Rating != 0)
                    {
                        TempData["Error"] = "Error";
                    }
                    else if (newcategory.Name != null && newcategory.Rating == 0)
                    {
                        TempData["Error"] = "Error";
                    }
                }
                else
                {
                    addedchoices = categories;
                    if (newcategory.Name != null && newcategory.Rating != 0)
                    {
                        TempData["Error"] = "Correct";
                        addedchoices.Add(newcategory);
                    }
                    else if (newcategory.Name == null && newcategory.Rating != 0)
                    {
                        TempData["Error"] = "Error";
                    }
                    else if (newcategory.Name != null && newcategory.Rating == 0)
                    {
                        TempData["Error"] = "Error";
                    }
                }
            }
            else
            {
                addedchoices = categories;
            }

            List<PartnerEventCategory> SortedNewCategories = new List<PartnerEventCategory>();
            if (addedchoices != null)
            {
                SortedNewCategories = addedchoices.OrderBy(d => d.Rating).ToList();
            }

            Session["Categories"] = SortedNewCategories;
            TempData["newcategories"] = SortedNewCategories;
            TempData["fromAddCotegory"] = "Correct";

            return RedirectToAction("TypeCategories", "Partners");
        }

        public dynamic CompleteCategories()
        {
            List<PartnerEventCategory> AddThese = (List<PartnerEventCategory>)TempData["CompleteCategories"];

            User LoggedUser = (User)Session["LoggedIn"];
            List<Event> AllEvents = db.Event.ToList();
            var thisEvent = AllEvents.Where(q => q.EventConfirmation == LoggedUser.AccountConfirmation).ToList();

            for (int i = 0; i < AddThese.Count; i++)
            {
                PartnerEventCategory add = AddThese[i];
                add.EventID = thisEvent[0].Id;
                db.PartnerEventCategory.Add(add);
                db.SaveChanges();

            }

            Session["Categories"] = null;

            return RedirectToAction("AddCompany", "Partners");
        }

        public ActionResult PartnerDetails(Guid IDofPartner, Guid IDofPartnerEventCategory)
        {
            User user = (User)Session["LoggedIn"];

            UserType ThisUser = db.UserType.Find(user.UserTypeID);
            ViewData["UserType"] = ThisUser.Name;

            PartnerEventCategory thiscategory = db.PartnerEventCategory.Find(IDofPartnerEventCategory);
            Partner thispartner = db.Partner.Find(IDofPartner);
            ViewData["PartnerDetails"] = thispartner;
            ViewData["CategoryDetails"] = thiscategory;
            return View();
        }

        public dynamic NewCategory(PartnerEventCategory newcategory, Guid? Company)
        {
            string CatErrors = "Correct";
            if (newcategory.Name == null || newcategory.Rating == 0)
            {
                CatErrors = "CatErrors";
            }
            TempData["CatErrors"] = CatErrors;

            User LoggedUser = (User)Session["LoggedIn"];
            List<Event> AllEvents = db.Event.ToList();
            var thisEvent = AllEvents.Where(q => q.EventConfirmation == LoggedUser.AccountConfirmation).ToList();
            newcategory.EventID = thisEvent[0].Id;
            db.PartnerEventCategory.Add(newcategory);
            db.SaveChanges();

            return RedirectToAction("SponsorshipDetails", "Partners",  new { PartnerID = Company});
        }

































































        // GET: Partners
        public ActionResult Index()
        {
            return View(db.Partner.ToList());
        }

        // GET: Partners/Details/5
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Partner partner = db.Partner.Find(id);
            if (partner == null)
            {
                return HttpNotFound();
            }
            return View(partner);
        }

        // GET: Partners/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Partners/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Introduction,CompanyLogo,Facebook,Twitter,Instagram,Linkedin,Website")] Partner partner)
        {
            if (ModelState.IsValid)
            {
                partner.Id = Guid.NewGuid();
                db.Partner.Add(partner);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(partner);
        }

        // GET: Partners/Edit/5
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Partner partner = db.Partner.Find(id);
            if (partner == null)
            {
                return HttpNotFound();
            }
            return View(partner);
        }

        // POST: Partners/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Introduction,CompanyLogo,Facebook,Twitter,Instagram,Linkedin,Website")] Partner partner)
        {
            if (ModelState.IsValid)
            {
                db.Entry(partner).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(partner);
        }

        // GET: Partners/Delete/5
        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Partner partner = db.Partner.Find(id);
            if (partner == null)
            {
                return HttpNotFound();
            }
            return View(partner);
        }

        // POST: Partners/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            Partner partner = db.Partner.Find(id);
            db.Partner.Remove(partner);
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
