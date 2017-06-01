using System.Collections.Generic;
using System.ComponentModel;

namespace SourceWrestlingSchool.Models
{
    /// <summary>Represents a record of an outstanding payment in the system.</summary>
    public class Seat
    {
        /// <summary>Gets or sets the id record of the seat in the database.</summary>
        public int SeatId { get; set; }
        /// <summary>Gets or sets the row/column number of the seat in the database.</summary>
        [DisplayName("Seat Number")]
        public string SeatNumber { get; set; }
        /// <summary>Gets or sets the current booking status of the seat in the database.</summary>
        [DisplayName("Current Status")]
        public SeatBookingStatus Status { get; set; }

        //navigational properties
        /// <summary>Gets or sets the list of events that the seat is used.</summary>
        public virtual ICollection<LiveEvent> Events { get; set; }

        /// <summary>Represents the booking status of the seat.</summary>
        public enum SeatBookingStatus
        {
            Free,Reserved,Booked
        }
        
    }
}
