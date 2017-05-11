using System;

namespace SourceWrestlingSchool.Models
{
    public class Booking
    {
        public int BookingID { get; set; }
        public DateTime BookingDate { get; set; }

        //navigation properties
        public string UserID { get; set; }
        public ApplicationUser User { get; set; }
    }
}
