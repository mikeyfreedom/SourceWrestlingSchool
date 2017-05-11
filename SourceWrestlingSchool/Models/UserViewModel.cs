using Microsoft.AspNet.Identity.EntityFramework;
using System.Collections.Generic;

namespace SourceWrestlingSchool.Models
{
    public class UserViewModel
    {
        public ICollection<ApplicationUser> Users { get; set; }
        public ICollection<IdentityRole> Roles { get; set; }
    }
}
