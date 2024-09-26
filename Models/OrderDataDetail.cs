using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SalesOrder.Models
{
    public class OrderDataDetail
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey(nameof(OrderData))]
        public int IdOrderData { get; set; }
        [Required]
        public string? ItemName { get; set; }
        [Required]
        public int Qty { get; set; }
        public double Price { get; set; }
        [Required]
        public double Total { get; set; }
    }
}
