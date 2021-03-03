using NUnit.Framework;
using soen390_team01.Data.Queries;

namespace soen390_team01Tests.Unit.Data.Queries
{
    class FilterTest
    {
        [Test]
        public void Test()
        {
            var filter = new Filter("material", "Name", "name");

            Assert.AreEqual("", filter.GetConditionString());
            Assert.IsFalse(filter.IsActive());
        }
    }
}
