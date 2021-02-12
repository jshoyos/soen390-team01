using System;
using System.Collections.Generic;

#nullable disable

namespace soen390_team01.Data.Entities
{
    public partial class Part : Item
    {
        public Part()
        {
            BikeParts = new HashSet<BikePart>();
            PartMaterials = new HashSet<PartMaterial>();
        }

        public string Size { get; set; }

        public virtual ICollection<BikePart> BikeParts { get; set; }
        public virtual ICollection<PartMaterial> PartMaterials { get; set; }
    }
}
