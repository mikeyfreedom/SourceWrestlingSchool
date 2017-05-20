﻿using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using SourceWrestlingSchool.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity;

namespace SourceWrestlingSchool.Controllers
{
    public class LessonsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Lessons
        public ActionResult Index()
        {
            var model = db.Lessons.Include(l => l.Students).ToList();
            return View(model);
        }

        // GET: Lessons/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Lesson lesson = db.Lessons
                            .Where(l => l.LessonId == id)
                            .Include(l => l.Students)
                            .Single();
            if (lesson == null)
            {
                return HttpNotFound();
            }
            return View(lesson);
        }
        
        // GET: Lessons/Create
        public ActionResult Create()
        {
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(db));
            var instructorRole = roleManager.Roles.Single(r => r.Name == RoleNames.ROLE_INSTRUCTOR);
            List<ApplicationUser> instructors = new List<ApplicationUser>(); 
            foreach (var user in instructorRole.Users)
            {
                instructors.Add(userManager.FindById(user.UserId));                
            }
            ViewBag.Instructors = instructors;
            return View();
        }

        // POST: Lessons/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "LessonID,ClassType,ClassLevel,ClassStartDate,ClassEndDate,ClassCost,InstructorName")] Lesson lesson)
        {
            if (ModelState.IsValid)
            {
                db.Lessons.Add(lesson);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(lesson);
        }

        // GET: Lessons/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Lesson lesson = db.Lessons
                            .Where(l => l.LessonId == id)
                            .Include(l => l.Students)
                            .Single(); 
            if (lesson == null)
            {
                return HttpNotFound();
            }

            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(db));
            var instructorRole = roleManager.Roles.Single(r => r.Name == RoleNames.ROLE_INSTRUCTOR);
            List<ApplicationUser> instructors = new List<ApplicationUser>();
            foreach (var user in instructorRole.Users)
            {
                instructors.Add(userManager.FindById(user.UserId));
            }
            ViewBag.Instructors = instructors;
            return View(lesson);
        }

        // POST: Lessons/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "LessonID,ClassType,ClassLevel,ClassStartDate,ClassEndDate,ClassCost,InstructorName")] Lesson lesson)
        {
            if (ModelState.IsValid)
            {
                db.Entry(lesson).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(lesson);
        }

        // GET: Lessons/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Lesson lesson = db.Lessons
                            .Where(l => l.LessonId == id)
                            .Include(l => l.Students)
                            .Single();
            if (lesson == null)
            {
                return HttpNotFound();
            }
            return View(lesson);
        }

        // POST: Lessons/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Lesson lesson = db.Lessons
                            .Where(l => l.LessonId == id)
                            .Include(l => l.Students)
                            .Single();
            db.Lessons.Remove(lesson);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult PrintClassAttendanceSheet()
        {
           
            return View("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
