using System.Collections.Generic;
using System.Linq;
using System.Text;
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

            var sb = new StringBuilder();
            sb.Append(activeList.ElementAt(0).GetConditionString());

            for (var i = 1; i < activeList.Count; i++)
            {
                var filter = activeList[i];
                sb.Append(" and " + filter.GetConditionString());
            }

            return sb.ToString();
        }

        public bool AnyActive()
        {
            return List.Count > 0 && List.Any(f => f.IsActive());
        }
    }
}
