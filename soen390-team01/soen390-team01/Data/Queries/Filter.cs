namespace soen390_team01.Data.Queries
{
    public class Filter
    {
        public string Table { get; protected set; }
        public string Column { get; protected set; }
        public string DisplayColumn { get; protected set; }
        public FilterInput Input { get; set; }

        public Filter(string table, string displayColumn, string column)
        {
            Input = null;
            Table = table;
            DisplayColumn = displayColumn;
            Column = column;
        }

        public virtual string GetConditionString()
        {
            return "";
        }

        public virtual bool IsActive()
        {
            return false;
        }
    }
}
