using soen390_team01.Data.Entities;
using System;
using System.Collections.Generic;

#nullable disable

namespace soen390_team01.Data.Entities
{
    public partial class PartMaterial
    {
        public long PartId { get; set; }
        public long MaterialId { get; set; }
        public int MaterialQuantity { get; set; }

        public virtual Material Material { get; set; }
        public virtual Part Part { get; set; }
    }
}
