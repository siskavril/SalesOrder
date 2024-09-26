using System.ComponentModel.DataAnnotations;

namespace SalesOrder.ViewModels
{
    public class OrderDataViewModel
    {
        public int Id { get; set; }
        public string? SalesOrder { get; set; }
        public DateTime OrderDate { get; set; }
        public int IdCustomer { get; set; }
        public string? CustomerName { get; set; }
        public string? Address { get; set; }

        public List<OrderDataDetailViewModel>? ListOrderDataDetail { get; set; }
        public List<int>? DeletedItems { get; set; } = new List<int>();

    }
}
