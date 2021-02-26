using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace soen390_team01.Data.Exceptions
{
    public class NotFoundException : DbContextException
    {
        public NotFoundException(string entity, string identifier, string value) 
            : base("Non existent entity", BuildMessage(entity, identifier, value))
        {
        }
        private static string BuildMessage(string entity, string identifier, string value)
        {
            return entity + " with " + identifier + "=" + value + " was not found.";
        }
    }
}
