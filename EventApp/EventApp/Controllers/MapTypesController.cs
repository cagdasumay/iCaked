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
    public class MapTypesController : Controller
    {
        private EventAppContext db = new EventAppContext();

        // GET: MapTypes
        public ActionResult Index()
        {
            return View(db.MapType.ToList());
        }

        // GET: MapTypes/Details/5
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MapType mapType = db.MapType.Find(id);
            if (mapType == null)
            {
                return HttpNotFound();
            }
            return View(mapType);
        }

        // GET: MapTypes/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: MapTypes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name")] MapType mapType)
        {
            if (ModelState.IsValid)
            {
                mapType.Id = Guid.NewGuid();
                db.MapType.Add(mapType);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(mapType);
        }

        // GET: MapTypes/Edit/5
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MapType mapType = db.MapType.Find(id);
            if (mapType == null)
            {
                return HttpNotFound();
            }
            return View(mapType);
        }

        // POST: MapTypes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name")] MapType mapType)
        {
            if (ModelState.IsValid)
            {
                db.Entry(mapType).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(mapType);
        }

        // GET: MapTypes/Delete/5
        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MapType mapType = db.MapType.Find(id);
            if (mapType == null)
            {
                return HttpNotFound();
            }
            return View(mapType);
        }

        // POST: MapTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            MapType mapType = db.MapType.Find(id);
            db.MapType.Remove(mapType);
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
