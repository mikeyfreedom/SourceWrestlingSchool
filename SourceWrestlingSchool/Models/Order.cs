using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SourceWrestlingSchool.Models
{
    public class Order
    {
        public int OrderID { get; set; }
        [DisplayName("Order Date")]
        public DateTime OrderDate { get; set; }
        [DisplayName("Order Time")]
        public TimeSpan OrderTime { get; set; }
        [DisplayName("Current Status")]
        public OrderStatus Status { get; set; }

        public Order()
        {
            OrderItems = new List<OrderItem>();
        }

        public List<OrderItem> OrderItems { get; set; }

        public enum OrderStatus
        {
            Open,PaymentStage,Completed,Cancelled
        }
    }
}
