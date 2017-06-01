using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SourceWrestlingSchool.Models
{
    /// <summary>Represents a private session that has been requested by a student.</summary>
    public class PrivateSession
    {
        /// <summary>Gets or sets the id record of the private session in the database.</summary>
        [Key]
        public int PrivateSessionId { get; set; }
        /// <summary>Gets or sets the user entity who made the session request.</summary>
        [DisplayName("Name")]
        public ApplicationUser User { get; set; }
        /// <summary>Gets or sets the requested start date and time of the session.</summary>
        [DisplayName("Start")]
        [DataType(DataType.DateTime)]
        public DateTime SessionStart { get; set; }
        /// <summary>Gets or sets the requested end date and time of the session.</summary>
        [DisplayName("End")]
        [DataType(DataType.DateTime)]
        public DateTime SessionEnd { get; set; }
        /// <summary>Gets or sets the id of the requested instructor to tesch the session.</summary>
        [DisplayName("Instructor")]
        public string InstructorId { get; set; }
        /// <summary>Gets or sets any notes supporting the request.</summary>
        public string Notes { get; set; }
        /// <summary>Gets or sets the current status of the request(Submitted, Accepted, Refused).</summary>
        [DisplayName("Current Status")]
        public RequestStatus Status { get; set; }

        /// <summary>Represents the status of the user request for a private session.</summary>
        public enum RequestStatus
        {
            Submitted,Accepted,Refused
        }

    }
}
