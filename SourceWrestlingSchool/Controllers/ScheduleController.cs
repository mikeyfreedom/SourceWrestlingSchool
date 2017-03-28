using DayPilot.Web.Mvc;
using DayPilot.Web.Mvc.Enums;
using DayPilot.Web.Mvc.Events.Calendar;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using SourceWrestlingSchool.Models;
using System;
using System.Collections.Generic;
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
            
        // GET: Schedule
        public ActionResult Index()
        {
            return View();
        }
        
        public ActionResult Backend()
        {
            return new Dpc().CallBack(this);
        }

        // GET: Booking
        public ActionResult Booking()
        {
            ApplicationDbContext db = new ApplicationDbContext();
            var instance = new Dpc();
            int id = eventID;

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Lesson lesson = db.Lessons.Find(id);
            if (lesson == null)
            {
                return HttpNotFound();
            }
            return View(lesson);
        }

        class Dpc : DayPilotCalendar
        {
            ApplicationDbContext db = new ApplicationDbContext();
                        
            protected override void OnInit(InitArgs e)
            {
                Update();
            }

            protected override void OnFinish()
            {
                if (UpdateType == CallBackUpdateType.None)
                {
                    return;
                }

                var store = new UserStore<ApplicationUser>(db);
                var userManager = new UserManager<ApplicationUser>(store);
                ApplicationUser user = userManager.FindByNameAsync(System.Web.HttpContext.Current.User.Identity.Name).Result;

                ClassLevel level = (ClassLevel) user.ClassLevel;
                
                if (level == ClassLevel.Open)
                    Events = from ev in db.Lessons select ev;
                else
                    Events = from ev in db.Lessons where ev.ClassLevel == level select ev  ;

                DataIdField = "ClassID";
                DataTextField = "ClassLevel";
                DataStartField = "ClassStartDate";
                DataEndField = "ClassEndDate";
            }

            protected override void OnEventClick(EventClickArgs e)
            {
                //Test to check the method was firing
                Debug.WriteLine("Hey I clicked you");
                //Parse the event ID for processing
                eventID = int.Parse(e.Id);
                //Redirect to the booking page
                Redirect("/Schedule/Booking");
            }
        }
    }
}