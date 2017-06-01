using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SourceWrestlingSchool.Models
{
    /// <summary> Reresents a model of a student application.</summary>
    public class ApplyViewModel
    {
        /// <summary>Gets or sets the id record of the application in the database.</summary>
        [Key]
        public int ApplicationId { get; set; }
        /// <summary>Gets or sets the age of the prospective applicant.</summary>
        [Required]
        public int Age { get; set; }
        /// <summary>Gets or sets the height in cm of the prospective applicant.</summary>
        [Required]
        [DisplayName("Height (in cm)")]
        public int Height { get; set; }
        /// <summary>Gets or sets the weight in kilos of the prospective applicant.</summary>
        [Required]
        [DisplayName("Weight (in kg)")]
        public int Weight { get; set; }
        /// <summary>Gets or sets any extra notes supporting the application.</summary>
        [DataType(DataType.MultilineText)]
        public string Notes { get; set; }
        /// <summary>Gets or sets the filename of the uploaded photo supporting the application.</summary>
        [DisplayName("Photo")]
        public string FileName { get; set; }

        //Navigation properties
        /// <summary>Gets or sets the id of the authenticated user associated with the application.</summary>
        public string UserId { get; set; }
        /// <summary>Gets or sets the authenticated user associated with the application.</summary>
        public ApplicationUser User { get; set; }

        /// <summary>Gets or sets the enumerated status of the application(Submitted,Accepted,Rejected).</summary>
        [DisplayName("Current Status")]
        public ApplicationStatus Status { get; set; }

        /// <summary>Represents the status of the application.</summary>
        public enum ApplicationStatus
        {
            Open,Accepted,Declined
        }
    }
}