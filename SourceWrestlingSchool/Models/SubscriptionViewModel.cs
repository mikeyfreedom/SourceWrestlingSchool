using System;
using System.ComponentModel.DataAnnotations;

namespace SourceWrestlingSchool.Models
{
    /// <summary>Represents the data model of a student's subscription agreement.</summary>
    public class SubscriptionViewModel
    {
        /// <summary>Gets or sets the id record of the seat in the database.</summary>
        [Key]
        public int SubId { get; set; }
        /// <summary>Gets or sets a value indicating whether.</summary>
        public bool IsSubscribed { get; set; }
        /// <summary>Gets or sets the id of the subscription agreement.</summary>
        public string SubscriptionId { get; set; }
        /// <summary>Gets or sets the last payment date of the subscription.</summary>
        public DateTime? LastPaymentDate { get; set; }
        /// <summary>Gets or sets the next payment date for the subscription.</summary>
        public DateTime? NextDueDate { get; set; }
        /// <summary>Gets or sets the name of the user attached to the subscription.</summary>
        public string Username { get; set; }
    }
}
