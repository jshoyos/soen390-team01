using System;
using System.Collections.Generic;

#nullable disable

namespace soen390_team01.Entities.Models
{
    public partial class Material: Items
    {
        public Material()
        {
            PartMaterials = new HashSet<PartMaterial>();
        }

        public long MaterialId { get; set; }
        public override string Name { get; set; }
        public override decimal Price { get; set; }
        public override string Grade { get; set; }

        public virtual ICollection<PartMaterial> PartMaterials { get; set; }
    }
}
