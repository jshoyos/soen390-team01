using System;
using System.Collections.Generic;

#nullable disable

namespace soen390_team01.Entities.Models
{
    public partial class Part: Items
    {
        public Part()
        {
            BikeParts = new HashSet<BikePart>();
        }

        public long PartId { get; set; }

        public virtual PartMaterial PartMaterial { get; set; }
        public virtual ICollection<BikePart> BikeParts { get; set; }
    }
}
