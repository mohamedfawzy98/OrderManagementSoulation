using Microsoft.EntityFrameworkCore;
using OrderManagement.DAL.Data;
using OrderManagement.DAL.Models;
namespace OrderManagement.Services
{
    public class OrderService
    {
        private readonly OrderManagementDbContext _context;
        private readonly IEmailService _emailService;

        public OrderService(OrderManagementDbContext context, IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        public async Task<Order?> CreateOrderAsync(Order order)
        {
            foreach (var item in order.OrderItems)
            {
                var product = await _context.Products.FindAsync(item.ProductId);
                if (product == null || product.Stock < item.Quantity)
                    return null;

                product.Stock -= item.Quantity;
                item.UnitPrice = product.Price;

                decimal subtotal = item.Quantity * item.UnitPrice;
                if (subtotal > 200) item.Discount = 0.10m * subtotal;
                else if (subtotal > 100) item.Discount = 0.05m * subtotal;
                else item.Discount = 0;
            }

            order.OrderDate = DateTime.Now;
            order.TotalAmount = order.OrderItems.Sum(i => i.Quantity * i.UnitPrice - i.Discount);
            order.Status = "Pending";

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            var invoice = new Invoice
            {
                OrderId = order.OrderId,
                InvoiceDate = DateTime.Now,
                TotalAmount = order.TotalAmount
            };
            _context.Invoices.Add(invoice);
            await _context.SaveChangesAsync();

            var customer = await _context.Customers.FindAsync(order.CustomerId);
            if (customer != null)
                await _emailService.SendEmailAsync(customer.Email, "تم إنشاء الطلب", $"تم إنشاء طلبك #{order.OrderId} بنجاح.");

            return order;
        }

        public async Task UpdateOrderStatusAsync(int orderId, string status)
        {
            var order = await _context.Orders.Include(o => o.Customer).FirstOrDefaultAsync(o => o.OrderId == orderId);
            if (order != null)
            {
                order.Status = status;
                await _context.SaveChangesAsync();

                await _emailService.SendEmailAsync(order.Customer.Email, "تحديث حالة الطلب", $"تم تحديث حالة طلبك #{order.OrderId} إلى: {status}");
            }
        }
    }
}
