using System;
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
using soen390_team01.Models;

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
                Input = new FilterInput(selectInput: new SelectFilterInput {
                    PossibleValues = new List<string>() { "some_value", "some_other_value" },
                    SelectValue = "some_value"
                })
            });
            filters.Add(new Filter("bike", "Grade", "grade") {
                Input = new FilterInput(checkboxInput: new CheckboxFilterInput {
                    PossibleValues = new List<string> { "some_value", "some_other_value" },
                    Values = new List<string> {"some_value"}
                })
            });
            filters.Add(new Filter("bike", "Price", "price") {
                Input = new FilterInput(rangeInput: new RangeFilterInput {
                    MinValue = 3,
                    MaxValue = 5
                })
            });
            filters.Add(new Filter("bike", "Price", "price")
            {
                Input = new FilterInput(dateRangeInput: new DateRangeFilterInput {
                    MinValue = new DateTime(2021, 01, 01),
                    MaxValue = new DateTime(2021, 02, 02)
                })
            });
            filters.Add(new Filter("bike", "Grade", "grade"));

            actionArguments["mobileFiltersInput"] = new MobileFiltersInput { Filters = filters };

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

            var modifiedFilters = (context.ActionArguments["mobileFiltersInput"] as MobileFiltersInput)?.Filters;

            Assert.NotNull(modifiedFilters);
            Assert.IsTrue(modifiedFilters.AnyActive());
            Assert.IsInstanceOf(typeof(StringFilter), modifiedFilters.List.ElementAt(0), "First filter should be a StringFilter");
            Assert.IsInstanceOf(typeof(SelectFilter), modifiedFilters.List.ElementAt(1), "Second filter should be a SelectFilter");
            Assert.IsInstanceOf(typeof(CheckboxFilter), modifiedFilters.List.ElementAt(2), "Third filter should be a CheckboxFilter");
            Assert.IsInstanceOf(typeof(RangeFilter), modifiedFilters.List.ElementAt(3), "Fourth filter should be a RangeFilter");
            Assert.IsInstanceOf(typeof(DateRangeFilter), modifiedFilters.List.ElementAt(4), "Fifth filter should be a DateRangeFilter");
            Assert.IsInstanceOf(typeof(Filter), modifiedFilters.List.ElementAt(5), "Fifth filter should be a generic Filter");
        }

        [Test]
        public void NullFiltersTest()
        {
            var attribute = new FiltersActionAttribute();
            var actionArguments = new Dictionary<string, object> {["mobileFiltersInput"] = null};

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

            Assert.Null(context.ActionArguments["mobileFiltersInput"]);
        }
    }
}
