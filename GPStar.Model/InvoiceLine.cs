using System.ComponentModel.DataAnnotations;

namespace GPStar.Model
{
    public class InvoiceLine
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal LinePrice { get; set; }
    }
}
