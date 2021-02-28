using Npgsql;

namespace soen390_team01.Data.Exceptions
{
    public abstract class DbAccessExceptionProvider
    {

        private DbAccessExceptionProvider()
        { }
        public static DataAccessException Provide(PostgresException exception)
        {
            return exception.SqlState switch
            {
                "23502" => new NullValueException(exception.ColumnName),
                "23505" => new NonUniqueValueException(exception.ColumnName),
                _ => new UnexpectedDataAccessException(exception.SqlState)
            };
        }
    }
}
