namespace GPStar.Contracts
{
    public class InvoicePostBase
    {
        public DateTime Date { get; set; }
        public decimal TotalAmount { get; set; }
        public string Description { get; set; }
    }
}