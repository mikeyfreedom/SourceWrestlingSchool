using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SourceWrestlingSchool.Models
{
    public class Seat
    {
        public int SeatID { get; set; }
        [DisplayName("Seat Number")]
        public string SeatNumber { get; set; }
        [DisplayName("Current Status")]
        public SeatBookingStatus Status { get; set; }

        public Seat()
        {
            Events = new List<LiveEvent>();
        }
        //navigational properties
        public virtual ICollection<LiveEvent> Events { get; set; }
        
        public enum SeatBookingStatus
        {
            Free,Reserved,Booked
        }
        
    }
}
