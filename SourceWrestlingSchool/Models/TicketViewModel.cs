using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SourceWrestlingSchool.Models
{
    public class TicketViewModel
    {
        public LiveEvent Event { get; set; }
        public ICollection<Seat> Seats { get; set; }

    }
}
