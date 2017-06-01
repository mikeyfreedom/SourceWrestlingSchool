using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using SourceWrestlingSchool.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Net.Mail;
using System.Web.Mvc;

namespace SourceWrestlingSchool.Controllers
{
    /// <summary>
    ///     Controller which handles all actions regarding private session requests.
    /// </summary>
    public class PrivateRequestsController : Controller
    {
        /// <summary>
        ///     Virtual representation of the database.
        /// </summary>
        private readonly ApplicationDbContext _db = new ApplicationDbContext();

        /// <summary>
        ///     Display a list of all private session requests for the current instructor user.
        /// </summary>
        /// <remarks>
        ///     Instantiate the UserManager to get the current user's ID.
        ///     Find all private session request that have the instructor's ID attached to it.
        ///     Send a list of those requests to the Index list view.
        /// </remarks>
        /// <returns>The Index view that displays a list of private session requests.</returns>
        // GET: PrivateRequests
        public ActionResult Index()
        {
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(_db));
            var userId = userManager.FindByEmail(User.Identity.Name).Id;
            var model = _db.PrivateSessions.Where(i => i.InstructorId == userId).Include(i => i.User);

            return View(model.ToList());
        }

        /// <summary>
        ///     Accept a request and add it to the lesson calendar.
        /// </summary>
        /// <remarks>
        ///     Use the pulled-in session ID to retrieve its details from the database.
        ///     Set the session status to Accepted.
        ///     Create a new lesson entity in the database, using the data of the session request.
        ///     Add the student who requested it to the booked in list.
        ///     Add the lesson to the database.
        ///     Create a new payment to be settled and add it to the database.
        ///     Send a notification email to the user via the SendEmail method.
        ///     Save the changes to the database.
        ///     Load the Index View, showing the new information for the request.
        /// </remarks>
        /// <returns>The index view with the reflected database changes</returns>
        [HttpPost]
        public ActionResult Accept()
        {
            //Get the ID of the session request that was selected
            string sId = Request.Form.Get("sessionID");
            int sessionId = int.Parse(sId);
            
            using (_db)
            {
                //Retrieve the session object
                var session = _db.PrivateSessions
                              .Where(p => p.PrivateSessionId == sessionId)
                              .Include(u => u.User)
                              .Single();
                //Update its status if found
                if (session != null)
                {
                    session.Status = PrivateSession.RequestStatus.Accepted;
                    //Create a new lesson based on the session requests details
                    Lesson lesson = new Lesson
                    {
                        ClassType = Lesson.LessonType.Private,
                        ClassStartDate = session.SessionStart,
                        ClassEndDate = session.SessionEnd,
                        ClassCost = 30,
                        InstructorName = (from i in _db.Users
                            where i.Id == session.InstructorId
                            select i.FirstName).FirstOrDefault(),
                        Students = new List<ApplicationUser>(),
                        ClassLevel = ClassLevel.Private
                    };

                    //Save the changes to lessons and privates
                    if (session.User != null)
                    {
                        //Add the student to the new lesson
                        lesson.Students.Add(session.User);
                        //Add the new lesson to the db
                        _db.Lessons.Add(lesson);
                        Payment privateFee = new Payment
                        {
                            PaymentAmount = 30,
                            PaymentDescription = "Private Session Booking Fee",
                            PaymentDate = DateTime.Today,
                            PaymentSettled = false,
                            User = session.User,
                            UserId = session.User.Id
                        };
                        _db.Payments.Add(privateFee);
                        SendEmail(session.User.Email, "accept");
                    }
                    _db.SaveChanges();
                }
            }
            return RedirectToAction("Index","PrivateRequests");
        }

        /// <summary>
        ///     Refuse a request and change its status in the database.
        /// </summary>
        /// <remarks>
        ///     Use the pulled-in session ID to retrieve its details from the database.
        ///     Set the session status to Refused.
        ///     Send a notification email to the user via the SendEmail method.
        ///     Save the changes to the database.
        ///     Load the Index View, showing the new information for the request.
        /// </remarks>
        /// <returns>The index view with the reflected database changes</returns>
        [HttpPost]
        public ActionResult Refuse()
        {
            int sessionId = int.Parse(Request.Form.Get("sessionID"));
            using (_db)
            {
                var request = (from p in _db.PrivateSessions
                               where p.PrivateSessionId == sessionId
                               select p).FirstOrDefault();
                if (request != null)
                {
                    request.Status = PrivateSession.RequestStatus.Refused;
                    _db.SaveChanges();
                    SendEmail(request.User.Email, "refuse");
                }
            }
            return RedirectToAction("Index", "PrivateRequests");
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
        public void SendEmail(string userMail,string reason)
        {
            MailMessage message = new MailMessage("lowlander_glen@yahoo.co.uk", userMail);
            if (reason.Equals("accept"))
            {
                message.Subject = "Private Session Request Accepted";
                message.Body = "You have successfully booked in for a class. We look forward to seeing you there!";
            }
            else if (reason.Equals("refuse"))
            {
                message.Subject = "Private Session Request Refused";
                message.Body = "Your requested instructor has refused the session.";
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