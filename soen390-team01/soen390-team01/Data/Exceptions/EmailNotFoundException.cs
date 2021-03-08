using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace soen390_team01.Data.Exceptions
{
    public class EmailNotFoundException : DataAccessException
    {
        public EmailNotFoundException() : base("Authentication", "Email was not found") { }
        public EmailNotFoundException(string message) : base("Authentication", message) { }
    }
}
