using FizzWare.NBuilder;
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
        public DbSet<Venue> Venues { get; set; }
        public DbSet<PrivateSession> PrivateSessions { get; set; }

        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
            Database.SetInitializer<ApplicationDbContext>(new ApplicationDbInitializer());
        }
        
        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        
    }

    public class ApplicationDbInitializer : DropCreateDatabaseAlways<ApplicationDbContext>
    {
        protected override void Seed(ApplicationDbContext context)
        {
            if (!context.Users.Any())
            {
                var roleStore = new RoleStore<IdentityRole>(context);
                var roleManager = new RoleManager<IdentityRole>(roleStore);
                var userStore = new UserStore<ApplicationUser>(context);
                var userManager = new UserManager<ApplicationUser>(userStore);
                userManager.UserValidator = new UserValidator<ApplicationUser>(userManager)
                {
                    AllowOnlyAlphanumericUserNames = false                    
                };

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

                    //Create standard user for testing purposes
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

                    //Create instructor for testing purposes
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

                //Seed Student Users - 150 as per requirements
                password = "123456";
                for (int i = 0;i < 150; i++)
                {
                    Random r = new Random();
                    ClassLevel level = (ClassLevel)r.Next(0, 3);

                    var newStudent = new ApplicationUser()
                    {
                        FirstName = Faker.Name.First(),
                        LastName = Faker.Name.Last(),
                        PhoneNumber = "01389999999",
                        MobileNumber = "07595543816",
                        Address = Faker.Address.StreetAddress(),
                        Town = Faker.Address.City(),
                        Postcode = Faker.Address.UkPostCode(),
                        Age = Faker.RandomNumber.Next(16, 40),
                        Height = Faker.RandomNumber.Next(150, 210),
                        Weight = Faker.RandomNumber.Next(40, 140),
                        ClassLevel = level,
                        EmailConfirmed = true                    
                    };

                    if (!(context.Users.Any(u => u.UserName == newStudent.UserName)))
                    {
                        newStudent.UserName = "" + newStudent.FirstName + "." + newStudent.LastName + "@example.com";
                        newStudent.Email = newStudent.UserName;
                        try
                        {
                            userManager.Create(newStudent, password);
                            userManager.AddToRole(newStudent.Id, RoleNames.ROLE_STUDENTUSER);
                        }
                        catch
                        {

                        }
                    }                    
                }  
                
                //Seed Lessons
                DateTime startDate = new DateTime(2017, 4, 24);
                DateTime endDate = new DateTime(2017, 5, 28);

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
                            InstructorName = "test@test.com",
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
                            InstructorName = "test@test.com",
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
                            InstructorName = "test@test.com",
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
                            InstructorName = "test@test.com",
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
                            InstructorName = "test@test.com",
                            Students = new List<ApplicationUser>()
                        };
                    }

                    if (lesson != null)
                        context.Lessons.Add(lesson);
                }
                //Seed Venue
                Venue school = new Venue()
                {
                    VenueName = "Source Wrestling School",
                    VenueLocation = "Pollokshaws, Glasgow",
                    VenueAddress = "26 Cogan Street",
                    VenuePostcode = "G43 1AP",
                    NoOfSeats = 64
                };
                context.Venues.Add(school);

                //Seed Live Event
                DateTime newDate = DateTime.Now.AddMonths(2);
                LiveEvent sampleEvent = new LiveEvent()
                {
                    EventDate = newDate.Date,
                    EventTime = new DateTime(newDate.Year, newDate.Month, newDate.Day, 18, 30, 00).TimeOfDay,
                    EventName = "Source Wrestling Showcase",
                    Venue = school
                };
                List<Seat> seats = sampleEvent.CreateSeatMap();
                foreach (Seat seat in seats)
                {
                    context.Seats.Add(seat);
                }
                context.LiveEvents.Add(sampleEvent);
            }
            //Complete the seeding of the database
            base.Seed(context);
            context.SaveChanges();
        }
    }
}
