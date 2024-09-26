using System.ComponentModel.DataAnnotations;

namespace SalesOrder.Models
{
    public class Temporary
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string? ItemName { get; set; }
        [Required]
        public int Qty { get; set; }
        public double? Price { get; set; }
        [Required]
        public double? Total { get; set; }
    }
}
