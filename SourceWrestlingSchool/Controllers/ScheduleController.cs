using DayPilot.Web.Mvc;
using DayPilot.Web.Mvc.Enums;
using DayPilot.Web.Mvc.Events.Calendar;
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
    ///     Controller that handles all of the booking actions
    /// </summary>
    public class ScheduleController : Controller
    {
        /// <summary>Holds a value representing the id of the currently selected Calendar event.</summary>
        public static int EventId;
        /// <summary>Holds a value representing the email address of the current user.</summary>
        public static string Email;
        /// <summary>Holds a value representing an errormessage generated from a schedule method.</summary>
        public static string Errormessage = " ";
        /// <summary>Holds a value representing an active private session request.</summary>
        public static PrivateSession request;
        /// <summary>A virtual representation of the database.</summary>
        private readonly ApplicationDbContext _db = new ApplicationDbContext();
        /// <summary>Holds a value representing the currently logged on user.</summary>
        private ApplicationUser _currentUser;
        
        /// <summary>
        ///     Loads the Index view, along with any error messages passed to the controller.
        /// </summary>
        /// <returns>The Index view, along with the current error message flag</returns>
        // GET: Schedule
        public ActionResult Index()
        {
            var model = Errormessage;
            Errormessage = "";
            return View(model:model);
        }
        
        /// <summary>
        ///     Callback action that deals with all calendar requests.
        /// </summary>
        /// <returns>A call to the DayPilot calendar class listed further down</returns>
        public ActionResult Backend()
        {
            return new Dpc().CallBack(this);
        }

        /// <summary>
        ///     Loads the Class Booking View when a valid calendar event is clicked on.
        /// </summary>
        /// <remarks>
        ///     Take in the Event Id of the clicked calendar event.
        ///     Get the lesson details from the database with the matching Id.
        ///     Load the Booking View with the lesson as a sending model.
        /// </remarks>
        /// <returns>Booking View alongside a Lesson entity.</returns>
        // GET: Booking
        public ActionResult Booking()
        {
            int id = EventId;
            var model = _db.Lessons.Where(l => l.LessonId == id).Include(l => l.Students).Single();
            
            return View(model);
        }

        /// <summary>
        ///     Load the View to request a private session, after user selects an empty time slot
        /// </summary>
        /// <remarks>
        ///     Take in the private session details created by the time slot request.
        ///     Save the current user details from the database.
        ///     Instantiate the UserManager and RoleManager.
        ///     Use the RoleManager to get all user ids attached to the instructor role.
        ///     Use the UserManager to get all the users by their ids in the instructor role.
        ///     Create a list of the instructor users and retain it in a ViewBag.
        /// </remarks>
        /// <returns> RequestPrivate form View along with a PrivateSession entity. </returns>
        public ActionResult RequestPrivate()
        {
            var model = request;
            _currentUser = _db.Users.Single(n => n.UserName == User.Identity.Name);
            model.User = _currentUser;

            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(_db));
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(_db));
            var instructorRole = roleManager.Roles.Single(r => r.Name == RoleNames.ROLE_INSTRUCTOR);
            List<ApplicationUser> instructors = new List<ApplicationUser>();
            foreach (var user in instructorRole.Users)
            {
                instructors.Add(userManager.FindById(user.UserId));
            }
            ViewBag.Instructors = instructors;
            
            return View(model);
        }

        /// <summary>
        ///     Complete the form action and submit a request for a private session.
        /// </summary>
        /// <remarks>
        ///     Pull in the binding model using the form data from the sending view
        ///     Set the session status to submitted.
        ///     Check the model is valid.
        ///     If it is valid
        ///         Add the new PrivateSession entity to the database.
        ///         Save the changes to the DB.
        ///         Call SendMail to send a confirmation to the user the request was received.
        ///         Load the Calendar Index view with a positive notification message.
        ///     Else
        ///         Refresh the form page view with the collected data.
        /// </remarks>
        /// <param name="model">The details for the PrivateSession entity To be added to the database</param>
        /// <returns>Redirect Action, leading the user back to the Schedule Index calendar view</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RequestPrivate([Bind(Include = "StudentName,SessionStart,SessionEnd,InstructorID,Notes")] PrivateSession model)
        {
            model.Status = PrivateSession.RequestStatus.Submitted;
            model.User = _db.Users.Single(u => u.UserName == User.Identity.Name);
            if (ModelState.IsValid)
            {
                _db.PrivateSessions.Add(model);
                _db.SaveChanges();
                SendEmail(User.Identity.Name,"private");
                Errormessage = "Private";
                return RedirectToAction("Index");
            }

            return View(model);
        }
        
        /// <summary>
        ///     Books a user into a scheduled lesson
        /// </summary>
        /// <remarks>
        ///     Get the user's details from the database.
        ///     Get the lesson ID from the sending data.
        ///     Get the lesson details from the database and attach the user as a booked student.
        ///     Save the changes to the database.
        ///     Send a notification email to the user.
        ///     Send the user back to the index View.
        /// </remarks>
        /// <returns>Redirects the user back to the index view with a postive notification.</returns>
        [HttpPost]
        public ActionResult BookUser()
        {
            //Gather required variables
            var user = (from u in _db.Users
                        where u.Email == User.Identity.Name
                        select u).First();
            _currentUser = user;
            int classId = int.Parse(Request.Form["classID"]);
            
            using (_db)
            {
                var editedLesson = _db.Lessons.Single(s => s.LessonId == classId);
                _db.Lessons.Attach(editedLesson);

                var editedUser = _currentUser;
                _db.Users.Attach(editedUser);

                editedLesson.Students.Add(editedUser);
                
                _db.SaveChanges();
            }
            Errormessage = "BookSuccess";
            SendEmail(_currentUser.UserName, "book");

            return RedirectToAction("Index","Schedule");
        }

        /// <summary>
        ///     Cancel a previously-made class booking, possibly incurring a fine.
        /// </summary>
        /// <remarks>
        ///     Get the user's details from the database.
        ///     Get the lesson ID from the sending data, and retrieve the lesson details from the database.
        ///     Remove the user from the list of booked students.
        ///     Check the time difference between the cancellation time and the start time of the class
        ///     If the time difference is less than 12 hours
        ///         Create a new payment which is 15% of the class cost
        ///         Label it as "Class Cancellation Fine" for historical purposes.
        ///         Add the payment to the database.
        ///     Save the changes to the database.
        ///     Send a notification email to the user using SendMail to acknowledge the cancellation.
        /// </remarks>
        /// <returns>Loads the Index view with a successful cancellation notification.</returns>
        [HttpPost]
        public ActionResult CancelUser()
        {
            using (_db)
            {
                ApplicationUser user = _db.Users.First(i => i.UserName == User.Identity.Name);
                int classId = int.Parse(Request.Form["classID"]);
                Lesson lesson = _db.Lessons
                                .Where(l => l.LessonId == classId)
                                .Include(l => l.Students)
                                .Single();
                lesson.Students.Remove(user);
                //Check if a fine needs to be levied
                TimeSpan difference = lesson.ClassStartDate - DateTime.Now;
                if(difference.TotalHours < 12)
                {
                    Errormessage = "CancelSuccessFine";
                    double cancelfine = lesson.ClassCost * 0.15;
                    Payment fine = new Payment
                    {
                        PaymentAmount = Math.Round((decimal) cancelfine, 2),
                        PaymentDate = DateTime.Today,
                        PaymentDescription = "Class Cancellation Fine",
                        PaymentSettled = false,
                        User = user,
                        UserId = user.Id
                    };
                    _db.Payments.Add(fine);
                }
                else
                {
                    Errormessage = "CancelSuccess";
                }
                                
                _db.SaveChanges();
            }
            
            SendEmail(User.Identity.Name, "cancel");

            return RedirectToAction("Index", "Schedule"); 
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
        private void SendEmail(string userMail,string reason)
        {
            MailMessage message = new MailMessage("lowlander_glen@yahoo.co.uk", userMail);
            if (reason.Equals("book"))
            {
                message.Subject = "Booking Accepted";
                message.Body = "You have successfully booked in for a class. We look forward to seeing you there!";

            }
            else if (reason.Equals("cancel"))
            {
                message.Subject = "Cancellation Processed";
                message.Body = "You have cancelled your participation in the class.";
            }
            else if (reason.Equals("private"))
            {
                message.Subject = "Private Request Received";
                message.Body = "We have received your request for a private session. Your chosen instructor will respond as soon as possible";
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

        /// <summary>
        ///     Loads a filtered lesson list, only showing the logged-in instructor's upcoming classes.
        /// </summary>
        /// <returns>A list of lessons from the database, where the current user mathes the assigned instructor.</returns>
        [Authorize(Roles ="Instructor")]
        public ActionResult PersonalSchedule()
        {
            using (_db)
            {
                var model = _db.Lessons
                            .Where(ev => ev.InstructorName == User.Identity.Name)
                            .Where(ev => ev.ClassStartDate > DateTime.Now)
                            .Include(ev => ev.Students)
                            .ToList();

                return View(model);
            }
        }

        /// <summary>
        ///     Class that handles all actions and events associated with the DayPilot Calendar plugin.
        /// </summary>
        class Dpc : DayPilotCalendar
        {
            /// <summary>
            ///     Virtual representation of the database.
            /// </summary>
            private readonly ApplicationDbContext _db = new ApplicationDbContext();

            ///<summary>
            ///     On load, update the calendar.     
            ///</summary>             
            protected override void OnInit(InitArgs e)
            {
                Update();
            }

            /// <summary>
            ///     Change the current viewed week of classes
            /// </summary>
            /// <remarks>
            ///     Check the incoming command data for its text string
            ///     If previous, change the calendar view to the previous week
            ///     If next, move the calendar on to the next week         
            /// </remarks>
            /// <param name="e">The data string gained from a calendar time control link</param>
            protected override void OnCommand(CommandArgs e)
            {
                switch (e.Command)
                {
                    case "previous":
                        StartDate = StartDate.AddDays(-7);
                        Update(CallBackUpdateType.Full);
                        break;
                    case "next":
                        StartDate = StartDate.AddDays(7);
                        Update(CallBackUpdateType.Full);
                        break;
                }
            }

            /// <summary>
            ///     Method that sets the colours of the class events on the calendar.
            /// </summary>
            /// <remarks>
            ///     Set the font colour to black.
            ///     Check the current user's class level.
            ///     If a class level is equal to the user's level, colour the event red to denote available.
            ///     Else, colour the event gray to denote not available.
            ///     However, if the user's class level is set to Open, then colour each class to be displayed depending on their level.
            /// </remarks>
            /// <param name="e">The incoming event data</param>
            protected override void OnBeforeEventRender(BeforeEventRenderArgs e)
            {
                e.FontColor = "black";

                string userLevel = _db.Users.Single(u => u.UserName
                    .Equals(System.Web.HttpContext.Current.User.Identity.Name)).ClassLevel.ToString();

                string classlevel = e.DataItem["ClassLevel"].ToString();
                if (!userLevel.Equals("Open"))
                {
                    e.BackgroundColor = classlevel.Equals(userLevel) ? "red" : "grey";
                }
                else
                {
                    if (classlevel.Equals("Beginner"))
                        e.BackgroundColor = "red";
                    else if (classlevel.Equals("Intermediate"))
                        e.BackgroundColor = "yellow";
                    else if (classlevel.Equals("Advanced"))
                        e.BackgroundColor = "green";
                    else if (classlevel.Equals("Womens"))
                        e.BackgroundColor = "pink";
                    else if (classlevel.Equals("Private"))
                    {
                        e.BackgroundColor = "black";
                        e.FontColor = "white";
                    }
                }
                    
                base.OnBeforeEventRender(e);
            }

            /// <summary>
            ///     Get all class events to be listed on the calendar and display them
            /// </summary>
            /// <remarks>
            ///     If there has been no update request made, then just refresh the calendar view
            ///     If there is an update request,
            ///     Get all lessons from the database, and bind their attributues to the calendar attributes
            /// </remarks>
            protected override void OnFinish()
            {
                if (UpdateType == CallBackUpdateType.None)
                {
                    return;
                }

                //string username = System.Web.HttpContext.Current.User.Identity.Name;
                //ApplicationUser user = db.Users.First(i => i.Email == username);
                //ClassLevel level = (ClassLevel) user.ClassLevel;
                Events = from ev in _db.Lessons
                         select ev;
                //if (level == ClassLevel.Open)
                //    Events = from ev in db.Lessons
                //             select ev;
                //else
                //    Events = from ev in db.Lessons
                //             where ev.ClassLevel == level || ev.ClassLevel == ClassLevel.Private
                //             select ev ;

                DataIdField = "LessonID";
                DataTextField = "ClassLevel";
                DataStartField = "ClassStartDate";
                DataEndField = "ClassEndDate";                
            }

            /// <summary>
            ///     Handle a mouse click on a scheduled lesson.
            /// </summary>
            /// <remarks>
            ///     Check that the user's current ClassLevel matches the level of the lesson clicked.
            ///     If it does match, or the user level is Open.
            ///         Set the global eventId to the id of the clicked lesson.
            ///         Redirect the user to the book attendance View.
            ///     Else
            ///         Call the OnFinish with no update request. 
            /// </remarks>
            /// <param name="e">The dataset of the lesson that was clicked</param>
            protected override void OnEventClick(EventClickArgs e)
            {
                //Check e.Text against user classlevel
                string userLevel = _db.Users.Single(u => u.UserName.Equals(System.Web.HttpContext.Current.User.Identity.Name)).ClassLevel.ToString();
                if (userLevel.Equals(e.Text) || userLevel.Equals("Open"))
                {
                    //Parse the event ID for processing
                    EventId = int.Parse(e.Id);
                    //Redirect to the booking page
                    Redirect("/Schedule/Booking");
                }
            }

            /// <summary>
            ///     Check the clicked time slot is available for a private session booking.
            /// </summary>
            /// <remarks>
            ///     Get all lessons on the same date as the clicked time slots.
            ///     Check against all lessons to see if there an overlap for another class.
            ///     If so, refresh the calendar age with a failure notification saying the slot cannot be booked.
            ///     if no overlap exists, create a PrivateSession and redirect the user to the PrivateSession request view.
            /// </remarks>
            /// <param name="e">The dataset of the timeslots selected by the user.</param>
            protected override void OnTimeRangeSelected(TimeRangeSelectedArgs e)
            {
                //Set a LINQ query to get all lessons that take place the same day as the requested session
                //DateTime startDate = e.Start;

                var lessons = _db.Lessons
                              .Where(l => DbFunctions.TruncateTime(l.ClassStartDate) == e.Start.Date)
                              .ToList();
                              

                //If there is are any other classes that day, loop through them to check for a time conflict
                if (lessons.Count != 0)
                {
                    //Set a flag to denote if an overlap exists
                    bool overlapExists = false;

                    //Loop though all the day's classes and check if the start or end overlaps with the request time
                    foreach (var lesson in lessons)
                    {
                        bool startCondition = (e.Start.TimeOfDay > lesson.ClassStartDate.TimeOfDay) &&
                                              (e.Start.TimeOfDay < lesson.ClassEndDate.TimeOfDay);
                        bool endCondition = (e.End.TimeOfDay > lesson.ClassStartDate.TimeOfDay) &&
                                            (e.End.TimeOfDay < lesson.ClassEndDate.TimeOfDay);

                        if (startCondition || endCondition)
                        {
                            //Break out the loop if a match is found
                            overlapExists = true;
                            break;
                        }
                    }

                    //If there's a match, set an error message for the page alert and refresh the page to show it
                    if (overlapExists)
                    {
                        Errormessage = "Overlap";
                        Redirect("/Schedule/Index");
                    }
                }
                
                //If no overlap occurs or there are no lessons booked on that day:
                //Create new session
                request = new PrivateSession
                {
                    SessionStart = e.Start,
                    SessionEnd = e.End
                };
                //Set the start and end times
                //Send the model to the request form
                Redirect("/Schedule/RequestPrivate");
            }
        }
    }
}