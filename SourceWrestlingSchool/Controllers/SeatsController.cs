using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using SourceWrestlingSchool.Models;

namespace SourceWrestlingSchool.Controllers
{
    public class SeatsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Seats
        public ActionResult Index()
        {
            return View(db.Seats.ToList());
        }

        // GET: Seats/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Seat seat = db.Seats.Find(id);
            if (seat == null)
            {
                return HttpNotFound();
            }
            return View(seat);
        }

        // GET: Seats/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Seats/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "SeatID,SeatNumber,Status")] Seat seat)
        {
            if (ModelState.IsValid)
            {
                db.Seats.Add(seat);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(seat);
        }

        // GET: Seats/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Seat seat = db.Seats.Find(id);
            if (seat == null)
            {
                return HttpNotFound();
            }
            return View(seat);
        }

        // POST: Seats/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "SeatID,SeatNumber,Status")] Seat seat)
        {
            if (ModelState.IsValid)
            {
                db.Entry(seat).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(seat);
        }

        // GET: Seats/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Seat seat = db.Seats.Find(id);
            if (seat == null)
            {
                return HttpNotFound();
            }
            return View(seat);
        }

        // POST: Seats/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Seat seat = db.Seats.Single(s => s.SeatID == id);
            if (seat != null)
            {
                db.Seats.Remove(seat);
            }
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
