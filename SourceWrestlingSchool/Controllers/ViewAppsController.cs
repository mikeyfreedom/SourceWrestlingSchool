using SourceWrestlingSchool.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace SourceWrestlingSchool.Controllers
{
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