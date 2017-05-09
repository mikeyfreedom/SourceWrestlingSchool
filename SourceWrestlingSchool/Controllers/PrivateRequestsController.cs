﻿using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using SourceWrestlingSchool.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;

namespace SourceWrestlingSchool.Controllers
{
    public class PrivateRequestsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: PrivateRequests
        public ActionResult Index()
        {
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
            var userID = userManager.FindByEmail(HttpContext.User.Identity.Name).Id;
            var model = db.PrivateSessions.Where(i => i.InstructorID == userID);

            return View(model);
        }

        [HttpPost]
        public ActionResult Accept()
        {
            //Get the ID of the session request that was selected
            string sID = Request.Form.Get("sessionID");
            int sessionID = int.Parse(sID);
            
            using (db)
            {
                //Retrieve the session object
                var session = (from p in db.PrivateSessions
                               where p.PrivateSessionID == sessionID
                               select p).FirstOrDefault();
                //Update its status
                session.Status = PrivateSession.RequestStatus.Accepted;
                //Create a new lesson based on the session requests details
                Lesson lesson = new Lesson()
                {
                    ClassType = Lesson.LessonType.Private,
                    ClassStartDate = session.SessionStart,
                    ClassEndDate = session.SessionEnd,
                    ClassLevel = ClassLevel.Private,
                    ClassCost = 30,
                    InstructorName = (from i in db.Users
                                      where i.Id == session.InstructorID
                                      select i.FirstName).FirstOrDefault(),
                    Students = new List<ApplicationUser>()
                };
                
                //Retrieve the user object from the database based on the name attached to the request
                var user = (from u in db.Users
                            where u.FirstName == session.StudentName
                            select u).FirstOrDefault();

                //Add the student to the new lesson
                lesson.Students.Add(user);
                //Add the new lesson to the db
                db.Lessons.Add(lesson);
                //Save the changes to lessons and privates
                Payment privateFee = new Payment
                {
                    PaymentAmount = 30,
                    PaymentDescription = "Private Session Booking Fee",
                    PaymentDate = DateTime.Today,
                    User = user,
                    UserID = user.Id
                };
                db.Payments.Add(privateFee);
                db.SaveChanges();
                sendEmail(user.UserName, "accept");                
            }
            return RedirectToAction("Index","PrivateRequests");
        }

        [HttpPost]
        public ActionResult Refuse()
        {
            int sessionID = int.Parse(Request.Form.Get("sessionID"));
            using (db)
            {
                var request = (from p in db.PrivateSessions
                               where p.PrivateSessionID == sessionID
                               select p).FirstOrDefault();
                request.Status = PrivateSession.RequestStatus.Refused;
                db.SaveChanges();
                sendEmail(request.StudentName, "refuse");
            }
            return RedirectToAction("Index", "PrivateRequests");
        }

        public void sendEmail(string user,string reason)
        {
            MailMessage message = new MailMessage("lowlander_glen@yahoo.co.uk", user);
            if (reason.Equals("accept"))
            {
                message.Subject = "Private Session Request Accepted";
                message.Body = "You have successfully booked in for a class. We look forward to seeing you there!";

            }
            else if (reason.Equals("refuse"))
            {
                message.Subject = "Private Session Request Refused";
                message.Body = "Your requested instructor has refused the session.";
            }
            
            SmtpClient smtpClient = new SmtpClient();
            smtpClient.Send(message);
        }
    }
}