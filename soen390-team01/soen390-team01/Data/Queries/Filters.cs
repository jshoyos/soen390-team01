using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace soen390_team01.Data.Queries
{
    public class Filters
    {
        public List<Filter> List { get; set; }
        public string Table { get; set; }

        public Filters(string table, List<Filter> list = null)
        {
            Table = table;
            List = list ?? new List<Filter>();
        }

        public void Add(Filter filter)
        {
            List.Add(filter);
        }

        public string GetConditionsString()
        {
            var activeList = List.FindAll(f => f.IsActive());

            if (activeList.Count == 0)
            {
                return "";
            }

            var conditionString = activeList.ElementAt(0).GetConditionString();

            foreach (var filter in activeList)
            {
                conditionString += " and " + filter.GetConditionString();
            }

            return conditionString;
        }

        public bool AnyActive()
        {
            return List.Any(f => f.IsActive());
        }
    }
}
