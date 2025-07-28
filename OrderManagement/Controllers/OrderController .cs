using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using OrderManagement.DAL.Data;
using OrderManagement.DAL.Models;
using OrderManagement.Services;
using OrderManagement.DTO;

namespace OrderManagement.Controllers
{
    [ApiController]
    [Route("api/orders")]
    public class OrderController : ControllerBase
    {
        private readonly OrderService _orderService;
        private readonly OrderManagementDbContext _context;

        public OrderController(OrderService orderService, OrderManagementDbContext context)
        {
            _orderService = orderService;
            _context = context;
        }



        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] OrderCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var order = new Order
            {
                CustomerId = dto.CustomerId,
                PaymentMethod = dto.PaymentMethod,
                OrderItems = dto.OrderItems.Select(i => new OrderItem
                {
                    ProductId = i.ProductId,
                    Quantity = i.Quantity
                }).ToList()
            };

            var newOrder = await _orderService.CreateOrderAsync(order);
            if (newOrder == null)
                return BadRequest("Insufficient stock or invalid product.");

            return Ok(newOrder);
        }




        [HttpGet("{orderId}")]
        public async Task<IActionResult> GetOrder(int orderId)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(o => o.OrderId == orderId);

            if (order == null) return NotFound();

            var dto = new OrderDto
            {
                OrderId = order.OrderId,
                OrderDate = order.OrderDate,
                TotalAmount = order.TotalAmount,
                PaymentMethod = order.PaymentMethod,
                Status = order.Status,
                OrderItems = order.OrderItems.Select(i => new OrderItemDto
                {
                    OrderItemId = i.OrderItemId,
                    ProductId = i.ProductId,
                    ProductName = i.Product?.Name,
                    UnitPrice = i.UnitPrice,
                    Quantity = i.Quantity,
                    Discount = i.Discount
                }).ToList()
            };

            return Ok(dto);
        }


        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllOrders() =>
            Ok(await _context.Orders.Include(o => o.OrderItems).ToListAsync());

        [HttpPut("{orderId}/status")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateOrderStatus(int orderId, [FromBody] StatusDto dto)
        {
            await _orderService.UpdateOrderStatusAsync(orderId, dto.Status);
            return Ok("Status updated.");
        }

        public record StatusDto(string Status);
    }
}
