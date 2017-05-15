using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SourceWrestlingSchool.Models
{
    public class ProfileViewModel
    {
        [Key]
        public int ProfileId { get; set; }
        public string ProfileImageFileName { get; set; }
        public string Name { get; set; }
        public int Height { get; set; }
        public int Weight { get; set; }
        public DateTime DateJoinedSchool { get; set; }
        public ClassLevel ClassLevel { get; set; }
        public string FacebookURL { get; set; }
        public string TwitterURL { get; set; }
        public string InstagramURL { get; set; }
        public string EmailAddress { get; set; }
        public string BioContent { get; set; }
        public string YoutubeEmbedLink { get; set; }
        public List<string> SlideshowImageFileNames { get; set; }
    }
}
