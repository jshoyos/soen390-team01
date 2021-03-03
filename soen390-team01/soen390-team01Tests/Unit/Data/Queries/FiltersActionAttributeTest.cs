using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Routing;
using Moq;
using NUnit.Framework;
using soen390_team01.Data.Queries;

namespace soen390_team01Tests.Unit.Data.Queries
{
    class FiltersActionAttributeTest
    {
        [Test]
        public void ValidFiltersTest()
        {
            var attribute = new FiltersActionAttribute();
            var actionArguments = new Dictionary<string, object>();
            var filters = new Filters("bike");
            filters.Add(new Filter("bike", "Grade", "grade") {
                Input = new FilterInput("some_value")
            });
            filters.Add(new Filter("bike", "Grade", "grade") {
                Input = new FilterInput(selectInput: new SelectFilterInput() {
                    PossibleValues = new List<string>() { "some_value", "some_other_value" },
                    SelectValue = "some_value"
                })
            });
            filters.Add(new Filter("bike", "Grade", "grade"));

            actionArguments["filters"] = filters;

            var context = new ActionExecutingContext(
                new ActionContext(
                    Mock.Of<HttpContext>(), 
                    Mock.Of<RouteData>(), 
                    Mock.Of<ActionDescriptor>(), 
                    new ModelStateDictionary()
                    ), 
                new List<IFilterMetadata>(),
                actionArguments,
                Mock.Of<Controller>()
            );

            attribute.OnActionExecuting(context);

            var modifiedFilters = context.ActionArguments["filters"] as Filters;

            Assert.NotNull(modifiedFilters);
            Assert.IsTrue(modifiedFilters.AnyActive());
            Assert.IsInstanceOf(typeof(StringFilter), modifiedFilters.List.ElementAt(0), "First filter should be a StringFilter");
            Assert.IsInstanceOf(typeof(SelectFilter), modifiedFilters.List.ElementAt(1), "Second filter should be a SelectFilter");
            Assert.IsInstanceOf(typeof(Filter), modifiedFilters.List.ElementAt(2), "Third filter should be a generic Filter");
        }

        [Test]
        public void NullFiltersTest()
        {
            var attribute = new FiltersActionAttribute();
            var actionArguments = new Dictionary<string, object> {["filters"] = null};

            var context = new ActionExecutingContext(
                new ActionContext(
                    Mock.Of<HttpContext>(),
                    Mock.Of<RouteData>(),
                    Mock.Of<ActionDescriptor>(),
                    new ModelStateDictionary()
                ),
                new List<IFilterMetadata>(),
                actionArguments,
                Mock.Of<Controller>()
            );

            attribute.OnActionExecuting(context);

            Assert.Null(context.ActionArguments["filters"]);
        }
    }
}
