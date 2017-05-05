using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using SourceWrestlingSchool.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;

namespace SourceWrestlingSchool.Controllers
{
    public class UserController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: User
        public ActionResult Index()
        {
            var users = db.Users.ToList();
            var roles = db.Roles.ToList();
            UserViewModel userView = new UserViewModel();
            userView.Users = users;
            userView.Roles = roles;
            return View(userView);
        }

        public ActionResult AdjustClassLevel()
        {
            var users = db.Users.ToList();
            List<ApplicationUser> students = new List<ApplicationUser>();
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
            foreach (ApplicationUser user in users)
            {
                if (userManager.IsInRole(user.Id,RoleNames.ROLE_STUDENTUSER))
                {
                    students.Add(user);
                }
            }
            
            return View(students);
        }

        public void ExportStudentListToExcel()
        {
            var users = db.Users.ToList();
            var students = new List<ApplicationUser>();
            var grid = new System.Web.UI.WebControls.GridView();
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
            foreach (ApplicationUser user in users)
            {
                if (userManager.IsInRole(user.Id, RoleNames.ROLE_STUDENTUSER))
                {
                    students.Add(user);
                }
            }
            var studentQuery = from s in students
                               select new { s.FirstName, s.LastName, s.ClassLevel, s.Age, s.Email, s.DateJoinedSchool };
            grid.DataSource = studentQuery.ToList();                              
            grid.DataBind();

            Response.ClearContent();
            Response.AddHeader("content-disposition", "attachment; filename=StudentRoster.xls");
            Response.ContentType = "application/excel";
            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);

            grid.RenderControl(htw);

            Response.Write(sw.ToString());
            Response.End();
        }

        [HttpGet]
        public ActionResult EditClass(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ApplicationUser model = db.Users.Where(u => u.Id == id).First();
            if (model == null)
            {
                return HttpNotFound();
            }

            return View(model);
        }

        [HttpPost]
        public ActionResult EditClass([Bind(Include = "ClassLevel")] ApplicationUser user)
        {
            if (ModelState.IsValid)
            {
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("AdjustClassLevel");
            }
            return View(user);
        }
    }
}