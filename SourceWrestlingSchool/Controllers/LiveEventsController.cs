using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.UI.WebControls;
using System.Web.Mvc;
using SourceWrestlingSchool.Models;
using System.IO;
using System.Web.UI;

namespace SourceWrestlingSchool.Controllers
{
    public class LiveEventsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: LiveEvents
        public ActionResult Index()
        {
            return View(db.LiveEvents
                        .Include(v => v.Venue)
                        .Include(s => s.Seats)                        
                        .ToList());
        }

        // GET: LiveEvents/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LiveEvent liveEvent = db.LiveEvents
                                  .Where(e => e.EventId == id)
                                  .Include(v => v.Venue)
                                  .Include(s => s.Seats)
                                  .First();
                                  
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
        public ActionResult Create([Bind(Include = "EventID,EventName,EventDate,EventTime,EventRevenue")] LiveEvent liveEvent)
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
            LiveEvent liveEvent = db.LiveEvents.Find(id);
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
        public ActionResult Edit([Bind(Include = "EventID,EventName,EventDate,EventTime,EventRevenue")] LiveEvent liveEvent)
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
            LiveEvent liveEvent = db.LiveEvents.Find(id);
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
            LiveEvent liveEvent = db.LiveEvents.Find(id);
            if (liveEvent != null)
            {
                db.LiveEvents.Remove(liveEvent);
            }
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public void ExportEventListToExcel()
        {
            GridView grid = new GridView();
            var eventQuery = from e in db.LiveEvents
                             select new { e.EventId, e.EventName, e.EventDate, e.EventRevenue };
            grid.DataSource = eventQuery.ToList();
            grid.DataBind();

            Response.ClearContent();
            Response.AddHeader("content-disposition", "attachment; filename=EventRevenue.xls");
            Response.ContentType = "application/excel";
            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);

            grid.RenderControl(htw);

            Response.Write(sw.ToString());
            Response.End();
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
