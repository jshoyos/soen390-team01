namespace soen390_team01.Data.Exceptions
{
    public class InvalidValueException : DataAccessException
    { 
        public InvalidValueException(string field, string value)
            : base("Invalid value", BuildMessage(field, value))
        {}

        private static string BuildMessage(string field, string value)
        {
            return field + " cannot have value " + value;
        }
    }
}
