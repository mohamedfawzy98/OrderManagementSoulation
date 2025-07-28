using System.ComponentModel.DataAnnotations;

namespace OrderManagement.DTO
{
    public class OrderCreateDto
    {
        [Required]
        public int CustomerId { get; set; }

        [Required, StringLength(50)]
        public string PaymentMethod { get; set; }
        [Required]
        [MinLength(1, ErrorMessage = "At least one order item is required.")]
        public List<OrderItemCreateDto> OrderItems { get; set; }
    }
}
