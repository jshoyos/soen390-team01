using System.Collections.Generic;
using NUnit.Framework;
using soen390_team01.Data.Queries;

namespace soen390_team01Tests.Unit.Data.Queries
{
    class FiltersTest
    {
        [Test]
        public void ActiveFiltersTest()
        {
            var filters = new Filters("bike");
            filters.Add(new StringFilter("bike", "Grade", "grade") {
                Value = "some_value"
            });
            filters.Add(new SelectFilter("bike", "Name", "name", new List<string>() { "some_value", "some_other_value" }) {
                    Value = "some_other_value"
            });

            filters.Add(new Filter("bike", "Invalid", "invalid"));

            Assert.AreEqual("grade LIKE '%some_value%' and name = 'some_other_value'", filters.GetConditionsString());
        }

        [Test]
        public void NoActiveFiltersTest()
        {
            var filters = new Filters("bike");
            filters.Add(new StringFilter("bike", "Grade", "grade"));
            filters.Add(new SelectFilter("bike", "Name", "name", new List<string>() { "some_value", "some_other_value" }));
            filters.Add(new Filter("bike", "Invalid", "invalid"));

            Assert.AreEqual("", filters.GetConditionsString(), "Filters with no value should not create a condition");
        }
    }
}
