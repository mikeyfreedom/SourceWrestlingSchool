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
    public class ScheduleController : Controller
    {
        public static int eventID;
        public static string email;
        public static string errormessage = " ";
        public static PrivateSession request;
        private ApplicationDbContext db = new ApplicationDbContext();
        private ApplicationUser currentUser;
        
        // GET: Schedule
        public ActionResult Index()
        {
            var model = errormessage;
            return View(model:model);
        }
        
        public ActionResult Backend()
        {
            return new Dpc().CallBack(this);
        }

        // GET: Booking
        public ActionResult Booking()
        {
            int id = eventID;
            var model = db.Lessons.Find(id);
            
            return View(model);
        }

        public ActionResult RequestPrivate()
        {
            var model = request;
            currentUser = db.Users.Single(n => n.UserName == User.Identity.Name);
            string name = currentUser.FirstName;
            model.StudentName = name;

            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(db));
            var instructorRole = roleManager.Roles.Single(r => r.Name == RoleNames.ROLE_INSTRUCTOR);
            List<ApplicationUser> instructors = new List<ApplicationUser>();
            foreach (var user in instructorRole.Users)
            {
                instructors.Add(userManager.FindById(user.UserId));
            }
            ViewBag.Instructors = instructors;
            
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RequestPrivate([Bind(Include = "StudentName,SessionStart,SessionEnd,InstructorID,Notes")] PrivateSession model)
        {
            model.Status = PrivateSession.RequestStatus.Submitted;
            if (ModelState.IsValid)
            {
                db.PrivateSessions.Add(model);
                db.SaveChanges();
                sendEmail(User.Identity.Name,"private");
                errormessage = "Private";
                return RedirectToAction("Index");
            }

            return View(model);
        }
        
        [HttpPost]
        public ActionResult BookUser()
        {
            //Gather required variables
            var user = (from u in db.Users
                        where u.Email == User.Identity.Name
                        select u).First();
            currentUser = user;
            int classID = int.Parse(Request.Form["classID"]);
            
            using (db)
            {
                var editedLesson = db.Lessons.Single(s => s.LessonID == classID);
                db.Lessons.Attach(editedLesson);

                var editedUser = db.Users.Single(s => s.Id == user.Id);
                db.Users.Attach(editedUser);

                editedLesson.Students.Add(editedUser);
                
                db.SaveChanges();
            }
            errormessage = "BookSuccess";
            sendEmail(currentUser.UserName, "book");

            return RedirectToAction("Index","Schedule");
        }
        
        [HttpPost]
        public ActionResult CancelUser()
        {
            using (db)
            {
                ApplicationUser user = db.Users.First(i => i.UserName == User.Identity.Name);
                int classID = int.Parse(Request.Form["classID"]);
                Lesson lesson = db.Lessons
                                .Where(l => l.LessonID == classID)
                                .Include(l => l.Students)
                                .Single();
                lesson.Students.Remove(user);
                TimeSpan difference = lesson.ClassStartDate - DateTime.Now;
                if(difference.TotalHours < 12)
                {
                    errormessage = "CancelSuccessFine";
                    double cancelfine = lesson.ClassCost * 0.15;
                    Payment fine = new Payment
                    {
                        PaymentAmount = Math.Round((decimal) cancelfine, 2),
                        PaymentDate = DateTime.Today,
                        PaymentDescription = "Class Cancellation Fine",
                        PaymentSettled = false,
                        User = user,
                        UserID = user.Id
                    };
                    db.Payments.Add(fine);
                }
                else
                {
                    errormessage = "CancelSuccess";
                }
                                
                db.SaveChanges();
            }
            
            //sendEmail(User.Identity.Name, "cancel");

            return RedirectToAction("Index", "Schedule"); 
        }
        
        private void sendEmail(string user,string reason)
        {
            MailMessage message = new MailMessage("lowlander_glen@yahoo.co.uk", user);
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
            smtpClient.Send(message);
        }

        [Authorize(Roles ="Instructor")]
        public ActionResult PersonalSchedule()
        {
            using (db)
            {
                var model = db.Lessons
                            .Where(ev => ev.InstructorName == User.Identity.Name)
                            .Where(ev => ev.ClassStartDate > DateTime.Now)
                            .Include(ev => ev.Students)
                            .ToList();

                return View(model);
            }
        }

        class Dpc : DayPilotCalendar
        {
            private ApplicationDbContext db = new ApplicationDbContext();
                        
            protected override void OnInit(InitArgs e)
            {
                Update();
            }

            protected override void OnBeforeEventRender(BeforeEventRenderArgs e)
            {
                e.FontColor = "black";
                
                string classlevel = e.DataItem["ClassLevel"].ToString();
                if (classlevel.Equals("Beginner"))
                    e.BackgroundColor = "red";
                else if (classlevel.Equals("Intermediate"))
                    e.BackgroundColor = "yellow";
                else if (classlevel.Equals("Advanced"))
                    e.BackgroundColor = "green";
                else if (classlevel.Equals("Womens"))
                    e.BackgroundColor = "pink";
                    
                base.OnBeforeEventRender(e);
            }

            protected override void OnFinish()
            {
                if (UpdateType == CallBackUpdateType.None)
                {
                    return;
                }

                string username = System.Web.HttpContext.Current.User.Identity.Name;
                ApplicationUser user = db.Users.First(i => i.Email == username);
                ClassLevel level = (ClassLevel) user.ClassLevel;
                
                if (level == ClassLevel.Open)
                    Events = from ev in db.Lessons
                             select ev;
                else
                    Events = from ev in db.Lessons
                             where ev.ClassLevel == level || ev.ClassLevel == ClassLevel.Private
                             select ev ;

                DataIdField = "LessonID";
                DataTextField = "ClassLevel";
                DataStartField = "ClassStartDate";
                DataEndField = "ClassEndDate";                
            }

            protected override void OnEventClick(EventClickArgs e)
            {
                //Parse the event ID for processing
                eventID = int.Parse(e.Id);
                //Redirect to the booking page
                Redirect("/Schedule/Booking");
            }

            protected override void OnTimeRangeSelected(TimeRangeSelectedArgs e)
            {
                //Set a LINQ query to get all lessons that take place the same day as the requested session
                //DateTime startDate = e.Start;

                var lessons = db.Lessons
                              .Where(l => DbFunctions.TruncateTime(l.ClassStartDate) == e.Start.Date)
                              .ToList();
                              

                //If there is are any other classes that day, loop through them to check for a time conflict
                if (lessons.Count() != 0)
                {
                    //Set a flag to denote if an overlap exists
                    bool overlapExists = false;

                    //Loop though all the day's classes and check if the start or end overlaps with the request time
                    foreach (var lesson in lessons)
                    {
                        bool startCondition = (e.Start.TimeOfDay > lesson.ClassStartDate.TimeOfDay) && (e.Start.TimeOfDay < lesson.ClassEndDate.TimeOfDay);
                        bool endCondition = (e.End.TimeOfDay > lesson.ClassStartDate.TimeOfDay) && (e.End.TimeOfDay < lesson.ClassEndDate.TimeOfDay);

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
                        Debug.WriteLine("Overlap Exists");
                        errormessage = "Overlap";
                        Redirect("/Schedule/Index");
                    }
                    else
                    {
                        //If no overlap occurs:
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
    }
}