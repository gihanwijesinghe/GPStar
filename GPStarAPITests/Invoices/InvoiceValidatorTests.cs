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
    }
}

