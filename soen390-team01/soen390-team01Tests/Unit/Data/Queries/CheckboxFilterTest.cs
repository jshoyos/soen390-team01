using System.Collections.Generic;
using NUnit.Framework;
using soen390_team01.Data.Queries;

namespace soen390_team01Tests.Unit.Data.Queries
{
    class CheckboxFilterTest
    {
        [Test]
        public void GetConditionStringTest()
        {
            var filter = new CheckboxFilter(
                new Filter("bike", "Grade", "grade") {
                    Input = new FilterInput(checkboxInput: new CheckboxFilterInput {
                        PossibleValues = new List<string> { "some_value", "some_other_value", "some_other_other_value" },
                        Values = new List<string> { "some_value", "some_other_other_value" }
                    })
                });

            Assert.AreEqual(2, filter.Values.Count);
            Assert.AreEqual(3, filter.PossibleCheckboxValues.Count);
            Assert.AreEqual("grade in ('some_value','some_other_other_value')", filter.GetConditionString());
        }

        [Test]
        public void IsActiveTest()
        {
            var filter = new CheckboxFilter(
                new Filter("bike", "Grade", "grade")
                {
                    Input = new FilterInput(checkboxInput: new CheckboxFilterInput
                    {
                        PossibleValues = new List<string> { "some_value", "some_other_value", "some_other_other_value" },
                    })
                });
            Assert.IsFalse(filter.IsActive(), "IsActive() should return false when Values is empty");

            filter = new CheckboxFilter(
                new Filter("bike", "Grade", "grade")
                {
                    Input = new FilterInput(checkboxInput: new CheckboxFilterInput
                    {
                        PossibleValues = new List<string> { "some_value", "some_other_value", "some_other_other_value" },
                        Values = new List<string> { "some_value", "some_other_other_value" }
                    })
                });

            Assert.IsTrue(filter.IsActive(), "IsActive() should return true when Values is not empty");
        }
    }
}
