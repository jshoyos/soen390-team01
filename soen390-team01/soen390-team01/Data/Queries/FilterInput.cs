namespace soen390_team01.Data.Queries
{
    public class FilterInput
    {
        public string StringValue { get; set; }
        public SelectFilterInput SelectInput { get; set; }
        public RangeFilterInput RangeInput { get; set; }

        public FilterInput(string stringValue = null, SelectFilterInput selectInput = null, RangeFilterInput rangeInput = null)
        {
            StringValue = stringValue;
            SelectInput = selectInput;
            RangeInput = rangeInput;
        }
    }
}
