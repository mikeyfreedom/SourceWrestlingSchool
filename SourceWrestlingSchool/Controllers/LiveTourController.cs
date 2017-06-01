using SourceWrestlingSchool.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Data.Entity;
using Braintree;
using System.Web.Script.Serialization;

namespace SourceWrestlingSchool.Controllers
{
    /// <summary>
    ///     Controller to handle LiveEvent booking actions
    /// </summary>
    public class LiveTourController : Controller
    {
        /// <summary>
        ///     Virtual representation of the database
        /// </summary>
        private readonly ApplicationDbContext _db = new ApplicationDbContext();
        
        /// <summary>
        ///     Display all events to the user
        /// </summary>
        /// <remarks>
        ///     Retrieve all events from the database
        ///     For each event, include the event venue and seats attached to the event
        /// </remarks>
        /// <returns>The Index View with the list of events as the sending model</returns>
        // GET: LiveTour
        public ActionResult Index()
        {
             var model = _db.LiveEvents.Include(ev => ev.Venue).Include(ev => ev.Seats);
                                      
            return View(model);
        }

        /// <summary>
        ///     Loads a seat map of the selected live event
        /// </summary>  
        /// <remarks>
        ///     Retrieve the event from the database with a matching id as the parameter.
        ///     Include the event venue and seats attached to the event in the entity.
        ///     Create a list of event seats that are set to reserved or booked
        ///     If the method was called from the booking screen
        ///         Add the sent message to a ViewBag for displaying as a notification
        ///     Add the seat list to a ViewBag for referencing in the View
        /// </remarks>
        /// <param name="id">The id of the event to be displayed</param>
        /// <returns> The seat map view of the sent model LiveEvent entity </returns>
        public ActionResult Tickets(int id)
        {
            var model = _db.LiveEvents
                        .Where(ev => ev.EventId == id)
                        .Include(ev => ev.Venue)
                        .Include(ev => ev.Seats)
                        .Single();

            var bookings = model.Seats
                           .Where(s => s.Status == Seat.SeatBookingStatus.Reserved || s.Status == Seat.SeatBookingStatus.Booked)
                           .Select(s => s.SeatNumber)
                           .ToArray();

            ViewBag.Message = TempData["message"];
            ViewBag.Unavailable = bookings;
            return View(model);
        }

        /// <summary>
        ///     Use the form data from the seat map to create a checkout page
        /// </summary>
        /// <remarks>
        ///     Instatiate a new ViewModel to send to the page.
        ///     Create a list to hold the selected seats.
        ///     Get the event id string from the form data and parse it to a string.
        ///     Get the string representation of the seatList and split it into usable data.
        ///     Retrieve the event details with the matching id as the passed in id parameter.
        ///     For each seat in the seatList, set its status to Reserved so no other user can book it
        ///     Add the seatList and event to the ViewModel.
        ///     Save the seat status changes to the database.
        ///     Search the Braintree payment gateway to check if the user is an existing customer in the Vault.
        ///     If a match is found
        ///         Generate a client token using the customer's vault ID.s
        ///     If no match exists
        ///         Use the gateway to generate a token for the checkout session.
        ///     In either case, store the clientToken in a ViewBag for clientside use.
        /// </remarks>
        /// <param name="form">The collection of data from the seat map form</param>
        /// <returns>The checkout view, populated by the TicketViewModel created</returns>
        [HttpPost]
        public ActionResult Booking(FormCollection form)
        {
            TicketViewModel model = new TicketViewModel();
            List<Seat> seats = new List<Seat>();
            var eventId = form["eventID"];
            int eId = int.Parse(eventId);
            var seatList = form["seatList"].Split(',');
            TempData["seatList"] = seatList;
                        
            using (_db)
            {
                LiveEvent newEvent = _db.LiveEvents
                                      .Where(ev => ev.EventId == eId)
                                      .Include(ev => ev.Venue)
                                      .Include(ev => ev.Seats)
                                      .FirstOrDefault();
                TempData["Event"] = newEvent;

                foreach (string seatNo in seatList)
                {
                    if (newEvent != null)
                    {
                        Seat seat = (from s in newEvent.Seats
                            where s.SeatNumber == seatNo
                            select s).First();

                        seats.Add(seat);
                        seat.Events.Add(newEvent);
                        seat.Status = Seat.SeatBookingStatus.Reserved;
                    }
                    model.Event = newEvent;
                    model.Seats = seats;
                    _db.SaveChanges();
                }
            }
            
            //Payment token generation
            var customerRequest = new CustomerSearchRequest().Email.Is(User.Identity.Name);
            ResourceCollection<Customer> collection = PaymentGateways.Gateway.Customer.Search(customerRequest);
            string clientToken;
            if (collection.Ids.Count != 0)
            {
                string custId = collection.FirstItem.Id;
                clientToken = PaymentGateways.Gateway.ClientToken.generate(
                    new ClientTokenRequest
                    {
                        CustomerId = custId
                    }
                );
            }
            else
            {
                clientToken = PaymentGateways.Gateway.ClientToken.generate();
            }
            ViewBag.ClientToken = clientToken;
            
            return View(model);
        }


        /// <summary>
        ///     Process payment of the seat reservation
        /// </summary>
        /// <remarks>
        ///     Retrieve the event entity and seatlist from the TempData generated on pageload.
        ///     Retrieve the sent payment nonce and amount of the transaction from the checkout form data.
        ///     Create a new Transaction request using the taken in data.
        ///     Add a description to the transaction for historical purposes.
        ///     Submit the transaction for settlement and receive a result object from the gateway.
        ///     If the payment is successful
        ///         Retrieve the event details from the database.
        ///         Set the seats attached to the request to a booked status.
        ///         Create a payment record to be held for historical purposes.
        ///         Store the seatIDs of the purchase in the record, the user who made the booking, the date, and description.
        ///         Create a positive notification
        ///     Otherwise
        ///         Retrieve the event details from the database.
        ///         Set the seats attached to the request to free.
        ///         Create a negative notification informing the user of the errors in payment.
        ///     In both cases
        ///         Save the changes to the database.
        ///         Load the seat map of the event with the reflected changes.
        /// </remarks>
        /// <param name="collection">Collection of data from the checkout form</param>
        /// <returns>
        ///     The Seat Map View if the payment was a success or failure
        /// </returns>
        [HttpPost]
        public ActionResult Checkout(FormCollection collection)
        {
            LiveEvent currentEvent = (LiveEvent)TempData["Event"];
            string[] seatlist = (string[])TempData["seatList"];
            
            using (_db)
            {
                string nonceFromTheClient = collection["payment_method_nonce"];
                decimal amount = decimal.Parse(collection["amount"]);

                var request = new TransactionRequest
                {
                    Amount = amount,
                    PaymentMethodNonce = nonceFromTheClient,
                    CustomFields = new Dictionary<string, string>
                {
                    { "description", "Booking of " + seatlist.Length + " seats for "+ currentEvent.EventName + "." },
                },
                    Options = new TransactionOptionsRequest
                    {
                        SubmitForSettlement = true
                    }
                };

                Result<Transaction> result = PaymentGateways.Gateway.Transaction.Sale(request);

                if (result.IsSuccess())
                {
                    LiveEvent newEvent = _db.LiveEvents
                                        .Where(ev => ev.EventId == currentEvent.EventId)
                                        .Include(ev => ev.Venue)
                                        .Include(ev => ev.Seats)
                                        .FirstOrDefault();

                    Payment payment = new Payment
                    {
                        PaymentSettled = true,
                        PaymentAmount = amount,
                        PaymentDate = DateTime.Now.Date,
                        PaymentDescription = "Booking of " + seatlist.Length + " seats for " + currentEvent.EventName + " on " + currentEvent.EventDate.ToShortDateString() + ".",
                        User = _db.Users.Single(u => u.Email == User.Identity.Name),
                        TransactionId = result.Target.Id
                    };
                    payment.UserId = payment.User.Id;
                    payment.Seats = new List<Seat>();

                    foreach (var seatNo in seatlist)
                    {
                        //Checks newEvent is not null before searching for a seat
                        Seat seat = newEvent?.Seats.FirstOrDefault(s => s.SeatNumber == seatNo);
                        if (seat != null)
                        {
                            seat.Status = Seat.SeatBookingStatus.Booked;
                            payment.Seats.Add(seat);
                        }
                    }
                    _db.Payments.Add(payment);
                    TempData["message"] = "Payment Successful, enjoy the event!";
                    TempData["model"] = new TicketViewModel { Event = newEvent,Seats = payment.Seats};
                    _db.SaveChanges();
                    return RedirectToAction("Success");
                }
                else
                {
                    LiveEvent newEvent = _db.LiveEvents
                                       .Where(ev => ev.EventId == currentEvent.EventId)
                                       .Include(ev => ev.Venue)
                                       .Include(ev => ev.Seats)
                                       .FirstOrDefault();

                    foreach (var seatNo in seatlist)
                    {
                        Seat seat = newEvent?.Seats.FirstOrDefault(s => s.SeatNumber == seatNo);
                        if (seat != null)
                        {
                            seat.Status = Seat.SeatBookingStatus.Free;
                            
                        }
                    }
                    TempData["message"] = "Payment Failed. Reason: " + result.Message;
                    Console.WriteLine(result.Message);
                }
                _db.SaveChanges();
            }
            
            return RedirectToAction("/Tickets/" + currentEvent.EventId);
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        /// <returns></returns> 
        public ActionResult Success()
        {
            var model = (TicketViewModel) TempData["model"];
            return View(model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult DownloadTicket()
        {
            var currentEvent = (LiveEvent) TempData["Event"];
            var currentSeats = (string[]) TempData["seatList"];
            var seatList = new List<Seat>();
            foreach (var seatNo in currentSeats)
            {
                Seat newSeat = currentEvent.Seats.FirstOrDefault(s => s.SeatNumber == seatNo);
                seatList.Add(newSeat);
            } 
            DownloadTicketViewModel model = 
                new DownloadTicketViewModel
                {
                    User = _db.Users.Single(u => u.Email == User.Identity.Name),
                    EventDetails = new TicketViewModel() { Event = currentEvent, Seats = seatList }
                };

            var filename = currentEvent.EventName + "_" + currentEvent.EventDate.ToShortDateString() + ".pdf";

            return new Rotativa.ViewAsPdf("PdfTicket",model) { FileName = filename };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult PdfTicket(DownloadTicketViewModel model)
        {
            return View(model);
        }

        /// <summary>
        ///     Update the status of all seats on the seat map
        /// </summary>
        /// <remarks>
        ///     Retrieve the event details from the database
        ///     Get all the seats for the event that are listed as reserved or booked
        ///     Store them in a dictionary
        ///     Serialize the dictionary into a JSON response object
        ///     Send the object back to the seat map view for processing
        /// </remarks>
        /// <param name="id">The id of the LiveEvent entity</param>
        /// <returns>A JSON representation of all unavailable seats</returns>
        public JsonResult UpdateStatus(int id)
        {
            var model = _db.LiveEvents
                        .Where(ev => ev.EventId == id)
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
            string response = new JavaScriptSerializer().Serialize(seatList);

            return Json(response, JsonRequestBehavior.AllowGet);
        }
    }
}