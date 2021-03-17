using NUnit.Framework;
using soen390_team01.Data.Queries;

namespace soen390_team01Tests.Unit.Data.Queries
{
    class RangeFilterTest
    {
        [Test]
        public void GetConditionStringMinTest()
        {
            var filter = new RangeFilter("material", "Price", "price") {
                Min = 5
            };
            Assert.AreEqual("price::numeric >= 5", filter.GetConditionString());
        }

        [Test]
        public void GetConditionStringMaxTest()
        {
            var filter = new RangeFilter("material", "Price", "price")
            {
                Max = 5
            };
            Assert.AreEqual("price::numeric <= 5", filter.GetConditionString());
        }

        [Test]
        public void GetConditionStringMinMaxTest()
        {
            var filter = new RangeFilter("material", "Price", "price")
            {
                Min = 3,
                Max = 5
            };
            Assert.AreEqual("price::numeric >= 3 and price::numeric <= 5", filter.GetConditionString());
        }

        [Test]
        public void IsActiveTest()
        {
            var filter = new RangeFilter("material", "Price", "price");
            Assert.IsFalse(filter.IsActive(), "IsActive() should return false when Min and Max are null");

            filter = new RangeFilter(
                new Filter("bike", "Grade", "grade") {
                    Input = new FilterInput(rangeInput: new RangeFilterInput {
                        MinValue = 5
                    })
                });
            Assert.IsTrue(filter.IsActive(), "IsActive() should return true when Min or Max is not null");
        }
    }
}
