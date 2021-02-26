namespace soen390_team01.Data.Exceptions
{
    public class NullValueException : DataAccessException
    {
        public NullValueException()
            : base("Null value", "Some required fields are missing")
        {}

        public NullValueException(string field)
            : base("Null value", BuildMessage(field))
        {}

        protected new static string BuildMessage(string field)
        {
            return field + " field is missing";
        }
    }
}
