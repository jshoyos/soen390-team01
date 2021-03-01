using System.Linq;
using Microsoft.AspNetCore.Mvc.Filters;

namespace soen390_team01.Data.Queries
{
    public class FiltersAction : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {

            var filters = (Filters) context.ActionArguments["filters"];

            if (filters == null)
            {
                return;
            }

            for (var i = 0; i < filters.List.Count; i++)
            {
                var filter = filters.List.ElementAt(i);
                var input = filter.Input;

                if (filter.Input == null)
                {
                    continue;
                }

                if (filter.Input.StringValue != null)
                {
                    filter = new StringFilter(filter) {Input = input};
                } else if (filter.Input.SelectInput != null)
                {
                    filter = new SelectFilter(filter) { Input = input };
                }

                filters.List[i] = filter;
            }
        }
    }
}
