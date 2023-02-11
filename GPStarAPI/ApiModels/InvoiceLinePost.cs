using GPStarAPI.Models;

namespace GPStarAPI.ApiModels
{
    public class InvoiceLinePost
    {
        public string Name { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal LinePrice { get; set; }
    }

    public class InvoiceLinePut : InvoiceLinePost
    {
        public Guid? Id { get; set; }
    }

    public class InvoiceLineGet : InvoiceLinePut
    {

    }
}
