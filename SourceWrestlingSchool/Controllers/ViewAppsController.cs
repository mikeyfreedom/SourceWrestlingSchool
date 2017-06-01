using System;
using SourceWrestlingSchool.Models;
using System.Data.Entity;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace SourceWrestlingSchool.Controllers
{
    /// <summary>
    ///     Controller handling the actions involved in reviewing school applications.
    /// </summary>
    [Authorize(Roles = "Instructor")]
    public class ViewAppsController : Controller
    {
        /// <summary>
        ///     Virtual representation of the database
        /// </summary>
        private readonly ApplicationDbContext _db = new ApplicationDbContext();

        /// <summary>
        ///     Display all applications to the school in a list.
        /// </summary>
        /// <returns>A list of school applications as a model for the Index View</returns>
        // GET: ViewApps
        public ActionResult Index()
        {
            return View(_db.Applications.Include(x => x.User).ToList());
        }

        /// <summary>
        ///     Show the details of a particular application.
        /// </summary>
        /// <remarks>
        ///     If no id is sent, display a bad request error page.
        ///     Else
        ///         Get the application from the database with a matching id.
        ///         Attach the user associated with the application.
        ///         Send the application as the model to populate the Detail view.
        /// </remarks>
        /// <param name="id">The id of the application record in the database</param>
        /// <returns>The Application Detail View, with an Application parameter</returns>
        // GET: ViewApps/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ApplyViewModel application = _db.Applications.Where(a => a.ApplicationId == id).Include(a => a.User).Single(); 
            Image source = Image.FromFile(Server.MapPath("/images/") + application.FileName);
            ImageFormat format = source.RawFormat;
            source.Dispose();
            ImageHelper.RotateImageByExifOrientationData(Server.MapPath("/images/") + application.FileName, Server.MapPath("/images/") + application.FileName,format);
            
            return View(application);
        }

        /// <summary>
        ///     Accept an application and promote the user to the Studenr role.
        /// </summary>
        /// <remarks>
        ///     Use the pulled-in application ID to retrieve its details from the database.
        ///     Set the application status to Accepted.
        ///     Retrieve the user associated with the application.
        ///     Add the application details to the user's attributes
        ///     Set the user's ClassLevel to Beginner.
        ///     Change the user's role from Standard to Student.
        ///     Create a new payment to be settled and add it to the database.
        ///     Send a notification email to the user via the SendEmail method.
        ///     Save the changes to the database.
        ///     Load the Index View, showing the new status of the application.
        /// </remarks>
        /// <returns>The index view with the reflected database changes</returns>
        [HttpPost]
        public ActionResult Accept()
        {
            string aId = Request.Form["appID"];
            string userMail;
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(_db));
            int appId = int.Parse(aId);
            using (_db)
            {
                var app = _db.Applications
                            .Where(a => a.ApplicationId == appId)
                            .Include(a => a.User)
                            .Single();
                app.User.Age = app.Age;
                app.User.Height = app.Height;
                app.User.Weight = app.Weight;
                app.User.ClassLevel = ClassLevel.Beginner;
                app.Status = ApplyViewModel.ApplicationStatus.Accepted;
                userManager.RemoveFromRole(app.User.Id, RoleNames.ROLE_STANDARDUSER);
                userManager.AddToRole(app.User.Id, RoleNames.ROLE_STUDENTUSER);
                userMail = "" + app.User.Email;
                _db.SaveChanges();
            }

            SendEmail(userMail,"accept");
            return RedirectToAction("Index");
        }

        /// <summary>
        ///     Refuse an application and change its status in the database.
        /// </summary>
        /// <remarks>
        ///     Use the pulled-in application ID to retrieve its details from the database.
        ///     Set the session status to Refused.
        ///     Send a notification email to the user via the SendEmail method.
        ///     Save the changes to the database.
        ///     Load the Index View, showing the new status of the application.
        /// </remarks>
        /// <returns>The index view with the reflected database changes</returns>
        [HttpPost]
        public ActionResult Refuse()
        {
            string aId = Request.Form["appID"];
            string userMail;
            int appId = int.Parse(aId);
            using (_db)
            {
                var app = _db.Applications
                            .Where(a => a.ApplicationId == appId)
                            .Include(a => a.User)
                            .Single(); 
                app.Status = ApplyViewModel.ApplicationStatus.Declined;
                userMail = "" + app.User.Email;
                _db.SaveChanges();
            }
                
            SendEmail("refuse",userMail);
            return RedirectToAction("Index");
        }

        /// <summary>
        ///     Send a notification by email to a user regarding their session request.
        /// </summary>
        /// <remarks>
        ///     Create a new mail message objects
        ///     Populate it with the acceptance or rejection email subject/body
        ///     Instantiate the SmtpClient used to send email
        ///     Send the message
        /// </remarks>
        /// <param name="userMail">The email address of the user to be emailed.</param>
        /// <param name="reason">The reason string to denote which email body to send.</param>
        public void SendEmail(string userMail, string reason)
        {
            MailMessage message = new MailMessage("lowlander_glen@yahoo.co.uk", userMail);
            if (reason.Equals("accept"))
            {
                message.Subject = "Membership Approved";
                message.Body = "Congratulations, your membership has been approved. Welcome to the school!";

            }
            else if (reason.Equals("refuse"))
            {
                message.Subject = "Membership Refused";
                message.Body = "Your application has been received, but unfortunately you have been turned down for student membership";
            }

            SmtpClient smtpClient = new SmtpClient();
            try
            {
                smtpClient.Send(message);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }
    }
}