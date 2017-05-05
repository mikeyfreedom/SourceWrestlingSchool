using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SourceWrestlingSchool.Models
{
    public class SubscriptionViewModel
    {
        [Key]
        public int SubID { get; set; }
        public bool IsSubscribed { get; set; }
        public int? SubscriptionID { get; set; }
        public DateTime? LastPaymentDate { get; set; }
        public DateTime? NextDueDate { get; set; }
        public string Username { get; set; }
    }
}
