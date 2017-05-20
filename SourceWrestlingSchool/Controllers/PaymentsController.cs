﻿using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using SourceWrestlingSchool.Models;
using Braintree;

namespace SourceWrestlingSchool.Controllers
{
    public class PaymentsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Payments
        public ActionResult Index()
        {
            var payments = db.Payments.Include(p => p.User);
            return View(payments.ToList());
        }

        // GET: Payments/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Payment payment = db.Payments.Find(id);
            if (payment == null)
            {
                return HttpNotFound();
            }
            return View(payment);
        }

        public ActionResult Outstanding()
        { 
            using (db)
            {
                var user = db.Users.Single(u => u.Email == User.Identity.Name);
                var model = db.Payments
                            .Where(p => p.UserId == user.Id && p.PaymentSettled == false)
                            .Include(p => p.User)
                            .ToList();
                ViewBag.Message = TempData["message"];
                return View(model);
            }
        }

        [HttpGet]
        public ActionResult SendPayment(int? paymentId)
        {
            if (paymentId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Payment payment = db.Payments
                              .Where(p => p.PaymentId == paymentId)
                              .Include(u =>u.User)
                              .Single();
            if (payment == null)
            {
                return HttpNotFound();
            }

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
            ViewBag.Message = TempData["message"];

            return View(payment);
        }

        [HttpPost]
        public ActionResult SendPayment(FormCollection collection)
        {
             using (db)
            {
                string nonceFromTheClient = collection["payment_method_nonce"];
                decimal amount = decimal.Parse(collection["amount"]);
                int pId = int.Parse(collection["payID"]);

                var request = new TransactionRequest
                {
                    Amount = amount,
                    PaymentMethodNonce = nonceFromTheClient,
                    CustomFields = new Dictionary<string, string>
                {
                    { "description",  collection["description"]},
                },
                    Options = new TransactionOptionsRequest
                    {
                        SubmitForSettlement = true
                    }
                };

                Result<Transaction> result = PaymentGateways.Gateway.Transaction.Sale(request);

                if (result.IsSuccess())
                {
                    Payment payment = db.Payments
                                      .Where(p => p.PaymentId == pId && p.PaymentSettled == false)
                                      .Include(p => p.User)
                                      .Single();

                    payment.PaymentSettled = true;
                    
                    TempData["message"] = "Payment Successful.";
                    db.SaveChanges();
                    return RedirectToAction("Outstanding");
                }
                else
                {
                    TempData["message"] = "";
                    foreach (var error in result.Errors.All())
                    {
                        TempData["message"] = TempData["message"] + error.Message + " ";
                    }
                }
                db.SaveChanges();
                return Redirect(HttpContext.Request.UrlReferrer.AbsoluteUri);
            }
        }

        public ActionResult PaymentHistory()
        {
            var model = db.Payments
                        .Where(p => p.User.Email == User.Identity.Name && p.PaymentSettled)
                        .ToList();
            foreach (Payment payment in model)
            {
                Transaction transaction = PaymentGateways.Gateway.Transaction.Find(payment.TransactionId);
                if (transaction.RefundedTransactionId != null)
                {
                    model.Remove(payment);
                }
            }

            //Get all payments from db associated with user
            //display to user

            return View(model);
        }

        public ActionResult Refund(string transId)
        {
            Result<Transaction> result = PaymentGateways.Gateway.Transaction.Refund(transId);
            if (result.IsSuccess())
            {
                ViewBag.Message = "Refund of Transaction " + transId + " Successful";
                Payment payment = db.Payments.Single(p => p.TransactionId.Equals(transId));
                if (payment.Seats.Count != 0)
                {
                    foreach (Seat seat in payment.Seats)
                    {
                        seat.Status = Seat.SeatBookingStatus.Free;
                    }
                }
                db.SaveChanges();
                return View("RefundSuccess");
            }
            else
            {
                ViewBag.Message = "";
                List<ValidationError> errors = result.Errors.DeepAll();
                foreach (var error in errors)
                {
                    ViewBag.Message = ViewBag.Message + error.Message + " ";
                }
                return RedirectToAction("PaymentHistory");
            }
        }

        public ActionResult RefundSuccess()
        {
            var model = ViewBag.Message;
            return View(model: model);
        }

        //// GET: Payments/Create
        //public ActionResult Create()
        //{
        //    ViewBag.UserID = new SelectList(db.Users, "Id", "FirstName");
        //    return View();
        //}

        //// POST: Payments/Create
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Create([Bind(Include = "PaymentID,TransactionID,PaymentDate,PaymentAmount,PaymentDescription,UserID")] Payment payment)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Payments.Add(payment);
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }

        //    ViewBag.UserID = new SelectList(db.Users, "Id", "FirstName", payment.UserID);
        //    return View(payment);
        //}

        //// GET: Payments/Edit/5
        //public ActionResult Edit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Payment payment = db.Payments.Find(id);
        //    if (payment == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    ViewBag.UserID = new SelectList(db.Users, "Id", "FirstName", payment.UserID);
        //    return View(payment);
        //}

        //// POST: Payments/Edit/5
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Edit([Bind(Include = "PaymentID,TransactionID,PaymentDate,PaymentAmount,PaymentDescription,UserID")] Payment payment)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Entry(payment).State = EntityState.Modified;
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }
        //    ViewBag.UserID = new SelectList(db.Users, "Id", "FirstName", payment.UserID);
        //    return View(payment);
        //}

        //// GET: Payments/Delete/5
        //public ActionResult Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Payment payment = db.Payments.Find(id);
        //    if (payment == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(payment);
        //}

        //// POST: Payments/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(int id)
        //{
        //    Payment payment = db.Payments.Find(id);
        //    db.Payments.Remove(payment);
        //    db.SaveChanges();
        //    return RedirectToAction("Index");
        //}

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
