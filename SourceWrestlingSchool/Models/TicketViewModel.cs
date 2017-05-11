using System.Collections.Generic;

namespace SourceWrestlingSchool.Models
{
    public class TicketViewModel
    {
        public LiveEvent Event { get; set; }
        public ICollection<Seat> Seats { get; set; }

    }
}
