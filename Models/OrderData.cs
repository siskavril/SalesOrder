using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SalesOrder.Models
{
    public class OrderData
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string? SalesOrder { get; set; }
        [Required]
        public DateTime OrderDate { get; set; }
        [ForeignKey(nameof(Customer))]
        public int IdCustomer { get; set; }
        public string? Address { get; set; }

        //reference table
        public Customer? Customer { get; set; }
    }
}
