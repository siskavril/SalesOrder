﻿using System.ComponentModel.DataAnnotations;

namespace SalesOrder.Models
{
    public class Customer
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string? Name { get; set; }
    }
}
