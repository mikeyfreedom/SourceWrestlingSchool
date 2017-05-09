using SourceWrestlingSchool.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using Braintree;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Web.Script.Serialization;

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
            var model = db.LiveEvents
                        .Where(ev => ev.EventID == id)
                        .Include(ev => ev.Venue)
                        .Include(ev => ev.Seats)
                        .Single();

            var bookings = model.Seats
                           .Where(s => s.Status == Seat.SeatBookingStatus.Reserved || s.Status == Seat.SeatBookingStatus.Booked)
                           .Select(s => s.SeatNumber)
                           .ToArray();

            ViewBag.Unavailable = bookings;
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
            LiveEvent currentEvent = (LiveEvent)TempData["Event"];
            string[] seatlist = (string[])TempData["seatList"];
            
            using (db)
            {
                var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
                string nonceFromTheClient = collection["payment_method_nonce"];

                var request = new TransactionRequest
                {
                    Amount = 10.00M,
                    PaymentMethodNonce = nonceFromTheClient,
                    CustomFields = new Dictionary<string, string>
                {
                    { "description", "Booking of " + seatlist.Count() + " seats for "+ currentEvent.EventName + "." },
                },
                    Options = new TransactionOptionsRequest
                    {
                        SubmitForSettlement = true
                    }
                };

                Result<Transaction> result = PaymentGateways.Gateway.Transaction.Sale(request);



                if (result.IsSuccess())
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

                    var user = userManager.FindByEmail(User.Identity.Name);

                    ViewBag.Message = "Payment Successful, enjoy the event!";
                    db.SaveChanges();
                    return RedirectToAction("/Tickets/" + currentEvent.EventID);
                }
                else
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
                        seat.Status = Seat.SeatBookingStatus.Free;
                    }
                    ViewBag.Message = result.Message;
                    Console.WriteLine(result.Message);
                }
                db.SaveChanges();
            }
            
            return RedirectToAction("Tickets","LiveTour");
        }

        public JsonResult UpdateStatus(int id)
        {
            var model = db.LiveEvents
                        .Where(ev => ev.EventID == id)
                        .Include(ev => ev.Venue)
                        .Include(ev => ev.Seats)
                        .Single();

            var bookings = model.Seats
                           .Where(s => s.Status == Seat.SeatBookingStatus.Reserved || s.Status == Seat.SeatBookingStatus.Booked)
                           .Select(s => s.SeatNumber);

            Dictionary<string, string> seatList = new Dictionary<string, string>();
            int i = 1;
            foreach (string seatNo in bookings)
            {
                seatList.Add(i.ToString(), seatNo);
                i++;
            }
            string response = (new JavaScriptSerializer()).Serialize(seatList);

            return Json(response, JsonRequestBehavior.AllowGet);
        }

        
    }
}