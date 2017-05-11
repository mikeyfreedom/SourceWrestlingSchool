using SourceWrestlingSchool.Models;
using System;
using System.Linq;
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
                          .Single(s => s.Id == id); 
            };

            ProfileViewModel model = new ProfileViewModel
            {
                BioContent = student.BioContent,
                ClassLevel = (ClassLevel) student.ClassLevel,
                DateJoinedSchool = (DateTime) student.DateJoinedSchool,
                EmailAddress = student.Email,
                FacebookURL = student.FacebookURL,
                Height = (int) student.Height,
                InstagramURL = student.InstagramURL,
                Name = student.FirstName + " " + student.LastName,
                ProfileImageFileName = student.ProfileImageFileName,
                SlideshowImageFileNames = student.SlideshowImageFileNames.ToArray(),
                TwitterURL = student.TwitterURL,
                Weight = (int) student.Weight,
                YoutubeEmbedLink = student.YoutubeEmbedLink
            };

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