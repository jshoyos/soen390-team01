using NUnit.Framework;
using soen390_team01.Data.Queries;

namespace soen390_team01Tests.Unit.Data.Queries
{
    class ProductQueryBuilderTest
    {
        [Test]
        public void FilterProductTest()
        {
            var filters = new Filters("bike");
            filters.Add(new StringFilter("bike", "Name", "name") { Value = "some_value"});

            Assert.AreEqual(
                "Select * From public.bike where name LIKE '%some_value%'",
                ProductQueryBuilder.FilterProduct(filters)
                );
        }

        [Test]
        public void GetProductTest()
        {
            Assert.AreEqual(
                "Select * From public.part where item_id = 5",
                ProductQueryBuilder.GetProduct("part", 5)
            );
        }
    }
}
