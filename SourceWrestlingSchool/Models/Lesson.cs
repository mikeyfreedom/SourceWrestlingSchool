using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SourceWrestlingSchool.Models
{
    public class Lesson
    {
        [Key]
        public int ClassID { get; set; }
        public LessonType ClassType { get; set; }
        public ClassLevel? ClassLevel { get; set; }
        public DateTime ClassStartDate { get; set; }
        public DateTime ClassEndDate { get; set; }
        public List<ApplicationUser> Attendees { get; set; }
        public float ClassCost { get; set; }

        //navigational properties
        public int? InstructorID { get; set; }

        public enum LessonType
        {
            Group,Private
        }

    }
}
