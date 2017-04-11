using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SourceWrestlingSchool.Controllers
{
    public class AdminController : Controller
    {
        // GET: Admin
        public ActionResult DashboardV1()
        {
            return View();
        }
        public ActionResult DashboardV2()
        {
            return View();
        }
        public ActionResult UserProfile()
        {
            return View();
        }
        public ActionResult StudentProfile()
        {
            return View();
        }
        public ActionResult InstructorProfile()
        {
            return View();
        }
        public ActionResult AdminProfile()
        {
            return View();
        }
        public ActionResult StaffProfile()
        {
            return View();
        }
    }
}