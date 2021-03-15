using NUnit.Framework;
using soen390_team01.Data.Queries;
using System.Collections.Generic;

namespace soen390_team01Tests.Unit.Data.Queries
{
    class TransfersQueryBuilderTest
    {
        [Test]
        public void FilterProcurementTest()
        {
            List<string> list = new List<string>
            {
                "Vendor1",
                "Vendor2",
                "Vendor3"
            };
            var filters = new Filters("procurement");
            filters.Add(new SelectFilter("procurement", "Vendor", "vendor",list) { Value = "Vendor1"});

            Assert.AreEqual(
                "Select procurement.procurement_id, procurement.item_id, procurement.payment_id, procurement.item_quantity, procurement.state, procurement.type, procurement.vendor_id, vendor.name " +
                "From public.procurement, public.vendor where vendor.vendor_id = procurement.vendor_id and vendor.name= 'Vendor1'",
                TransfersQueryBuilder.FilterProcurement(filters)
                );
        }

        [Test]
        public void FilterOrderTest()
        {
            List<string> list = new List<string>
            {
                "Canceled",
                "Pending",
                "Approved"
            };
            List<string> list2 = new List<string>
            {
                "Canceled"
            };
            var filters = new Filters("order");
            filters.Add(new CheckboxFilter("order", "Status", "state", list) { Values = list2 });

            Assert.AreEqual(
                "Select * From public.order where order.state in ('Canceled')",
                TransfersQueryBuilder.FilterOrder(filters)
            );
        }


        // return $"Select * From public.{filters.Table} where {filters.GetConditionsString()}";
    }
}
