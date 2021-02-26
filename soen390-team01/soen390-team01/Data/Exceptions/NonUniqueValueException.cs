﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace soen390_team01.Data.Exceptions
{
    public class NonUniqueValueException : DbContextException
    {
        public NonUniqueValueException(string field) 
            : base("Duplicated value", BuildMessage(field))
        {
        }

        protected new static string BuildMessage(string field)
        {
            return "Field " + field + " should be unique.";
        }
    }
}
