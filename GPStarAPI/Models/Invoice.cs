using System.ComponentModel.DataAnnotations;

namespace GPStarAPI.Models
{
    public class Invoice
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public DateTime Date { get; set; }
        public decimal TotalAmount { get; set; }
        public ICollection<InvoiceLine> InvoiceLines { get; set; }
    }
}
