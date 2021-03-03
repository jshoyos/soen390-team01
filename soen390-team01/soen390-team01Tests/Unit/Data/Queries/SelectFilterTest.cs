using System.Collections.Generic;
using NUnit.Framework;
using soen390_team01.Data.Queries;

namespace soen390_team01Tests.Unit.Data.Queries
{
    class SelectFilterTest
    {
        [Test]
        public void GetConditionStringTest()
        {
            var filter = new SelectFilter(
                new Filter("bike", "Grade", "grade") {
                    Input = new FilterInput(selectInput: new SelectFilterInput() {
                        PossibleValues = new List<string>() { "some_value", "some_other_value" },
                        SelectValue = "some_value"
                    })
                });

            Assert.AreEqual(2, filter.SelectValues.Count);
            Assert.AreEqual("grade = 'some_value'", filter.GetConditionString());
        }
    }
}
