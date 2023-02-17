using Microsoft.AspNetCore.Mvc;
using GPStar.Contracts.Invoice;
using GPStar.Systems.Invoices;
using GPStar.Utils;

namespace GPStar.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InvoicesController : ControllerBase
    {
        private readonly InvoiceSystem _invoiceSystem;

        public InvoicesController(InvoiceSystem invoiceSystem)
        {
            _invoiceSystem = invoiceSystem;
        }

        //// GET: api/Invoices
        //[HttpGet]
        //public async Task<ActionResult<IEnumerable<Invoice>>> GetInvoice()
        //{
        //    return await _context.Invoices.ToListAsync();
        //}

        //GET: api/Invoices/5
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
        [HttpPut("{id}")]
        public async Task<IActionResult> PutInvoice(Guid id, InvoicePut invoicePut)
        {
            var result = await _invoiceSystem.UpdateInvoice(id, invoicePut);
            return ApiResult(result);
        }

        // POST: api/Invoices
        [HttpPost]
        public async Task<ActionResult<Guid>> PostInvoice(InvoicePost invoicePost)
        {
            var result = await _invoiceSystem.CreateInvoice(invoicePost);
            return ApiResult(result);
        }

        private ActionResult ApiResult<T>(AppResult<T> result)
        {
            if (result.Result)
            {
                return Ok(result.Data);
            }

            var errors = string.Join(", ", result.Errors.Select(e => e.Message));
            if (result.ErrorType != null)
            {
                switch (result.ErrorType)
                {
                    case ErrorType.NotFound:
                        return NotFound(errors);
                    case ErrorType.ArgumentException:
                        return BadRequest(errors);
                }
            }

            return BadRequest(errors ?? "Something went wrong");
        }
    }
}
