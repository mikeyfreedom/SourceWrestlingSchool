using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SourceWrestlingSchool.Models
{
    /// <summary>Represents a live wrestling event.</summary>
    public class LiveEvent
    {
        /// <summary>Gets or sets the id record of the event in the database.</summary>
        [Key]
        public int EventId { get; set; }
        /// <summary>Gets or sets the name of the event.</summary>
        [DisplayName("Event Name")]
        public string EventName { get; set; }
        /// <summary>Gets or sets the date of the event.</summary>
        [DisplayName("Event Date")]
        [DataType(DataType.Date)]
        public DateTime EventDate { get; set; }
        /// <summary>Gets or sets the time of the event.</summary>
        [DisplayName("Event Time")]
        [DataType(DataType.Time)]
        public TimeSpan EventTime { get; set; }
        /// <summary>Gets or sets the revenue of the event, through ticket sales.</summary>
        /// <remarks>Newly-created events default to zero revenue.</remarks>
        [DefaultValue(0)]
        public float EventRevenue { get; set; }
        /// <summary>Gets or sets the venue entity where the event is taking place.</summary>
        public virtual Venue Venue { get; set; }
        /// <summary>Gets or sets the collection of available seats for booking at the event.</summary>
        public virtual ICollection<Seat> Seats { get; set; }
        
        /// <summary>
        ///     Creates a list of seats for the event and sets them to free status for booking
        /// </summary>
        /// <returns>A list of seats, all set to BookingStatus.Free</returns>
        public List<Seat> CreateSeatMap()
        {
            int rows = 8;
            int rowSeats = 8;
            string rowNums = "12345678";
            string rowLetters = "ABCDEFGH";
            var seatList = new List<Seat>();
            for (int i = 1; i <= rows; i++)
            {
                for (int j = 1; j <= rowSeats; j++)
                {
                    Seat newSeat = new Seat()
                    {
                        SeatNumber = "" + rowNums.Substring(i-1,1) + rowLetters.Substring(j-1,1),
                        Status = Seat.SeatBookingStatus.Free,
                        Events = new List<LiveEvent>()
                    };
                    newSeat.Events.Add(this);
                    seatList.Add(newSeat);
                }
            }
            return seatList;            
        }
    }
}
