namespace SourceWrestlingSchool.Models
{
    /// <summary>
    ///     This class holds a collection of Roles, the names of these roles will be used during the generation
    ///     of the database to populate the context-specfic RoleManager
    /// </summary>
    public class RoleNames
    {
        /// <summary>Constant representing the name of the Administrator Role.</summary>
        public const string ROLE_ADMINISTRATOR = "Administrator";
        /// <summary>Constant representing the name of the Staff Role.</summary>
        public const string ROLE_STAFFMEMBER = "Staff_Member";
        /// <summary>Constant representing the name of the Student Role.</summary>
        public const string ROLE_STUDENTUSER = "Student_User";
        /// <summary>Constant representing the name of the Instructor Role.</summary>
        public const string ROLE_INSTRUCTOR = "Instructor";
        /// <summary>Constant representing the name of the Standard User Role.</summary>
        public const string ROLE_STANDARDUSER = "Standard_User";
    }
}
