using SourceWrestlingSchool.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;

namespace SourceWrestlingSchool.Controllers
{
    public class LiveTourController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        
        // GET: LiveTour
        public ActionResult Index()
        {
            var model = db.LiveEvents.ToList();
            
            return View(model);
        }

        public ActionResult Tickets(int id)
        {
            var model = (from ev in db.LiveEvents
                         where ev.EventID == id
                         select ev).First();

            Seat[][] seatModel = new Seat[8][];
            int j = 0;
            for (int i = 0; i<8; i++)
            {
                seatModel[i] = new Seat[]
                        {
                        model.Seats.ElementAt(j),
                        model.Seats.ElementAt(j+1),
                        model.Seats.ElementAt(j+2),
                        model.Seats.ElementAt(j+3),
                        model.Seats.ElementAt(j+4),
                        model.Seats.ElementAt(j+5),
                        model.Seats.ElementAt(j+6),
                        model.Seats.ElementAt(j+7),
                        };
                j = j + 8;
            }

            ViewBag.seatInfo = seatModel;
            return View(model);
        }

        [HttpPost]
        public ActionResult Booking(FormCollection form)
        {
            TicketViewModel model = new TicketViewModel();
            List<Seat> Seats = new List<Seat>();
            var eventID = form["eventID"];
            int eID = int.Parse(eventID);
            var seatList = form["seatList"].Split(',');
            
            using (db)
            {
                LiveEvent newEvent = db.LiveEvents
                                      .Where(ev => ev.EventID == eID)
                                      .Include(ev => ev.Venue)
                                      .Include(ev => ev.Seats)
                                      .FirstOrDefault();
                model.Event = newEvent;

                foreach (string seatNo in seatList)
                {
                    Seat seat = (from s in newEvent.Seats
                                 where s.SeatNumber == seatNo
                                 select s).First();

                    Seats.Add(seat);
                    seat.Events.Add(newEvent);
                    seat.Status = Seat.SeatBookingStatus.Reserved;
                    //db.Entry(seat).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }
            
            model.Seats = Seats;
            
            return View(model);
        }
    }
}