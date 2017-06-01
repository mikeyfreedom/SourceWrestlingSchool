using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Faker;

namespace SourceWrestlingSchool.Models
{
    /// <summary>Represents a record of an outstanding payment in the system.</summary>
    public class Payment
    {
        /// <summary>Gets or sets the id record of the payment name of the instructor teaching the lesson.</summary>
        public int PaymentId { get; set; }
        /// <summary>Gets or sets the date the payment was added to the system.</summary>
        [DataType(DataType.Date)]
        [Display(Name ="Date Added")]
        public DateTime PaymentDate { get; set; }
        /// <summary>Gets or sets the payment amount.</summary>
        [DataType(DataType.Currency)]
        [Display(Name = "Amount")] 
        public decimal PaymentAmount { get; set; }
        /// <summary>Gets or sets a description of what the payment is for.</summary>
        [Display(Name = "Description")]
        public string PaymentDescription { get; set; }
        /// <summary>Gets or sets a value indiccated whether the payement was completed or not.</summary>
        [Display(Name = "Settled?")]
        public bool PaymentSettled { get; set; }
        /// <summary>Gets or sets the Braintree transaction ID of the payment if it was settled.</summary>
        public string TransactionId  { get; set; }
        /// <summary>Gets or sets a list of seats that is attached to a live event booking payment.</summary>
        public virtual ICollection<Seat> Seats  { get; set; }

        //navigational Properties
        /// <summary>Gets or sets the id of the user that has been charged with the payment.</summary>
        public string UserId { get; set; }
        /// <summary>Gets or sets the user that has been charged with the payment.</summary>
        public ApplicationUser User { get; set; }
    }
}
