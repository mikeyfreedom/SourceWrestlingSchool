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
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Web.UI.WebControls;

namespace SourceWrestlingSchool.Controllers
{
    public class ScheduleController : Controller
    {
        public static int eventID;
        public static string email;
        public static string errormessage = " ";
        public static PrivateSession request;
        private ApplicationDbContext db = new ApplicationDbContext();
        
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
            string name = db.Users.Single(n => n.UserName == User.Identity.Name).FirstName;
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
            return RedirectToAction("Index","Schedule");
        }

        [HttpPost]
        public ActionResult CancelUser()
        {
            ApplicationUser user = db.Users.First(i => i.UserName == User.Identity.Name);
            int classID = int.Parse(Request.Form["classID"]);
            db.Lessons.Find(classID).Students.Remove(user);
            db.SaveChanges();

            errormessage = "CancelSuccess";
            return RedirectToAction("Index", "Schedule"); 
        }

        public ActionResult PersonalSchedule()
        {
            var firstName = (from u in db.Users
                        where u.Email == User.Identity.Name
                        select u.FirstName).FirstOrDefault();

            var model = (from ev in db.Lessons
                          where ev.InstructorName == firstName
                          select ev).ToList();
            
            return View(model);
        }

        class Dpc : DayPilotCalendar
        {
            ApplicationDbContext db = new ApplicationDbContext();
                        
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
                DateTime startDate = e.Start.Date;
                int startDay = e.Start.Day;
                int startMonth = e.Start.Month;

                var lessons = from l in db.Lessons
                              where (l.ClassStartDate.Day == startDay && l.ClassStartDate.Month == startMonth)
                              select l;

                //If there is are any other classes that day, loop through them to check for a time conflict
                if (lessons != null)
                {
                    //Set a flag to denote if an overlap exists
                    bool overlapExists = false;

                    //Loop though all the day's classes and check if the start or end overlaps with the request time
                    foreach (var lesson in lessons)
                    {
                        if ((lesson.ClassStartDate.TimeOfDay < e.End.TimeOfDay) || (lesson.ClassEndDate.TimeOfDay < e.Start.TimeOfDay))
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
                        request = new PrivateSession();
                        //Set the start and end times
                        request.SessionStart = e.Start;
                        request.SessionEnd = e.End;
                        //Send the model to the request form
                        Redirect("/Schedule/RequestPrivate");
                    }
                }
            }
        }
    }
}