using System;
using System.Collections.Generic;

#nullable disable

namespace soen390_team01.Data.Models
{
    public partial class Part
    {
        public Part()
        {
            BikeParts = new HashSet<BikePart>();
        }

        public long PartId { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Grade { get; set; }
        public string Size { get; set; }

        public virtual PartMaterial PartMaterial { get; set; }
        public virtual ICollection<BikePart> BikeParts { get; set; }
    }
}
