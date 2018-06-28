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

namespace EventApp.Controllers
{
    public class DocumentsController : Controller
    {
        private EventAppContext db = new EventAppContext();

        public dynamic toView()
        {
            var TypeOfActivity = db.ActivityCategory.ToList();
            var ConferenceType = TypeOfActivity.Where(k => k.Name == "Konferans").ToList();
            var PresentationType = TypeOfActivity.Where(k => k.Name == "Sunum").ToList();
            var PanelType = TypeOfActivity.Where(k => k.Name == "Panel").ToList();

            List<DocumentFormat> Formats = db.DocumentFormat.ToList();

            List<DocumentFormat> SortedList = new List<DocumentFormat>();
            SortedList = Formats.OrderBy(d => (d.Name)).ToList();

            TempData["DocumentFormat"] = SortedList;

            User LoggedUser = (User)Session["LoggedIn"];
            List<Event> AllEvents = db.Event.ToList();
            var thisEvent = AllEvents.Where(q => q.EventConfirmation == LoggedUser.AccountConfirmation).ToList();

            var AllEventActivities = db.EventActivity.ToList();
            List<EventActivity> ThisEventActivities = new List<EventActivity>();
            for (int i = 0; i < AllEventActivities.Count; i++)
            {
                if (AllEventActivities[i].EventID == thisEvent[0].Id)
                {
                    EventActivity addthis = db.EventActivity.Find(AllEventActivities[i].Id);
                    ThisEventActivities.Add(addthis);
                }
            }

            var AllActivities = db.Activity.ToList();
            List<Activity> ThisEventsActivities = new List<Activity>();
            for (int k = 0; k < ThisEventActivities.Count; k++)
            {
                Activity add = db.Activity.Find(ThisEventActivities[k].ActivityID);
                if ((add.ActivityCategoryID == PanelType[0].Id || add.ActivityCategoryID == PresentationType[0].Id || add.ActivityCategoryID == ConferenceType[0].Id) && add.IsActive == true)
                {
                    ThisEventsActivities.Add(add);
                }
            }

            List<Activity> SortedActivityList = new List<Activity>();
            SortedActivityList = ThisEventsActivities.OrderBy(d => (d.Name)).ToList();

            TempData["ThisEventActivities"] = SortedActivityList;

            var AllDocuments = db.DocumentEventActivity.ToList();
            List<DocumentEventActivity> EventsDocuments = new List<DocumentEventActivity>();
            for (int i = 0; i < AllDocuments.Count; i++)
            {
                if (AllDocuments[i].EventOrEventActivityID == thisEvent[0].Id)
                {
                    DocumentEventActivity addthis = db.DocumentEventActivity.Find(AllDocuments[i].Id);
                    EventsDocuments.Add(addthis);
                }
            }

            List<Document> docinfo = new List<Document>();
            for (int j = 0; j < EventsDocuments.Count; j++)
            {
                Document add = db.Document.Find(EventsDocuments[j].DocumentID);
                docinfo.Add(add);
            }

            List<Document> Sorteddocinfo = new List<Document>();
            Sorteddocinfo = docinfo.OrderBy(d => (d.Name)).ToList();

            TempData["Documents"] = Sorteddocinfo;

            List<string> types = new List<string>();
            for (int t = 0; t < Sorteddocinfo.Count; t++)
            {
                Document thisdoc = db.Document.Find(Sorteddocinfo[t].Id);
                DocumentFormat thisformat = db.DocumentFormat.Find(thisdoc.DocumentFormatID);
                types.Add(thisformat.Name);
            }

            TempData["Types"] = types;

            return "";
        }

        public ActionResult AddDocument()
        {
            if (Session["LoggedIn"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            toView();
            ViewData["DocumentFormat"] = TempData["DocumentFormat"];
            ViewData["ThisEventActivities"] = TempData["ThisEventActivities"];
            ViewData["Documents"] = TempData["Documents"];
            ViewData["Types"] = TempData["Types"];

            List<string> Errors = new List<string>();
            string correct = "Correct";
            Errors.Add(correct);
            Errors.Add(correct);
            Errors.Add(correct);
            Errors.Add(correct);
            ViewData["Errors"] = Errors;

            return View();
        }

        [HttpPost]
        public ActionResult AddDocument(Document doc, HttpPostedFileBase uploadeddoc, Guid TypeOfDoc, Guid? ActID)
        {
            var FormatList = db.DocumentFormat.ToList();
            var CheckDocType = FormatList.Where(k => k.Id == TypeOfDoc).ToList();

            List<string> Errors = Validation(doc, uploadeddoc, TypeOfDoc);
            if (Errors[0] != "Correct" || Errors[1] != "Correct" || Errors[2] != "Correct" || Errors[3] != "Correct")
            {
                ViewData["Errors"] = Errors;
                toView();
                ViewData["DocumentFormat"] = TempData["DocumentFormat"];
                ViewData["ThisEventActivities"] = TempData["ThisEventActivities"];
                ViewData["Documents"] = TempData["Documents"];
                ViewData["Types"] = TempData["Types"];
                return View();
            }

            if (CheckDocType[0].Name == "Power Point Sunumu (.pptx)")
            {
                using (EventAppContext db = new EventAppContext())
                {
                    doc.DocumentFormatID = TypeOfDoc;
                    db.Document.Add(doc);
                    db.SaveChanges();
                    doc.DownloadAddress = ("../Documents/" + doc.Id.ToString() + ".pptx");

                    // EventDocument diye kaydediyoruz
                    DocumentEventActivity docEvent = new DocumentEventActivity();
                    using (EventAppContext dbME = new EventAppContext())
                    {
                        docEvent.DocumentID = doc.Id;
                        User USR = (User)Session["LoggedIn"];
                        string eventconfirmation = USR.AccountConfirmation;

                        List<Event> EventList = db.Event.ToList();
                        int count = EventList.Count;
                        for (int i = 0; i < count; i++)
                        {
                            if (eventconfirmation == EventList[i].EventConfirmation)
                            {
                                docEvent.EventOrEventActivityID = EventList[i].Id;
                            }
                        }
                        db.DocumentEventActivity.Add(docEvent);
                        db.SaveChanges();
                    }

                    // EventActivityDocument diye kaydediyoruz
                    if (ActID.ToString() != "00000000-0000-0000-0000-000000000000")
                    {
                        DocumentEventActivity docEventActivity = new DocumentEventActivity();
                        using (EventAppContext dbME = new EventAppContext())
                        {
                            docEventActivity.DocumentID = doc.Id;
                            var AllEventActivities = db.EventActivity.ToList();
                            var ThisEventActivity = AllEventActivities.Where(k => k.ActivityID == ActID).ToList();
                            docEventActivity.EventOrEventActivityID = ThisEventActivity[0].Id;

                            db.DocumentEventActivity.Add(docEventActivity);
                            db.SaveChanges();
                        }
                    }

                }

                string path = System.IO.Path.Combine(Server.MapPath("~/Documents/" + doc.Id + ".pptx"));
                uploadeddoc.SaveAs(path);

                toView();
                ViewData["DocumentFormat"] = TempData["DocumentFormat"];
                ViewData["ThisEventActivities"] = TempData["ThisEventActivities"];
                ViewData["Documents"] = TempData["Documents"];
                ViewData["Types"] = TempData["Types"];

                ViewData["Errors"] = Errors;

                return RedirectToAction("AddDocument", "Documents");
            }
            else if (CheckDocType[0].Name == "Excel Dosyası (.xlsx)")
            {
                using (EventAppContext db = new EventAppContext())
                {
                    doc.DocumentFormatID = TypeOfDoc;
                    db.Document.Add(doc);
                    db.SaveChanges();
                    doc.DownloadAddress = ("../Documents/" + doc.Id.ToString() + ".xlsx");

                    // EventDocument diye kaydediyoruz
                    DocumentEventActivity docEvent = new DocumentEventActivity();
                    using (EventAppContext dbME = new EventAppContext())
                    {
                        docEvent.DocumentID = doc.Id;
                        User USR = (User)Session["LoggedIn"];
                        string eventconfirmation = USR.AccountConfirmation;

                        List<Event> EventList = db.Event.ToList();
                        int count = EventList.Count;
                        for (int i = 0; i < count; i++)
                        {
                            if (eventconfirmation == EventList[i].EventConfirmation)
                            {
                                docEvent.EventOrEventActivityID = EventList[i].Id;
                            }
                        }
                        db.DocumentEventActivity.Add(docEvent);
                        db.SaveChanges();
                    }

                    // EventActivityDocument diye kaydediyoruz
                    if (ActID.ToString() != null)
                    {
                        DocumentEventActivity docEventActivity = new DocumentEventActivity();
                        using (EventAppContext dbME = new EventAppContext())
                        {
                            docEventActivity.DocumentID = doc.Id;
                            var AllEventActivities = db.EventActivity.ToList();
                            var ThisEventActivity = AllEventActivities.Where(k => k.ActivityID == ActID).ToList();
                            docEventActivity.EventOrEventActivityID = ThisEventActivity[0].Id;

                            db.DocumentEventActivity.Add(docEventActivity);
                            db.SaveChanges();
                        }
                    }

                }

                string path = System.IO.Path.Combine(Server.MapPath("~/Documents/" + doc.Id + ".xlsx"));
                uploadeddoc.SaveAs(path);

                toView();
                ViewData["DocumentFormat"] = TempData["DocumentFormat"];
                ViewData["ThisEventActivities"] = TempData["ThisEventActivities"];

                TempData["Errors"] = Errors;

                return RedirectToAction("AddDocument", "Documents");
            }
            else if (CheckDocType[0].Name == "Excel Dosyası (.xlsm)")
            {
                using (EventAppContext db = new EventAppContext())
                {
                    doc.DocumentFormatID = TypeOfDoc;
                    db.Document.Add(doc);
                    db.SaveChanges();
                    doc.DownloadAddress = ("../Documents/" + doc.Id.ToString() + ".xlsm");

                    // EventDocument diye kaydediyoruz
                    DocumentEventActivity docEvent = new DocumentEventActivity();
                    using (EventAppContext dbME = new EventAppContext())
                    {
                        docEvent.DocumentID = doc.Id;
                        User USR = (User)Session["LoggedIn"];
                        string eventconfirmation = USR.AccountConfirmation;

                        List<Event> EventList = db.Event.ToList();
                        int count = EventList.Count;
                        for (int i = 0; i < count; i++)
                        {
                            if (eventconfirmation == EventList[i].EventConfirmation)
                            {
                                docEvent.EventOrEventActivityID = EventList[i].Id;
                            }
                        }
                        db.DocumentEventActivity.Add(docEvent);
                        db.SaveChanges();
                    }

                    // EventActivityDocument diye kaydediyoruz
                    if (ActID.ToString() != null)
                    {
                        DocumentEventActivity docEventActivity = new DocumentEventActivity();
                        using (EventAppContext dbME = new EventAppContext())
                        {
                            docEventActivity.DocumentID = doc.Id;
                            var AllEventActivities = db.EventActivity.ToList();
                            var ThisEventActivity = AllEventActivities.Where(k => k.ActivityID == ActID).ToList();
                            docEventActivity.EventOrEventActivityID = ThisEventActivity[0].Id;

                            db.DocumentEventActivity.Add(docEventActivity);
                            db.SaveChanges();
                        }
                    }

                }

                string path = System.IO.Path.Combine(Server.MapPath("~/Documents/" + doc.Id + ".xlsm"));
                uploadeddoc.SaveAs(path);

                toView();
                ViewData["DocumentFormat"] = TempData["DocumentFormat"];
                ViewData["ThisEventActivities"] = TempData["ThisEventActivities"];

                TempData["Errors"] = Errors;

                return RedirectToAction("AddDocument", "Documents");
            }
            else if (CheckDocType[0].Name == "Word Dosyası (.docx)")
            {
                using (EventAppContext db = new EventAppContext())
                {
                    doc.DocumentFormatID = TypeOfDoc;
                    db.Document.Add(doc);
                    db.SaveChanges();
                    doc.DownloadAddress = ("../Documents/" + doc.Id.ToString() + ".docx");

                    // EventDocument diye kaydediyoruz
                    DocumentEventActivity docEvent = new DocumentEventActivity();
                    using (EventAppContext dbME = new EventAppContext())
                    {
                        docEvent.DocumentID = doc.Id;
                        User USR = (User)Session["LoggedIn"];
                        string eventconfirmation = USR.AccountConfirmation;

                        List<Event> EventList = db.Event.ToList();
                        int count = EventList.Count;
                        for (int i = 0; i < count; i++)
                        {
                            if (eventconfirmation == EventList[i].EventConfirmation)
                            {
                                docEvent.EventOrEventActivityID = EventList[i].Id;
                            }
                        }
                        db.DocumentEventActivity.Add(docEvent);
                        db.SaveChanges();
                    }

                    // EventActivityDocument diye kaydediyoruz
                    if (ActID.ToString() != null)
                    {
                        DocumentEventActivity docEventActivity = new DocumentEventActivity();
                        using (EventAppContext dbME = new EventAppContext())
                        {
                            docEventActivity.DocumentID = doc.Id;
                            var AllEventActivities = db.EventActivity.ToList();
                            var ThisEventActivity = AllEventActivities.Where(k => k.ActivityID == ActID).ToList();
                            docEventActivity.EventOrEventActivityID = ThisEventActivity[0].Id;

                            db.DocumentEventActivity.Add(docEventActivity);
                            db.SaveChanges();
                        }
                    }

                }

                string path = System.IO.Path.Combine(Server.MapPath("~/Documents/" + doc.Id + ".docx"));
                uploadeddoc.SaveAs(path);

                toView();
                ViewData["DocumentFormat"] = TempData["DocumentFormat"];
                ViewData["ThisEventActivities"] = TempData["ThisEventActivities"];

                TempData["Errors"] = Errors;

                return RedirectToAction("AddDocument", "Documents");
            }
            else if (CheckDocType[0].Name == "PDF")
            {
                using (EventAppContext db = new EventAppContext())
                {
                    doc.DocumentFormatID = TypeOfDoc;
                    db.Document.Add(doc);
                    db.SaveChanges();
                    doc.DownloadAddress = ("../Documents/" + doc.Id.ToString() + ".pdf");

                    // EventDocument diye kaydediyoruz
                    DocumentEventActivity docEvent = new DocumentEventActivity();
                    using (EventAppContext dbME = new EventAppContext())
                    {
                        docEvent.DocumentID = doc.Id;
                        User USR = (User)Session["LoggedIn"];
                        string eventconfirmation = USR.AccountConfirmation;

                        List<Event> EventList = db.Event.ToList();
                        int count = EventList.Count;
                        for (int i = 0; i < count; i++)
                        {
                            if (eventconfirmation == EventList[i].EventConfirmation)
                            {
                                docEvent.EventOrEventActivityID = EventList[i].Id;
                            }
                        }
                        db.DocumentEventActivity.Add(docEvent);
                        db.SaveChanges();
                    }

                    // EventActivityDocument diye kaydediyoruz
                    if (ActID.ToString() != "00000000-0000-0000-0000-000000000000")
                    {
                        DocumentEventActivity docEventActivity = new DocumentEventActivity();
                        using (EventAppContext dbME = new EventAppContext())
                        {
                            docEventActivity.DocumentID = doc.Id;
                            var AllEventActivities = db.EventActivity.ToList();
                            var ThisEventActivity = AllEventActivities.Where(k => k.ActivityID == ActID).ToList();
                            docEventActivity.EventOrEventActivityID = ThisEventActivity[0].Id;

                            db.DocumentEventActivity.Add(docEventActivity);
                            db.SaveChanges();
                        }
                    }

                }

                string path = System.IO.Path.Combine(Server.MapPath("~/Documents/" + doc.Id + ".pdf"));
                uploadeddoc.SaveAs(path);

                toView();
                ViewData["DocumentFormat"] = TempData["DocumentFormat"];
                ViewData["ThisEventActivities"] = TempData["ThisEventActivities"];

                TempData["Errors"] = Errors;

                return RedirectToAction("AddDocument", "Documents");
            }

            return View();
        }

        public ActionResult DisplayDocument(Guid? docID)
        {
            User user = (User)Session["LoggedIn"];

            UserType ThisUser = db.UserType.Find(user.UserTypeID);
            ViewData["UserType"] = ThisUser.Name;

            Document thisdoc = db.Document.Find(docID);
            DocumentFormat thisformat = db.DocumentFormat.Find(thisdoc.DocumentFormatID);
            ViewData["Document"] = thisdoc;
            return View();
        }

        public ActionResult DeleteApprove(Guid docID)
        {
            Guid DeleteID = docID;
            Document doc = db.Document.Find(docID);
            db.Document.Remove(doc);
            db.SaveChanges();

            var AllDocumentEvents = db.DocumentEventActivity.ToList();
            var DeleteDoc = AllDocumentEvents.Where(k => k.DocumentID == DeleteID).ToList();

            for (int i = 0; i < DeleteDoc.Count; i++)
            {
                DocumentEventActivity DocEvent = db.DocumentEventActivity.Find(DeleteDoc[i].Id);
                db.DocumentEventActivity.Remove(DocEvent);
                db.SaveChanges();
            }

            return RedirectToAction("AddDocument", "Documents");
        }

        public ActionResult List()
        {
            toView();
            ViewData["Documents"] = TempData["Documents"];
            ViewData["Types"] = TempData["Types"];

            return View();
        }

        public dynamic Validation(Document doc, HttpPostedFileBase uploadeddoc, Guid TypeOfDoc)
        {
            string NameError = "Correct";
            string FileError = "Correct";
            string TypeError = "Correct";
            string MatchError = "Correct";
            List<string> Errors = new List<string>();

            if (doc.Name == null || uploadeddoc == null || TypeOfDoc.ToString() == "00000000-0000-0000-0000-000000000000")
            {
                if (doc.Name == null)
                {
                    NameError = "NameError";
                }
                if (uploadeddoc == null)
                {
                    FileError = "FileError";
                }
                if (TypeOfDoc.ToString() == "00000000-0000-0000-0000-000000000000")
                {
                    TypeError = "TypeError";
                }
                Errors.Add(NameError);
                Errors.Add(FileError);
                Errors.Add(TypeError);
                Errors.Add(MatchError);

                return Errors;
            }

            string uploadtype = uploadeddoc.ContentType;
            var FormatList = db.DocumentFormat.ToList();
            var CheckDocType = FormatList.Where(k => k.Id == TypeOfDoc).ToList();

            if (CheckDocType[0].Name == "PDF")
            {
                if (uploadtype != "application/pdf")
                {
                    MatchError = "MatchError";
                }
            }
            else if (CheckDocType[0].Name == "Power Point Sunumu (.pptx)")
            {
                if (uploadtype != "application/vnd.openxmlformats-officedocument.presentationml.presentation")
                {
                    MatchError = "MatchError";
                }
            }
            else if (CheckDocType[0].Name == "Excel Dosyası (.xlsx)")
            {
                if (uploadtype != "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
                {
                    MatchError = "MatchError";
                }
            }
            else if (CheckDocType[0].Name == "Excel Dosyası (.xlsm)")
            {
                if (uploadtype != "application/vnd.ms-excel.sheet.macroEnabled.12")
                {
                    MatchError = "MatchError";
                }
            }
            else if (CheckDocType[0].Name == "Word Dosyası (.docx)")
            {
                if (uploadtype != "application/vnd.openxmlformats-officedocument.wordprocessingml.document")
                {
                    MatchError = "MatchError";
                }
            }

            Errors.Add(NameError);
            Errors.Add(FileError);
            Errors.Add(TypeError);
            Errors.Add(MatchError);

            return Errors;
        }

























































        //public ActionResult AddDocumentSpeaker()
        //{
        //    if (Session["LoggedIn"] == null)
        //    {
        //        return RedirectToAction("Login", "Account");
        //    }

        //    toViewSpeaker();
        //    ViewData["DocumentFormat"] = TempData["DocumentFormat"];
        //    ViewData["ThisEventActivities"] = TempData["ThisEventActivities"];
        //    ViewData["Documents"] = TempData["Documents"];
        //    ViewData["Types"] = TempData["Types"];

        //    List<string> Errors = new List<string>();
        //    string correct = "Correct";
        //    Errors.Add(correct);
        //    Errors.Add(correct);
        //    Errors.Add(correct);
        //    Errors.Add(correct);
        //    ViewData["Errors"] = Errors;

        //    return View();
        //}

        //[HttpPost]
        //public ActionResult AddDocumentSpeaker(Document doc, HttpPostedFileBase uploadeddoc, Guid TypeOfDoc, Guid? ActID)
        //{
        //    var FormatList = db.DocumentFormat.ToList();
        //    var CheckDocType = FormatList.Where(k => k.Id == TypeOfDoc).ToList();

        //    List<string> Errors = Validation(doc, uploadeddoc, TypeOfDoc);
        //    if (Errors[0] != "Correct" || Errors[1] != "Correct" || Errors[2] != "Correct" || Errors[3] != "Correct")
        //    {
        //        ViewData["Errors"] = Errors;
        //        toViewSpeaker();
        //        ViewData["DocumentFormat"] = TempData["DocumentFormat"];
        //        ViewData["ThisEventActivities"] = TempData["ThisEventActivities"];
        //        ViewData["Documents"] = TempData["Documents"];
        //        ViewData["Types"] = TempData["Types"];
        //        return View();
        //    }

        //    if (CheckDocType[0].Name == "Power Point Sunumu (.pptx)")
        //    {
        //        using (EventAppContext db = new EventAppContext())
        //        {
        //            doc.DocumentFormatID = TypeOfDoc;
        //            db.Document.Add(doc);
        //            db.SaveChanges();
        //            doc.DownloadAddress = ("../Documents/" + doc.Id.ToString() + ".pptx");

        //            // EventDocument diye kaydediyoruz
        //            DocumentEventActivity docEvent = new DocumentEventActivity();
        //            using (EventAppContext dbME = new EventAppContext())
        //            {
        //                docEvent.DocumentID = doc.Id;
        //                User USR = (User)Session["LoggedIn"];
        //                string eventconfirmation = USR.AccountConfirmation;

        //                List<Event> EventList = db.Event.ToList();
        //                int count = EventList.Count;
        //                for (int i = 0; i < count; i++)
        //                {
        //                    if (eventconfirmation == EventList[i].EventConfirmation)
        //                    {
        //                        docEvent.EventOrEventActivityID = EventList[i].Id;
        //                    }
        //                }
        //                db.DocumentEventActivity.Add(docEvent);
        //                db.SaveChanges();
        //            }

        //            // EventActivityDocument diye kaydediyoruz
        //            if (ActID.ToString() != "00000000-0000-0000-0000-000000000000")
        //            {
        //                DocumentEventActivity docEventActivity = new DocumentEventActivity();
        //                using (EventAppContext dbME = new EventAppContext())
        //                {
        //                    docEventActivity.DocumentID = doc.Id;
        //                    var AllEventActivities = db.EventActivity.ToList();
        //                    var ThisEventActivity = AllEventActivities.Where(k => k.ActivityID == ActID).ToList();
        //                    docEventActivity.EventOrEventActivityID = ThisEventActivity[0].Id;

        //                    db.DocumentEventActivity.Add(docEventActivity);
        //                    db.SaveChanges();
        //                }
        //            }

        //        }

        //        string path = System.IO.Path.Combine(Server.MapPath("~/Documents/" + doc.Id + ".pptx"));
        //        uploadeddoc.SaveAs(path);

        //        toViewSpeaker();
        //        ViewData["DocumentFormat"] = TempData["DocumentFormat"];
        //        ViewData["ThisEventActivities"] = TempData["ThisEventActivities"];
        //        ViewData["Documents"] = TempData["Documents"];
        //        ViewData["Types"] = TempData["Types"];

        //        ViewData["Errors"] = Errors;

        //        return RedirectToAction("AddDocumentSpeaker", "Documents");
        //    }
        //    else if (CheckDocType[0].Name == "Excel Dosyası (.xlsx)")
        //    {
        //        using (EventAppContext db = new EventAppContext())
        //        {
        //            doc.DocumentFormatID = TypeOfDoc;
        //            db.Document.Add(doc);
        //            db.SaveChanges();
        //            doc.DownloadAddress = ("../Documents/" + doc.Id.ToString() + ".xlsx");

        //            // EventDocument diye kaydediyoruz
        //            DocumentEventActivity docEvent = new DocumentEventActivity();
        //            using (EventAppContext dbME = new EventAppContext())
        //            {
        //                docEvent.DocumentID = doc.Id;
        //                User USR = (User)Session["LoggedIn"];
        //                string eventconfirmation = USR.AccountConfirmation;

        //                List<Event> EventList = db.Event.ToList();
        //                int count = EventList.Count;
        //                for (int i = 0; i < count; i++)
        //                {
        //                    if (eventconfirmation == EventList[i].EventConfirmation)
        //                    {
        //                        docEvent.EventOrEventActivityID = EventList[i].Id;
        //                    }
        //                }
        //                db.DocumentEventActivity.Add(docEvent);
        //                db.SaveChanges();
        //            }

        //            // EventActivityDocument diye kaydediyoruz
        //            if (ActID.ToString() != null)
        //            {
        //                DocumentEventActivity docEventActivity = new DocumentEventActivity();
        //                using (EventAppContext dbME = new EventAppContext())
        //                {
        //                    docEventActivity.DocumentID = doc.Id;
        //                    var AllEventActivities = db.EventActivity.ToList();
        //                    var ThisEventActivity = AllEventActivities.Where(k => k.ActivityID == ActID).ToList();
        //                    docEventActivity.EventOrEventActivityID = ThisEventActivity[0].Id;

        //                    db.DocumentEventActivity.Add(docEventActivity);
        //                    db.SaveChanges();
        //                }
        //            }

        //        }

        //        string path = System.IO.Path.Combine(Server.MapPath("~/Documents/" + doc.Id + ".xlsx"));
        //        uploadeddoc.SaveAs(path);

        //        toViewSpeaker();
        //        ViewData["DocumentFormat"] = TempData["DocumentFormat"];
        //        ViewData["ThisEventActivities"] = TempData["ThisEventActivities"];

        //        TempData["Errors"] = Errors;

        //        return RedirectToAction("AddDocumentSpeaker", "Documents");
        //    }
        //    else if (CheckDocType[0].Name == "Excel Dosyası (.xlsm)")
        //    {
        //        using (EventAppContext db = new EventAppContext())
        //        {
        //            doc.DocumentFormatID = TypeOfDoc;
        //            db.Document.Add(doc);
        //            db.SaveChanges();
        //            doc.DownloadAddress = ("../Documents/" + doc.Id.ToString() + ".xlsm");

        //            // EventDocument diye kaydediyoruz
        //            DocumentEventActivity docEvent = new DocumentEventActivity();
        //            using (EventAppContext dbME = new EventAppContext())
        //            {
        //                docEvent.DocumentID = doc.Id;
        //                User USR = (User)Session["LoggedIn"];
        //                string eventconfirmation = USR.AccountConfirmation;

        //                List<Event> EventList = db.Event.ToList();
        //                int count = EventList.Count;
        //                for (int i = 0; i < count; i++)
        //                {
        //                    if (eventconfirmation == EventList[i].EventConfirmation)
        //                    {
        //                        docEvent.EventOrEventActivityID = EventList[i].Id;
        //                    }
        //                }
        //                db.DocumentEventActivity.Add(docEvent);
        //                db.SaveChanges();
        //            }

        //            // EventActivityDocument diye kaydediyoruz
        //            if (ActID.ToString() != null)
        //            {
        //                DocumentEventActivity docEventActivity = new DocumentEventActivity();
        //                using (EventAppContext dbME = new EventAppContext())
        //                {
        //                    docEventActivity.DocumentID = doc.Id;
        //                    var AllEventActivities = db.EventActivity.ToList();
        //                    var ThisEventActivity = AllEventActivities.Where(k => k.ActivityID == ActID).ToList();
        //                    docEventActivity.EventOrEventActivityID = ThisEventActivity[0].Id;

        //                    db.DocumentEventActivity.Add(docEventActivity);
        //                    db.SaveChanges();
        //                }
        //            }

        //        }

        //        string path = System.IO.Path.Combine(Server.MapPath("~/Documents/" + doc.Id + ".xlsm"));
        //        uploadeddoc.SaveAs(path);

        //        toViewSpeaker();
        //        ViewData["DocumentFormat"] = TempData["DocumentFormat"];
        //        ViewData["ThisEventActivities"] = TempData["ThisEventActivities"];

        //        TempData["Errors"] = Errors;

        //        return RedirectToAction("AddDocumentSpeaker", "Documents");
        //    }
        //    else if (CheckDocType[0].Name == "Word Dosyası (.docx)")
        //    {
        //        using (EventAppContext db = new EventAppContext())
        //        {
        //            doc.DocumentFormatID = TypeOfDoc;
        //            db.Document.Add(doc);
        //            db.SaveChanges();
        //            doc.DownloadAddress = ("../Documents/" + doc.Id.ToString() + ".docx");

        //            // EventDocument diye kaydediyoruz
        //            DocumentEventActivity docEvent = new DocumentEventActivity();
        //            using (EventAppContext dbME = new EventAppContext())
        //            {
        //                docEvent.DocumentID = doc.Id;
        //                User USR = (User)Session["LoggedIn"];
        //                string eventconfirmation = USR.AccountConfirmation;

        //                List<Event> EventList = db.Event.ToList();
        //                int count = EventList.Count;
        //                for (int i = 0; i < count; i++)
        //                {
        //                    if (eventconfirmation == EventList[i].EventConfirmation)
        //                    {
        //                        docEvent.EventOrEventActivityID = EventList[i].Id;
        //                    }
        //                }
        //                db.DocumentEventActivity.Add(docEvent);
        //                db.SaveChanges();
        //            }

        //            // EventActivityDocument diye kaydediyoruz
        //            if (ActID.ToString() != null)
        //            {
        //                DocumentEventActivity docEventActivity = new DocumentEventActivity();
        //                using (EventAppContext dbME = new EventAppContext())
        //                {
        //                    docEventActivity.DocumentID = doc.Id;
        //                    var AllEventActivities = db.EventActivity.ToList();
        //                    var ThisEventActivity = AllEventActivities.Where(k => k.ActivityID == ActID).ToList();
        //                    docEventActivity.EventOrEventActivityID = ThisEventActivity[0].Id;

        //                    db.DocumentEventActivity.Add(docEventActivity);
        //                    db.SaveChanges();
        //                }
        //            }

        //        }

        //        string path = System.IO.Path.Combine(Server.MapPath("~/Documents/" + doc.Id + ".docx"));
        //        uploadeddoc.SaveAs(path);

        //        toViewSpeaker();
        //        ViewData["DocumentFormat"] = TempData["DocumentFormat"];
        //        ViewData["ThisEventActivities"] = TempData["ThisEventActivities"];

        //        TempData["Errors"] = Errors;

        //        return RedirectToAction("AddDocumentSpeaker", "Documents");
        //    }
        //    else if (CheckDocType[0].Name == "PDF")
        //    {
        //        using (EventAppContext db = new EventAppContext())
        //        {
        //            doc.DocumentFormatID = TypeOfDoc;
        //            db.Document.Add(doc);
        //            db.SaveChanges();
        //            doc.DownloadAddress = ("../Documents/" + doc.Id.ToString() + ".pdf");

        //            // EventDocument diye kaydediyoruz
        //            DocumentEventActivity docEvent = new DocumentEventActivity();
        //            using (EventAppContext dbME = new EventAppContext())
        //            {
        //                docEvent.DocumentID = doc.Id;
        //                User USR = (User)Session["LoggedIn"];
        //                string eventconfirmation = USR.AccountConfirmation;

        //                List<Event> EventList = db.Event.ToList();
        //                int count = EventList.Count;
        //                for (int i = 0; i < count; i++)
        //                {
        //                    if (eventconfirmation == EventList[i].EventConfirmation)
        //                    {
        //                        docEvent.EventOrEventActivityID = EventList[i].Id;
        //                    }
        //                }
        //                db.DocumentEventActivity.Add(docEvent);
        //                db.SaveChanges();
        //            }

        //            // EventActivityDocument diye kaydediyoruz
        //            if (ActID.ToString() != "00000000-0000-0000-0000-000000000000")
        //            {
        //                DocumentEventActivity docEventActivity = new DocumentEventActivity();
        //                using (EventAppContext dbME = new EventAppContext())
        //                {
        //                    docEventActivity.DocumentID = doc.Id;
        //                    var AllEventActivities = db.EventActivity.ToList();
        //                    var ThisEventActivity = AllEventActivities.Where(k => k.ActivityID == ActID).ToList();
        //                    docEventActivity.EventOrEventActivityID = ThisEventActivity[0].Id;

        //                    db.DocumentEventActivity.Add(docEventActivity);
        //                    db.SaveChanges();
        //                }
        //            }

        //        }

        //        string path = System.IO.Path.Combine(Server.MapPath("~/Documents/" + doc.Id + ".pdf"));
        //        uploadeddoc.SaveAs(path);

        //        toViewSpeaker();
        //        ViewData["DocumentFormat"] = TempData["DocumentFormat"];
        //        ViewData["ThisEventActivities"] = TempData["ThisEventActivities"];

        //        TempData["Errors"] = Errors;

        //        return RedirectToAction("AddDocumentSpeaker", "Documents");
        //    }

        //    return View();
        //}

        //public dynamic toViewSpeaker()
        //{
        //    List<DocumentFormat> Formats = db.DocumentFormat.ToList();

        //    List<DocumentFormat> SortedList = new List<DocumentFormat>();
        //    SortedList = Formats.OrderBy(d => (d.Name)).ToList();

        //    TempData["DocumentFormat"] = SortedList;

        //    User LoggedUser = (User)Session["LoggedIn"];
        //    List<Event> AllEvents = db.Event.ToList();
        //    var thisEvent = AllEvents.Where(q => q.EventConfirmation == LoggedUser.AccountConfirmation).ToList();

        //    var AllSEA = db.SpeakerEventActivity.ToList();
        //    var SpeakersActivities = AllSEA.Where(l => l.SpeakerID == LoggedUser.Id).ToList();
        //    List<EventActivity> SpeakersEventActivites = new List<EventActivity>();
        //    for (int i = 0; i < SpeakersActivities.Count; i++)
        //    {
        //        EventActivity EA = db.EventActivity.Find(SpeakersActivities[i].EventActivityID);
        //        SpeakersEventActivites.Add(EA);
        //    }


        //    var AllActivities = db.Activity.ToList();
        //    List<Activity> ThisEventsActivities = new List<Activity>();
        //    for (int k = 0; k < SpeakersEventActivites.Count; k++)
        //    {
        //        Activity add = db.Activity.Find(SpeakersEventActivites[k].ActivityID);
        //        ThisEventsActivities.Add(add);
        //    }

        //    List<Activity> SortedActivityList = new List<Activity>();
        //    SortedActivityList = ThisEventsActivities.OrderBy(d => (d.Name)).ToList();

        //    TempData["ThisEventActivities"] = SortedActivityList;
        //    // Yukarıda Speaker'ın konuşmacı olduğu Activity'leri aldık

        //    List<EventActivity> EEAA = new List<EventActivity>();
        //    var AllEA = db.EventActivity.ToList();
        //    for (int i = 0; i < SortedActivityList.Count; i++)
        //    {
        //        var addthis = AllEA.Where(k => k.ActivityID == SortedActivityList[i].Id).ToList();
        //        EventActivity add = db.EventActivity.Find(addthis[0].Id);
        //        EEAA.Add(add);
        //    }

        //    var EventsDocuments = db.DocumentEventActivity.ToList();
        //    List<DocumentEventActivity> EventsDocs = new List<DocumentEventActivity>();
        //    for (int i = 0; i < EEAA.Count; i++)
        //    {
        //        var add = EventsDocuments.Where(k => k.EventOrEventActivityID == EEAA[i].Id).ToList();
        //        DocumentEventActivity addthis = db.DocumentEventActivity.Find(add[0].Id );
        //        EventsDocuments.Add(addthis);
        //    }

        //    List<Document> docinfo = new List<Document>();
        //    for (int j = 0; j < EventsDocuments.Count; j++)
        //    {
        //        Document add = db.Document.Find(EventsDocuments[j].DocumentID);
        //        docinfo.Add(add);
        //    }

        //    List<Document> Sorteddocinfo = new List<Document>();
        //    Sorteddocinfo = docinfo.OrderBy(d => (d.Name)).ToList();

        //    TempData["Documents"] = Sorteddocinfo;

        //    List<string> types = new List<string>();
        //    for (int t = 0; t < Sorteddocinfo.Count; t++)
        //    {
        //        Document thisdoc = db.Document.Find(Sorteddocinfo[t].Id);
        //        DocumentFormat thisformat = db.DocumentFormat.Find(thisdoc.DocumentFormatID);
        //        types.Add(thisformat.Name);
        //    }

        //    TempData["Types"] = types;

        //    return "";
        //}

















































        // GET: Documents
        public ActionResult Index()
        {
            return View(db.Document.ToList());
        }

        // GET: Documents/Details/5
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Document document = db.Document.Find(id);
            if (document == null)
            {
                return HttpNotFound();
            }
            return View(document);
        }

        // GET: Documents/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Documents/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,DocumentFormatID,DownloadAddress,Name")] Document document)
        {
            if (ModelState.IsValid)
            {
                document.Id = Guid.NewGuid();
                db.Document.Add(document);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(document);
        }

        // GET: Documents/Edit/5
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Document document = db.Document.Find(id);
            if (document == null)
            {
                return HttpNotFound();
            }
            return View(document);
        }

        // POST: Documents/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,DocumentFormatID,DownloadAddress,Name")] Document document)
        {
            if (ModelState.IsValid)
            {
                db.Entry(document).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(document);
        }

        // GET: Documents/Delete/5
        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Document document = db.Document.Find(id);
            if (document == null)
            {
                return HttpNotFound();
            }
            return View(document);
        }

        // POST: Documents/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            Document document = db.Document.Find(id);
            db.Document.Remove(document);
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
