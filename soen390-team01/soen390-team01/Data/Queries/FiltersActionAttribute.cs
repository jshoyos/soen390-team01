using System.Linq;
using Microsoft.AspNetCore.Mvc.Filters;
using soen390_team01.Models;

namespace soen390_team01.Data.Queries
{
    public class FiltersActionAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// Creates specific filter types from filter inputs received as an action parameter
        /// </summary>
        /// <param name="context">action call context</param>
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var mobileFiltersInput = (MobileFiltersInput) context.ActionArguments["mobileFiltersInput"];
            Filters filters;

            if (mobileFiltersInput == null || (filters = mobileFiltersInput.Filters) == null)
            {
                return;
            }

            for (var i = 0; i < filters.List.Count; i++)
            {
                var filter = filters.List.ElementAt(i);

                if (filter.Input == null)
                {
                    continue;
                }

                if (filter.Input.StringValue != null)
                {
                    filter = new StringFilter(filter);
                } 
                else if (filter.Input.SelectInput != null)
                {
                    filter = new SelectFilter(filter);
                } 
                else if (filter.Input.CheckboxInput != null)
                {
                    filter = new CheckboxFilter(filter);
                }
                else if (filter.Input.RangeInput != null)
                {
                    filter = new RangeFilter(filter);
                }
                else if (filter.Input.DateRangeInput != null)
                {
                    filter = new DateRangeFilter(filter);
                }

                filters.List[i] = filter;
            }
        }
    }
}
