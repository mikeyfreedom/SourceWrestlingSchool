using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SourceWrestlingSchool.Models
{
    public class Seat
    {
        public int SeatID { get; set; }
        public string SeatNumber { get; set; }
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
