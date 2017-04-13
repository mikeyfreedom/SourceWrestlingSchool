using SourceWrestlingSchool.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

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
    }
}