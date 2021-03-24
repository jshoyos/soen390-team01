using System;
using System.Collections.Generic;

#nullable disable

namespace soen390_team01.Data.Entities
{
    public partial class Production
    {
        public string State { get; set; }
        public DateTime Added { get; set; }
        public DateTime Modified { get; set; }
        public long BikeId { get; set; }
        public long ProductionId { get; set; }

        public virtual Bike Bike { get; set; }
    }
}
