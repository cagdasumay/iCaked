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
using System.Data.SqlClient;
using System.Configuration;
using System.Collections;
using System.Drawing;
using System.IO;

namespace EventApp.Controllers
{
    public class MapsController : Controller
    {
        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["EventAppContext"].ConnectionString);
        private EventAppContext db = new EventAppContext();

        public ActionResult CreateMap()
        {
            if (Session["LoggedIn"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            ViewData["Mpslist"] = ControllerToView();

            List<MapType> maptype = db.MapType.ToList();
            ViewData["MapType"] = maptype;

            string CorrectAll = "Correct";
            List<string> ValidationErrors = new List<string>();
            ValidationErrors.Add(CorrectAll);
            ValidationErrors.Add(CorrectAll);
            ValidationErrors.Add(CorrectAll);
            ValidationErrors.Add(CorrectAll);
            ValidationErrors.Add(CorrectAll);
            ValidationErrors.Add(CorrectAll);
            ViewData["ValidationList"] = ValidationErrors;
            ViewData["UsageError"] = "Correct";

            return View();
        }

        [HttpPost]
        public ActionResult CreateMap(Map Mps, HttpPostedFileBase uploadedmap, Guid TypeOfMap, string Usage)
        {
            if (Session["LoggedIn"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            List<MapType> maptype = db.MapType.ToList();
            ViewData["MapType"] = maptype;

            var MapTypeList = db.MapType.ToList();
            var CheckMapType = MapTypeList.Where(k => k.Id == TypeOfMap).ToList();

            List<string> ErrorControl = Validations(Mps, uploadedmap, TypeOfMap);
            if (ErrorControl[0] != "Correct" || ErrorControl[1] != "Correct" || ErrorControl[2] != "Correct" || ErrorControl[3] != "Correct" || ErrorControl[4] != "Correct" || ErrorControl[5] != "Correct" || (Usage == "Boş" && CheckMapType[0].Name != "Google Haritası"))
            {
                ViewData["Mpslist"] = ControllerToView();
                ViewData["ValidationList"] = ErrorControl;
                if (Usage == "Boş")
                {
                    ViewData["UsageError"] = "UsageError";
                }
                return View();
            }
            else
            {
                ViewData["ValidationList"] = ErrorControl;
                ViewData["UsageError"] = "Correct";
            }

            if (CheckMapType[0].Name == "Resim")
            {
                using (EventAppContext db = new EventAppContext())
                {
                    Mps.Usage = Usage;
                    Mps.MapTypeID = TypeOfMap;
                    db.Map.Add(Mps);
                    db.SaveChanges();
                    Mps.MapDirectory = ("../ Images /Maps/" + Mps.Id.ToString() + ".jpg");

                    MapEventActivity MapEvent = new MapEventActivity();
                    using (EventAppContext dbME = new EventAppContext())
                    {
                        MapEvent.MapID = Mps.Id;
                        User USR = (User)Session["LoggedIn"];
                        string eventconfirmation = USR.AccountConfirmation;

                        List<Event> EventList = db.Event.ToList();
                        int count = EventList.Count;
                        for (int i = 0; i < count; i++)
                        {
                            if (eventconfirmation == EventList[i].EventConfirmation)
                            {
                                MapEvent.EventOrEventActivityID = EventList[i].Id;
                            }
                        }
                        db.MapEventActivity.Add(MapEvent);
                        db.SaveChanges();
                    }
                }

                System.IO.File.Copy(Server.MapPath("/Images/Maps/default_thumbnail.png"), Server.MapPath("/Images/Maps/" + Mps.Id + ".png"));
                string path = System.IO.Path.Combine(Server.MapPath("~/Images/Maps/" + Mps.Id + ".jpg"));
                Image sourceimage = Image.FromStream(uploadedmap.InputStream);
                sourceimage.Save(path);

                ViewData["Mpslist"] = ControllerToView();
                return RedirectToAction("CreateMap", "Maps");
            }
            else if (CheckMapType[0].Name == "PDF")
            {
                using (EventAppContext db = new EventAppContext())
                {
                    Mps.Usage = Usage;
                    Mps.MapTypeID = TypeOfMap;
                    db.Map.Add(Mps);
                    db.SaveChanges();
                    Mps.MapDirectory = ("../ Images /Maps/" + Mps.Id.ToString() + ".jpg");

                    MapEventActivity MapEvent = new MapEventActivity();
                    using (EventAppContext dbME = new EventAppContext())
                    {
                        MapEvent.MapID = Mps.Id;
                        User USR = (User)Session["LoggedIn"];
                        string eventconfirmation = USR.AccountConfirmation;

                        List<Event> EventList = db.Event.ToList();
                        int count = EventList.Count;
                        for (int i = 0; i < count; i++)
                        {
                            if (eventconfirmation == EventList[i].EventConfirmation)
                            {
                                MapEvent.EventOrEventActivityID = EventList[i].Id;
                            }
                        }
                        db.MapEventActivity.Add(MapEvent);
                        db.SaveChanges();
                    }
                }

                System.IO.File.Copy(Server.MapPath("/Images/Maps/default_thumbnail.pdf"), Server.MapPath("/Images/Maps/" + Mps.Id + ".pdf"));
                string path = System.IO.Path.Combine(Server.MapPath("~/Images/Maps/" + Mps.Id + ".pdf"));
                uploadedmap.SaveAs(path);

                ViewData["Mpslist"] = ControllerToView();
                return RedirectToAction("CreateMap", "Maps");
            }
            else if (CheckMapType[0].Name == "Google Haritası")
            {
                using (EventAppContext db = new EventAppContext())
                {
                    Mps.Usage = "Google";
                    Mps.MapTypeID = TypeOfMap;
                    db.Map.Add(Mps);
                    db.SaveChanges();

                    MapEventActivity MapEvent = new MapEventActivity();
                    using (EventAppContext dbME = new EventAppContext())
                    {
                        MapEvent.MapID = Mps.Id;
                        User USR = (User)Session["LoggedIn"];
                        string eventconfirmation = USR.AccountConfirmation;

                        List<Event> EventList = db.Event.ToList();
                        int count = EventList.Count;
                        for (int i = 0; i < count; i++)
                        {
                            if (eventconfirmation == EventList[i].EventConfirmation)
                            {
                                MapEvent.EventOrEventActivityID = EventList[i].Id;
                            }
                        }
                        db.MapEventActivity.Add(MapEvent);
                        db.SaveChanges();
                    }
                    ViewData["Mpslist"] = ControllerToView();
                    return RedirectToAction("CreateMap", "Maps");
                }
            }

            return View();
        }

        public Map GetMapDetails(Guid? id)
        {
            Map map = db.Map.Find(id);
            return map;
        }

        public ActionResult DisplayMap(Guid mapdet)
        {
            User user = (User)Session["LoggedIn"];

            UserType ThisUser = db.UserType.Find(user.UserTypeID);
            ViewData["UserType"] = ThisUser.Name;

            if (ModelState.IsValid)
            {
                // Map Type
                var MapList = db.Map.ToList();
                var SelectedMap = MapList.Where(g => g.Id == mapdet).ToList();
                Guid TypeOfMap = SelectedMap[0].MapTypeID;

                // Map Type as String
                var MapTypeList = db.MapType.ToList();
                var CheckMapType = MapTypeList.Where(k => k.Id == TypeOfMap).ToList();

                ViewData["DisplayedMapType"] = CheckMapType[0].Name;

                if (CheckMapType[0].Name == "PDF" || CheckMapType[0].Name == "Resim")
                {
                    ViewData["DisplayMapInformation"] = GetMapDetails(mapdet);
                    return View();
                }
                else if (CheckMapType[0].Name == "Google Haritası")
                {
                    return RedirectToAction("GMapDisplay", new { MapID = mapdet });
                }
                return View();
            }
            else
            {
                return RedirectToAction("CreateMap", "Maps");
            }

        }

        public dynamic ControllerToView()
        {
            if (Session["LoggedIn"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Giriş yapan kullanıcının bilgilerini alıyoruz
            User LoggedUser = (User)Session["LoggedIn"];
            // Giriş yapan kullanıcının Account Confirmation bilgisini alıyoruz
            string LoggedEventConfirmation = LoggedUser.AccountConfirmation;

            // Tüm event listesini alıyoruz
            var EventList = db.Event.ToList();
            // Giriş yapılan Event'in bilgilerini alıyoruz
            var LoggedEvent = EventList.Where(k => k.EventConfirmation == LoggedEventConfirmation).ToList();

            // MapEventActivity'de kayıtlı olan bütün Map'lerin listesini alıyoruz
            var MapList = db.MapEventActivity.ToList();
            // Yeni bir liste oluşturuyoruz, içine Event'in Map'lerini atıcaz
            List<Map> LoggedEventsMapIDList = new List<Map>();

            // Burada giriş yapılan Event'in tüm Map'lerinin ID'lerini çekiyoruz
            for (int i = 0; i < MapList.Count; i++)
            {
                if (MapList[i].EventOrEventActivityID == LoggedEvent[0].Id)
                {
                    LoggedEventsMapIDList.Add(new Map() { Id = (MapList[i].MapID) });
                }               
            }

            // Yeni bir Map List oluşturuyoruz
            List<Map> LoggedMapModels = new List<Map>();
            // Kayıtlı bütün MAP'leri alıyoruz
            var CreatedAllMaps = db.Map.ToList();

            // İstediğimiz listeyi burası veriyor
            for (int j = 0; j < LoggedEventsMapIDList.Count; j++)
            {
                List<Map> LoggedMap1Row = new List<Map>();
                LoggedMap1Row = CreatedAllMaps.Where(l => l.Id == LoggedEventsMapIDList[j].Id).ToList();
                LoggedMapModels.Add(new Map() { Id = (LoggedMap1Row[0].Id), MapTypeID = (LoggedMap1Row[0].MapTypeID), Name = (LoggedMap1Row[0].Name), MapDirectory = (LoggedMap1Row[0].MapDirectory), Usage = (LoggedMap1Row[0].Usage) });
            }

            List<Map> SortedLoggedMapList = new List<Map>();
            SortedLoggedMapList = LoggedMapModels.OrderBy(d => (d.Name)).ToList();

            return SortedLoggedMapList;
        }

        public dynamic Validations(Map Mps, HttpPostedFileBase uploadedmap, Guid TypeOfMap)
        {
            string MapTypeError = "Correct";
            string MapImageError = "Correct";
            string MapNameError = "Correct";
            string UploadError = "Correct";
            string LattitudeError = "Correct";
            string LongitudeError = "Correct";

            var MapTypeList = db.MapType.ToList();
            var CheckMapType = MapTypeList.Where(k => k.Id == TypeOfMap).ToList();

            if (TypeOfMap.ToString() == "00000000-0000-0000-0000-000000000000")
            {
                MapTypeError = "MapTypeError";
            }

            if (MapTypeError == "MapTypeError")
            {
                List<string> ValidationErrors = new List<string>();
                ValidationErrors.Add(MapTypeError);
                ValidationErrors.Add(MapImageError);
                ValidationErrors.Add(MapNameError);
                ValidationErrors.Add(UploadError);
                ValidationErrors.Add(LattitudeError);
                ValidationErrors.Add(LongitudeError);

                return ValidationErrors;
            }
            else if (CheckMapType[0].Name == "PDF" || CheckMapType[0].Name == "Resim")
            {
                if (uploadedmap != null)
                {
                    string searchWithinThis = uploadedmap.ContentType;

                    string searchForThisPDF = "application/";
                    int firstCharacterPDF = searchWithinThis.IndexOf(searchForThisPDF);
                    if (firstCharacterPDF != 0 && CheckMapType[0].Name == "PDF")
                    {
                        UploadError = "Lütfen seçtiğiniz Harita Format'ıyla aynı Format'ta bir harita yükleyin";
                    }

                    string searchForThisImage = "image/";
                    int firstCharacterImage = searchWithinThis.IndexOf(searchForThisImage);
                    if (firstCharacterImage != 0 && CheckMapType[0].Name == "Resim")
                    {
                        UploadError = "Lütfen seçtiğiniz Harita Format'ıyla aynı Format'ta bir harita yükleyin";
                    }
                }
                else
                {
                    MapImageError = "MapImageError";
                }

                if (Mps.Name == null)
                {
                    MapNameError = "MapNameError";
                }
                List<string> ValidationErrors = new List<string>();
                ValidationErrors.Add(MapTypeError);
                ValidationErrors.Add(MapImageError);
                ValidationErrors.Add(MapNameError);
                ValidationErrors.Add(UploadError);
                ValidationErrors.Add(LattitudeError);
                ValidationErrors.Add(LongitudeError);

                return ValidationErrors;
            }
            else if (CheckMapType[0].Name == "Google Haritası")
            {
                if (Mps.Lattitude == null)
                {
                    LattitudeError = "LattitudeError";
                }
                else if (Mps.Lattitude < -90 || Mps.Lattitude > 90)
                {
                    Mps.Lattitude = null;
                    LattitudeError = "LattitudeError";
                }
                else if (Mps.Lattitude == 0)
                {
                    Mps.Lattitude = null;
                    LattitudeError = "LattitudeError";
                }

                if (Mps.Longitude == null)
                {
                    LongitudeError = "LongitudeError";
                }
                else if (Mps.Longitude < -180 || Mps.Longitude > 180)
                {
                    LongitudeError = "LongitudeError";
                }
                else if (Mps.Longitude == 0)
                {
                    LongitudeError = "LongitudeError";
                }

                if (Mps.Name == null)
                {
                    MapNameError = "MapNameError";
                }

                List<string> ValidationErrors = new List<string>();
                ValidationErrors.Add(MapTypeError);
                ValidationErrors.Add(MapImageError);
                ValidationErrors.Add(MapNameError);
                ValidationErrors.Add(UploadError);
                ValidationErrors.Add(LattitudeError);
                ValidationErrors.Add(LongitudeError);

                return ValidationErrors;
            }
            else
            {
                List<string> ValidationErrors = new List<string>();
                ValidationErrors.Add(MapTypeError);
                ValidationErrors.Add(MapImageError);
                ValidationErrors.Add(MapNameError);
                ValidationErrors.Add(UploadError);
                ValidationErrors.Add(LattitudeError);
                ValidationErrors.Add(LongitudeError);

                return ValidationErrors;
            }
        }

        public ActionResult DeleteApprove(Guid id)
        {
            Guid DeleteID = id;
            Map map = db.Map.Find(id);
            db.Map.Remove(map);
            db.SaveChanges();

            var AllMapEvents = db.MapEventActivity.ToList();
            var DeleteMap = AllMapEvents.Where(k => k.MapID == DeleteID).ToList();

            for (int i = 0; i < DeleteMap.Count; i++)
            {
                MapEventActivity MapEvent = db.MapEventActivity.Find(DeleteMap[i].Id);
                db.MapEventActivity.Remove(MapEvent);
                db.SaveChanges();
            }

            return RedirectToAction("CreateMap", "Maps");
        }

        public ActionResult GMapDisplay (Guid MapID)
        {
            User user = (User)Session["LoggedIn"];

            UserType ThisUser = db.UserType.Find(user.UserTypeID);
            ViewData["UserType"] = ThisUser.Name;
            ViewData["DisplayMapInformation"] = GetMapDetails(MapID);
            return View();
        }

        public ActionResult List()
        {
            ViewData["MapList"] = ControllerToView();

            return View();
        }




































        // GET: Maps
        public ActionResult Index()
        {
            return View(db.Map.ToList());
        }

        // GET: Maps/Details/5
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Map map = db.Map.Find(id);
            if (map == null)
            {
                return HttpNotFound();
            }
            return View(map);
        }

        // GET: Maps/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Maps/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,MapTypeID,Name,MapDirectory")] Map map)
        {
            if (ModelState.IsValid)
            {
                map.Id = Guid.NewGuid();
                db.Map.Add(map);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(map);
        }

        // GET: Maps/Edit/5
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Map map = db.Map.Find(id);
            if (map == null)
            {
                return HttpNotFound();
            }
            return View(map);
        }

        // POST: Maps/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,MapTypeID,Name,MapDirectory")] Map map)
        {
            if (ModelState.IsValid)
            {
                db.Entry(map).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(map);
        }

        // GET: Maps/Delete/5
        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Map map = db.Map.Find(id);
            if (map == null)
            {
                return HttpNotFound();
            }
            return View(map);
        }

        // POST: Maps/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            Map map = db.Map.Find(id);
            db.Map.Remove(map);
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
