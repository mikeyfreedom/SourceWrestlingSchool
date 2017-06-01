using System.Collections.Generic;

namespace SourceWrestlingSchool.Models
{
    /// <summary>Represents a viewmodel linking the list of seats attached to a specific event.</summary>

    public class TicketViewModel
    {
        /// <summary>Gets or sets the live event attached to the data model.</summary>
        public LiveEvent Event { get; set; }
        /// <summary>Gets or sets the list of seats attached to the event.</summary>
        public ICollection<Seat> Seats { get; set; }

    }
}
