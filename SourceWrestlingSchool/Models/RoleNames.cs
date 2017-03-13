using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SourceWrestlingSchool.Models
{
    /// <summary>
    ///     This class holds a collection of Roles, the names of these roles will be used during the generation
    ///     of the database to populate the context-specfic RoleManager
    /// </summary>
    public class RoleNames
    {
        public const string ROLE_ADMINISTRATOR = "Administrator";
        public const string ROLE_STAFFMEMBER = "Staff_Member";
        public const string ROLE_STUDENTUSER = "Student_User";
        public const string ROLE_INSTRUCTOR = "Instructor";
        public const string ROLE_STANDARDUSER = "Standard_User";
    }
}
