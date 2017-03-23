using SourceWrestlingSchool.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
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
            var applications = db.Applications;
            return View(applications.ToList());
        }

        // GET: ViewApps/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ApplyViewModel application = db.Applications.Find(id);
            application.User = db.Users.Find(application.UserID);
            Image source = Bitmap.FromFile(Server.MapPath("/images/") + application.FileName);
            ImageFormat format = source.RawFormat;
            source.Dispose();
            ImageHelper.RotateImageByExifOrientationData(Server.MapPath("/images/") + application.FileName, Server.MapPath("/images/") + application.FileName,format);
            //Image source = Bitmap.FromFile(Server.MapPath("/images/") + application.FileName);

            //Image tmpImage;
            //Bitmap returnImage;

            //using (var fs = new FileStream(Server.MapPath("/images/") + application.FileName, FileMode.Open, FileAccess.Read))
            //{
            //    tmpImage = Image.FromStream(fs);
            //    returnImage = new Bitmap(tmpImage);
            //    tmpImage.Dispose();
            //}
            
            //System.IO.File.Delete(Server.MapPath("/images/") + application.FileName);
            //foreach (int i in returnImage.PropertyIdList)
            //{
            //    returnImage.RemovePropertyItem(i);
            //}

            //try
            //{
            //    returnImage.Save(Server.MapPath("/images/") + application.FileName, returnImage.RawFormat);
            //}
            //catch (Exception ex)
            //{
            //    Console.Write(ex.Message);
            //}

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
            int appID = int.Parse(aID);
            db.Applications.Find(aID).Status = ApplyViewModel.ApplicationStatus.Accepted;
            return View();
        }

        [HttpPost]
        public ActionResult Refuse()
        {
            string aID = Request.Form["appID"];
            int appID = int.Parse(aID);
            db.Applications.Find(aID).Status = ApplyViewModel.ApplicationStatus.Declined;
            return View();
        }
    }
}