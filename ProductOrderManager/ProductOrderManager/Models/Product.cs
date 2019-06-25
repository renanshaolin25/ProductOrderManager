using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ProductOrderManager.Models
{
    public class Product
    {
        public long Id { get; set; }

        [Required]
        public string name { get; set; }

        [Required]
        [StringLength(200)]
        [Index(IsUnique = true)]
        public string model { get; set; }

        [Required]
        [StringLength(200)]
        [Index(IsUnique = true)]
        public string code { get; set; }

        public string description { get; set; }

        public string color { get; set; }

        public string imageURL { get; set; }

        public decimal price { get; set; }

        public decimal weight { get; set; }

        public decimal height { get; set; }

        public decimal width { get; set; }

        public decimal length { get; set; }

        public decimal diameter { get; set; }
    }
}