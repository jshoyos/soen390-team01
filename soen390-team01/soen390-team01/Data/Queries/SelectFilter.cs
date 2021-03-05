using System.Collections.Generic;

namespace soen390_team01.Data.Queries
{
    public class SelectFilter : StringFilter
    {
        public List<string> PossibleSelectValues { get; set; }

        public SelectFilter(Filter filter) : base(filter)
        {
            Input = filter.Input;
            Value = filter.Input.SelectInput.SelectValue;
            PossibleSelectValues = filter.Input.SelectInput.PossibleValues;
        }
        public SelectFilter(string table, string displayColumn, string column, List<string> possibleSelectValues) : base(table, displayColumn, column)
        {
            PossibleSelectValues = possibleSelectValues;
        }

        public override string GetConditionString()
        {
            return Column + " = '" + Value + "'";
        }
    }
}
