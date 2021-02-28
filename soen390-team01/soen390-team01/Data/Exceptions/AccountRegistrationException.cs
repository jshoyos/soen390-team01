namespace soen390_team01.Data.Exceptions
{
    public class AccountRegistrationException : DataAccessException
    { 
        public AccountRegistrationException()
            : base("Account registration failed", "Try again later.")
        {}
    }
}
