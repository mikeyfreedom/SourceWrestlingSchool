using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SourceWrestlingSchool.Models
{
    public class Product
    {
        public int ProductID { get; set; }
        [DisplayName("Name")]
        public string ProductName { get; set; }
        [DisplayName("Description")]
        public string ProductDescription { get; set; }
        [DisplayName("Unit Price")]
        public float UnitPrice { get; set; }

        //navigational properties
        public Product()
        {
            OrderItems = new List<OrderItem>();
        }
        public List<OrderItem> OrderItems { get; set; }
    }
}
