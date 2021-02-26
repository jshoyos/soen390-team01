namespace soen390_team01.Data.Exceptions
{
    public class UnexpectedDbContextException : DbContextException
    {
        public UnexpectedDbContextException(string code) 
            : base("Database error", BuildMessage(code))
        {
        }

        protected new static string BuildMessage(string code)
        {
            return "Code " + code + ", please contact system admin.";
        }
    }
}
