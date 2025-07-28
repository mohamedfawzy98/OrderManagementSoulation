using System.ComponentModel.DataAnnotations;
namespace OrderManagement.DTO
{
    public class CustomerDto
    {
        public int CustomerId { get; set; }
        [Required, StringLength(100)]
        public string Name { get; set; }
        [Required, EmailAddress]
        public string Email { get; set; }
    }
}
