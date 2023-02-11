using GPStarAPI.ApiModels;
using GPStarAPI.Data;
using GPStarAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace GPStarAPI.Systems
{
    public class InvoiceSystem
    {
        private readonly GPStarContext _context;

        public InvoiceSystem(GPStarContext context)
        {
            _context = context;
        }

        public async Task<InvoiceGet> GetInvoiceById(Guid invoiceId)
        {
            var invoice = await _context.Invoices.Where(invoice => invoice.Id == invoiceId).Select(invoice => new InvoiceGet
            {
                Id = invoice.Id,
                Date = invoice.Date,
                TotalAmount = invoice.TotalAmount,
                InvoiceLinePuts = invoice.InvoiceLines.Select(line => new InvoiceLineGet
                {
                    Id = line.Id,
                    Name = line.Name,
                    Quantity = line.Quantity,
                    UnitPrice = line.UnitPrice,
                    LinePrice = line.LinePrice,
                })
            }).FirstOrDefaultAsync();

            if(invoice == null)
            {
                throw new Exception("sd");
            }
            return invoice;
        }

        public async Task<Guid> CreateInvoice(InvoicePost invoicePost)
        {
            var invoiceDb = new Invoice
            {
                Date = invoicePost.Date,
                TotalAmount = invoicePost.TotalAmount,
                InvoiceLines = invoicePost.InvoiceLinePosts.Select(line => new InvoiceLine
                {
                    Name = line.Name,
                    Quantity = line.Quantity,
                    UnitPrice = line.UnitPrice,
                    LinePrice = line.LinePrice
                }).ToList()
            };

            _context.Invoices.Add(invoiceDb);

            try
            {
                await _context.SaveChangesAsync();
                return invoiceDb.Id;
            }
            catch (DbUpdateException)
            {
                throw;
            }
        }
    }
}
