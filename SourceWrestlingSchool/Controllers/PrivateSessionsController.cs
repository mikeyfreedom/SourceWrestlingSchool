using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using SourceWrestlingSchool.Models;

namespace SourceWrestlingSchool.Controllers
{
    /// <summary>
    ///     Controller to handle all database actions dealing with Private Session. These sessions are created by a session request, before being converted to lessons on acceptance.
    /// </summary>
    public class PrivateSessionsController : Controller
    {
        /// <summary>
        ///     Virtual representation of the database.
        /// </summary>
        private readonly ApplicationDbContext _db = new ApplicationDbContext();

        /// <summary>
        ///     Display a list of all Private Sessions in the database
        /// </summary>
        /// <returns>The Index view, displaying all private sessions in a list</returns>
        // GET: PrivateSessions
        public ActionResult Index()
        {
            return View(_db.PrivateSessions.Include(p => p.User).ToList());
        }

        /// <summary>
        ///     Show the details of a particular session.
        /// </summary>
        /// <remarks>
        ///     If no id is sent, display a bad request error page.
        ///     Else
        ///         Get the session from the database with a matching id.
        ///         Attach the user entity associated with the session entity.
        ///         Send the sesson as the model to populate the Detail view.
        /// </remarks>
        /// <param name="id">The id of the session entity</param>
        /// <returns>The Session Detail View, with a PrivateSession parameter</returns>
        // GET: PrivateSessions/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PrivateSession privateSession = _db.PrivateSessions.Find(id);
            if (privateSession == null)
            {
                return HttpNotFound();
            }
            return View(privateSession);
        }

        // COMMENTED OUT AS AN ADMIN WOULD NEVER CREATE A SESSION, THESE ARE USER-CREATED ONLY.
        //// GET: PrivateSessions/Create
        //public ActionResult Create()
        //{
        //    return View();
        //}

        //// POST: PrivateSessions/Create
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Create([Bind(Include = "PrivateSessionID,SessionStart,SessionEnd,InstructorID,Notes")] PrivateSession privateSession)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _db.PrivateSessions.Add(privateSession);
        //        _db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }

        //    return View(privateSession);
        //}

        /// <summary>
        ///     Load the Form and Model to Edit a Session
        /// </summary>
        /// <remarks>
        ///     Retrieve the session from the database with a matching id
        ///     Load the Edit View
        /// </remarks>
        /// <param name="id">The id of the session entity to be edited</param>
        /// <returns>The edit session form with the session to be edited as the sending model</returns>
        // GET: PrivateSessions/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PrivateSession privateSession = _db.PrivateSessions.Find(id);
            if (privateSession == null)
            {
                return HttpNotFound();
            }
            return View(privateSession);
        }

        /// <summary>
        ///     Use the data from the edit form to make changes to a PrivateSession
        /// </summary>
        /// <remarks>
        ///     Take in the binded model from the form view
        ///     Check the model is valid
        ///     If it is valid
        ///         Save the edited session to the database
        ///         Save the changes to the database
        ///         Load the index view with the new session information shown
        ///     If not valid
        ///         Refresh the edit session view for further editing
        /// </remarks>
        /// <param name="privateSession">The edited session info to be saved</param>
        /// <returns>
        ///     Returns the edit session view if the model or attached file is not valid
        ///     Returns the PrivateSession index view if all criteria are met.
        /// </returns>
        // POST: PrivateSessions/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "PrivateSessionID,SessionStart,SessionEnd,InstructorID,Notes")] PrivateSession privateSession)
        {
            if (ModelState.IsValid)
            {
                _db.Entry(privateSession).State = EntityState.Modified;
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(privateSession);
        }

        /// <summary>
        ///     Load the Delete Session Form
        /// </summary>
        /// <remarks>
        ///     Retrieve the session from the database with a matching id
        ///     Load the Delete Session View
        /// </remarks>
        /// <param name="id">The id of the session to be deleted</param>
        /// <returns>The Delete View with the matching session as a parameter</returns>
        // GET: PrivateSessions/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PrivateSession privateSession = _db.PrivateSessions.Find(id);
            if (privateSession == null)
            {
                return HttpNotFound();
            }
            return View(privateSession);
        }

        /// <summary>
        ///     Remove a PrivateSession
        /// </summary>
        /// <remarks>
        ///     Retrieve the session from the database with a matching id
        ///     Remove the session entity from the database
        ///     Save the change to the database
        ///     Load the Index view with the new information
        /// </remarks>
        /// <param name="id">The id of the session to be deleted</param>
        /// <returns>The updated PrivateSession Index View</returns>
        // POST: PrivateSessions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            PrivateSession privateSession = _db.PrivateSessions.Find(id);
            if (privateSession != null)
            {
                _db.PrivateSessions.Remove(privateSession);
            }
            _db.SaveChanges();
            return RedirectToAction("Index");
        }

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