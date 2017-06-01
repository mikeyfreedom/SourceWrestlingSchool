using SourceWrestlingSchool.Models;
using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace SourceWrestlingSchool.Controllers
{
    /// <summary>
    ///     Controller that handles all of the admin panel functions
    /// </summary>
    public class AdminController : Controller
    {
        /// <summary>
        ///     Virtual representation of the database
        /// </summary>
        private readonly ApplicationDbContext _db = new ApplicationDbContext();

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

        /// <summary>
        ///     Loads the StudentProfile view.
        /// </summary>
        /// <remarks>
        ///     Checks the database for an existing profile viewmodel for the user.
        ///     IF a profile model isn't found\n 
        ///         Get the user attributes from the database.\n 
        ///         Populate a new ProfileViewModel.\n 
        ///         Add the new model to the database.\n 
        ///         Save the changes to the DB.\n 
        ///     In either case, send the View with the profile model.\n 
        /// </remarks>
        /// <returns>The populated profile page of the student</returns>
        public ActionResult StudentProfile()
        {
            using (_db)
            {
                var model = _db.Profiles.FirstOrDefault(p => p.EmailAddress.Equals(User.Identity.Name));
                if (model == null)
                {
                    ApplicationUser student = _db.Users.Single(s => s.Email.Equals(User.Identity.Name));
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
                    _db.Profiles.Add(model);
                    _db.SaveChanges();
                }
                
                return View(model);
            }
        }

        /// <summary>
        ///     Loads the EditProfile View
        /// </summary>
        /// <remarks>
        ///     Get the profile from the database with a matching id as the input parameter
        ///     Send the View along with the profile model
        /// </remarks>
        /// <param name="id">The id number of the profile in the database</param>
        /// <returns>The EditProfile View, populated with the profile data</returns>
        public ActionResult EditProfile(int id)
        {
            var model = _db.Profiles.Single(p => p.ProfileId == id);
            
            return View(model);
        }

        /// <summary>
        ///     Takes in form values posted from the pageview and updated the relevant profile.
        /// </summary>
        /// <remarks>
        ///     Pull in form data from the view.\n 
        ///     Attach it to the profile.\n 
        ///     Check the profile model is valid.\n 
        ///     IF it is valid:\n 
        ///         Update the profile stored in the database.\n 
        ///         Save the changes to the DB.\n 
        ///         Load the Profile View.\n 
        ///     ELSE \n 
        ///         Reload the edit page. \n  
        /// </remarks>
        /// <param name="profile">The ProfileViewModel to be updated.</param>
        /// <param name="collection">The collection of form entries used to update the profile.</param>
        /// <returns>
        ///     StudentProfile View if the profile model is valid.\n 
        ///     EditProfile View if the model is not saved successfully.
        /// </returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditProfile([Bind(Include = "ProfileId,ProfileImageFileName,Name,Height,Weight,DateJoinedSchool,ClassLevel,FacebookUrl,TwitterUrl,InstagramUrl,EmailAddress,BioContent,YoutubeEmbedLink")] ProfileViewModel profile, FormCollection collection)
        {
            profile.FacebookUrl = Request.Form["facebookPage"];
            profile.TwitterUrl = Request.Form["twitterPage"];
            profile.YoutubeEmbedLink = Request.Form["youtubeLink"];

            if (ModelState.IsValid)
            {
                
                _db.Entry(profile).State = EntityState.Modified;
                _db.SaveChanges();
                return RedirectToAction("StudentProfile");
            }
            return View(profile);
        }

        /// <summary>
        ///     Loads the view that allows a user to edit their bio information.
        /// </summary>
        /// <remarks>
        ///     Get the bio content to be edited from the database.\n 
        ///     Add the id of the profile to a ViewBag for referencing in the view.\n 
        /// </remarks>
        /// <param name="id">The id number of the profile in the database.</param>
        /// <returns> The EditBio view with the content portion of the profile as the sending model.</returns>
        public ActionResult EditBio(int id)
        {
            var bio = _db.Profiles
                      .Where(p => p.ProfileId == id)
                      .Select(p => p.BioContent)
                      .Single();

            ViewBag.Id = id;

            return View(model : bio);
        }

        /// <summary>
        ///     Updates the BioContent attribute of the profile in question
        /// </summary>
        /// <remarks>
        ///     Take in the edited text and profile id from the form inputs.\n 
        ///     Get the profile from the database with the attached id.\n 
        ///     Update the BioContent attribute in the profile.\n 
        ///     Save the changes to the database.\n 
        ///     Load the updated StudentProfile View\n 
        /// </remarks>
        /// <param name="collection">Collection of the form inputs from the view</param>
        /// <returns>
        ///     StudentProfile View if the profile model is valid.
        /// </returns>
        [HttpPost, ValidateInput(false)]
        public ActionResult EditBio(FormCollection collection)
        {
            using (_db)
            {
                string bioText = HttpContext.Request.Unvalidated.Form["editor1"];
                int profileId = int.Parse(Request.Form["profileID"]);
                ProfileViewModel profile = _db.Profiles.Single(p => p.ProfileId == profileId);
                profile.BioContent = bioText;
                _db.SaveChanges();
            }

            return RedirectToAction("StudentProfile");
        }

        /// <summary>
        ///     Loads the InstructorProfile page
        /// </summary>
        /// <returns>The InstructorProfile view</returns>
        public ActionResult InstructorProfile()
        {
            return View();
        }

        /// <summary>
        ///     Loads the mockup Admin Profile View
        /// </summary>
        /// <returns>The AdminProfile view</returns>
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