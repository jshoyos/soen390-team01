using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace soen390_team01.Data.Exceptions
{
    public abstract class DbContextException : Exception
    {
        private readonly string _name;

        protected DbContextException(string name, string message) : base(message)
        {
            _name = name;
        }

        public override string ToString()
        {
            return _name + ": " + Message;
        }

        protected static string BuildMessage(string messageInput)
        {
            return messageInput;
        }
    }
}
