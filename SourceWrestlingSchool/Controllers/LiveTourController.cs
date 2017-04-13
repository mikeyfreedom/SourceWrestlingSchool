using SourceWrestlingSchool.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

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
        public ActionResult Booking(int id,string seatNo)
        {
            return View();
        }
    }
}