using Npgsql;
using NUnit.Framework;
using soen390_team01.Data.Exceptions;

namespace soen390_team01Tests.Services
{
    public class DbContextExceptionProviderTest
    {
        [Test]
        public void ProvideTest()
        {
            Assert.IsInstanceOf(
                typeof(NullValueException),
                DbAccessExceptionProvider.Provide(new PostgresException("", "", "", "23502"))
                );

            Assert.IsInstanceOf(
                typeof(NonUniqueValueException),
                DbAccessExceptionProvider.Provide(new PostgresException("", "", "", "23505"))
            );
            Assert.IsInstanceOf(
                typeof(UnexpectedDataAccessException),
                DbAccessExceptionProvider.Provide(new PostgresException("", "", "", "12345"))
            );
        }
    }
}
