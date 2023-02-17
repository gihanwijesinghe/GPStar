using GPStar.Model;

namespace GPStar.Services.Invoices
{
    public interface IInvoiceService
    {
        Task<Invoice> GetAsync(string id);
        Task AddAsync(Invoice invoice);
        Task UpdateAsync(string id, Invoice item);
    }
}
