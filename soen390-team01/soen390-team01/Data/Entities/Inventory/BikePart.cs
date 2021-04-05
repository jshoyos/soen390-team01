using System;
using System.Collections.Generic;

#nullable disable

namespace soen390_team01.Data.Entities
{
    public partial class BikePart
    {
        public long BikeId { get; set; }
        public long PartId { get; set; }
        public int PartQuantity { get; set; }

        public virtual Bike Bike { get; set; }
        public virtual Part Part { get; set; }
    }
}
