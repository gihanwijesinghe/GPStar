using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using GPStarAPI.Models;

namespace GPStarAPI.Data
{
    public class GPStarContext : DbContext
    {
        public GPStarContext(DbContextOptions<GPStarContext> options)
            : base(options)
        {
        }

        public DbSet<GPStarAPI.Models.Invoice> Invoices { get; set; }
        public DbSet<GPStarAPI.Models.InvoiceLine> InvoiceLines { get; set; }
    }
}
