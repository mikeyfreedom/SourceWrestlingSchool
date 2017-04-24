using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SourceWrestlingSchool.Models
{
    public class PrivateSession
    {
        [Key]
        public int PrivateSessionID { get; set; }
        [DisplayName("Name")]
        public string StudentName { get; set; }
        [DisplayName("Start")]
        public DateTime SessionStart { get; set; }
        [DisplayName("End")]
        public DateTime SessionEnd { get; set; }
        [DisplayName("Instructor")]
        public string InstructorID { get; set; }
        public string Notes { get; set; }
        [DisplayName("Current Status")]
        public RequestStatus Status { get; set; }

        public enum RequestStatus
        {
            Submitted,Accepted,Refused
        }

    }
}
