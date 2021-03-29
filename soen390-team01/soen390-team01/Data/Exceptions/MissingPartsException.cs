using System;
using System.Collections.Generic;
using soen390_team01.Services;

namespace soen390_team01.Data.Exceptions
{
    public class MissingPartsException : Exception
    {
        public List<MissingPart> MissingParts { get; }

        public MissingPartsException(List<MissingPart> missingParts) : base("Some parts cannot be built")
        {
            MissingParts = missingParts;
        }
    }
}
