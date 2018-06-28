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
    public class DocumentFormatsController : Controller
    {
        private EventAppContext db = new EventAppContext();

        // GET: DocumentFormats
        public ActionResult Index()
        {
            return View(db.DocumentFormat.ToList());
        }

        // GET: DocumentFormats/Details/5
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DocumentFormat documentFormat = db.DocumentFormat.Find(id);
            if (documentFormat == null)
            {
                return HttpNotFound();
            }
            return View(documentFormat);
        }

        // GET: DocumentFormats/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: DocumentFormats/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name")] DocumentFormat documentFormat)
        {
            if (ModelState.IsValid)
            {
                documentFormat.Id = Guid.NewGuid();
                db.DocumentFormat.Add(documentFormat);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(documentFormat);
        }

        // GET: DocumentFormats/Edit/5
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DocumentFormat documentFormat = db.DocumentFormat.Find(id);
            if (documentFormat == null)
            {
                return HttpNotFound();
            }
            return View(documentFormat);
        }

        // POST: DocumentFormats/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name")] DocumentFormat documentFormat)
        {
            if (ModelState.IsValid)
            {
                db.Entry(documentFormat).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(documentFormat);
        }

        // GET: DocumentFormats/Delete/5
        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DocumentFormat documentFormat = db.DocumentFormat.Find(id);
            if (documentFormat == null)
            {
                return HttpNotFound();
            }
            return View(documentFormat);
        }

        // POST: DocumentFormats/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            DocumentFormat documentFormat = db.DocumentFormat.Find(id);
            db.DocumentFormat.Remove(documentFormat);
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
