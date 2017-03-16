using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SourceWrestlingSchool.Models
{
    public class LiveEvent
    {
        [Key]
        public int EventID { get; set; }
        public string EventName { get; set; }
        public DateTime EventDate { get; set; }
        public TimeSpan EventTime { get; set; }
        public string Location { get; set; }
        
        public LiveEvent()
        {
            Seats = new List<Seat>();

        }
        public List<Seat> Seats { get; set; }

    }
}
