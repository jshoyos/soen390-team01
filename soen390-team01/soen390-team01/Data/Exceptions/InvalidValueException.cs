#region Header

// Author: Tommy Andrews
// File: NullValueException.cs
// Project: soen390-team01
// Created: 02/26/2021
// 

#endregion

namespace soen390_team01.Data.Exceptions
{
    public class InvalidValueException : DataAccessException
    { 
        public InvalidValueException(string field, string value)
            : base("Invalid value", BuildMessage(field, value))
        {}

        private static string BuildMessage(string field, string value)
        {
            return field + " cannot have value " + value;
        }
    }
}