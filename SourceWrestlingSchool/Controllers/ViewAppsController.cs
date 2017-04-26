using SourceWrestlingSchool.Models;
using System.Data.Entity;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;

namespace SourceWrestlingSchool.Controllers
{
    [Authorize(Roles = "Instructor")]
    public class ViewAppsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: ViewApps
        public ActionResult Index()
        {
            var applications = db.Applications.Include(x => x.User);
            return View(applications.ToList());
        }

        // GET: ViewApps/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ApplyViewModel application = db.Applications.Where(a => a.ApplicationID == id).Include(a => a.User).Single(); 
            //application.User = db.Users.Find(application.UserID);
            Image source = Bitmap.FromFile(Server.MapPath("/images/") + application.FileName);
            ImageFormat format = source.RawFormat;
            source.Dispose();
            ImageHelper.RotateImageByExifOrientationData(Server.MapPath("/images/") + application.FileName, Server.MapPath("/images/") + application.FileName,format);
            
            if (application == null)
            {
                return HttpNotFound();
            }
            return View(application);
        }

        [HttpPost]
        public ActionResult Accept()
        {
            string aID = Request.Form["appID"];
            string user = "";
            int appID = int.Parse(aID);
            using (db)
            {
                var app = db.Applications
                            .Where(a => a.ApplicationID == appID)
                            .Include(a => a.User)
                            .Single();
                app.User.Age = app.Age;
                app.User.Height = app.Height;
                app.User.Weight = app.Weight;
                app.User.ClassLevel = ClassLevel.Beginner;
                app.Status = ApplyViewModel.ApplicationStatus.Accepted;
                db.SaveChanges();
            }

            sendEmail("accept", user);    
            return View();
        }

        [HttpPost]
        public ActionResult Refuse()
        {
            string aID = Request.Form["appID"];
            string user = "";
            int appID = int.Parse(aID);
            using (db)
            {
                var app = db.Applications
                            .Where(a => a.ApplicationID == appID)
                            .Include(a => a.User)
                            .Single(); 
                app.Status = ApplyViewModel.ApplicationStatus.Declined;
                db.SaveChanges();
            }
                
            sendEmail("refuse",user);
            return View();
        }

        public void sendEmail(string reason, string userMail)
        {
            MailMessage message = new MailMessage("lowlander_glen@yahoo.co.uk", userMail);
            if (reason.Equals("accept"))
            {
                message.Subject = "Membership Approved";
                message.Body = "Congratulations, your membership has been approved. Welcome to the school!";

            }
            else if (reason.Equals("refuse"))
            {
                message.Subject = "Membership Refused";
                message.Body = "Your application has been received, but unfortunately you have been turned down for student membership";
            }

            SmtpClient smtpClient = new SmtpClient();
            smtpClient.Send(message);
        }
    }
}