using SourceWrestlingSchool.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SourceWrestlingSchool.Controllers
{
    public class AdminController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

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
        public ActionResult StudentProfile(string id)
        {
            ApplicationUser student;

            using (db)
            {
                student = db.Users
                          .Where(s => s.Id == id)
                          .Single(); 
            };

            ProfileViewModel model = new ProfileViewModel();
            model.BioContent = student.BioContent;
            model.ClassLevel = (ClassLevel) student.ClassLevel;
            model.DateJoinedSchool = (DateTime) student.DateJoinedSchool;
            model.EmailAddress = student.Email;
            model.FacebookURL = student.FacebookURL;
            model.Height = (int) student.Height;
            model.InstagramURL = student.InstagramURL;
            model.Name = student.FirstName + " " + student.LastName;
            model.ProfileImageFileName = student.ProfileImageFileName;
            model.SlideshowImageFileNames = student.SlideshowImageFileNames.ToArray();
            model.TwitterURL = student.TwitterURL;
            model.Weight = (int) student.Weight;
            model.YoutubeEmbedLink = student.YoutubeEmbedLink;
            
            return View(model);
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