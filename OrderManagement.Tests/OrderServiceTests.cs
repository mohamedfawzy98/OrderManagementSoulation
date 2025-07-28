using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OrderManagement.DAL.Data;
using OrderManagement.DAL.Models;
using OrderManagement.Services;
using Xunit;


public class OrderServiceTests
{
    private OrderService GetOrderService(out OrderManagementDbContext context)
    {
        var options = new DbContextOptionsBuilder<OrderManagementDbContext>()
      .UseInMemoryDatabase("TestDB")
      .Options;


        context = new OrderManagementDbContext(options);

        context.Products.Add(new Product { ProductId = 1, Name = "Laptop", Price = 1000, Stock = 5 });
        context.SaveChanges();

        return new OrderService(context, new FakeEmailService());
    }

    [Fact]
    public async Task CreateOrder_Should_Return_Null_When_Stock_Insufficient()
    {
        var service = GetOrderService(out var context);

        var order = new Order
        {
            CustomerId = 1,
            PaymentMethod = "Credit Card",
            OrderItems = new List<OrderItem>
            {
                new OrderItem { ProductId = 1, Quantity = 10 } 
            }
        };

        var result = await service.CreateOrderAsync(order);
        Assert.Null(result);
    }

    [Fact]
    public async Task CreateOrder_Should_Apply_Discount_When_Total_Above_200()
    {
        var service = GetOrderService(out var context);

        var order = new Order
        {
            CustomerId = 1,
            PaymentMethod = "Credit Card",
            OrderItems = new List<OrderItem>
            {
                new OrderItem { ProductId = 1, Quantity = 1 }
            }
        };

        var result = await service.CreateOrderAsync(order);
        Assert.NotNull(result);
        Assert.True(result.TotalAmount < 1000); 
    }

    private class FakeEmailService : IEmailService
    {
        public Task SendEmailAsync(string toEmail, string subject, string body) => Task.CompletedTask;
    }
}
