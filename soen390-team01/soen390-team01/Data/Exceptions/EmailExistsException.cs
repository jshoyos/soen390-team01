using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace soen390_team01.Data.Exceptions
{
    public class EmailExistsException : DataAccessException
    {
        public EmailExistsException() : base("Authentication","Email already exists"){}
        public EmailExistsException(string message) : base("Authentication", message) {}
    }
}
