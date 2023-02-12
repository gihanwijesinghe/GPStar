using Microsoft.VisualStudio.TestTools.UnitTesting;
using GPStarAPI.Invoices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPStarAPI.Invoices.Tests
{
    [TestClass()]
    public class InvoiceValidatorTests
    {
        [TestMethod()]
        public void ValidateInvoiceDbNull()
        {

            var validator = new InvoiceValidator();
            var result = validator.Validate(null, null);

            Assert.AreEqual(result.Errors[0].Message, "invoice not found");
        }

        [TestMethod()]
        public void ValidateInvoicePutNull()
        {

            var validator = new InvoiceValidator();
            var result = validator.Validate(new Models.Invoice { }, null);

            Assert.AreEqual(result.Errors[0].Message, "invoice put modal not found");
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
