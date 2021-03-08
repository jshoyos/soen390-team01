using System;
using System.Collections.Generic;
using soen390_team01.Data;

#nullable disable

namespace soen390_team01.Data.Entities
{
    public partial class Bike : Item
    {
        public Bike()
        {
            BikeParts = new HashSet<BikePart>();
        }
        public string Size { get; set; }
        public virtual ICollection<BikePart> BikeParts { get; set; }
    }
}
