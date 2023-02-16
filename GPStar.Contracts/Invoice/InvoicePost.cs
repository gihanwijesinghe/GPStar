using GPStar.Contracts.InvoiceLine;

namespace GPStar.Contracts.Invoice
{
    public class InvoicePost : InvoicePostBase
    {
        public IEnumerable<InvoiceLinePost> InvoiceLinePosts { get; set; }
    }
}
