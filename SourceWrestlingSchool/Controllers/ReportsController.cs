using System;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using System.Web.UI;
using Braintree;
using SourceWrestlingSchool.Models;

namespace SourceWrestlingSchool.Controllers
{
    /// <summary>
    ///     Controller that handles the actions of the reports section of the admin panel
    /// </summary>
    public class ReportsController : Controller
    {
        /// <summary>
        ///     Virtual representation of the database
        /// </summary>
        private readonly ApplicationDbContext _db = new ApplicationDbContext();
        
        /// <summary>
        ///     Loads the Index View
        /// </summary>
        /// <returns>The Index View of the Reports section</returns>
        // GET: Reports
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        ///     Retrieve all the revenue collected during a selected month/year.
        /// </summary>
        /// <remarks>
        ///     Take in the data string from the form view.
        ///     Send the string to the GetRevenue method to retrieve a RevenueViewModel.
        ///     Send the model along with the Revenue View
        /// </remarks>
        /// <param name="collection">The Month and year selected in the client-side DatePicker.</param>
        /// <returns>A RevenueReportModel detailing all revenue for the selected Month and Year.</returns>
        public ActionResult Revenue(FormCollection collection)
        {
            var dataString = collection["selectedMonth"];
            
            return View(GetRevenueData(dataString));
        }

        /// <summary>
        ///     Respond to an Ajax request for specific revenue data.
        /// </summary>
        /// <remarks>
        ///     Send the incoming form data to the GetRevenue method to retrieve a data model
        ///     Parse the model into JSon format and send this back to the Ajax method that called the routine.
        /// </remarks>
        /// <param name="selected"></param>
        /// <returns></returns>
        public JsonResult GetData(string selected)
        {
            return Json(GetRevenueData(selected), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        ///     Gather all revenue date during a month/year combination.
        /// </summary>
        /// <remarks>
        ///     Separate the incoming data string into the month and year component.
        ///     Check if any live event took place during the month in question.
        ///         If so, add their combined revenue to the data model.
        ///     Search the financial gateways for membership renewals payments.
        ///         Add any matches to the relevant gold, silver or bronze revenue totals in the model.
        ///     Search the database for any accepted private session requested that had been paid for.
        ///         Add any matches to the private fee count, and add the total revenue to the model.
        ///     Search for any class cancellation fines settled during the month in question.
        ///         Add the total of any fines to the viewmodel.
        ///     Instantiate the model and return it.
        /// </remarks>
        /// <param name="selectedData">A <see cref="string"/> containing a numeric month and year</param>
        /// <returns></returns>
        public RevenueReportModel GetRevenueData(string selectedData)
        {
            int year = int.Parse(selectedData.Substring(0, 4));
            int month = int.Parse(selectedData.Substring(5, 2));
            RevenueReportModel model;

            //Create ViewModel
            //Add Data to Sending Model
            using (_db)
            {
                //Get Event Revenue
                var events = _db.LiveEvents
                    .Where(e => e.EventDate.Month == month && e.EventDate.Year == year)
                    .ToList();

                float eventRevenueTotal = 0;
                foreach (var show in events)
                {
                    eventRevenueTotal = eventRevenueTotal + show.EventRevenue;
                }

                //Get Membership Revenue
                DateTime startDate = new DateTime(year, month, 1);
                DateTime endDate = new DateTime(year, month, DateTime.DaysInMonth(year, month));

                var bronzeRequest = new TransactionSearchRequest().SettledAt.Between(startDate, endDate).Amount.Is(40);
                var silverRequest = new TransactionSearchRequest().SettledAt.Between(startDate, endDate).Amount.Is(55);
                var goldRequest = new TransactionSearchRequest().SettledAt.Between(startDate, endDate).Amount.Is(70);

                ResourceCollection<Transaction> bResults = PaymentGateways.Gateway.Transaction.Search(bronzeRequest);
                var bFeesCollected = bResults.Ids.Count;
                var bTotal = bFeesCollected * 40;
                ResourceCollection<Transaction> sResults = PaymentGateways.Gateway.Transaction.Search(silverRequest);
                var sFeesCollected = sResults.Ids.Count;
                var sTotal = sFeesCollected * 55;
                ResourceCollection<Transaction> gResults = PaymentGateways.Gateway.Transaction.Search(goldRequest);
                var gFeesCollected = gResults.Ids.Count;
                var gTotal = gFeesCollected * 70;

                //Get Private Session Revenue
                var pFeesCollected = 0;
                foreach (var payment in _db.Payments.Where(p => p.PaymentDate.Month == month && p.PaymentDate.Year == year).ToList())
                {
                    if (payment.PaymentDescription.Equals("Private Session Booking Fee") && payment.PaymentSettled)
                    {
                        pFeesCollected++;
                    }
                }
                var privateTotal = pFeesCollected * 30;

                //Get Fine Revenue
                var fineRequest = new TransactionSearchRequest().SettledAt.Between(startDate, endDate).Amount.Is(30).Refund.Is(1.5);
                ResourceCollection<Transaction> fineResults = PaymentGateways.Gateway.Transaction.Search(fineRequest);
                var finesCollected = fineResults.Ids.Count;
                double fineTotal = finesCollected * 1.5;

                model = new RevenueReportModel
                {
                    BronzeFeesCollected = bTotal,
                    CurrentDate = new DateTime(year, month, 1),
                    Events = events.ToList(),
                    FinesCollected = fineTotal,
                    GoldFeesCollected = gTotal,
                    NoBronzeMemberships = bFeesCollected,
                    NoCancellationFines = finesCollected,
                    NoGoldMemberships = gFeesCollected,
                    NoPrivateSessions = pFeesCollected,
                    NoSilverMemberships = sFeesCollected,
                    PrivateFeesCollected = privateTotal,
                    SilverFeesCollected = sTotal,
                    TotalEventRevenue = eventRevenueTotal,
                    TotalMembershipRevenue = bTotal + sTotal + gTotal,
                    TotalOtherRevenue = privateTotal + fineTotal,
                    TotalMonthRevenue = eventRevenueTotal + bTotal + sTotal + gTotal + privateTotal + fineTotal
                };
            }
            return model;
        }
        
        /// <summary>
        ///     Export a report for the year's revenue to a PDF file.
        /// </summary>
        public void ExportYearlyRevenueToPdf()
        {
            Response.ClearContent();
            Response.AddHeader("content-disposition", "attachment; filename=StudentRoster.xls");
            Response.ContentType = "application/excel";
            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);
            Response.Write(sw.ToString());
            Response.End();
        }
    }
}