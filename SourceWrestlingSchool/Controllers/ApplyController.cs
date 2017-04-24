using SourceWrestlingSchool.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Net.Mail;

namespace SourceWrestlingSchool.Controllers
{
    public class ApplyController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: ApplyForm
        public ActionResult ApplyForm()
        {
            ApplyViewModel viewModel = new ApplyViewModel();
            //Build User & UserID
            string username = User.Identity.Name;
            using (db)
            {
                var user = (from u in db.Users
                             where u.UserName == username
                             select u).Single();

                viewModel.User = user;
                viewModel.UserID = user.Id;
            }
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ApplyForm([Bind(Include = "Age,Height,Weight,Notes,UserID")] ApplyViewModel model, HttpPostedFileBase uploadFile)
        {
            model.User = db.Users.Find(model.UserID);
            
            //Upload File to Directory
            if (uploadFile != null)
            {
                string path = Server.MapPath("/images/");
                string imagePath = "";
                try
                {
                    string extension = Path.GetExtension(uploadFile.FileName);
                    string currentTime = "" + DateTime.Now.Hour + DateTime.Now.Minute + DateTime.Now.Second + DateTime.Now.Millisecond;
                    imagePath = imagePath + model.User.FirstName + "_" + currentTime + "_ApplicationPhoto" + extension;
                    uploadFile.SaveAs(path + imagePath);
                }
                catch (Exception ex)
                {
                    Console.Write("ERROR:" + ex.Message.ToString());
                }
                model.FileName = imagePath;

            }
            
            //Save Application to DB
            if (ModelState.IsValid)
            {
                db.Applications.Add(model);
                db.SaveChanges();
                sendAppliedConfirmation(model.User);
                return RedirectToAction("Success");
            }
            
            return View();
        }

        private void sendAppliedConfirmation(ApplicationUser user)
        {
            MailMessage message = new MailMessage("lowlander_glen@yahoo.co.uk",user.UserName);
            message.Subject = "School Application Received";
            message.Body = "We have received your application to join the school on a student member basis.";
            SmtpClient smtpClient = new SmtpClient();
            smtpClient.Send(message);
        }

        public ActionResult Success()
        {
            return View();
        }
    }
}