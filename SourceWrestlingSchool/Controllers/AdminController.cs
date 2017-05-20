using SourceWrestlingSchool.Models;
using System;
using System.Data.Entity;
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
        public ActionResult StudentProfile()
        {
            using (db)
            {
                var model = db.Profiles.FirstOrDefault(p => p.EmailAddress.Equals(User.Identity.Name));
                if (model == null)
                {
                    ApplicationUser student = db.Users.Single(s => s.Email.Equals(User.Identity.Name));
                    model = new ProfileViewModel
                    {
                        BioContent = student.BioContent,
                        ClassLevel = (ClassLevel) student.ClassLevel,
                        DateJoinedSchool = (DateTime) student.DateJoinedSchool,
                        EmailAddress = student.Email,
                        FacebookUrl = student.FacebookUrl,
                        Height = (int) student.Height,
                        InstagramUrl = student.InstagramUrl,
                        Name = student.FirstName + " " + student.LastName,
                        ProfileImageFileName = student.ProfileImageFileName,
                        TwitterUrl = student.TwitterUrl,
                        Weight = (int) student.Weight,
                        YoutubeEmbedLink = student.YoutubeEmbedLink
                    };
                    db.Profiles.Add(model);
                    db.SaveChanges();
                }
                
                return View(model);
            }
        }

        public ActionResult EditProfile(int id)
        {
            var model = db.Profiles.Single(p => p.ProfileId == id);
            
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditProfile([Bind(Include = "ProfileId,ProfileImageFileName,Name,Height,Weight,DateJoinedSchool,ClassLevel,FacebookURL,TwitterURL,InstagramURL,EmailAddress,BioContent,YoutubeEmbedLink")] ProfileViewModel profile)
        {
            if (ModelState.IsValid)
            {
                db.Entry(profile).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("StudentProfile");
            }
            return View(profile);
        }

        public ActionResult EditBio(int id)
        {
            var bio = db.Profiles
                      .Where(p => p.ProfileId == id)
                      .Select(p => p.BioContent)
                      .Single();

            ViewBag.Id = id;

            return View(model : bio);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult EditBio(FormCollection collection)
        {
            using (db)
            {
                string bioText = HttpContext.Request.Unvalidated.Form["editor1"];
                int profileId = int.Parse(Request.Form["profileID"]);
                ProfileViewModel profile = db.Profiles.Single(p => p.ProfileId == profileId);
                profile.BioContent = bioText;
                db.SaveChanges();
            }

            return RedirectToAction("StudentProfile");
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