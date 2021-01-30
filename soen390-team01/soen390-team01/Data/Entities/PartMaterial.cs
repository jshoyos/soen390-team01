using System;
using System.Collections.Generic;

#nullable disable

namespace soen390_team01.Data.Models
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
