namespace GPStarAPI.ApiModels
{
    public class InvoicePostBase
    {
        public DateTime Date { get; set; }
        public decimal TotalAmount { get; set; }
    }

    public class InvoicePost : InvoicePostBase
    {
        public IEnumerable<InvoiceLinePost> InvoiceLinePosts { get; set; }
    }

    public class InvoicePut : InvoicePostBase
    {
        public Guid Id { get; set; }
        public IEnumerable<InvoiceLinePut> InvoiceLinePuts { get; set; }
    }

    public class InvoiceGet : InvoicePut
    {
        
    }
}
