using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using SourceWrestlingSchool.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity;

namespace SourceWrestlingSchool.Controllers
{
    /// <summary>
    ///     Controller that handling of training class objects
    /// </summary>
    public class LessonsController : Controller
    {
        /// <summary>
        ///     Virtual representation of the database
        /// </summary>
        private readonly ApplicationDbContext _db = new ApplicationDbContext();

        /// <summary>
        ///     Get all lessons listed in the database.
        ///     Attach the students who have booked in for each lesson.
        ///     Add all lessons to a list.
        ///     Send the list as a ViewModel to populate the Index View.
        /// </summary>
        /// <returns>The index view, along with the list of attached lessons</returns>
        // GET: Lessons
        public ActionResult Index()
        {
            var model = _db.Lessons.Include(l => l.Students).ToList();
            return View(model);
        }

        /// <summary>
        ///     Show the details of a particular lesson.
        /// </summary>
        /// <remarks>
        ///     If no id is sent, display a bad request error page
        ///     Else
        ///         Get the lesson from the database with a matching id
        ///         Send the lesson as the model to populate the Detail view
        /// </remarks>
        /// <param name="id">The id of the lesson entity</param>
        /// <returns>The Lesson Detail View, with a Lesson parameter</returns>
        // GET: Lessons/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Lesson lesson = _db.Lessons
                            .Where(l => l.LessonId == id)
                            .Include(l => l.Students)
                            .Single();
            if (lesson == null)
            {
                return HttpNotFound();
            }
            return View(lesson);
        }

        /// <summary>
        ///     Load the form to create a new lesson.
        /// </summary>
        /// <remarks>
        ///     Call the GetInstructors method to retrieve a list of instructor users
        ///     Add the return list to a ViewBag for SelectList use
        ///     Load the Create form
        /// </remarks>
        /// <returns>The create new lesson form view</returns>
        // GET: Lessons/Create
        public ActionResult Create()
        {
            ViewBag.Instructors = GetInstructors();
            return View();
        }

        /// <summary>
        ///     Use the create lesson form data to create a new lesson 
        /// </summary>
        /// <remarks>
        ///     Take in the binded model from the form view
        ///     Check the model is valid
        ///     If it is valid
        ///         Save the new lesson to the database
        ///         Save the changes to the database
        ///         Load the index view with the new lesson added to the list
        ///     If not valid
        ///         Refresh the create lesson view for editing
        /// </remarks>
        /// <param name="lesson">The lesson model to be saved to the database</param>
        /// <returns>
        ///     Returns the create lesson view if the model or attached file is no valid
        ///     Returns the Lesson index view if all criteria are met.
        /// </returns>
        // POST: Lessons/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "LessonID,ClassType,ClassLevel,ClassStartDate,ClassEndDate,ClassCost,InstructorName")] Lesson lesson)
        {
            if (ModelState.IsValid)
            {
                _db.Lessons.Add(lesson);
                _db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(lesson);
        }

        /// <summary>
        ///     Load the Form and Model to Edit a Lesson
        /// </summary>
        /// <remarks>
        ///     Retrieve the lesson from the database with a matching id
        ///     Call the GetInstructors method to get a list of instructors for the SelectList
        ///     Load the Edit View
        /// </remarks>
        /// <param name="id">The id of the lesson entity to be edited</param>
        /// <returns>The edit lesson form with the instructor list as a parameter</returns>
        // GET: Lessons/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Lesson lesson = _db.Lessons
                            .Where(l => l.LessonId == id)
                            .Include(l => l.Students)
                            .Single(); 
            if (lesson == null)
            {
                return HttpNotFound();
            }
            
            ViewBag.Instructors = GetInstructors();
            return View(lesson);
        }

        /// <summary>
        ///     Use the data from the edit form to make changes to a Lesson
        /// </summary>
        /// <remarks>
        ///     Take in the binded model from the form view
        ///     Check the model is valid
        ///     If it is valid
        ///         Save the edited lesson to the database
        ///         Save the changes to the database
        ///         Load the index view with the new lesson information shown
        ///     If not valid
        ///         Refresh the edit lesson view for further editing
        /// </remarks>
        /// <param name="lesson">The edited lesson info to be saved</param>
        /// <returns>
        ///     Returns the edit lesson view if the model or attached file is not valid
        ///     Returns the Lesson index view if all criteria are met.
        /// </returns>
        // POST: Lessons/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "LessonID,ClassType,ClassLevel,ClassStartDate,ClassEndDate,ClassCost,InstructorName")] Lesson lesson)
        {
            if (ModelState.IsValid)
            {
                _db.Entry(lesson).State = EntityState.Modified;
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(lesson);
        }

        /// <summary>
        ///     Load the Delete Lesson Form
        /// </summary>
        /// <remarks>
        ///     Retrieve the lesson from the database with a matching id
        ///     Load the Delete Lesson View
        /// </remarks>
        /// <param name="id">The id of the lesson to be deleted</param>
        /// <returns>The Delete View with the matching lesson parameter</returns>
        // GET: Lessons/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Lesson lesson = _db.Lessons
                            .Where(l => l.LessonId == id)
                            .Include(l => l.Students)
                            .Single();
            if (lesson == null)
            {
                return HttpNotFound();
            }
            return View(lesson);
        }

        /// <summary>
        ///     Remove a lesson
        /// </summary>
        /// <remarks>
        ///     Retrieve the lesson from the database with a matching id
        ///     Remove the lesson entity from the database
        ///     Save the change to the database
        ///     Load the Index view with the new information
        /// </remarks>
        /// <param name="id">The id of the lesson to be deleted</param>
        /// <returns>The updated Lesson Index View</returns>
        // POST: Lessons/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Lesson lesson = _db.Lessons
                            .Where(l => l.LessonId == id)
                            .Include(l => l.Students)
                            .Single();
            _db.Lessons.Remove(lesson);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }

        /// <summary>
        ///     Retrieves a list of all users in the instructor role
        /// </summary>
        /// <remarks>
        ///     Instantiate the UserManager and RoleManager.
        ///     Use the RoleManager to get all user ids attached to the instructor role.
        ///     Use the UserManager to get all the users by their ids in the instructor role.
        ///     Create a list of the instructor users and return it.
        /// </remarks>
        /// <returns>A list of users who are in the instructor role</returns>
        public List<ApplicationUser> GetInstructors()
        {
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(_db));
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(_db));
            var instructorRole = roleManager.Roles.Single(r => r.Name == RoleNames.ROLE_INSTRUCTOR);
            List<ApplicationUser> instructors = new List<ApplicationUser>();
            foreach (var user in instructorRole.Users)
            {
                instructors.Add(userManager.FindById(user.UserId));
            }
            return instructors;
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
