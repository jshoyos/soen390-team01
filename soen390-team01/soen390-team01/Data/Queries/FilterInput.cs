namespace soen390_team01.Data.Queries
{
    public class FilterInput
    {
        public string StringValue { get; set; }
        public SelectFilterInput SelectInput { get; set; }

        public FilterInput(string stringValue = null, SelectFilterInput selectInput = null)
        {
            StringValue = stringValue;
            SelectInput = selectInput;
        }
    }
}
