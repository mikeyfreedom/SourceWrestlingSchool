using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SourceWrestlingSchool.Models;

namespace SourceWrestlingSchool.Controllers
{
    public class LiveEventsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: LiveEvents
        public ActionResult Index()
        {
            var model = db.LiveEvents.Include(ev => ev.Venue).Include(ev => ev.Seats).ToList();
            return View(model);
        }

        // GET: LiveEvents/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LiveEvent liveEvent = db.LiveEvents.Where(ev => ev.EventID == id).Include(ev => ev.Venue).Include(ev => ev.Seats).Single();
            if (liveEvent == null)
            {
                return HttpNotFound();
            }
            return View(liveEvent);
        }

        // GET: LiveEvents/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: LiveEvents/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "EventID,EventName,EventDate,EventTime,Location")] LiveEvent liveEvent)
        {
            if (ModelState.IsValid)
            {
                db.LiveEvents.Add(liveEvent);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(liveEvent);
        }

        // GET: LiveEvents/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LiveEvent liveEvent = db.LiveEvents.Where(ev => ev.EventID == id).Include(ev => ev.Venue).Include(ev => ev.Seats).Single();
            if (liveEvent == null)
            {
                return HttpNotFound();
            }
            return View(liveEvent);
        }

        // POST: LiveEvents/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "EventID,EventName,EventDate,EventTime,Location")] LiveEvent liveEvent)
        {
            if (ModelState.IsValid)
            {
                db.Entry(liveEvent).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(liveEvent);
        }

        // GET: LiveEvents/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LiveEvent liveEvent = db.LiveEvents.Where(ev => ev.EventID == id).Include(ev => ev.Venue).Include(ev => ev.Seats).Single();
            if (liveEvent == null)
            {
                return HttpNotFound();
            }
            return View(liveEvent);
        }

        // POST: LiveEvents/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            LiveEvent liveEvent = db.LiveEvents.Where(ev => ev.EventID == id).Include(ev => ev.Venue).Include(ev => ev.Seats).Single();
            db.LiveEvents.Remove(liveEvent);
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
