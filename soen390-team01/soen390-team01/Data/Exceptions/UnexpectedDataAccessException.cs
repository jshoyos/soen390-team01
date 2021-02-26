namespace soen390_team01.Data.Exceptions
{
    public class UnexpectedDataAccessException : DataAccessException
    {
        public UnexpectedDataAccessException(string code) 
            : base("Database error", BuildMessage(code))
        {}

        protected new static string BuildMessage(string code)
        {
            return "Code " + code + ", please contact system admin.";
        }
    }
}
