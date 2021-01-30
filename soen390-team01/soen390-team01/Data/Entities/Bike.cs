using System;
using System.Collections.Generic;

#nullable disable

namespace soen390_team01.Data.Models
{
    public partial class Bike: Items
    {
        public Bike()
        {
            BikeParts = new HashSet<BikePart>();
        }

        public long BikeId { get; set; }
        public override string Name { get; set; }
        public override decimal Price { get; set; }
        public override string Grade { get; set; }
        public string Size { get; set; }

        public virtual ICollection<BikePart> BikeParts { get; set; }
    }
}
