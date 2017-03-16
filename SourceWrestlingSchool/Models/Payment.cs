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
        
        //Navigational Properties
        public int PaymentMethodID { get; set; }
        public PaymentMethod PaymentMethod { get; set; }

        public string UserID { get; set; }
        public ApplicationUser User { get; set; }
    }
}
