using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using SourceWrestlingSchool.Models;

namespace SourceWrestlingSchool.Controllers
{
    /// <summary>
    ///     Controller to handle the database actions in associations with Seat entities.
    /// </summary>
    public class SeatsController : Controller
    {
        /// <summary>
        ///     Virtual representation of the database
        /// </summary>
        private readonly ApplicationDbContext _db = new ApplicationDbContext();

        /// <summary>
        ///     Get all seats in the database and show them in a list View.
        /// </summary>
        /// <returns> Index View, with a sending model of a list of Seats</returns>
        // GET: Seats
        public ActionResult Index()
        {
            return View(_db.Seats.Include(s => s.Events).ToList());
        }

        /// <summary>
        ///     View the details of a particular seat
        /// </summary>
        /// <remarks>
        ///     Get the details of the seat with a matching id from the database.
        ///     Send the details of the seat as a the model for the Details view.
        /// </remarks>
        /// <param name="id">The id of the seat to be shown in detail</param>
        /// <returns>The Detail View with the seat details sent as the model</returns>
        // GET: Seats/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Seat seat = _db.Seats.Find(id);
            if (seat == null)
            {
                return HttpNotFound();
            }
            return View(seat);
        }

        //SEATS WOULD NEVER BE CREATED OR EDITED MANUALLY, SO THEY HAVE BEEN COMMENTED OUT
        //// GET: Seats/Create
        //public ActionResult Create()
        //{
        //    return View();
        //}

        //// POST: Seats/Create
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Create([Bind(Include = "SeatID,SeatNumber,Status")] Seat seat)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _db.Seats.Add(seat);
        //        _db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }

        //    return View(seat);
        //}

        //// GET: Seats/Edit/5
        //public ActionResult Edit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Seat seat = _db.Seats.Find(id);
        //    if (seat == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(seat);
        //}

        //// POST: Seats/Edit/5
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Edit([Bind(Include = "SeatID,SeatNumber,Status")] Seat seat)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _db.Entry(seat).State = EntityState.Modified;
        //        _db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }
        //    return View(seat);
        //}

        //// GET: Seats/Delete/5
        //public ActionResult Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Seat seat = _db.Seats.Find(id);
        //    if (seat == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(seat);
        //}

        //// POST: Seats/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(int id)
        //{
        //    Seat seat = _db.Seats.Single(s => s.SeatId == id);
        //    if (seat != null)
        //    {
        //        _db.Seats.Remove(seat);
        //    }
        //    _db.SaveChanges();
        //    return RedirectToAction("Index");
        //}

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
