using System;

namespace soen390_team01.Data.Exceptions
{
    public abstract class DataAccessException : Exception
    {
        private readonly string _name;

        protected DataAccessException(string name, string message) : base(message)
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
