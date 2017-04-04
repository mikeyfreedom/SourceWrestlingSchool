using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SourceWrestlingSchool.Models
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Lesson> Lessons { get; set; }
        public DbSet<LiveEvent> LiveEvents { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<PaymentMethod> PaymentMethods { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Seat> Seats { get; set; }
        public DbSet<ApplyViewModel> Applications { get; set; }
                
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
            Database.SetInitializer<ApplicationDbContext>(new ApplicationDbInitializer());
        }
        
        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        public System.Data.Entity.DbSet<SourceWrestlingSchool.Models.PrivateSession> PrivateSessions { get; set; }
    }

    public class ApplicationDbInitializer : DropCreateDatabaseAlways<ApplicationDbContext>
    {
        protected override void Seed(ApplicationDbContext context)
        {
            if (!context.Users.Any())
            {
                var roleStore = new RoleStore<IdentityRole>(context);
                var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
                var userStore = new UserStore<ApplicationUser>(context);
                var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));

                //Populate Role Table
                if (!roleManager.RoleExists(RoleNames.ROLE_ADMINISTRATOR))
                {
                    var roleResult = roleManager.Create(new IdentityRole(RoleNames.ROLE_ADMINISTRATOR));
                }

                if (!roleManager.RoleExists(RoleNames.ROLE_STAFFMEMBER))
                {
                    var roleResult = roleManager.Create(new IdentityRole(RoleNames.ROLE_STAFFMEMBER));
                }

                if (!roleManager.RoleExists(RoleNames.ROLE_STUDENTUSER))
                {
                    var roleResult = roleManager.Create(new IdentityRole(RoleNames.ROLE_STUDENTUSER));
                }

                if (!roleManager.RoleExists(RoleNames.ROLE_INSTRUCTOR))
                {
                    var roleResult = roleManager.Create(new IdentityRole(RoleNames.ROLE_INSTRUCTOR));
                }

                if (!roleManager.RoleExists(RoleNames.ROLE_STANDARDUSER))
                {
                    var roleResult = roleManager.Create(new IdentityRole(RoleNames.ROLE_STANDARDUSER));
                }

                string userName = "admin@admin.com";
                string password = "123456";

                //No need to hash passwords using this initializer method.
                //If using migration strategy, use PasswordHasher to hash the password.

                //Create Admin user and role
                var user = userManager.FindByName(userName);
                if (user == null)
                {
                    var newUser = new ApplicationUser()
                    {
                        FirstName = "Mr",
                        LastName = "Admin",
                        PhoneNumber = "01419999999",
                        MobileNumber = "079999999999",
                        Address = "1 Main Street",
                        Town = "Glasgow",
                        Postcode = "G1 A99",
                        ClassLevel = ClassLevel.Open,
                        UserName = userName,
                        Email = userName,
                        EmailConfirmed = true

                    };
                    userManager.Create(newUser, password);
                    userManager.AddToRole(newUser.Id, RoleNames.ROLE_ADMINISTRATOR);

                    password = "freedom78";
                    var newStandardUser = new ApplicationUser()
                    {
                        FirstName = "Michael",
                        LastName = "Devlin",
                        PhoneNumber = "01389999999",
                        MobileNumber = "07595543816",
                        Address = "187 Crosslet Road",
                        Town = "Dumbarton",
                        Postcode = "G82 2LQ",
                        UserName = "lowlander_glen@yahoo.co.uk",
                        Email = "lowlander_glen@yahoo.co.uk",
                        EmailConfirmed = true

                    };
                    userManager.Create(newStandardUser, password);
                    userManager.AddToRole(newStandardUser.Id, RoleNames.ROLE_STANDARDUSER);

                    password = "123456";
                    var newInstructor = new ApplicationUser()
                    {
                        FirstName = "Test",
                        LastName = "Instructor",
                        PhoneNumber = "01419999999",
                        MobileNumber = "07999999998",
                        Address = "187 Crosslet Road",
                        Town = "Dumbarton",
                        Postcode = "G82 2LQ",
                        UserName = "test@test.com",
                        Email = "test@test.com",
                        EmailConfirmed = true,
                        ClassLevel = ClassLevel.Open

                    };

                    userManager.Create(newInstructor, password);
                    userManager.AddToRole(newInstructor.Id, RoleNames.ROLE_INSTRUCTOR);
                }

                //Seed Lessons
                DateTime startDate = new DateTime(2017, 3, 25);
                DateTime endDate = new DateTime(2017, 4, 30);

                for (DateTime date = startDate; date.Date <= endDate.Date; date = date.AddDays(1))
                {
                    Lesson lesson = null;
                    if (date.DayOfWeek == DayOfWeek.Sunday)
                    {
                        lesson = new Lesson()
                        {
                            ClassType = Lesson.LessonType.Group,
                            ClassStartDate = new DateTime(date.Year, date.Month, date.Day, 13, 00, 00),
                            ClassEndDate = new DateTime(date.Year, date.Month, date.Day, 17, 00, 00),
                            ClassCost = 10,
                            ClassLevel = ClassLevel.Beginner,
                            Students = new List<ApplicationUser>()
                        };
                    }
                    else if (date.DayOfWeek == DayOfWeek.Tuesday)
                    {
                        lesson = new Lesson()
                        {
                            ClassType = Lesson.LessonType.Group,
                            ClassStartDate = new DateTime(date.Year, date.Month, date.Day, 18, 00, 00),
                            ClassEndDate = new DateTime(date.Year, date.Month, date.Day, 21, 00, 00),
                            ClassCost = 10,
                            ClassLevel = ClassLevel.Intermediate,
                            Students = new List<ApplicationUser>()
                        };
                    }
                    else if (date.DayOfWeek == DayOfWeek.Wednesday)
                    {
                        lesson = new Lesson()
                        {
                            ClassType = Lesson.LessonType.Group,
                            ClassStartDate = new DateTime(date.Year, date.Month, date.Day, 18, 00, 00),
                            ClassEndDate = new DateTime(date.Year, date.Month, date.Day, 21, 00, 00),
                            ClassCost = 10,
                            ClassLevel = ClassLevel.Beginner,
                            Students = new List<ApplicationUser>()
                        };
                    }
                    else if (date.DayOfWeek == DayOfWeek.Thursday)
                    {
                        lesson = new Lesson()
                        {
                            ClassType = Lesson.LessonType.Group,
                            ClassStartDate = new DateTime(date.Year, date.Month, date.Day, 18, 00, 00),
                            ClassEndDate = new DateTime(date.Year, date.Month, date.Day, 21, 00, 00),
                            ClassCost = 10,
                            ClassLevel = ClassLevel.Advanced,
                            Students = new List<ApplicationUser>()
                        };
                    }
                    else if (date.DayOfWeek == DayOfWeek.Saturday)
                    {
                        lesson = new Lesson()
                        {
                            ClassType = Lesson.LessonType.Group,
                            ClassStartDate = new DateTime(date.Year, date.Month, date.Day, 11, 00, 00),
                            ClassEndDate = new DateTime(date.Year, date.Month, date.Day, 14, 00, 00),
                            ClassCost = 10,
                            ClassLevel = ClassLevel.Womens,
                            Students = new List<ApplicationUser>()
                        };
                    }

                    if (lesson != null)
                        context.Lessons.Add(lesson);
                }
                string id = userManager.FindByEmail("test@test.com").Id;                
            }
            //Complete the seeding of the database
            base.Seed(context);
            context.SaveChanges();
        }
    }
}
