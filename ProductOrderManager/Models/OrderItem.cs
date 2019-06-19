using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ProductOrderManager.Models
{
    public class OrderItem
    {
        public long Id { get; set; }

        public int qttProduct { get; set; }

        public long productId { get; set; }

        public long orderId { get; set; }

        public Product product { get; set; }
    }
}