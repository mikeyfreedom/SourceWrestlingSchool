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
        public int LessonID { get; set; }
        public LessonType ClassType { get; set; }
        public ClassLevel? ClassLevel { get; set; }
        public DateTime ClassStartDate { get; set; }
        public DateTime ClassEndDate { get; set; }
        public float ClassCost { get; set; }
        public string InstructorName { get; set; }
        
        public virtual ICollection<ApplicationUser> Students { get; set; }
        
        public enum LessonType
        {
            Group,Private
        }

    }
}
