using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SourceWrestlingSchool.Models
{
    /// <summary>Represents a venue that holds a live event.</summary>
    public class Venue
    {
        /// <summary>Gets or sets the id record of the venue in the database.</summary>
        [Key]
        public int VenueId { get; set; }
        /// <summary>Gets or sets the name of the venue.</summary>
        [DisplayName("Venue")]
        public string VenueName { get; set; }
        /// <summary>Gets or sets the location of the venue.</summary>
        [DisplayName("Location")]
        public string VenueLocation { get; set; }
        /// <summary>Gets or sets the address of the venue.</summary>
        [DisplayName("Venue Address")]
        public string VenueAddress { get; set; }
        /// <summary>Gets or sets the postcode of the venue.</summary>
        [DisplayName("Venue Postcode")]
        public string VenuePostcode { get; set; }
        /// <summary>Gets or sets the number of available seats.</summary>
        [DisplayName("No of Seats")]
        public int NoOfSeats { get; set; }
        /// <summary>Gets or sets the list of events that the venue hosts.</summary>
        public virtual ICollection<LiveEvent> Events { get; set; }

    }
}
