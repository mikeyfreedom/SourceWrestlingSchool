using System;
using System.Collections.Generic;

namespace SourceWrestlingSchool.Models
{
    public class Payment
    {
        public int PaymentID { get; set; }
        public DateTime PaymentDate { get; set; }
        public decimal PaymentAmount { get; set; }
        public string PaymentDescription { get; set; }
        public bool PaymentSettled { get; set; }
        public string TransactionId  { get; set; }
        public virtual ICollection<Seat> Seats  { get; set; }
        
        //navigational Properties
        public string UserID { get; set; }
        public ApplicationUser User { get; set; }
    }
}
