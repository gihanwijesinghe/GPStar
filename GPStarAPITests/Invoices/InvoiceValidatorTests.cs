using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GPStarAPI.Models;
using GPStarAPI.ApiModels;

namespace GPStarAPI.Invoices.Tests
{
    [TestClass()]
    public class InvoiceValidatorTests
    {
        [TestMethod()]
        public void ValidateInvoiceDbNull()
        {

            var validator = new InvoiceValidator();
            var result = validator.ValidatePut(null, null, null);

            Assert.AreEqual(result.Errors[0].Message, "invoice not found");
        }

        [TestMethod()]
        public void ValidateInvoicePutNull()
        {

            var validator = new InvoiceValidator();
            var result = validator.ValidatePut(new Models.Invoice { }, null, null);

            Assert.AreEqual(result.Errors[0].Message, "invoice put modal not found");
        }

        [TestMethod()]
        public void ValidateInvoicePutLinesExistsWithEmptyDbLines()
        {

            var validator = new InvoiceValidator();
            var linePuts = new List<ApiModels.InvoiceLinePut>()
            {
                new ApiModels.InvoiceLinePut { Id = new Guid("d3528351-e405-4352-9266-1f3145652360") },
                new ApiModels.InvoiceLinePut { Id = new Guid("d3528351-e405-4352-9266-1f3145652555") },
                new ApiModels.InvoiceLinePut { },
            };

            var notExitingLineIds = linePuts.Where(putLine => putLine.Id != null).Select(putLine => putLine.Id);
            var message = string.Join(", ", notExitingLineIds);

            var result = validator.ValidatePut(new Invoice { }, new InvoicePut { InvoiceLinePuts = new List<InvoiceLinePut>(linePuts) }, null);

            Assert.AreEqual(result.Errors[0].Message, "No invoice line found for ids: " + message);
        }

        [TestMethod()]
        public void ValidateInvoicePutLinesExistsWithExistingDbLines()
        {

            var validator = new InvoiceValidator();
            var dblines = new List<InvoiceLine>()
            {
                new InvoiceLine { Id = new Guid("d3528351-e405-4352-9266-1f3145651111") },
                new InvoiceLine { Id = new Guid("d3528351-e405-4352-9266-1f3145652222") },
            };

            var linePuts = new List<ApiModels.InvoiceLinePut>()
            {
                new ApiModels.InvoiceLinePut { Id = new Guid("d3528351-e405-4352-9266-1f3145652360") },
                new ApiModels.InvoiceLinePut { Id = new Guid("d3528351-e405-4352-9266-1f3145652555") },
                new ApiModels.InvoiceLinePut { },
            };

            var notExitingLineIds = linePuts.Where(putLine => putLine.Id != null).Select(putLine => putLine.Id);
            var message = string.Join(", ", notExitingLineIds);

            var result = validator.ValidatePut(new Invoice { }, new InvoicePut { InvoiceLinePuts = linePuts }, dblines);

            Assert.AreEqual(result.Errors[0].Message, "No invoice line found for ids: " + message);
        }

        [TestMethod()]
        public void ValidateDuplicatePutLines()
        {

            var validator = new InvoiceValidator();
            var dblines = new List<InvoiceLine>()
            {
                new InvoiceLine { Id = new Guid("d3528351-e405-4352-9266-1f3145651111") },
                new InvoiceLine { Id = new Guid("d3528351-e405-4352-9266-1f3145652222") },
            };

            var linePuts = new List<ApiModels.InvoiceLinePut>()
            {
                new ApiModels.InvoiceLinePut { Id = new Guid("d3528351-e405-4352-9266-1f3145651111") },
                new ApiModels.InvoiceLinePut { Id = new Guid("d3528351-e405-4352-9266-1f3145651111") },
                new ApiModels.InvoiceLinePut { },
            };

            var result = validator.ValidatePut(new Invoice { }, new InvoicePut { InvoiceLinePuts = linePuts }, dblines);

            Assert.AreEqual(result.Errors[0].Message, "duplicate put lines exists");
        }

        public void ValidatePutLineTotalWithInvoiceSum()
        {

            var validator = new InvoiceValidator();

            var linePuts = new List<ApiModels.InvoiceLinePut>()
            {
                new InvoiceLinePut { LinePrice = 100 },
                new InvoiceLinePut { LinePrice = 300 },
            };

            var result = validator.ValidatePut(new Invoice { }, new InvoicePut { InvoiceLinePuts = linePuts }, null);

            Assert.AreEqual(result.Errors[0].Message, "Line total not equal to invoice sum");
        }

        public void ValidateIndividualPutLineTotalWithUnitPriceAndQuantity()
        {
            var validator = new InvoiceValidator();

            var linePuts = new List<ApiModels.InvoiceLinePut>()
            {
                new InvoiceLinePut { LinePrice = 100, Quantity = 2, UnitPrice = 100, Name = "Line 1" },
                new InvoiceLinePut { LinePrice = 300, Quantity = 3, UnitPrice = 50, Name = "line 2" },
            };

            var expectedErrorMessage = string.Join(", ", linePuts.Select(linePut => linePut.Name + " line sum not aligned with quatity and unit Price"));
            var result = validator.ValidatePut(new Invoice { }, new InvoicePut { InvoiceLinePuts = linePuts }, null);
            var errorMessage = string.Join(", ", result.Errors);

            Assert.AreEqual(expectedErrorMessage, errorMessage);
        }

        public void ValidateInvoiceLineCount()
        {
            var validator = new InvoiceValidator();

            var linePuts = new List<ApiModels.InvoiceLinePut>();

            var result = validator.ValidatePut(new Invoice { }, new InvoicePut { InvoiceLinePuts = linePuts }, null);

            Assert.AreEqual(result.Errors[0].Message, "Atleast require one invoice line");
        }

        [TestMethod()]
        public async Task ValidateInvoiceRouteIdAndPutId()
        {
            var invoiceId1 = new Guid("d3528351-e405-4352-9266-1f3145652360");
            var invoiceSystem = new InvoiceSystem(null, null);
            var result = await invoiceSystem.UpdateInvoice(invoiceId1, new ApiModels.InvoicePut { Id = Guid.NewGuid() });

            Assert.AreEqual(result.Errors[0].Message, "invoice route id and modal id not equal");
        }
    }
}
