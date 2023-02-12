using Microsoft.VisualStudio.TestTools.UnitTesting;
using GPStarAPI.Invoices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            var result = validator.Validate(null, null, null);

            Assert.AreEqual(result.Errors[0].Message, "invoice not found");
        }

        [TestMethod()]
        public void ValidateInvoicePutNull()
        {

            var validator = new InvoiceValidator();
            var result = validator.Validate(new Models.Invoice { }, null, null);

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

            var result = validator.Validate(new Invoice { }, new InvoicePut { InvoiceLinePuts = new List<InvoiceLinePut>(linePuts) }, null);

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

            var result = validator.Validate(new Invoice { }, new InvoicePut { InvoiceLinePuts = linePuts }, dblines);

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

            var result = validator.Validate(new Invoice { }, new InvoicePut { InvoiceLinePuts = linePuts }, dblines);

            Assert.AreEqual(result.Errors[0].Message, "duplicate put lines exists");
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
