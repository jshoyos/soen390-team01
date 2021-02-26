#region Header

// Author: Tommy Andrews
// File: NullValueException.cs
// Project: soen390-team01
// Created: 02/26/2021
// 

#endregion

namespace soen390_team01.Data.Exceptions
{
    public class NullValueException : DbContextException
    {
        public NullValueException()
            : base("Null value", "Some required fields are missing")
        {
        }

        public NullValueException(string field)
            : base("Null value", BuildMessage(field))
        {
        }

        protected new static string BuildMessage(string field)
        {
            return field + " field is missing";
        }
    }
}