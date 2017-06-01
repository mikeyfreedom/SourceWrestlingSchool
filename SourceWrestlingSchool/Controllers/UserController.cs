using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using SourceWrestlingSchool.Models;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using System.Web.UI;

namespace SourceWrestlingSchool.Controllers
{
    /// <summary>
    ///     Controller to handle the view actions of the user base
    /// </summary>
    public class UserController : Controller
    {
        /// <summary>
        ///     Virtual representation of the database
        /// </summary>
        private readonly ApplicationDbContext _db = new ApplicationDbContext();

        /// <summary>
        ///     Display all users on the system
        /// </summary>
        /// <remarks>
        ///     Get all users from the database
        ///     Get all roles from the database
        ///     Create a Viewmodel to hold the users and roles
        ///     Send it to the Index View
        /// </remarks>
        /// <returns>The Index View which shows all users and the role they have</returns>
        // GET: User
        [Authorize(Roles = "Administrator")]
        public ActionResult Index()
        {
            var users = _db.Users.ToList();
            var roles = _db.Roles.ToList();
            UserViewModel userView = new UserViewModel
            {
                Users = users,
                Roles = roles
            };
            return View(userView);
        }

        /// <summary>
        ///     Display a list of all students on the roster, with an option to set a user's class level
        /// </summary>
        /// <remarks>
        ///     Get all students on the database via the GetStudents routine.
        ///     Send the student list to the View.
        /// </remarks>
        /// <returns></returns>
        [Authorize(Roles="Instructor")]
        public ActionResult AdjustClassLevel()
        {
            return View(GetStudents());
        }

        /// <summary>
        ///     Export the student roster to an excel file
        /// </summary>
        /// <remarks>
        ///     Retrieve all students from the database using the GetStudents routine.
        ///     Instantiate the GridView that holds the student data.
        ///     Create the file to be downloaded.
        ///     Instatiate a StringWriter to deal with the data.
        ///     Write the data to the file via the StringWriter.
        ///     End the file write and send it to the user.
        /// </remarks>
        public void ExportStudentListToExcel()
        {
            var students = GetStudents();
            var grid = new System.Web.UI.WebControls.GridView();
            var studentQuery = from s in students
                               select new { s.FirstName, s.LastName, s.ClassLevel, s.Age, s.Email, s.DateJoinedSchool };
            grid.DataSource = studentQuery.ToList();                              
            grid.DataBind();

            Response.ClearContent();
            Response.AddHeader("content-disposition", "attachment; filename=StudentRoster.xls");
            Response.ContentType = "application/excel";
            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);

            grid.RenderControl(htw);

            Response.Write(sw.ToString());
            Response.End();
        }

        /// <summary>
        ///     Use the Rotativa HTMLtoPDF library to call the studentPdf page and output it as a downloadable Pdf
        /// </summary>
        /// <returns>A Pdf file names StudentRoster, containing all students on the roster</returns>
        public ActionResult ExportStudentListToPdf()
        {
            return new Rotativa.ViewAsPdf("StudentPdf",GetStudents()){ FileName = "StudentRoster.pdf" };
        }
        
        /// <summary>
        ///     View used as a basis for the Pdf format of the student rosetr
        /// </summary>
        /// <returns>A view containing the result of the GetStudents routine</returns>
        public ActionResult StudentPdf()
        {
            return View(GetStudents());
        }

        /// <summary>
        ///     Edit the elgible class level of a student
        /// </summary>
        /// <remarks>
        ///     Take in the userID and ClassLevel data from the button included on the View.
        ///     Get the student entity from the database with a matching id.
        ///     Set the user's class level to the new class level.
        ///     Save the changes to the database.
        /// </remarks>
        /// <param name="collection">The id of the user to be edited, and the classLevel to be edited to</param>
        /// <returns>The student index view, reflecting rhe new information.</returns>
        [HttpPost]
        public ActionResult EditClass(FormCollection collection)
        {
            string uId = collection["userId"];
            string level = collection["item.ClassLevel"];
            
            using (_db)
            {
                var user = _db.Users.Single(u => u.Id.Equals(uId));
                user.ClassLevel = (ClassLevel) int.Parse(level);
                _db.SaveChanges();
            }

            return RedirectToAction("AdjustClassLevel");
        }

        /// <summary>
        ///     Get a list of all students on the system
        /// </summary>
        /// <remarks>
        ///     Retrieve all the users on the database.
        ///     Instantiate the UserManager.
        ///     Check through all users and create a list of users that are in the Student Role.
        /// </remarks>
        /// <returns>A list of all users in the Student Role</returns>
        public List<ApplicationUser> GetStudents()
        {
            var users = _db.Users.ToList();
            var students = new List<ApplicationUser>();
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(_db));
            foreach (ApplicationUser user in users)
            {
                if (userManager.IsInRole(user.Id, RoleNames.ROLE_STUDENTUSER))
                {
                    students.Add(user);
                }
            }

            return students;
        }
    }
}