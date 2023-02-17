using GPStar.Contracts.Invoice;
using GPStar.Contracts.InvoiceLine;
using GPStar.Model;
using GPStar.Services.Invoices;
using GPStar.Utils;

namespace GPStar.Systems.Invoices
{
    public class InvoiceSystem
    {
        private readonly InvoiceValidator _invoiceValidator;
        private readonly IInvoiceService _invoiceService;

        public InvoiceSystem(InvoiceValidator invoiceValidator, IInvoiceService invoiceService)
        {
            _invoiceValidator = invoiceValidator;
            _invoiceService = invoiceService;
        }

        public async Task<InvoiceGet> GetInvoiceById(Guid invoiceId)
        {
            var invoiceDb = await _invoiceService.GetAsync(invoiceId.ToString());

            if (invoiceDb == null)
            {
                throw new Exception("Not found");
            }

            var invoiceGet = new InvoiceGet
            {
                Id = invoiceDb.Id,
                Date = invoiceDb.Date,
                TotalAmount = invoiceDb.TotalAmount,
                Description = invoiceDb.Description,
                InvoiceLinePuts = invoiceDb.InvoiceLines.Select(line => new InvoiceLineGet
                {
                    Id = line.Id,
                    Name = line.Name,
                    Quantity = line.Quantity,
                    UnitPrice = line.UnitPrice,
                    LinePrice = line.LinePrice,
                })
            };

            return invoiceGet;
        }

        public async Task<AppResult<Guid>> CreateInvoice(InvoicePost invoicePost)
        {
            var result = _invoiceValidator.ValidatePost(invoicePost);
            if (!result.Result)
            {
                return result;
            }

            var invoiceDb = new Invoice
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
                }).ToArray()
            };

            try
            {
                await _invoiceService.AddAsync(invoiceDb);
                return AppResult<Guid>.Value(invoiceDb.Id);
            }
            catch (Exception ex)
            {
                return AppResult<Guid>.Fail(new ErrorModel { Message = "DB exception"});
            }
        }

        public async Task<AppResult<Guid>> UpdateInvoice(Guid invoiceId, InvoicePut invoicePut)
        {
            if (invoiceId != invoicePut.Id)
            {
                return AppResult<Guid>.Fail(new ErrorModel
                {
                    Message = "invoice route id and modal id not equal",
                    ErrorType = ErrorType.ArgumentException
                });
            }

            var invoiceDb = await _invoiceService.GetAsync(invoiceId.ToString());
            var dbLines = invoiceDb.InvoiceLines.ToList();

            var result = _invoiceValidator.ValidatePut(invoiceDb, invoicePut, dbLines);

            if (!result.Result)
            {
                return result;
            }
            var updatedLines = MergeInvoiceLines(invoiceId, dbLines, invoicePut);

            invoiceDb.TotalAmount = invoicePut.TotalAmount;
            invoiceDb.Date = invoicePut.Date;
            invoiceDb.Description = invoicePut.Description;
            invoiceDb.InvoiceLines = updatedLines.ToArray();
            

            try
            {
                await _invoiceService.UpdateAsync(invoiceId.ToString(), invoiceDb);
            }
            catch (Exception e)
            {
                return AppResult<Guid>.Fail(new ErrorModel { Message = "DB exception: " + e.Message });
            }

            return AppResult<Guid>.Value(invoiceId);
        }

        private IList<InvoiceLine> MergeInvoiceLines(Guid invoiceId, List<InvoiceLine> invoiceDbLines, InvoicePut invoicePut)
        {
            invoiceDbLines = RemoveInvoiceLines(invoiceDbLines, invoicePut.InvoiceLinePuts.ToList());
            invoiceDbLines = UpdateInvoiceLines(invoiceDbLines, invoicePut.InvoiceLinePuts.ToList());
            invoiceDbLines = AddInvoiceLines(invoiceId, invoiceDbLines, invoicePut.InvoiceLinePuts.ToList());

            return invoiceDbLines;
        }

        private List<InvoiceLine> RemoveInvoiceLines(List<InvoiceLine> invoiceDbLines, List<InvoiceLinePut> invoiceLinePuts)
        {
            if (invoiceDbLines != null)
            {
                var dbLineIds = invoiceDbLines.Select(line => line.Id).ToList();
                foreach (var lineId in dbLineIds)
                {
                    if (invoiceLinePuts == null || !invoiceLinePuts.Any() || !invoiceLinePuts.Any(putLine => lineId == putLine.Id))
                    {
                        var line = invoiceDbLines.FirstOrDefault(l => l.Id == lineId);
                        invoiceDbLines.Remove(line);
                    }
                }
            }

            return invoiceDbLines;
        }

        private List<InvoiceLine> UpdateInvoiceLines(List<InvoiceLine> invoiceDbLines, List<InvoiceLinePut> InvoiceLinePuts)
        {
            if (invoiceDbLines == null || InvoiceLinePuts == null || !invoiceDbLines.Any() || !InvoiceLinePuts.Any())
            {
                return new List<InvoiceLine>();
            }

            var toUpdateLines = InvoiceLinePuts.Where(putLine => putLine.Id != null);

            if (invoiceDbLines.Count() != toUpdateLines.Count())
            {
                throw new Exception("updated lines are not correct");
            }

            foreach (var invoiceLine in invoiceDbLines)
            {
                var toUpdateLine = toUpdateLines.Where(putLine => putLine.Id == invoiceLine.Id).FirstOrDefault();
                invoiceLine.Name = toUpdateLine.Name;
                invoiceLine.UnitPrice = toUpdateLine.UnitPrice;
                invoiceLine.Quantity = toUpdateLine.Quantity;
                invoiceLine.LinePrice = toUpdateLine.LinePrice;
            }

            return invoiceDbLines;
        }

        private List<InvoiceLine> AddInvoiceLines(Guid invoiceId, List<InvoiceLine> invoiceDbLines, List<InvoiceLinePut> invoiceLinePuts)
        {
            var newLines = invoiceLinePuts.Where((putLine) => putLine.Id == null);

            foreach (var invoiceLine in newLines)
            {
                var lineDb = new InvoiceLine
                {
                    Quantity = invoiceLine.Quantity,
                    Name = invoiceLine.Name,
                    UnitPrice = invoiceLine.UnitPrice,
                    LinePrice = invoiceLine.LinePrice
                };
                invoiceDbLines.Add(lineDb);
            }

            return invoiceDbLines;
        }
    }
}
