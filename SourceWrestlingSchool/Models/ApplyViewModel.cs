using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SourceWrestlingSchool.Models
{
    public class ApplyViewModel
    {
        [Key]
        public int ApplicationID { get; set; }
        public int Age { get; set; }
        public int Height { get; set; }
        public int Weight { get; set; }
        public string Notes { get; set; }

        //Navigation properties
        public string UserID { get; set; }
        public ApplicationUser User { get; set; }

        //enumerated status of application
        public ApplicationStatus Status { get; set; }

        public enum ApplicationStatus
        {
            Open,Accepted,Declined
        }

    }
}
