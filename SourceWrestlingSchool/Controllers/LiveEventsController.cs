using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.UI.WebControls;
using System.Web.Mvc;
using SourceWrestlingSchool.Models;
using System.IO;
using System.Web.UI;

namespace SourceWrestlingSchool.Controllers
{
    /// <summary>
    ///     Controller to handle the database actions of LiveEvent entities
    /// </summary>
    public class LiveEventsController : Controller
    {
        /// <summary>
        ///     Virtual represenatation of the database
        /// </summary>
        private readonly ApplicationDbContext _db = new ApplicationDbContext();

        /// <summary>
        ///     Get all events listed in the database.
        ///     Attach the venue object used for the event.
        ///     Attach the list of Seats associated with the event.
        ///     Add all events to a list.
        ///     Send the list as a ViewModel to populate the Index View.
        /// </summary>
        /// <returns>The index view, along with the list of attached events</returns>
        // GET: LiveEvents
        public ActionResult Index()
        {
            return View(_db.LiveEvents
                        .Include(v => v.Venue)
                        .Include(s => s.Seats)                        
                        .ToList());
        }

        /// <summary>
        ///     Show the details of a particular event.
        /// </summary>
        /// <remarks>
        ///     If no id is sent, display a bad request error page.
        ///     Else
        ///         Get the event from the database with a matching id.
        ///         Attach the venue and seats associated with the event entity.
        ///         Send the event as the model to populate the Detail view.
        /// </remarks>
        /// <param name="id">The id of the event entity</param>
        /// <returns>The Event Detail View, with a LiveEvent parameter</returns>
        // GET: LiveEvents/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LiveEvent liveEvent = _db.LiveEvents
                                  .Where(e => e.EventId == id)
                                  .Include(v => v.Venue)
                                  .Include(s => s.Seats)
                                  .First();
                                  
            if (liveEvent == null)
            {
                return HttpNotFound();
            }
            return View(liveEvent);
        }

        /// <summary>
        ///     Load the form to create a new event.
        /// </summary>
        /// <remarks>
        ///     Retrieve all venues from the database.
        ///     Add the list of venues to a ViewBag for SelectList use.
        ///     Load the Create form.
        /// </remarks>
        /// <returns>The create new event form view</returns>
        // GET: LiveEvents/Create
        public ActionResult Create()
        {
            ViewBag.Venues = _db.Venues.ToList();
            return View();
        }

        /// <summary>
        ///     Use the create event form data to create a new event 
        /// </summary>
        /// <remarks>
        ///     Take in the binded model from the form view.
        ///     Set the revenue of the new event to zero.
        ///     Check the model is valid.
        ///     If it is valid
        ///         Save the new event to the database.
        ///         Save the changes to the database.
        ///         Load the index view with the new event added to the list.
        ///     If not valid
        ///         Refresh the create event view for editing.
        /// </remarks>
        /// <param name="liveEvent">The event model to be saved to the database</param>
        /// <returns>
        ///     Returns the create event view if the model or attached file is not valid.
        ///     Returns the LiveEvent index view if all criteria are met.
        /// </returns>
        // POST: LiveEvents/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "EventID,EventName,EventDate,EventTime")] LiveEvent liveEvent)
        {
            liveEvent.EventRevenue = 0;
            if (ModelState.IsValid)
            {
                liveEvent.Venue = _db.Venues.First();
                liveEvent.Seats = liveEvent.CreateSeatMap();
                _db.LiveEvents.Add(liveEvent);
                _db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(liveEvent);
        }

        /// <summary>
        ///     Load the Form and Model to Edit an Event
        /// </summary>
        /// <remarks>
        ///     Retrieve the event from the database with a matching id
        ///     Retrieve all venues from the database.
        ///     Add the list of venues to a ViewBag for SelectList use.
        ///     Load the Edit View
        /// </remarks>
        /// <param name="id">The id of the event entity to be edited</param>
        /// <returns>The edit event form with the event to be edited as the sending model</returns>
        // GET: LiveEvents/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            LiveEvent liveEvent;
            using (_db)
            {
                liveEvent = _db.LiveEvents.Find(id);
                ViewBag.Venues = _db.Venues.ToList();
            }
               
            if (liveEvent == null)
            {
                return HttpNotFound();
            }
            return View(liveEvent);
        }

        /// <summary>
        ///     Use the data from the edit form to make changes to a LiveEvent
        /// </summary>
        /// <remarks>
        ///     Take in the binded model from the form view
        ///     Check the model is valid
        ///     If it is valid
        ///         Save the edited event to the database
        ///         Save the changes to the database
        ///         Load the index view with the new event information shown
        ///     If not valid
        ///         Refresh the edit event view for further editing
        /// </remarks>
        /// <param name="liveEvent">The edited event info to be saved</param>
        /// <returns>
        ///     Returns the edit event view if the model or attached file is not valid
        ///     Returns the LiveEvent index view if all criteria are met.
        /// </returns>
        // POST: LiveEvents/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "EventID,EventName,EventDate,EventTime,EventRevenue")] LiveEvent liveEvent)
        {
            if (ModelState.IsValid)
            {
                _db.Entry(liveEvent).State = EntityState.Modified;
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(liveEvent);
        }

        /// <summary>
        ///     Load the Delete Event Form
        /// </summary>
        /// <remarks>
        ///     Retrieve the event from the database with a matching id
        ///     Load the Delete Event View
        /// </remarks>
        /// <param name="id">The id of the event to be deleted</param>
        /// <returns>The Delete View with the matching event as a parameter</returns>
        // GET: LiveEvents/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LiveEvent liveEvent = _db.LiveEvents.Find(id);
            if (liveEvent == null)
            {
                return HttpNotFound();
            }
            return View(liveEvent);
        }

        /// <summary>
        ///     Remove a LiveEvent
        /// </summary>
        /// <remarks>
        ///     Retrieve the event from the database with a matching id
        ///     Remove the event entity from the database
        ///     Save the change to the database
        ///     Load the Index view with the new information
        /// </remarks>
        /// <param name="id">The id of the event to be deleted</param>
        /// <returns>The updated LiveEvent Index View</returns>
        // POST: LiveEvents/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            LiveEvent liveEvent = _db.LiveEvents.Find(id);
            if (liveEvent != null)
            {
                _db.LiveEvents.Remove(liveEvent);
            }
            _db.SaveChanges();
            return RedirectToAction("Index");
        }

        /// <summary>
        ///     Create an Excel spreadsheet of event info and download it
        /// </summary>
        /// <remarks>
        ///     Instantiate a GridView to hold the data for exporting
        ///     Retrieve the names, dates, and revenues of all events in the database
        ///     Add them to a list to be used as the data source for exporting
        ///     Create the file to be downloaded
        ///     Instatiate a StringWriter to deal with the data
        ///     Write the data to the file via the StringWriter
        ///     End the file write and send it to the user
        /// </remarks>
        public void ExportEventListToExcel()
        {
            GridView grid = new GridView();
            var eventQuery = from e in _db.LiveEvents
                             select new { e.EventId, e.EventName, e.EventDate, e.EventRevenue };
            grid.DataSource = eventQuery.ToList();
            grid.DataBind();

            Response.ClearContent();
            Response.AddHeader("content-disposition", "attachment; filename=EventRevenue.xls");
            Response.ContentType = "application/excel";
            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);

            grid.RenderControl(htw);

            Response.Write(sw.ToString());
            Response.End();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
