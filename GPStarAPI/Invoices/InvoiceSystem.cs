using GPStarAPI.ApiModels;
using GPStarAPI.Data;
using GPStarAPI.Errors;
using GPStarAPI.Helpers;
using GPStarAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace GPStarAPI.Invoices
{
    public class InvoiceSystem
    {
        private readonly GPStarContext _context;
        private readonly InvoiceValidator _invoiceValidator;

        public InvoiceSystem(GPStarContext context, InvoiceValidator invoiceValidator)
        {
            _context = context;
            _invoiceValidator = invoiceValidator;
        }

        public async Task<InvoiceGet> GetInvoiceById(Guid invoiceId)
        {
            var lines = await _context.InvoiceLines.Where(line => line.InvoiceId == invoiceId).Select(line => new InvoiceLineGet
            {
                Id = line.Id,
                Name = line.Name,
                Quantity = line.Quantity,
                UnitPrice = line.UnitPrice,
                LinePrice = line.LinePrice,
            }).ToListAsync();

            var invoice = await _context.Invoices.Where(invoice => invoice.Id == invoiceId).Select(invoice => new InvoiceGet
            {
                Id = invoice.Id,
                Date = invoice.Date,
                TotalAmount = invoice.TotalAmount,
                InvoiceLinePuts = lines
            }).FirstOrDefaultAsync();

            if(invoice == null)
            {
                throw new Exception("sd");
            }
            return invoice;
        }

        public async Task<AppResult<Guid>> CreateInvoice(InvoicePost invoicePost)
        {
            var result = _invoiceValidator.ValidatePost(invoicePost);
            if (!result.Result)
            {
                return result;
            }

            var invoiceDb = new Models.Invoice
            {
                Date = invoicePost.Date,
                TotalAmount = invoicePost.TotalAmount,
                Description = invoicePost.Description,
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
                return AppResult<Guid>.Value(invoiceDb.Id);
            }
            catch (DbUpdateException)
            {
                throw;
            }
        }

        public async Task<AppResult<Guid>> UpdateInvoice(Guid invoiceId, InvoicePut invoicePut)
        {
            if (invoiceId != invoicePut.Id)
            {
                return AppResult<Guid>.Fail(new ErrorModel { 
                    Message = "invoice route id and modal id not equal", 
                    ErrorType = ErrorType.ArgumentException 
                });
            }

            var invoiceDb = await _context.Invoices.FirstOrDefaultAsync(invoice => invoice.Id == invoiceId);
            var dbLines = await _context.InvoiceLines.Where(line => line.InvoiceId == invoiceId).ToListAsync();

            var result = _invoiceValidator.ValidatePut(invoiceDb, invoicePut, dbLines);

            if (!result.Result)
            {
                return result;
            }

            invoiceDb.TotalAmount = invoicePut.TotalAmount;
            invoiceDb.Date = invoicePut.Date;
            invoiceDb.Description = invoicePut.Description;

            MergeInvoiceLines(invoiceId, dbLines, invoicePut);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new Exception("data base update issue");
            }

            return AppResult<Guid>.Value(invoiceId);
        }

        private IList<InvoiceLine> MergeInvoiceLines(Guid invoiceId, List<InvoiceLine> invoiceDbLines, InvoicePut invoicePut)
        {
            RemoveInvoiceLines(invoiceDbLines, invoicePut.InvoiceLinePuts.ToList());
            UpdateInvoiceLines(invoiceDbLines, invoicePut.InvoiceLinePuts.ToList());
            AddInvoiceLines(invoiceId, invoiceDbLines, invoicePut.InvoiceLinePuts.ToList());

            return invoiceDbLines;
        }

        private void RemoveInvoiceLines(List<InvoiceLine> invoiceDbLines, List<InvoiceLinePut> invoiceLinePuts)
        {
            if(invoiceDbLines != null)
            {
                var dbLineIds = invoiceDbLines.Select(line => line.Id).ToList();
                foreach (var lineId in dbLineIds)
                {
                    if(invoiceLinePuts == null || !invoiceLinePuts.Any() || !invoiceLinePuts.Any(putLine => lineId == putLine.Id))
                    {
                        var line = invoiceDbLines.FirstOrDefault(l => l.Id == lineId);
                        invoiceDbLines.Remove(line);
                        _context.InvoiceLines.Remove(line);
                    }
                }
            }
        }

        private void UpdateInvoiceLines(List<InvoiceLine> invoiceDbLines, List<InvoiceLinePut> InvoiceLinePuts)
        {
            if(invoiceDbLines == null || InvoiceLinePuts == null || !invoiceDbLines.Any() || !InvoiceLinePuts.Any())
            {
                return;
            }

            var toUpdateLines = InvoiceLinePuts.Where(putLine => putLine.Id != null);

            if(invoiceDbLines.Count() != toUpdateLines.Count())
            {
                throw new Exception("updated lines are not correct");
            }

            foreach(var invoiceLine in invoiceDbLines)
            {
                var toUpdateLine = toUpdateLines.Where(putLine => putLine.Id == invoiceLine.Id).FirstOrDefault();
                invoiceLine.Name = toUpdateLine.Name;
                invoiceLine.UnitPrice = toUpdateLine.UnitPrice;
                invoiceLine.Quantity = toUpdateLine.Quantity;
                invoiceLine.LinePrice = toUpdateLine.LinePrice;
            }
        }

        private void AddInvoiceLines(Guid invoiceId, List<InvoiceLine> invoiceDbLines, List<InvoiceLinePut> invoiceLinePuts)
        {
            var newLines = invoiceLinePuts.Where((putLine) => putLine.Id == null);

            foreach(var invoiceLine in newLines)
            {
                var lineDb = new InvoiceLine
                {
                    Quantity = invoiceLine.Quantity,
                    Name = invoiceLine.Name,
                    UnitPrice = invoiceLine.UnitPrice,
                    LinePrice = invoiceLine.LinePrice,
                    InvoiceId = invoiceId,
                };
                _context.InvoiceLines.Add(lineDb);
            }
        }
    }
}
