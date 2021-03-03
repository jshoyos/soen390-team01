using NUnit.Framework;
using soen390_team01.Data.Queries;

namespace soen390_team01Tests.Unit.Data.Queries
{
    class StringFilterTest
    {
        [Test]
        public void GetConditionStringTest()
        {
            var filter = new StringFilter("material", "Name", "name") {
                Value = "some_value"
            };
            Assert.AreEqual("name LIKE '%some_value%'", filter.GetConditionString());
        }

        [Test]
        public void IsActiveTest()
        {
            var filter = new StringFilter("material", "Name", "name");
            Assert.IsFalse(filter.IsActive(), "IsActive() should return false when Value is empty");

            filter = new StringFilter(
                new Filter("bike", "Grade", "grade") {
                    Input = new FilterInput("some_value")
                });
            Assert.IsTrue(filter.IsActive(), "IsActive() should return true when Value is not empty");
        }
    }
}
