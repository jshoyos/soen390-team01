#region Header

// Author: Tommy Andrews
// File: UnauthorizedInsertionException.cs
// Project: soen390-team01
// Created: 02/26/2021
// 

#endregion

namespace soen390_team01.Data.Exceptions
{
    public class UnauthorizedInsertionException : DbContextException
    {
        public UnauthorizedInsertionException(string entity)
            : base("Unauthorized addition", BuildMessage(entity))
        {
        }

        protected new static string BuildMessage(string entity)
        {
            return "Trying to add an already existing " + entity;
        }
    }
}