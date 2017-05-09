using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SourceWrestlingSchool.Models
{
    public class Venue
    {
        [Key]
        public int VenueID { get; set; }
        [DisplayName("Venue")]
        public string VenueName { get; set; }
        [DisplayName("Location")]
        public string VenueLocation { get; set; }
        [DisplayName("Venue Address")]
        public string VenueAddress { get; set; }
        [DisplayName("Venue Postcode")]
        public string VenuePostcode { get; set; }
        [DisplayName("No of Seats")]
        public int NoOfSeats { get; set; }
        public Venue()
        {
            Events = new List<LiveEvent>();
        }
        public virtual ICollection<LiveEvent> Events { get; set; }

    }
}
