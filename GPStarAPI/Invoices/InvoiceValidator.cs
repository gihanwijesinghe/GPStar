using GPStarAPI.ApiModels;

namespace GPStarAPI.Invoices
{
    public class InvoiceValidator
    {
        public void Validate(Models.Invoice invoiceDb, InvoicePut invoicePut)
        {
            if (invoiceDb == null)
            {
                throw new Exception("invoice not found");
            }
        }
    }
}
