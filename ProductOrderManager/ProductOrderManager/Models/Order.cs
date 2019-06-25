using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProductOrderManager.Models
{
    public class Order
    {
        public long Id { get; set; }

        public string email { get; set; }

        public DateTime orderDate { get; set; }

        public DateTime deliveryDate { get; set; }

        public string orderStatus { get; set; }

        public decimal totalPrice { get; set; }

        public decimal totalWeight { get; set; }

        public decimal freightPrice { get; set; }

        public virtual ICollection<OrderItem> OrderItems { get; set; }
    }
}