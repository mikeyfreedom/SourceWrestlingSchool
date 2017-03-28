using DayPilot.Web.Mvc;
using DayPilot.Web.Mvc.Events.Calendar;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SourceWrestlingSchool.Models
{
    public class Dpc : DayPilotCalendar
    {
        public int eventID;
        protected override void OnInit(InitArgs e)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            Events = from ev in db.Lessons select ev;

            DataIdField = "ClassID";
            DataTextField = "ClassLevel";
            DataStartField = "ClassStartDate";
            DataEndField = "ClassEndDate";

            Update();
        }
       
        protected override void OnEventClick(EventClickArgs e)
        {
            //Test to check the method was firing
            Debug.WriteLine("Hey I clicked you");
            //Parse the event ID for processing
            eventID = int.Parse(e.Id);
            //Redirect to the booking page
            Redirect("Schedule/Booking/{eventID}");
        }
    }
}
