using System.Collections.Generic;
using System.ComponentModel;

namespace SourceWrestlingSchool.Models
{
    public class Seat
    {
        public int SeatId { get; set; }
        [DisplayName("Seat Number")]
        public string SeatNumber { get; set; }
        [DisplayName("Current Status")]
        public SeatBookingStatus Status { get; set; }
        
        //navigational properties
        public virtual ICollection<LiveEvent> Events { get; set; }
        
        public enum SeatBookingStatus
        {
            Free,Reserved,Booked
        }
        
    }
}
