using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SourceWrestlingSchool.Models
{
    public class PaymentMethod
    {
        public int PaymentMethodID { get; set; }
        public string PaymentMethodDescription { get; set; }

        public PaymentMethod()
        {
            Payments = new List<Payment>();
        }

        public List<Payment> Payments { get; set; }
    }
}
