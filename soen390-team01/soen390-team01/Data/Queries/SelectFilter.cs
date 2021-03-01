using System.Collections.Generic;

namespace soen390_team01.Data.Queries
{
    public class SelectFilter : StringFilter
    {
        public List<string> SelectValues { get; set; }

        public SelectFilter(Filter filter) : base(filter)
        {
            Value = filter.Input.SelectInput.SelectValue;
            SelectValues = filter.Input.SelectInput.PossibleValues;
        }
        public SelectFilter(string table, string displayColumn, string column, List<string> selectValues) : base(table, displayColumn, column)
        {
            SelectValues = selectValues;
        }
    }
}
