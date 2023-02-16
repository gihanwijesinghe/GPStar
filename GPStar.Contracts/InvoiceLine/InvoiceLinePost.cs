namespace GPStar.Contracts.InvoiceLine
{
    public class InvoiceLinePost
    {
        public string Name { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal LinePrice { get; set; }
    }
}
