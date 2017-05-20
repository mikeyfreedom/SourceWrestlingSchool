using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SourceWrestlingSchool.Models
{
    public class Lesson
    {
        [Key]
        public int LessonId { get; set; }
        [DisplayName("Class Type")]
        public LessonType ClassType { get; set; }
        [DisplayName("Level")]
        public ClassLevel? ClassLevel { get; set; }
        [DisplayName("Start")]
        public DateTime ClassStartDate { get; set; }
        [DisplayName("Finish")]
        public DateTime ClassEndDate { get; set; }
        [DisplayName("Cost")]
        public float ClassCost { get; set; }
        [DisplayName("Instructor")]
        public string InstructorName { get; set; }
        
        public virtual ICollection<ApplicationUser> Students { get; set; }
        
        public enum LessonType
        {
            Group,Private
        }

    }
}
