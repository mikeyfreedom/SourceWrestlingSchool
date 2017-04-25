using SourceWrestlingSchool.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using Braintree;

namespace SourceWrestlingSchool.Controllers
{
    public class LiveTourController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        
        // GET: LiveTour
        public ActionResult Index()
        {
             var model = db.LiveEvents.Include(ev => ev.Venue).Include(ev => ev.Seats);
                                      
            return View(model);
        }

        public ActionResult Tickets(int id)
        {
            var model = db.LiveEvents.Where(ev => ev.EventID == id).Include(ev => ev.Venue).Include(ev => ev.Seats).Single();

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
            TempData["seatList"] = seatList;
                        
            using (db)
            {
                LiveEvent newEvent = db.LiveEvents
                                      .Where(ev => ev.EventID == eID)
                                      .Include(ev => ev.Venue)
                                      .Include(ev => ev.Seats)
                                      .FirstOrDefault();
                model.Event = newEvent;
                TempData["Event"] = newEvent;

                foreach (string seatNo in seatList)
                {
                    Seat seat = (from s in newEvent.Seats
                                 where s.SeatNumber == seatNo
                                 select s).First();

                    Seats.Add(seat);
                    seat.Events.Add(newEvent);
                    seat.Status = Seat.SeatBookingStatus.Reserved;
                    db.SaveChanges();
                }
            }
            
            model.Seats = Seats;
            var clientToken = PaymentGateways.Gateway.ClientToken.generate();
            ViewBag.ClientToken = clientToken;

            return View(model);
        }

        [HttpPost]
        public ActionResult Checkout(FormCollection collection)
        {
            string nonceFromTheClient = collection["payment_method_nonce"];

            var request = new TransactionRequest
            {
                Amount = 10.00M,
                PaymentMethodNonce = nonceFromTheClient,
                Options = new TransactionOptionsRequest
                {
                    SubmitForSettlement = true                    
                }
            };

            Result<Transaction> result = PaymentGateways.Gateway.Transaction.Sale(request);
            if (result.IsSuccess())
            {
                string[] seatlist = (string[]) TempData["seatList"];
                LiveEvent currentEvent = (LiveEvent) TempData["Event"];
                using (db)
                {
                    LiveEvent newEvent = db.LiveEvents
                                      .Where(ev => ev.EventID == currentEvent.EventID)
                                      .Include(ev => ev.Venue)
                                      .Include(ev => ev.Seats)
                                      .FirstOrDefault();
                    foreach (var seatNo in seatlist)
                    {
                        Seat seat = newEvent.Seats
                                    .Where(s => s.SeatNumber == seatNo)
                                    .FirstOrDefault();
                        seat.Status = Seat.SeatBookingStatus.Booked;
                    }
                }
                return RedirectToAction("/Tickets/" + currentEvent.EventID);
            }
            else
            {
                Console.WriteLine(result.Message);
            }

            return RedirectToAction("/LiveTour/Index");
        }

        
    }
}