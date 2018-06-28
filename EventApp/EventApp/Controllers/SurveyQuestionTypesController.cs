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
    public class SurveyQuestionTypesController : Controller
    {
        private EventAppContext db = new EventAppContext();

        // GET: SurveyQuestionTypes
        public ActionResult Index()
        {
            return View(db.SurveyQuestionType.ToList());
        }

        // GET: SurveyQuestionTypes/Details/5
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SurveyQuestionType surveyQuestionType = db.SurveyQuestionType.Find(id);
            if (surveyQuestionType == null)
            {
                return HttpNotFound();
            }
            return View(surveyQuestionType);
        }

        // GET: SurveyQuestionTypes/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: SurveyQuestionTypes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name")] SurveyQuestionType surveyQuestionType)
        {
            if (ModelState.IsValid)
            {
                surveyQuestionType.Id = Guid.NewGuid();
                db.SurveyQuestionType.Add(surveyQuestionType);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(surveyQuestionType);
        }

        // GET: SurveyQuestionTypes/Edit/5
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SurveyQuestionType surveyQuestionType = db.SurveyQuestionType.Find(id);
            if (surveyQuestionType == null)
            {
                return HttpNotFound();
            }
            return View(surveyQuestionType);
        }

        // POST: SurveyQuestionTypes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name")] SurveyQuestionType surveyQuestionType)
        {
            if (ModelState.IsValid)
            {
                db.Entry(surveyQuestionType).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(surveyQuestionType);
        }

        // GET: SurveyQuestionTypes/Delete/5
        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SurveyQuestionType surveyQuestionType = db.SurveyQuestionType.Find(id);
            if (surveyQuestionType == null)
            {
                return HttpNotFound();
            }
            return View(surveyQuestionType);
        }

        // POST: SurveyQuestionTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            SurveyQuestionType surveyQuestionType = db.SurveyQuestionType.Find(id);
            db.SurveyQuestionType.Remove(surveyQuestionType);
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
