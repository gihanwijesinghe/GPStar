using GPStarAPI.ApiModels;
using GPStarAPI.Errors;
using GPStarAPI.Helpers;
using GPStarAPI.Models;

namespace GPStarAPI.Invoices
{
    public class InvoiceValidator
    {
        public AppResult<Guid> ValidatePost(InvoicePost invoicePost)
        {
            var postLines = invoicePost.InvoiceLinePosts ?? new List<InvoiceLinePost>();

            var linesValidationsResult = ValidatePosttLines(postLines.ToList(), invoicePost.TotalAmount);
            if (!linesValidationsResult.Result)
            {
                return linesValidationsResult;
            }

            return AppResult<Guid>.Success;
        }

        public AppResult<Guid> ValidatePut(Models.Invoice invoiceDb, InvoicePut invoicePut, List<InvoiceLine> invoiceLines)
        {
            // Null Check validations
            if (invoiceDb == null)
            {
                return AppResult<Guid>.Fail(new ErrorModel { Message = "invoice not found", ErrorType = ErrorType.NotFound } );
            }

            if (invoicePut == null)
            {
                return AppResult<Guid>.Fail(new ErrorModel { Message = "invoice put modal not found", ErrorType = ErrorType.NotFound });
            }

            var dbLines = invoiceLines ?? new List<InvoiceLine>();
            var putLines = invoicePut.InvoiceLinePuts ?? new List<InvoiceLinePut>();

            var putLineValidationResult = ValidatePosttLines(MapToPostLines(putLines.ToList()), invoicePut.TotalAmount);
            if (!putLineValidationResult.Result)
            {
                return putLineValidationResult;
            }

            // Amount, Quantity and Total calculations 
            //var errors = new List<ErrorModel>();
            //foreach (var putLine in putLines)
            //{
            //    if(putLine.Quantity * putLine.UnitPrice != putLine.LinePrice)
            //    {
            //        errors.Add(new ErrorModel { Message = putLine.Name + " line sum not aligned with quatity and unit Price", ErrorType = ErrorType.ArgumentException });
            //    }
            //}
            //if (errors.Any())
            //{
            //    return AppResult<Guid>.Fail(errors);
            //}

            //if (putLines.Select(putLine => putLine.LinePrice).DefaultIfEmpty(0).Sum() != invoicePut.TotalAmount)
            //{
            //    return AppResult<Guid>.Fail(new ErrorModel { Message = "Line total not equal to invoice sum", ErrorType = ErrorType.ArgumentException });
            //}

            // db lines and put lines mismatching validations
            if (!dbLines.Any())
            {
                if(putLines.Any(putLine => putLine.Id != null))
                {
                    var notExitingLineIds = putLines.Where(putLine => putLine.Id != null).Select(putLine => putLine.Id);
                    var message = string.Join(", ", notExitingLineIds);
                    return AppResult<Guid>.Fail(new ErrorModel { Message = "No invoice line found for ids: " + message, ErrorType = ErrorType.NotFound });
                }
            }
            else
            {
                var notExitingLineIds = putLines.Where(putLine => putLine.Id != null && !dbLines
                        .Any(line => line.Id == putLine.Id))
                        .Select(putLine => putLine.Id);
                if (notExitingLineIds.Any())
                {
                    var message = string.Join(", ", notExitingLineIds);
                    return AppResult<Guid>.Fail(new ErrorModel { Message = "No invoice line found for ids: " + message, ErrorType = ErrorType.NotFound });
                }
            }

            // Validating put line duplicates
            if (putLines.Where(putLine => putLine.Id != null).GroupBy(putLine => putLine.Id).Where(x => x.Skip(1).Any()).Any())
            {
                return AppResult<Guid>.Fail(new ErrorModel { Message = "duplicate put lines exists", ErrorType = ErrorType.ArgumentException });
            }

            return AppResult<Guid>.Success;
        }

        private AppResult<Guid> ValidatePosttLines(List<InvoiceLinePost> postLines, decimal invoiceTotal)
        {
            // Amount, Quantity and Total calculations 
            var errors = new List<ErrorModel>();
            foreach (var posttLine in postLines)
            {
                if (posttLine.Quantity * posttLine.UnitPrice != posttLine.LinePrice)
                {
                    errors.Add(new ErrorModel { Message = posttLine.Name + " line sum not aligned with quatity and unit Price", ErrorType = ErrorType.ArgumentException });
                }
            }
            if (errors.Any())
            {
                return AppResult<Guid>.Fail(errors);
            }

            if (postLines.Select(putLine => putLine.LinePrice).DefaultIfEmpty(0).Sum() != invoiceTotal)
            {
                return AppResult<Guid>.Fail(new ErrorModel { Message = "Line total not equal to invoice sum", ErrorType = ErrorType.ArgumentException });
            }

            return AppResult<Guid>.Success;
        }

        private List<InvoiceLinePost> MapToPostLines(List<InvoiceLinePut> putLines)
        {
            return putLines.Select(putLine => new InvoiceLinePost
            {
                Name = putLine.Name,
                LinePrice = putLine.LinePrice,
                UnitPrice = putLine.UnitPrice,
                Quantity = putLine.Quantity,
            }).ToList();
        }
    }
}
