using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.ComponentModel.DataAnnotations;

namespace SourceWrestlingSchool.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        [Required]
        [MaxLength(20)]
        public string Address { get; set; }
        public int? Age { get; set; }
        public ClassLevel? ClassLevel { get; set; }
        [Required]
        [MaxLength(20)]
        public string FirstName { get; set; }
        public int? Height { get; set; }
        [Required]
        [MaxLength(20)]
        public string LastName { get; set; }
        [Phone]
        public string MobileNumber { get; set; }
        [Required]
        [MinLength(6)]
        [MaxLength(8)]
        public string Postcode { get; set; }
        [Required]
        [MaxLength(20)]
        public string Town { get; set; }
        public int? Weight { get; set; }
        
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }
   
}