using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SourceWrestlingSchool.Models
{
    public class Product
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public string ProductDescription { get; set; }
        public float UnitPrice { get; set; }

        //navigational properties
        public Product()
        {
            OrderItems = new List<OrderItem>();
        }
        public List<OrderItem> OrderItems { get; set; }
    }
}
