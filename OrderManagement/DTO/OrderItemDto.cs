using System.ComponentModel.DataAnnotations;
namespace OrderManagement.DTO
{
    public class OrderItemDto
    {
        public int OrderItemId { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal UnitPrice { get; set; }
        [Required, Range(1, 100)]
        public int Quantity { get; set; }
        public decimal Discount { get; set; }
    }
}
