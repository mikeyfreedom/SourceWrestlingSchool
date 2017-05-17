using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

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
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime DateJoinedSchool { get; set; }
        public ClassLevel ClassLevel { get; set; }
        public string FacebookUrl { get; set; }
        public string TwitterUrl { get; set; }
        public string InstagramUrl { get; set; }
        public string EmailAddress { get; set; }
        [AllowHtml]
        public string BioContent { get; set; }
        public string YoutubeEmbedLink { get; set; }
        public List<string> SlideshowImageFileNames { get; set; }
    }
}
