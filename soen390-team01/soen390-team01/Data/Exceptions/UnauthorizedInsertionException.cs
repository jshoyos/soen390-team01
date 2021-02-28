namespace soen390_team01.Data.Exceptions
{
    public class UnauthorizedInsertionException : DataAccessException
    {
        public UnauthorizedInsertionException(string entity)
            : base("Unauthorized addition", BuildMessage(entity))
        { }

        protected new static string BuildMessage(string entity)
        {
            return "Trying to add an already existing " + entity;
        }
    }
}