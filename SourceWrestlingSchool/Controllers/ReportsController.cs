using System;
using System.IO;
using System.Linq;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.UI;
using Braintree;
using SourceWrestlingSchool.Models;

namespace SourceWrestlingSchool.Controllers
{
    public class ReportsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: Reports
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Revenue(FormCollection collection)
        {
            var dataString = collection["selectedMonth"];
            int year = int.Parse(dataString.Substring(0, 4));
            int month = int.Parse(dataString.Substring(5, 2));
            RevenueReportModel model; 

            //Create ViewModel
            //Add Data to Sending Model
            using (db)
            {
                //Get Event Revenue
                var events = db.LiveEvents
                             .Where(e => e.EventDate.Month == month && e.EventDate.Year == year)
                             .ToList();

                float eventRevenueTotal = 0;
                foreach (var show in events)
                {
                    eventRevenueTotal = eventRevenueTotal + show.EventRevenue;
                }

                //Get Membership Revenue
                DateTime startDate = new DateTime(year,month,1);
                DateTime endDate = new DateTime(year,month, DateTime.DaysInMonth(year, month));

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
                var privateRequest = new TransactionSearchRequest().SettledAt.Between(startDate, endDate).Amount.Is(30).Refund.Is(false);
                ResourceCollection<Transaction> privateResults = PaymentGateways.Gateway.Transaction.Search(privateRequest);
                var pFeesCollected = privateResults.Ids.Count;
                var privateTotal = bFeesCollected * 30;

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

            return View(model);
        }

        public JsonResult GetData(string selected)
        {
            int year = int.Parse(selected.Substring(0, 4));
            int month = int.Parse(selected.Substring(5, 2));
            RevenueReportModel model;

            //Create ViewModel
            //Add Data to Sending Model
            using (db)
            {
                //Get Event Revenue
                var events = db.LiveEvents
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
                var privateRequest = new TransactionSearchRequest().SettledAt.Between(startDate, endDate).Amount.Is(30).Refund.Is(false);
                ResourceCollection<Transaction> privateResults = PaymentGateways.Gateway.Transaction.Search(privateRequest);
                var pFeesCollected = privateResults.Ids.Count;
                var privateTotal = bFeesCollected * 30;

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
            return Json(model, JsonRequestBehavior.AllowGet);
        }

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