using SourceWrestlingSchool.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SourceWrestlingSchool.Controllers
{
    public class ApplyController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: ApplyForm
        public ActionResult ApplyForm()
        {
            ApplyViewModel viewModel = new ApplyViewModel();
            viewModel.UserID = User.Identity.GetUserId();

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ApplyForm([Bind(Include = "Age,Height,Weight,Notes")] ApplyViewModel model)
        {
            model.UserID = HttpContext.User.Identity.GetUserId();
            model.User = db.Users.Find(model.UserID);
            model.Status = ApplyViewModel.ApplicationStatus.Open;
            if (ModelState.IsValid)
            {
                db.Applications.Add(model);
                db.SaveChanges();
                return RedirectToAction("Success");
            }
            return View();
        }

        public ActionResult Success()
        {
            return View();
        }
    }
}