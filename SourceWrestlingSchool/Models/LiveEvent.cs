﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SourceWrestlingSchool.Models
{
    public class LiveEvent
    {
        [Key]
        public int EventID { get; set; }
        [DisplayName("Event Name")]
        public string EventName { get; set; }
        [DisplayName("Event Date")]
        [DataType(DataType.Date)]
        public DateTime EventDate { get; set; }
        [DisplayName("Event Time")]
        public TimeSpan EventTime { get; set; }
        public float EventRevenue { get; set; }
        public virtual Venue Venue { get; set; }
        
        public LiveEvent()
        {
            Seats = new List<Seat>();

        }
        public virtual ICollection<Seat> Seats { get; set; }
        
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
                    };
                    newSeat.Events.Add(this);
                    seatList.Add(newSeat);
                }
            }
            return seatList;            
        }

    }
}
