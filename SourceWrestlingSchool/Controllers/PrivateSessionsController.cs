using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using SourceWrestlingSchool.Models;

namespace SourceWrestlingSchool.Controllers
{
    public class PrivateSessionsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: PrivateSessions
        public ActionResult Index()
        {
            return View(db.PrivateSessions.ToList());
        }

        // GET: PrivateSessions/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PrivateSession privateSession = db.PrivateSessions.Find(id);
            if (privateSession == null)
            {
                return HttpNotFound();
            }
            return View(privateSession);
        }

        // GET: PrivateSessions/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: PrivateSessions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "PrivateSessionID,SessionStart,SessionEnd,InstructorID,Notes")] PrivateSession privateSession)
        {
            if (ModelState.IsValid)
            {
                db.PrivateSessions.Add(privateSession);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(privateSession);
        }

        // GET: PrivateSessions/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PrivateSession privateSession = db.PrivateSessions.Find(id);
            if (privateSession == null)
            {
                return HttpNotFound();
            }
            return View(privateSession);
        }

        // POST: PrivateSessions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "PrivateSessionID,SessionStart,SessionEnd,InstructorID,Notes")] PrivateSession privateSession)
        {
            if (ModelState.IsValid)
            {
                db.Entry(privateSession).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(privateSession);
        }

        // GET: PrivateSessions/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PrivateSession privateSession = db.PrivateSessions.Find(id);
            if (privateSession == null)
            {
                return HttpNotFound();
            }
            return View(privateSession);
        }

        // POST: PrivateSessions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            PrivateSession privateSession = db.PrivateSessions.Find(id);
            if (privateSession != null)
            {
                db.PrivateSessions.Remove(privateSession);
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
