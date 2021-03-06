using System.Text;

namespace soen390_team01.Data.Queries
{
    public class RangeFilter : Filter
    {
        public decimal? Min { get; set; }
        public decimal? Max { get; set; }

        public RangeFilter(Filter filter) : this(filter.Table, filter.DisplayColumn, filter.Column)
        {
            Input = filter.Input;
            Min = filter.Input.RangeInput.MinValue;
            Max = filter.Input.RangeInput.MaxValue;
        }
        public RangeFilter(string table, string displayColumn, string column) : base(table, displayColumn, column)
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
                sb.Append($"{Column}::numeric >= {Min}");
                validMin = true;
            }
            if (Max != null)
            {
                if (validMin)
                {
                    sb.Append(" and ");
                }
                sb.Append($"{Column}::numeric <= {Max}");
            }

            return sb.ToString();
        }

        public override bool IsActive()
        {
            return Min != null || Max != null;
        }
    }
}
