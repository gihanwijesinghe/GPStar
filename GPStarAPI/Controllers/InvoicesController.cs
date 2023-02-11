using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GPStarAPI.Data;
using GPStarAPI.Models;
using GPStarAPI.Systems;
using GPStarAPI.ApiModels;

namespace GPStarAPI
{
    [Route("api/[controller]")]
    [ApiController]
    public class InvoicesController : ControllerBase
    {
        private readonly GPStarContext _context;
        private readonly InvoiceSystem _invoiceSystem;

        public InvoicesController(GPStarContext context, InvoiceSystem invoiceSystem)
        {
            _context = context;
            _invoiceSystem = invoiceSystem;
        }

        // GET: api/Invoices
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Invoice>>> GetInvoice()
        {
            return await _context.Invoices.ToListAsync();
        }

        // GET: api/Invoices/5
        [HttpGet("{id}")]
        public async Task<ActionResult<InvoiceGet>> GetInvoice(Guid id)
        {
            var invoice = await _invoiceSystem.GetInvoiceById(id);

            if (invoice == null)
            {
                return NotFound();
            }

            return invoice;
        }

        // PUT: api/Invoices/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutInvoice(Guid id, InvoicePut invoicePut)
        {
            if (id != invoicePut.Id)
            {
                return BadRequest();
            }

            await _invoiceSystem.UpdateInvoice(id, invoicePut);

            return Ok(id);
        }

        // POST: api/Invoices
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Guid>> PostInvoice(InvoicePost invoicePost)
        {
            var id = await _invoiceSystem.CreateInvoice(invoicePost);

            return CreatedAtAction("GetInvoice", new { id = id }, id);
        }
    }
}
