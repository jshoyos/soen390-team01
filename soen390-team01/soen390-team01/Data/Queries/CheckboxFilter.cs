using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace soen390_team01.Data.Queries
{
    public class CheckboxFilter : Filter
    {
        public List<string> Values { get; set; }
        public List<string> PossibleCheckboxValues { get; set; }

        public CheckboxFilter(Filter filter) : this(filter.Table, filter.DisplayColumn, filter.Column, filter.Input.CheckboxInput.PossibleValues)
        {
            Input = filter.Input;
            Values = filter.Input.CheckboxInput.Values ?? new List<string>();
            PossibleCheckboxValues = filter.Input.CheckboxInput.PossibleValues;
        }
        public CheckboxFilter(string table, string displayColumn, string column, List<string> possibleCheckboxValues) : base(table, displayColumn, column)
        {
            Values = new List<string>();
            PossibleCheckboxValues = possibleCheckboxValues;
        }

        public override string GetConditionString()
        {
            var sb = new StringBuilder();

            sb.Append($"{Table}.{Column} in (");

            foreach (var value in Values)
            {
                sb.Append($"'{value}',");
            }

            // Removing last comma
            sb.Length--;

            sb.Append(')');

            return sb.ToString();
        }

        public override bool IsActive()
        {
            return Values.Count > 0 && Values.All(v => !string.IsNullOrEmpty(v));
        }
    }
}
