using Braintree;
using SourceWrestlingSchool.Models;
using System.Diagnostics;
using System.Linq;
using System.Web.Mvc;

namespace SourceWrestlingSchool.Controllers
{
    public class SubscriptionController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: Subscription
        public ActionResult Index()
        {
            SubscriptionViewModel model = new SubscriptionViewModel();
            
            using (db)
            {
                ApplicationUser user = db.Users.First(u => u.UserName.Equals(User.Identity.Name));

                //Ternary operator to evaluate if a user has a subscription
                model.IsSubscribed = (user.MemberLevel != null) ? true : false;
                model.Username = user.FirstName + " " + user.LastName;
                var request = new CustomerSearchRequest().Email.Is(User.Identity.Name);
                ResourceCollection<Customer> collection = PaymentGateways.Gateway.Customer.Search(request);
                if (collection.Ids.Count != 0)
                {
                    foreach (var entry in collection.FirstItem.CreditCards)
                    {
                        foreach (var sub in entry.Subscriptions)
                        {
                            if (sub.Status == SubscriptionStatus.ACTIVE)
                            {
                                model.LastPaymentDate = sub.PaidThroughDate.Value.Date;
                                model.NextDueDate = sub.NextBillingDate.Value.Date;
                                model.SubscriptionID = sub.Id;
                            }                                                        
                        }
                    }
                }
            };
            

            return View(model);
        }

        [HttpGet]
        public ActionResult CreateSubscription(int id)
        {
            //check for customer, if exists, generate a token off them
            //if not, generate a generic and create the customer on post
            CreateCustomerViewModel model = new CreateCustomerViewModel {PlanID = id};

            using (db)
            {
                model.User = db.Users.First(u => u.UserName.Equals(User.Identity.Name));
            };

            var customerRequest = new CustomerSearchRequest().Email.Is(User.Identity.Name);
            ResourceCollection<Customer> collection = PaymentGateways.Gateway.Customer.Search(customerRequest);
            var clientToken = "";
            if (collection.Ids.Count != 0)
            {
                string custID = collection.FirstItem.Id;
                clientToken = PaymentGateways.Gateway.ClientToken.generate(
                    new ClientTokenRequest
                    {
                        CustomerId = custID
                    }
                );
            }
            else
            {
                clientToken = PaymentGateways.Gateway.ClientToken.generate();
            }
            ViewBag.ClientToken = clientToken;
            ViewBag.PlanID = model.PlanID;

            return View(model);
        }

        [HttpPost]
        public ActionResult CreateSubscription(FormCollection collection)
        {
            using (db)
            {
                string nonceFromTheClient = collection["payment_method_nonce"];
                string planid = collection["planid"];
                var customerRequest = new CustomerSearchRequest().Email.Is(User.Identity.Name);
                ResourceCollection<Customer> results = PaymentGateways.Gateway.Customer.Search(customerRequest);
                if (results.Ids.Count == 0)
                {
                    var user = db.Users.Single(u => u.Email == User.Identity.Name);
                    var request = new CustomerRequest
                    {
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Email = User.Identity.Name,
                        Phone = "",
                        CreditCard = new CreditCardRequest
                        {
                            PaymentMethodNonce = nonceFromTheClient,
                            Options = new CreditCardOptionsRequest
                            {
                                VerifyCard = true
                            }
                        }
                    };

                    Result<Customer> result = PaymentGateways.Gateway.Customer.Create(request);
                    if (!result.IsSuccess())
                    {
                        Debug.WriteLine(result.Message);
                        foreach (var error in result.Errors.All())
                        {
                            Debug.WriteLine(error.Message);
                        }
                        return View();
                    }
                    else
                    {
                        Customer customer = result.Target;
                        string customerId = customer.Id;
                        string cardToken = customer.PaymentMethods[0].Token;

                        var newSub = new SubscriptionRequest
                        {
                            PaymentMethodToken = cardToken,
                            PlanId = planid,
                            NeverExpires = true,
                            Options = new SubscriptionOptionsRequest
                            {
                                StartImmediately = true
                            }
                        };

                        Result<Subscription> subResult = PaymentGateways.Gateway.Subscription.Create(newSub);
                        if (!subResult.IsSuccess())
                        {
                            Debug.WriteLine(subResult.Message);
                            foreach (var error in subResult.Errors.All())
                            {
                                Debug.WriteLine(error.Message);
                            }
                            return View();
                        }
                        else
                        {
                            switch (int.Parse(planid))
                            {
                                case 1:
                                    user.MemberLevel = MemberLevel.Bronze;
                                    break;
                                case 2:
                                    user.MemberLevel = MemberLevel.Silver;
                                    break;
                                case 3:
                                    user.MemberLevel = MemberLevel.Gold;
                                    break;
                                default:
                                    Debug.WriteLine("Invalid Plan Number");
                                    break;
                            }

                            db.SaveChanges();
                            return View("Success");
                        }
                    }
                }
                else
                {
                    ApplicationUser user = db.Users.Single(u => u.Email == User.Identity.Name);
                    Customer customer = results.FirstItem;
                    string cardToken = customer.PaymentMethods[0].Token;

                    var newSub = new SubscriptionRequest
                    {
                        PaymentMethodToken = cardToken,
                        PlanId = planid,
                        NeverExpires = true,
                        Options = new SubscriptionOptionsRequest
                        {
                            StartImmediately = true
                        }
                    };

                    Result<Subscription> subResult = PaymentGateways.Gateway.Subscription.Create(newSub);
                    if (!subResult.IsSuccess())
                    {
                        Debug.WriteLine(subResult.Message);
                        foreach (var error in subResult.Errors.All())
                        {
                            Debug.WriteLine(error.Message);
                        }
                        return View();
                    }
                    else
                    {
                        switch (int.Parse(planid))
                        {
                            case 1:
                                user.MemberLevel = MemberLevel.Bronze;
                                break;
                            case 2:
                                user.MemberLevel = MemberLevel.Silver;
                                break;
                            case 3:
                                user.MemberLevel = MemberLevel.Gold;
                                break;
                            default:
                                Debug.WriteLine("Invalid Plan Number");
                                break;
                        }
                        db.SaveChanges();
                        return View("Index");
                    }
                }
            }
        }
    }
}