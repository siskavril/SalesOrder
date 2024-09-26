using SalesOrder.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace SalesOrder.ViewModels
{
    public class OrderDataDetailViewModel
    {
        public int? Id { get; set; }
        public int IdOrderData { get; set; }
        public string? ItemName { get; set; }
        public int Qty { get; set; }
        public double Price { get; set; }
        public double Total { get; set; }
    }
}
