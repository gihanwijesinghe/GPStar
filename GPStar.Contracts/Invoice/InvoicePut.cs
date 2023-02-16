using GPStar.Contracts.InvoiceLine;

namespace GPStar.Contracts.Invoice
{
    public class InvoicePut : InvoicePostBase
    {
        public Guid Id { get; set; }
        public IEnumerable<InvoiceLinePut> InvoiceLinePuts { get; set; }
    }
}
