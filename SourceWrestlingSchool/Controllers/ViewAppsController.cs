using SourceWrestlingSchool.Models;
using System.Data.Entity;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

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
            ApplyViewModel application = db.Applications.Where(a => a.ApplicationId == id).Include(a => a.User).Single(); 
            Image source = Image.FromFile(Server.MapPath("/images/") + application.FileName);
            ImageFormat format = source.RawFormat;
            source.Dispose();
            ImageHelper.RotateImageByExifOrientationData(Server.MapPath("/images/") + application.FileName, Server.MapPath("/images/") + application.FileName,format);
            
            return View(application);
        }

        [HttpPost]
        public ActionResult Accept()
        {
            string aId = Request.Form["appID"];
            string userMail;
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
            int appId = int.Parse(aId);
            using (db)
            {
                var app = db.Applications
                            .Where(a => a.ApplicationId == appId)
                            .Include(a => a.User)
                            .Single();
                app.User.Age = app.Age;
                app.User.Height = app.Height;
                app.User.Weight = app.Weight;
                app.User.ClassLevel = ClassLevel.Beginner;
                app.Status = ApplyViewModel.ApplicationStatus.Accepted;
                userManager.RemoveFromRole(app.User.Id, RoleNames.ROLE_STANDARDUSER);
                userManager.AddToRole(app.User.Id, RoleNames.ROLE_STUDENTUSER);
                userMail = "" + app.User.Email;
                db.SaveChanges();
            }

            SendEmail("accept", userMail);    
            return View("Index");
        }

        [HttpPost]
        public ActionResult Refuse()
        {
            string aId = Request.Form["appID"];
            string userMail;
            int appId = int.Parse(aId);
            using (db)
            {
                var app = db.Applications
                            .Where(a => a.ApplicationId == appId)
                            .Include(a => a.User)
                            .Single(); 
                app.Status = ApplyViewModel.ApplicationStatus.Declined;
                userMail = "" + app.User.Email;
                db.SaveChanges();
            }
                
            SendEmail("refuse",userMail);
            return View("Index");
        }

        public void SendEmail(string reason, string userMail)
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