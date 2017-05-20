using System;
using System.ComponentModel.DataAnnotations;

namespace SourceWrestlingSchool.Models
{
    public class SubscriptionViewModel
    {
        [Key]
        public int SubId { get; set; }
        public bool IsSubscribed { get; set; }
        public string SubscriptionId { get; set; }
        public DateTime? LastPaymentDate { get; set; }
        public DateTime? NextDueDate { get; set; }
        public string Username { get; set; }
    }
}
