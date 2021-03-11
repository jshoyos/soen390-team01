using System;
using System.Text;

namespace soen390_team01.Data.Queries
{
    public class DateRangeFilter : Filter
    {
        public DateTime? Min { get; set; }
        public DateTime? Max { get; set; }

        public DateRangeFilter(Filter filter) : this(filter.Table, filter.DisplayColumn, filter.Column)
        {
            Input = filter.Input;
            Min = filter.Input.DateRangeInput.MinValue;
            Max = filter.Input.DateRangeInput.MaxValue;
        }
        public DateRangeFilter(string table, string displayColumn, string column) : base(table, displayColumn, column)
        {
            Min = null;
            Max = null;
        }

        public override string GetConditionString()
        {
            var sb = new StringBuilder();
            var validMin = false;

            if (Min != null)
            {
                sb.Append($"{Column}>= {new NpgsqlTypes.NpgsqlDate((DateTime)Min)}");
                validMin = true;
            }
            if (Max != null)
            {
                if (validMin)
                {
                    sb.Append(" and ");
                }
                sb.Append($"{Column} <= {new NpgsqlTypes.NpgsqlDate((DateTime)Max)}");
            }

            return sb.ToString();
        }

        public override bool IsActive()
        {
            return Min != null || Max != null;
        }
    }
}
