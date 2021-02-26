#region Header

// Author: Tommy Andrews
// File: NullValueException.cs
// Project: soen390-team01
// Created: 02/26/2021
// 

#endregion

namespace soen390_team01.Data.Exceptions
{
    public class AccountRegistrationException : DataAccessException
    { 
        public AccountRegistrationException()
            : base("Account registration failed", "Try again later.")
        {}
    }
}