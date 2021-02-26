using Npgsql;

namespace soen390_team01.Data.Exceptions
{
    public abstract class DbContextExceptionProvider
    {
        private static readonly string _name;

        private DbContextExceptionProvider()
        { }


        public static DbContextException Provide(PostgresException exception)
        {
            return exception.SqlState switch
            {
                "23502" => new NullValueException(exception.ColumnName),
                "23505" => new NonUniqueValueException(exception.ColumnName),
                _ => new UnexpectedDbContextException(exception.SqlState)
            };
        }
    }
}
