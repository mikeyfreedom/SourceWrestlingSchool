using System;

namespace SourceWrestlingSchool.Models
{
    public class ProfileViewModel
    {
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
        public string[] SlideshowImageFileNames { get; set; }
    }
}
