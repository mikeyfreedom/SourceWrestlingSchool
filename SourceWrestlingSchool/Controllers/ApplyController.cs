using SourceWrestlingSchool.Models;
using System;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Net.Mail;

namespace SourceWrestlingSchool.Controllers
{
    /// <summary>
    ///     Controller that handles the student application process.
    /// </summary>
    public class ApplyController : Controller
    {
        /// <summary>
        ///     Virtual representation of the database
        /// </summary>
        private readonly ApplicationDbContext _db = new ApplicationDbContext();

        /// <summary>
        ///     Loads the student application form view
        /// </summary>
        /// <remarks>
        ///     Create a new instance of the ApplyViewModel
        ///     Get the current user details from the database
        ///     Add the user and their id to the ViewModel
        ///     Return the view with the viewmodel data
        /// </remarks>
        /// <returns>The ApplyForm view with attached user data</returns>
        // GET: ApplyForm
        [Authorize(Roles = "Standard_User")]
        public ActionResult ApplyForm()
        {
            ApplyViewModel viewModel = new ApplyViewModel();
            //Build User & UserID
            using (_db)
            {
                //var user = (from u in db.Users
                //             where u.UserName == User.Identity.Name
                //            select u).Single();
                var user = _db.Users.Single(u => u.UserName == User.Identity.Name);

                viewModel.User = user;
                viewModel.UserId = user.Id;
            }
            return View(viewModel);
        }

        /// <summary>
        ///     Create a new application and save it to the database for review
        /// </summary>
        /// <remarks>
        ///     Add the User from the Db with attached UserId to the application
        ///     Check to make sure there is a file attached and its content type is image
        ///     Rename the image file and save it to the images directory of the site
        ///         If the file does not upload, throw an error message
        ///     Add the full pathname of the image to the application
        ///     If the application model is valid
        ///         Save the completed application to the database
        ///         Save the changes to the database
        ///         Call the sendConfirmation method to send the user an email
        ///         Load the successful application view
        ///     If not valid
        ///         Refresh the application page, populated with the stored information
        /// </remarks>
        /// <param name="model">The attached ApplyViewModel to be saved to the database</param>
        /// <param name="uploadFile">The file included as part of the application</param>
        /// <returns>
        ///     Returns the application form view if the model or attached file is no valid
        ///     Returns the Success view if all criteria are met.
        /// </returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ApplyForm([Bind(Include = "Age,Height,Weight,Notes,UserID")] ApplyViewModel model, HttpPostedFileBase uploadFile)
        {
            model.User = _db.Users.Find(model.UserId);
            
            //Upload File to Directory
            if (uploadFile != null && uploadFile.ContentType.Contains("image"))
            {
                string path = Server.MapPath("/images/");
                string imagePath = "";
                try
                {
                    string extension = Path.GetExtension(uploadFile.FileName);
                    string currentTime = "" + DateTime.Now.Hour + DateTime.Now.Minute + DateTime.Now.Second +
                                         DateTime.Now.Millisecond;
                    imagePath = imagePath + model.User.FirstName + "_" + currentTime + "_ApplicationPhoto" + extension;
                    uploadFile.SaveAs(path + imagePath);
                }
                catch (Exception ex)
                {
                    Console.Write("ERROR:" + ex.Message);
                }
                model.FileName = imagePath;
            }
            else return View(model);
            
            //Save Application to DB
            if (ModelState.IsValid)
            {
                _db.Applications.Add(model);
                _db.SaveChanges();
                SendAppliedConfirmation(model.User.UserName);
                return RedirectToAction("Success");
            }
            
            return View(model);
        }

        /// <summary>
        ///     Send a confirmation email to the current user
        /// </summary>
        /// <remarks>
        ///     Create a new mail message stating successful application
        ///     Instantiate the smtpClient used to send the message
        ///     Send the message
        /// </remarks>
        /// <param name="email">The email address to send the confirmation to</param>
        private void SendAppliedConfirmation(string email)
        {
            MailMessage message =
                new MailMessage("lowlander_glen@yahoo.co.uk", email)
                {
                    Subject = "School Application Received",
                    Body = "We have received your application to join the school on a student member basis."
                };
            SmtpClient smtpClient = new SmtpClient();
            try
            {
                smtpClient.Send(message);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        /// <summary>
        ///     Loads the Success view
        /// </summary>
        /// <returns>The Success View</returns>
        public ActionResult Success()
        {
            return View();
        }
    }
}