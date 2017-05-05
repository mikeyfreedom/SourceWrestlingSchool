using Braintree;
using SourceWrestlingSchool.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SourceWrestlingSchool.Controllers
{
    public class SubscriptionController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private BraintreeGateway gateway = new BraintreeGateway();
        // GET: Subscription
        public ActionResult Index(string username)
        {
            SubscriptionViewModel model = new SubscriptionViewModel();
            var request = new CustomerSearchRequest().Email.Is(username);
            ResourceCollection<Customer> collection = gateway.Customer.Search(request);
            foreach (Customer person in collection)
            {
                foreach (var method in person.PaymentMethods)
                {
                                        
                }
            }
            

            using (db)
            {
                ApplicationUser user = db.Users
                                       .Where(u => u.UserName.Equals(username))
                                       .First();
            };

            return View(model);
        }

        [HttpGet]
        public ActionResult CreateSubscription(int id)
        {
            //Create a customer
            //Create a subscription
            CreateCustomerViewModel model = new CreateCustomerViewModel();
            model.PlanID = id;
            
            using (db)
            {
                model.User = db.Users
                             .Where(u => u.UserName.Equals(User.Identity.Name))
                             .First();
            };

            var customerRequest = new CustomerSearchRequest().Email.Is(User.Identity.Name);
            ResourceCollection<Customer> collection = gateway.Customer.Search(customerRequest);
            var clientToken = "";
            if (collection != null)
            {
                string custID = collection.FirstItem.Id;
                clientToken = gateway.ClientToken.generate(
                    new ClientTokenRequest
                    {
                        CustomerId = custID
                    }
                );
            }
            else
            {
                clientToken = gateway.ClientToken.generate();
            }
            ViewBag.ClientToken = clientToken;
            ViewBag.PlanID = model.PlanID;

            return View(model);
        }

        [HttpPost]
        public ActionResult CreateSubscription(FormCollection collection)
        {
            string nonceFromTheClient = collection["payment_method_nonce"];
            var request = new CustomerRequest
            {
                FirstName = "",
                LastName = "",
                Email = "",
                Phone = "",
                PaymentMethodNonce = nonceFromTheClient,
            };
            
            Result<Customer> result = gateway.Customer.Create(request);

            bool success = result.IsSuccess();
            // true

            Customer customer = result.Target;
            string customerId = customer.Id;
            // e.g. 160923

            string cardToken = customer.PaymentMethods[0].Token;
            // e.g. f28w

            var newSub = new SubscriptionRequest
            {
                PaymentMethodToken = cardToken,
                PlanId = ViewBag.PlanID
            };

            Result<Subscription> subResult = gateway.Subscription.Create(newSub);

            if (subResult.IsSuccess())
            {
                
            }
            else
            {
                Console.WriteLine(subResult.Message);
            }
            
            return View();
        }
    }
}