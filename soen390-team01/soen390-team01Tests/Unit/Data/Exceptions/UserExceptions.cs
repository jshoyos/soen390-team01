using NUnit.Framework;
using soen390_team01.Data.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace soen390_team01Tests.Unit.Data.Exceptions
{
    public class UserExceptions
    {
        [Test]
        public void UserTest()
        {
            Assert.IsInstanceOf(typeof(DataAccessException), new EmailExistsException("email exists"));
            Assert.IsInstanceOf(typeof(DataAccessException), new EmailNotFoundException("user doesn't exist"));
        }
    }
}
