using Braintree;
using SourceWrestlingSchool.Models;
using System.Diagnostics;
using System.Linq;
using System.Web.Mvc;

namespace SourceWrestlingSchool.Controllers
{
    /// <summary>
    ///     Controller to handles all actions regarding student monthly subscriptions
    /// </summary>
    public class SubscriptionController : Controller
    {
        /// <summary>
        ///     Virtual representation of the database.
        /// </summary>
        private readonly ApplicationDbContext _db = new ApplicationDbContext();

        /// <summary>
        ///     Display a student user's subscription information, or offer a number of membership plans
        /// </summary>
        /// <remarks>
        ///     Create a new Subscription model
        ///     Get the current user's details from the database
        ///     Check to see if the user has a subscription.
        ///     If the user is subscribed,
        ///          Checks the financial gateway for the matching customer record as the user.
        ///          When the match is found, add the relevant subscription information to the data model.
        ///     Send the model along with the View
        /// </remarks>
        /// <returns>
        ///     The details of the student's subscription if the student has a membership.
        ///     The list of membership plans available if not subscribed.
        /// </returns>
        // GET: Subscription
        public ActionResult Index()
        {
            SubscriptionViewModel model = new SubscriptionViewModel();
            
            using (_db)
            {
                ApplicationUser user = _db.Users.First(u => u.UserName.Equals(User.Identity.Name));

                //Ternary operator to evaluate if a user has a subscription
                model.IsSubscribed = (user.MemberLevel != null);
                if (model.IsSubscribed)
                {
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
                                    model.SubscriptionId = sub.Id;
                                }
                            }
                        }
                    }
                }
            }
            return View(model);
        }

        /// <summary>
        ///     Load the view to set up a new subscription agreement.
        /// </summary>
        /// <remarks>
        ///     Instantiate a new ViewModel, using the id parameter of the subscription plan to be started.
        ///     Get the current user's details from the database.
        ///     Search the Braintree payment gateway to check if the user is an existing customer in the Vault.
        ///     If a match is found
        ///         Generate a client token using the customer's vault ID.s
        ///     If no match exists
        ///         Use the gateway to generate a token for the checkout session.
        ///     In either case, store the clientToken in a ViewBag for clientside use.
        ///     Send the ViewModel along with the ViewBag saved token and plan number.
        /// </remarks>
        /// <param name="id">The id number of the subscription plan to be started</param>
        /// <returns>The view which includes a checkout form to start an agreement, and the agreement details</returns>
        [HttpGet]
        public ActionResult CreateSubscription(int id)
        {
            //check for customer, if exists, generate a token off them
            //if not, generate a generic token and create the customer on post
            CreateCustomerViewModel model = new CreateCustomerViewModel {PlanId = id};

            using (_db)
            {
                model.User = _db.Users.Single(u => u.Email == User.Identity.Name);
            }

            //Token generation 
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
            ViewBag.PlanID = model.PlanId;

            return View(model);
        }

        /// <summary>
        ///     Takes in data from the checkout form on the view, and processes a new subscription.
        /// </summary>
        /// <remarks>
        ///     Pull in the payment nonce and planID from the checkout form.
        ///     Get the user's details from the database.
        ///     Search for a customer record in the Braintree vault database.
        ///     If no matches exist,
        ///         Create a new customer using the pulled in payment nonce and user details.
        ///         Create a cardToken paymentmethod using the card details associated with the nonce.
        ///     In any case, check there is a cardToken available.
        ///     If there is no token, then there already was a Vaulted customer , so create the subscription request using the nonce itself).
        ///     Otherwise, use the newly generated cardToken from the new customer to create the subscription request.
        ///     In either case, submit a create request and receive a result object.
        ///     If the submission was successful, set the member level attribute of the user to the appropriate level.
        ///     If not successful, refresh the checkout page with a negative notification.         
        /// </remarks>
        /// <param name="collection">The data values gained from the form entries on the View page</param>
        /// <returns>
        ///     Refresh of the current page if the subscription request fails.
        ///     The Subscription Index page if the subscription request is successful.
        /// </returns>
        [HttpPost]
        public ActionResult CreateSubscription(FormCollection collection)
        {
            using (_db)
            {
                string nonceFromTheClient = collection["payment_method_nonce"];
                string planid = collection["planid"];
                string cardToken = "";
                var user = _db.Users.Single(u => u.Email == User.Identity.Name);
                
                //Search for customer
                var customerRequest = new CustomerSearchRequest().Email.Is(User.Identity.Name);
                ResourceCollection<Customer> results = PaymentGateways.Gateway.Customer.Search(customerRequest);
                if (results.Ids.Count == 0)
                {
                    //  If no result, create the customer, create cardToken
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
                        TempData["message"] = result.Message;
                        return Redirect(HttpContext.Request.UrlReferrer.AbsoluteUri);
                    }
                    
                    cardToken = result.Target.PaymentMethods[0].Token;
                }

                SubscriptionRequest newSub;
                if (!cardToken.Equals(""))
                {
                    newSub = new SubscriptionRequest
                    {
                        PaymentMethodToken = cardToken,
                        PlanId = planid,
                        NeverExpires = true,
                        Options = new SubscriptionOptionsRequest
                        {
                            StartImmediately = true
                        }
                    };
                }
                else
                {
                    newSub = new SubscriptionRequest
                    {
                        PaymentMethodNonce = nonceFromTheClient,
                        PlanId = planid,
                        NeverExpires = true,
                        Options = new SubscriptionOptionsRequest
                        {
                            StartImmediately = true
                        }
                    };
                }
                
                Result<Subscription> subResult = PaymentGateways.Gateway.Subscription.Create(newSub);
                if (!subResult.IsSuccess())
                {
                    TempData["message"] = subResult.Message;

                    return Redirect(HttpContext.Request.UrlReferrer.AbsoluteUri);
                }

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
                _db.SaveChanges();
                TempData["message"] = "success";
                return View("Index");
            }
        }
    }
}