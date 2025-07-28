using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using OrderManagement.DAL.Data;
using OrderManagement.DAL.Models;
using OrderManagement.DTO;
namespace OrderManagement.Controllers
{
    [ApiController]
    [Route("api/customers")]
    public class CustomerController : ControllerBase
    {
        private readonly OrderManagementDbContext _context;

        public CustomerController(OrderManagementDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> CreateCustomer([FromBody] CustomerCreateDto dto)
        {
            var customer = new Customer { Name = dto.Name, Email = dto.Email };
            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();
            return Ok(customer);
        }


        [HttpGet("{customerId}/orders")]
        [Authorize(Roles = "Admin,Customer")]
        public async Task<IActionResult> GetCustomerOrders(int customerId)
        {
            var orders = await _context.Orders
                .Include(o => o.OrderItems)
                .Where(o => o.CustomerId == customerId)
                .ToListAsync();

            return Ok(orders);
        }
    }
}
