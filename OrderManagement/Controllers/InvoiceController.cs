using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using OrderManagement.DAL.Data;
using OrderManagement.DAL.Models;

namespace OrderManagement.Controllers
{
    [ApiController]
    [Route("api/invoices")]
    public class InvoiceController : ControllerBase
    {
        private readonly OrderManagementDbContext _context;

        public InvoiceController(OrderManagementDbContext context)
        {
            _context = context;
        }

        [HttpGet("{invoiceId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetInvoice(int invoiceId)
        {
            var invoice = await _context.Invoices.Include(i => i.Order).FirstOrDefaultAsync(i => i.InvoiceId == invoiceId);
            return invoice == null ? NotFound() : Ok(invoice);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllInvoices() =>
            Ok(await _context.Invoices.Include(i => i.Order).ToListAsync());
    }
}
