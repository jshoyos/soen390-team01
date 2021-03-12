using System;
using NUnit.Framework;
using soen390_team01.Data.Queries;

namespace soen390_team01Tests.Unit.Data.Queries
{
    class DateRangeFilterTest
    {
        [Test]
        public void GetConditionStringMinTest()
        {
            var filter = new DateRangeFilter("material", "Date", "date")
            {
                MinDate = new DateTime(2021, 01, 01)
            };
            Assert.AreEqual("date >= '2021-01-01 00:00:00'", filter.GetConditionString());
        }

        [Test]
        public void GetConditionStringMaxTest()
        {
            var filter = new DateRangeFilter("material", "Date", "date")
            {
                MaxDate = new DateTime(2021, 02, 02)
            };
            Assert.AreEqual("date <= '2021-02-02 00:00:00'", filter.GetConditionString());
        }

        [Test]
        public void GetConditionStringMinMaxTest()
        {
            var filter = new DateRangeFilter("material", "Date", "date")
            {
                MinDate = new DateTime(2021, 01, 01),
                MaxDate = new DateTime(2021, 02, 02)
            };
            Assert.AreEqual("date >= '2021-01-01 00:00:00' and date <= '2021-02-02 00:00:00'", filter.GetConditionString());
        }

        [Test]
        public void IsActiveTest()
        {
            var filter = new DateRangeFilter("material", "Price", "price");
            Assert.IsFalse(filter.IsActive(), "IsActive() should return false when MinDate and MaxDate are null");

            filter = new DateRangeFilter(
                new Filter("bike", "Date", "date")
                {
                    Input = new FilterInput(dateRangeInput: new DateRangeFilterInput
                    {
                        MinValue = new DateTime(2021, 01, 01),
                    })
                });
            Assert.IsTrue(filter.IsActive(), "IsActive() should return true when MinDate or MaxDate is not null");
        }
    }
}