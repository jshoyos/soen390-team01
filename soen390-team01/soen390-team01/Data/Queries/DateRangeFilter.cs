using System;
using System.Text;

namespace soen390_team01.Data.Queries
{
    public class DateRangeFilter : Filter
    {
        public DateTime? MinDate { get; set; }
        public DateTime? MaxDate { get; set; }

        public DateRangeFilter(Filter filter) : this(filter.Table, filter.DisplayColumn, filter.Column)
        {
            Input = filter.Input;
            MinDate = filter.Input.DateRangeInput.MinValue;
            MaxDate = filter.Input.DateRangeInput.MaxValue;
        }
        public DateRangeFilter(string table, string displayColumn, string column) : base(table, displayColumn, column)
        {
            MinDate = null;
            MaxDate = null;
        }

        public override string GetConditionString()
        {
            var sb = new StringBuilder();
            var validMin = false;

            if (MinDate != null)
            {
                sb.Append($"{Column} >= '{new NpgsqlTypes.NpgsqlDateTime((DateTime)MinDate)}'");
                validMin = true;
            }
            if (MaxDate != null)
            {
                if (validMin)
                {
                    sb.Append(" and ");
                }
                sb.Append($"{Column} <= '{new NpgsqlTypes.NpgsqlDateTime((DateTime)MaxDate)}'");
            }

            return sb.ToString();
        }

        public override bool IsActive()
        {
            return MinDate != null || MaxDate != null;
        }
    }
}
