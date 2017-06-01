using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using System.Web.UI;
using SourceWrestlingSchool.Models;
using Braintree;

namespace SourceWrestlingSchool.Controllers
{
    /// <summary>
    ///     Controller to handle historical and outstanding payments
    /// </summary>
    public class PaymentsController : Controller
    {
        /// <summary>
        ///     Virtual representation of the database
        /// </summary>
        private readonly ApplicationDbContext _db = new ApplicationDbContext();

        /// <summary>
        ///     Get all payments listed in the database.
        ///     Attach the user who made the payment to each entity.
        ///     Send the list as a ViewModel to populate the Index View.
        /// </summary>
        /// <returns>The index view, along with the list of attached payments</returns>
        // GET: Payments
        public ActionResult Index()
        {
            var payments = _db.Payments.Include(p => p.User);
            return View(payments.ToList());
        }

        /// <summary>
        ///     Show the details of a particular payment.
        /// </summary>
        /// <remarks>
        ///     If no id is sent, display a bad request error page.
        ///     Else
        ///         Get the payement from the database with a matching id.
        ///         Attach the user associated with the payment entity.
        ///         Send the payment as the model to populate the Detail view.
        /// </remarks>
        /// <param name="id">The id of the payment entity</param>
        /// <returns>The Payment Detail View, with a Payment model parameter</returns>
        // GET: Payments/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Payment payment = _db.Payments.Where(p => p.PaymentId == id).Include(u => u.User).Single();
            payment.UserId = payment.User.Id;
            
            return View(payment);
        }

        /// <summary>
        ///     Display all outstanding payments of the current user 
        /// </summary>
        /// <remarks>
        ///     Retrieve all non-settled payments from the database, that have the user's email address attached to them.
        ///     If the page request came from the payment gateway, load the notification message sent into a TempData object.
        ///     Load the Outstaning Payment list, with the payment list as a sent model to populate it.
        /// </remarks>
        /// <returns>The Ouststanding Payment view, with the filtered user's own payments as the send model. </returns>
        public ActionResult Outstanding()
        { 
            using (_db)
            {
                //var user = _db.Users.Single(u => u.Email == User.Identity.Name);
                //var model = _db.Payments
                //            .Where(p => p.UserId == user.Id && p.PaymentSettled == false)
                //            .Include(p => p.User)
                //            .ToList();
                var email = User.Identity.Name;
                var model = _db.Payments
                            .Where(p => p.User.Email == email && p.PaymentSettled == false)
                            .Include(p => p.User)
                            .ToList();
                ViewBag.Message = TempData["message"];
                return View(model);
            }
        }

        /// <summary>
        ///     Load a payment gateway to settle an outstanding payment. 
        /// </summary>
        /// <remarks>
        ///     Retrieve the payment details with a matching paymentID as the passed parameter.
        ///     Search the Braintree payment gateway to check if the user is an existing customer in the Vault.
        ///     If a match is found
        ///         Generate a client token using the customer's vault ID.s
        ///     If no match exists
        ///         Use the gateway to generate a token for the checkout session.
        ///     In either case, store the clientToken in a ViewBag for clientside use. 
        /// </remarks>
        /// <param name="paymentId">The id of the payment to be processed</param>
        /// <returns> The SendPayment view, sending the Payment entity as the viewmodel</returns>
        [HttpGet]
        public ActionResult SendPayment(int? paymentId)
        {
            if (paymentId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Payment payment = _db.Payments
                              .Where(p => p.PaymentId == paymentId)
                              .Include(u =>u.User)
                              .Single();
            if (payment == null)
            {
                return HttpNotFound();
            }

            //Generate payment token
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

        /// <summary>
        ///     Process an attempt at payment.
        /// </summary>
        /// <remarks>
        ///     Collect the paymentID, payment nonce, and transaction amount from the client checkout form.
        ///     Create a new Transaction request using the retrieved data
        ///     Submit the transaction for settlement and receive a result object from the gateway.
        ///     If the payment is successful
        ///         Retrive the payment details from the database
        ///         Set the payment status to settled
        ///         Create a postive notification
        ///         Save the change to the database
        ///         Load the outstanding payment list
        ///     Else
        ///         Use the list of errors in the result to create a negative notification
        ///         Refresh the page to highlght the payment failure
        /// </remarks>
        /// <param name="collection">The collection of data from the checkout form</param>
        /// <returns>
        ///     The Outstanding Payment list if the payment was successful.
        ///     Refresh for the current page if the payment failed.
        /// </returns>
        [HttpPost]
        public ActionResult SendPayment(FormCollection collection)
        {
             using (_db)
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
                    Payment payment = _db.Payments
                                      .Where(p => p.PaymentId == pId && p.PaymentSettled == false)
                                      .Include(p => p.User)
                                      .Single();

                    payment.PaymentSettled = true;
                    payment.TransactionId = result.Target.Id;
                    
                    TempData["message"] = "Payment Successful.";
                    _db.SaveChanges();
                    return RedirectToAction("Outstanding");
                }
                
                TempData["message"] = "";
                foreach (var error in result.Errors.All())
                {
                    TempData["message"] = TempData["message"] + error.Message + " ";
                }
                _db.SaveChanges();

                return Redirect(HttpContext.Request.UrlReferrer.AbsoluteUri);
            }
        }

        /// <summary>
        ///     Display a user's purchase history.
        /// </summary>
        /// <remarks>
        ///     Retrieve all payments from the database that have the user's email attached to them.
        ///     Remove any payments that have been previously refunded.
        ///     Load the PaymentHistory view.
        /// </remarks>
        /// <returns>A view displayng all previous settled purchases.</returns>
        public ActionResult PaymentHistory()
        {
            var model = _db.Payments
                        .Where(p => p.User.Email == User.Identity.Name && p.PaymentSettled)
                        .ToList();
            var user = _db.Users.Single(u => u.Email == User.Identity.Name);
            ViewBag.Username = user.FirstName + " " + user.LastName;
            foreach (Payment payment in model)
            {
                Transaction transaction = PaymentGateways.Gateway.Transaction.Find(payment.TransactionId);
                if (transaction.RefundedTransactionId != null)
                {
                    model.Remove(payment);
                }
            }
            
            return View(model);
        }

        /// <summary>
        ///     Refund a previous purchase
        /// </summary>
        /// <remarks>
        ///     Create a refund request using the passed in ID and receive a result object.
        ///     If the result is successful
        ///         Check if the payment was an event booking.
        ///         If so, release the seats attached to the booking.
        ///         Load the success view.
        ///     Else
        ///         Create a failure notification using the error list in the result object.
        ///         Save the notification to a TempData object.
        ///         Load the Payment History list View to reflect the notification.
        ///         
        /// </remarks>
        /// <param name="transId">The Braintree transactionID of the payment</param>
        /// <returns>
        ///     A refund success view if the refund was successful
        ///     The PaymentHistory view if the refund failed
        /// </returns>
        public ActionResult Refund(string transId)
        {
            Result<Transaction> result = PaymentGateways.Gateway.Transaction.Refund(transId);
            if (result.IsSuccess())
            {
                ViewBag.Message = "Refund of Transaction " + transId + " Successful";
                Payment payment = _db.Payments.Single(p => p.TransactionId.Equals(transId));
                if (payment.Seats.Count != 0)
                {
                    foreach (Seat seat in payment.Seats)
                    {
                        seat.Status = Seat.SeatBookingStatus.Free;
                    }
                }
                _db.SaveChanges();
                return View("RefundSuccess");
            }
            
            ViewBag.Message = "";
            List<ValidationError> errors = result.Errors.DeepAll();
            foreach (var error in errors)
            {
                ViewBag.Message = ViewBag.Message + error.Message + " ";
            }
            return RedirectToAction("PaymentHistory");
            
        }

        /// <summary>
        ///     Notify user of a successful refund
        /// </summary>
        /// <remarks>
        ///     Takes in the ViewBag data saved during the payment process and saves it as the sending model.
        /// </remarks>
        /// <returns>The RefundSuccess view, sending the message string as the sending model</returns>
        public ActionResult RefundSuccess()
        {
            var model = ViewBag.Message;
            return View(model: model);
        }

        /// <summary>
        ///     Exports a list of outstanding payments to Excel.
        /// </summary>
        /// <remarks>
        ///     Retrieve all the unsettled payments in the database and add them to a list.
        ///     Instantiate a new GridView to hold the data and bind the payment list to it.
        ///     Create the response excel file.
        ///     Start a new StringWriter to write to the file.
        ///     Write the data to the file, then send it to the user as a download.
        /// </remarks>
        public void ExportOutstandingToExcel()
        {
            List<Payment> payments = _db.Payments.Where(p => p.PaymentSettled).ToList();

            var grid = new System.Web.UI.WebControls.GridView {DataSource = payments};
            grid.DataBind();

            Response.ClearContent();
            Response.AddHeader("content-disposition", "attachment; filename=StudentRoster.xls");
            Response.ContentType = "application/excel";
            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);

            grid.RenderControl(htw);

            Response.Write(sw.ToString());
            Response.End();
        }

        /// <summary>
        ///     Use the partial paymentPDF view to generate a PDF file and download it to the user.
        /// </summary>
        /// <remarks>
        ///     The Rotativa library loads the PaymentPdf view, and returns the HTML view as a PDF file for consumption.
        /// </remarks>
        /// <returns>A PDF named OutstandingPayments</returns>
        public ActionResult ExportOutstandingToPdf()
        {
            return new Rotativa.ViewAsPdf("PaymentPdf") { FileName = "OustandingPayments.pdf" };
        }

        /// <summary>
        ///     A list view of all outstanding payments, to be converted to a PDF list
        /// </summary>
        /// <returns>The list view of all outstanding payments </returns>
        public ActionResult PaymentPdf()
        {
            List<Payment> payments = _db.Payments.Where(p => !p.PaymentSettled).ToList();
            
            return View(payments);
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
                _db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
