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
    public class InfoBoothsController : Controller
    {
        private EventAppContext db = new EventAppContext();

        public ActionResult InfoBoothOrg()
        {
            User LoggedUser = (User)Session["LoggedIn"];

            var AllEvents = db.Event.ToList();
            var IDofEvent = AllEvents.Where(p => p.EventConfirmation == LoggedUser.AccountConfirmation).ToList();

            // Bu Event'in InfoBooth'larını aldık
            var allInfoBoothEvents = db.InfoBoothEvent.ToList();
            var ThisEventInfoBooth = allInfoBoothEvents.Where(k => k.EventID == IDofEvent[0].Id).ToList();

            // InfoBooth'ları çekiyor
            var allInfoBooths = db.InfoBooth.ToList();
            List<InfoBooth> ThisInfoBooths = new List<InfoBooth>();
            for (int i = 0; i < ThisEventInfoBooth.Count; i++)
            {
                InfoBooth details = db.InfoBooth.Find(ThisEventInfoBooth[i].InfoBoothID);
                ThisInfoBooths.Add(details);
            }

            List<InfoBooth> SortedInfoBoothList = new List<InfoBooth>();
            SortedInfoBoothList = ThisInfoBooths.OrderByDescending(d => d.Name).ToList();
            ViewData["InfoBoothList"] = SortedInfoBoothList;

            return View();
        }

        public ActionResult NewInfoBooth()
        {
            ViewData["NameError"] = "Correct";
            ViewData["ContentError"] = "Correct";
            ViewData["UsageError"] = "Correct";
            return View();
        }

        [HttpPost]
        public ActionResult NewInfoBooth(InfoBooth info, HttpPostedFileBase imageinput, string Usage)
        {
            string NameError = "Correct";
            string ContentError = "Correct";
            string UsageError = "Correct";
            if (info.Name == null || info.Content == null || Usage == "Boş")
            {
                if (info.Name == null)
                {
                    NameError = "NameError";
                }
                if (info.Content == null)
                {
                    ContentError = "ContentError";
                }
                if (Usage == "Boş")
                {
                    UsageError = "UsageError";
                }
                ViewData["NameError"] = NameError;
                ViewData["ContentError"] = ContentError;
                ViewData["UsageError"] = UsageError;
                return View(info);
            }

            if (ModelState.IsValid)
            {
                using (EventAppContext db = new EventAppContext())
                {
                    InfoBooth AddThis = new InfoBooth();
                    AddThis.Name = info.Name;
                    AddThis.Content = info.Content;
                    AddThis.Usage = Usage;

                    if (imageinput == null)
                    {
                        AddThis.PhotoDirectory = "Foto Yok";
                    }
                    else
                    {
                        AddThis.PhotoDirectory = "Foto Var";
                    }

                    db.InfoBooth.Add(AddThis);
                    db.SaveChanges();

                    User LoggedUser = (User)Session["LoggedIn"];
                    var AllEvents = db.Event.ToList();
                    var IDofEvent = AllEvents.Where(p => p.EventConfirmation == LoggedUser.AccountConfirmation).ToList();

                    InfoBoothEvent withEvent = new InfoBoothEvent();
                    withEvent.EventID = IDofEvent[0].Id;
                    withEvent.InfoBoothID = AddThis.Id;
                    db.InfoBoothEvent.Add(withEvent);
                    db.SaveChanges();

                    if (imageinput != null)
                    {
                        System.IO.File.Copy(Server.MapPath("/Images/InfoBooth/infobooth.png"), Server.MapPath("/Images/InfoBooth/" + AddThis.Id + ".jpg"));
                        string path = System.IO.Path.Combine(Server.MapPath("~/Images/InfoBooth/" + AddThis.Id + ".jpg"));
                        Image sourceimage = Image.FromStream(imageinput.InputStream);
                        var newImage = ScaleImage(sourceimage, 250, 250);
                        newImage.Save(path);
                    }
                }      
                return RedirectToAction("InfoBoothOrg", "InfoBooths");
            }
            return View();
        }

        public static Image ScaleImage(Image image, int newWidth, int newHeight)
        {
            var newImage = new Bitmap(newWidth, newHeight);

            using (var graphics = Graphics.FromImage(newImage))
                graphics.DrawImage(image, 0, 0, newWidth, newHeight);

            return newImage;
        }

        public ActionResult EditInfoBooth (Guid EditID, string From, Guid? FromDetails)
        {
            InfoBooth EditThis = db.InfoBooth.Find(EditID);

            if (EditThis.PhotoDirectory == "Foto Var")
            {
                ViewData["Photo"] = EditThis.Id;
            }
            else
            {
                ViewData["Photo"] = null;
            }

            ViewData["ThisInfoBooth"] = EditThis.Id;
            ViewData["NameInfoBooth"] = EditThis.Name;
            ViewData["FromPage"] = From;

            if (FromDetails != null)
            {
                ViewData["FromDetails"] = FromDetails;
            }
            else
            {
                ViewData["FromDetails"] = null;
            }

            ViewData["Usage"] = EditThis.Usage;
            return View(EditThis);
        }

        public dynamic RoutingFrom(string From, Guid? FromDetails)
        {
            if (From == "InfoBoothDetailsOrg")
            {
                return RedirectToAction("InfoBoothDetailsOrg", "InfoBooths", new { ThisIB = FromDetails });
            }
            else if (From == "InfoBoothOrg")
            {
                return RedirectToAction("InfoBoothOrg", "InfoBooths");
            }

            return 111;
        }

        [HttpPost]

        public ActionResult ChangeInfoBoot(InfoBooth info, Guid? ThisInfoBooth, HttpPostedFileBase imageinput, string Usage)
        {
            string from = (string)TempData["From"];
            InfoBooth ChangeThis = db.InfoBooth.Find(ThisInfoBooth);

            ChangeThis.Usage = Usage;
            if (info.Name != null)
            {
                ChangeThis.Name = info.Name;
            }

            if (info.Content != null)
            {
                ChangeThis.Content = info.Content;
            }

            if (imageinput != null)
            {
                ChangeThis.PhotoDirectory = "Foto Var";
            }

            db.SaveChanges();

            if (imageinput != null)
            {
                string path = System.IO.Path.Combine(Server.MapPath("~/Images/InfoBooth/" + ThisInfoBooth + ".jpg"));
                Image sourceimage = Image.FromStream(imageinput.InputStream);
                var newImage = ScaleImage(sourceimage, 200, 200);
                newImage.Save(path);
            }

            return RedirectToAction("RoutingFrom", "InfoBooths", new { From = from, FromDetails = ChangeThis.Id });
        }

        public ActionResult DeleteInfoBooth (Guid DeleteIB)
        {
            InfoBooth DeleteThis = db.InfoBooth.Find(DeleteIB);
            db.InfoBooth.Remove(DeleteThis);
            db.SaveChanges();

            var fromEvent = db.InfoBoothEvent.ToList();
            var thiseventinfobooth = fromEvent.Where(k => k.InfoBoothID == DeleteIB).ToList();
            InfoBoothEvent deleteIBE = db.InfoBoothEvent.Find(thiseventinfobooth[0].Id);
            db.InfoBoothEvent.Remove(deleteIBE);
            db.SaveChanges();

            return RedirectToAction("InfoBoothOrg", "InfoBooths");
        }

        public ActionResult InfoBoothDetailsOrg(Guid ThisIB)
        {
            InfoBooth ThisInfoBooth = db.InfoBooth.Find(ThisIB);
            ViewData["InfoBoothDetails"] = ThisInfoBooth;
            ViewData["InfoBoothID"] = ThisInfoBooth.Id;

            return View();
        }

        public ActionResult List()
        {
            User LoggedUser = (User)Session["LoggedIn"];

            var AllEvents = db.Event.ToList();
            var IDofEvent = AllEvents.Where(p => p.EventConfirmation == LoggedUser.AccountConfirmation).ToList();

            // Bu Event'in InfoBooth'larını aldık
            var allInfoBoothEvents = db.InfoBoothEvent.ToList();
            var ThisEventInfoBooth = allInfoBoothEvents.Where(k => k.EventID == IDofEvent[0].Id).ToList();

            // InfoBooth'ları çekiyor
            var allInfoBooths = db.InfoBooth.ToList();
            List<InfoBooth> ThisInfoBooths = new List<InfoBooth>();
            for (int i = 0; i < ThisEventInfoBooth.Count; i++)
            {
                InfoBooth details = db.InfoBooth.Find(ThisEventInfoBooth[i].InfoBoothID);
                ThisInfoBooths.Add(details);
            }

            List<InfoBooth> SortedInfoBoothList = new List<InfoBooth>();
            SortedInfoBoothList = ThisInfoBooths.OrderByDescending(d => d.Name).ToList();

            ViewData["InfoBoothList"] = SortedInfoBoothList;
            return View();
        }

        public ActionResult InfoBoothDetails(Guid ThisIB)
        {
            InfoBooth ThisInfoBooth = db.InfoBooth.Find(ThisIB);
            ViewData["InfoBoothDetails"] = ThisInfoBooth;
            ViewData["InfoBoothID"] = ThisInfoBooth.Id;

            return View();
        }



























































        // GET: InfoBooths
        public ActionResult Index()
        {
            return View(db.InfoBooth.ToList());
        }

        // GET: InfoBooths/Details/5
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InfoBooth infoBooth = db.InfoBooth.Find(id);
            if (infoBooth == null)
            {
                return HttpNotFound();
            }
            return View(infoBooth);
        }

        // GET: InfoBooths/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: InfoBooths/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Content,PhotoDirectory")] InfoBooth infoBooth)
        {
            if (ModelState.IsValid)
            {
                infoBooth.Id = Guid.NewGuid();
                db.InfoBooth.Add(infoBooth);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(infoBooth);
        }

        // GET: InfoBooths/Edit/5
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InfoBooth infoBooth = db.InfoBooth.Find(id);
            if (infoBooth == null)
            {
                return HttpNotFound();
            }
            return View(infoBooth);
        }

        // POST: InfoBooths/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Content,PhotoDirectory")] InfoBooth infoBooth)
        {
            if (ModelState.IsValid)
            {
                db.Entry(infoBooth).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(infoBooth);
        }

        // GET: InfoBooths/Delete/5
        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InfoBooth infoBooth = db.InfoBooth.Find(id);
            if (infoBooth == null)
            {
                return HttpNotFound();
            }
            return View(infoBooth);
        }

        // POST: InfoBooths/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            InfoBooth infoBooth = db.InfoBooth.Find(id);
            db.InfoBooth.Remove(infoBooth);
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
