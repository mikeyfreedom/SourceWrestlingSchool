using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;

namespace SourceWrestlingSchool.Models
{
    /// <summary>Rrpresents a scheduled training class at the school.</summary>
    public class Lesson
    {
        /// <summary>Gets or sets the id record of the lesson in the database.</summary>
        [Key]
        public int LessonId { get; set; }
        /// <summary>Gets or sets the type of lesson(Group or Private).</summary>
        [Required]
        [DisplayName("Class Type")]
        [DefaultValue(LessonType.Group)]
        public LessonType ClassType { get; set; }
        /// <summary>Gets or sets the skill level of the lesson(Beginner,Intermediate,Advanced or Womens).</summary>
        [Required]
        [DisplayName("Level")]
        public ClassLevel ClassLevel { get; set; }
        /// <summary>Gets or sets the start date and time of the lesson.</summary>
        [Required]
        [DisplayName("Start")]
        [DataType(DataType.Date)]
        public DateTime ClassStartDate { get; set; }
        /// <summary>Gets or sets the date and time that the lesson concludes.</summary>
        [Required]
        [DisplayName("Finish")]
        [DataType(DataType.Date)]
        public DateTime ClassEndDate { get; set; }
        /// <summary>Gets or sets the cost of the lesson.</summary>
        [DisplayName("Cost")]
        public float ClassCost { get; set; }
        /// <summary>Gets or sets the name of the instructor teaching the lesson.</summary>
        [DisplayName("Instructor")]
        public string InstructorName { get; set; }
        /// <summary>Gets or sets the list of student users who have booked in to take part in the lesson.</summary>
        public virtual ICollection<ApplicationUser> Students { get; set; }

        /// <summary>Represents the type of lesson(Group or Private).</summary>
        public enum LessonType
        {
            Group,Private
        }
    }
}
