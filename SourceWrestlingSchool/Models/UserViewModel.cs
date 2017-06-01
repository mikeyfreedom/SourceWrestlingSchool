using Microsoft.AspNet.Identity.EntityFramework;
using System.Collections.Generic;

namespace SourceWrestlingSchool.Models
{
    /// <summary>Represents the collection of all users and roles on the system.</summary>

    public class UserViewModel
    {
        /// <summary>Gets or sets the list of users in the data model.</summary>
        public ICollection<ApplicationUser> Users { get; set; }
        /// <summary>Gets or sets the list of roles in the data model.</summary>
        public ICollection<IdentityRole> Roles { get; set; }
    }
}
