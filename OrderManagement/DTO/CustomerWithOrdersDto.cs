namespace OrderManagement.DTO
{
    public class CustomerWithOrdersDto
    {
        public int CustomerId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public List<OrderDto> Orders { get; set; }
    }
}
