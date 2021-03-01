namespace soen390_team01.Data.Queries
{
    public class StringFilter : Filter
    {
        public string Value { get; set; }

        public StringFilter(Filter filter) : this(filter.Table, filter.DisplayColumn, filter.Column)
        {
            Value = filter.Input.StringValue;
        }
        public StringFilter(string table, string displayColumn, string column) : base(table, displayColumn, column)
        {
            Value = "";
        }

        public override string GetConditionString()
        {
            return Column + " = '" + Value + "'";
        }

        public override bool IsActive()
        {
            return !string.IsNullOrEmpty(Value);
        }
    }
}
