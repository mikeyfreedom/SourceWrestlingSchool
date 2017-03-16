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
        public int EventID { get; set; }
        public bool IsBooked { get; set; }
        public SeatBookingStatus Status { get; set; }
        
        public enum SeatBookingStatus
        {
            Free,Reserved,Booked
        }
        //Put a seat status enumeration in here
    }
}
