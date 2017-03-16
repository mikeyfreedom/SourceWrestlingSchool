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
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
            Database.SetInitializer<ApplicationDbContext>(new ApplicationDbInitializer());
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        public System.Data.Entity.DbSet<SourceWrestlingSchool.Models.Booking> Bookings { get; set; }

        public System.Data.Entity.DbSet<SourceWrestlingSchool.Models.ApplicationUser> ApplicationUsers { get; set; }

        public System.Data.Entity.DbSet<SourceWrestlingSchool.Models.Lesson> Lessons { get; set; }

        public System.Data.Entity.DbSet<SourceWrestlingSchool.Models.LiveEvent> LiveEvents { get; set; }

        public System.Data.Entity.DbSet<SourceWrestlingSchool.Models.Order> Orders { get; set; }

        public System.Data.Entity.DbSet<SourceWrestlingSchool.Models.Payment> Payments { get; set; }

        public System.Data.Entity.DbSet<SourceWrestlingSchool.Models.PaymentMethod> PaymentMethods { get; set; }

        public System.Data.Entity.DbSet<SourceWrestlingSchool.Models.Seat> Seats { get; set; }
    }

    public class ApplicationDbInitializer : DropCreateDatabaseIfModelChanges<ApplicationDbContext>
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

                string userName = "admin";
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
                        UserName = "admin",
                        Email = userName,
                        EmailConfirmed = true

                    };
                    userManager.Create(newUser, password);
                    userManager.AddToRole(newUser.Id, RoleNames.ROLE_ADMINISTRATOR);
                }
            }
            //Complete the seeding of the database
            base.Seed(context);
            context.SaveChanges();
        }
    }
}
