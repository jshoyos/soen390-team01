namespace soen390_team01.Data.Exceptions
{
    public class NotFoundException : DataAccessException
    {
        public NotFoundException(string entity, string identifier, string value) 
            : base("Non existent entity", BuildMessage(entity, identifier, value))
        {}
        private static string BuildMessage(string entity, string identifier, string value)
        {
            return entity + " with " + identifier + "=" + value + " was not found.";
        }
    }
}
