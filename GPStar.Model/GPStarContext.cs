using Microsoft.EntityFrameworkCore;

namespace GPStar.Model
{
    public class GPStarContext : DbContext
    {
        public GPStarContext(DbContextOptions<GPStarContext> options)
            : base(options)
        {
        }

        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<InvoiceLine> InvoiceLines { get; set; }
    }
}
