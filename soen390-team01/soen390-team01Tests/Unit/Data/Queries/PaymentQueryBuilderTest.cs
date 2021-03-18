using System.Collections.Generic;
using NUnit.Framework;
using soen390_team01.Data.Queries;

namespace soen390_team01Tests.Unit.Data.Queries
{
    class PaymentQueryBuilderTest
    {
        [Test]
        public void FilterPaymentTest()
        {
            List<string> list = new List<string>
            {
                "pending",
                "completed",
                "canceled"
            };
            var filters = new Filters("payment");
            var tabName = "payment";
            filters.Add(new CheckboxFilter("payment", $"State-{tabName}", "state", list) { Values = { "filtered_state" } });

            Assert.AreEqual(
                "Select * From public.payment where payment.state in ('filtered_state') and amount >= '0.0'",
                PaymentQueryBuilder.FilterPayment(filters, "and amount >= '0.0'")
                );
        }
    }
}
