using System;

namespace soen390_team01.Data.Exceptions
{
    public class InsufficientBikePartsException : Exception
    { 
        public InsufficientBikePartsException() : base("Less than 5 BikeParts for this Bike"){}
    }
}
