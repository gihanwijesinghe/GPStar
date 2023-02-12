using GPStarAPI.ApiModels;
using GPStarAPI.Errors;
using GPStarAPI.Helpers;

namespace GPStarAPI.Invoices
{
    public class InvoiceValidator
    {
        public AppResult<Guid> Validate(Models.Invoice invoiceDb, InvoicePut invoicePut)
        {
            if (invoiceDb == null)
            {
                return AppResult<Guid>.Fail(new ErrorModel { Message = "invoice not found", ErrorType = ErrorType.NotFound } );
            }

            if (invoicePut == null)
            {
                return AppResult<Guid>.Fail(new ErrorModel { Message = "invoice put modal not found", ErrorType = ErrorType.NotFound });
            }

            return AppResult<Guid>.Success;
        }
    }
}
