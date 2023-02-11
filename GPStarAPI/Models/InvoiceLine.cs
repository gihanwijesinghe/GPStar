using System.ComponentModel.DataAnnotations;

namespace GPStarAPI.Models
{
    public class InvoiceLine
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal LinePrice { get; set; }
        public Invoice Invoice { get; set; }
    }
}
