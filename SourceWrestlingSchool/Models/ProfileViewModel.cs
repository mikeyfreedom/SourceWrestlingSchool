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
        [RegularExpression("/^(http(s)?:\\/\\/[a-zA-Z0-9\\-_]+\\.[a-zA-Z]+(.)+)+/gm", ErrorMessage = "Incorrect URL format")]
        public string FacebookUrl { get; set; }
        [RegularExpression("/^(http(s)?:\\/\\/[a-zA-Z0-9\\-_]+\\.[a-zA-Z]+(.)+)+/gm", ErrorMessage = "Incorrect URL format")]
        public string TwitterUrl { get; set; }
        [RegularExpression("/^(http(s)?:\\/\\/[a-zA-Z0-9\\-_]+\\.[a-zA-Z]+(.)+)+/gm", ErrorMessage = "Incorrect URL format")]
        public string InstagramUrl { get; set; }
        public string EmailAddress { get; set; }
        [AllowHtml]
        public string BioContent { get; set; }
        [RegularExpression("/[(http(s) ?)://(www.)?a-zA-Z0-9@:%._+~#=]{2,256}.[a-z]{2,6}\b([-a-zA-Z0-9@:%_+.~#?&//=]*)/ig",ErrorMessage = "Incorrect URL format")]
        public string YoutubeEmbedLink { get; set; }
        public List<string> SlideshowImageFileNames { get; set; }
    }
}
