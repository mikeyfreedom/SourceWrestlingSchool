using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SourceWrestlingSchool.Models
{
    public class UserViewModel
    {
        public ICollection<ApplicationUser> Users { get; set; }
        public ICollection<IdentityRole> Roles { get; set; }
    }
}
