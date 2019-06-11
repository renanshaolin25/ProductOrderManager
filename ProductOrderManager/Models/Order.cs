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

        public string totalPrice { get; set; }

        public string totalWeight { get; set; }

        public string freightPrice { get; set; }

        public List<OrderItem> orderItems { get; set; }
    }
}