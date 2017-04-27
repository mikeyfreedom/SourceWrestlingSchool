using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SourceWrestlingSchool.Models
{
    public class Payment
    {
        public int PaymentID { get; set; }
        public string TransactionID { get; set; }
        public DateTime PaymentDate { get; set; }
        public float PaymentAmount { get; set; }
        public string PaymentDescription { get; set; }
        
        //navigational Properties
        public string UserID { get; set; }
        public ApplicationUser User { get; set; }
    }
}
