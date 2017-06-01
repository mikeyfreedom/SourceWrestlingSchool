using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace SourceWrestlingSchool.Models
{
    /// <summary>Represents a data model containing the information contained on a student user's profile page</summary>
    public class ProfileViewModel
    {
        /// <summary>Gets or sets the id record of the profile in the database.</summary>
        [Key]
        public int ProfileId { get; set; }
        /// <summary>Gets or sets the filename and path to the user's profile image saved in the system.</summary>
        [Display(Name = "Profile Image")]
        public string ProfileImageFileName { get; set; }
        /// <summary>Gets or sets the student user's name.</summary>
        public string Name { get; set; }
        /// <summary>Gets or sets the student user's height in cm.</summary>
        public int Height { get; set; }
        /// <summary>Gets or sets the student user's weight in kg.</summary>
        public int Weight { get; set; }
        /// <summary>Gets or sets the date user was updated to Student level.</summary>
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        [Display(Name = "Date Joined")]
        public DateTime DateJoinedSchool { get; set; }
        /// <summary>Gets or sets the skill level of class the student is eligible for.</summary>
        [Display(Name = "Class Level")]
        public ClassLevel ClassLevel { get; set; }
        /// <summary>Gets or sets the http address of the student's Facebook page.</summary>
        [Display(Name = "Facebook URL")]
        [DataType(DataType.Url)]
        public string FacebookUrl { get; set; }
        /// <summary>Gets or sets the http address of the student's Twitter page.</summary>
        [Display(Name = "Twitter URL")]
        [DataType(DataType.Url)]
        public string TwitterUrl { get; set; }
        /// <summary>Gets or sets the http address of the student's Instagram page.</summary>
        [Display(Name = "Instagram URL")]
        [DataType(DataType.Url)]
        public string InstagramUrl { get; set; }
        /// <summary>Gets or sets the student's email address.</summary>
        [DataType(DataType.EmailAddress)]
        public string EmailAddress { get; set; }
        /// <summary>Gets or sets the content of the student's biography section of the profile.</summary>
        [AllowHtml]
        [Display(Name = "Biography")]
        public string BioContent { get; set; }
        /// <summary>Gets or sets the http address of the student's youtube video hightlight embedded link.</summary>
        [Display(Name = "Youtube URL")]
        [DataType(DataType.Url)]
        public string YoutubeEmbedLink { get; set; }
        /// <summary>Gets or sets a list of pathnames to student promo photos.</summary>
        public List<string> SlideshowImageFileNames { get; set; }
    }
}
