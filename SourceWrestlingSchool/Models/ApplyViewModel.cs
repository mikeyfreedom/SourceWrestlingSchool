using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SourceWrestlingSchool.Models
{
    public class ApplyViewModel
    {
        [Key]
        public int ApplicationID { get; set; }
        public int Age { get; set; }
        [DisplayName("Height (in cm)")]
        public int Height { get; set; }
        [DisplayName("Weight (in kg)")]
        public int Weight { get; set; }
        [DataType(DataType.MultilineText)]
        public string Notes { get; set; }
        [DisplayName("Photo")]
        public string FileName { get; set; }

        //Navigation properties
        public string UserID { get; set; }
        public ApplicationUser User { get; set; }

        //enumerated status of application
        [DisplayName("Current Status")]
        public ApplicationStatus Status { get; set; }

        public enum ApplicationStatus
        {
            Open,Accepted,Declined
        }

    }
}
