using System;
using System.Collections.Generic;

#nullable disable

namespace soen390_team01.Data.Models
{
    public partial class Material
    {
        public Material()
        {
            PartMaterials = new HashSet<PartMaterial>();
        }

        public long MaterialId { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Grade { get; set; }

        public virtual ICollection<PartMaterial> PartMaterials { get; set; }
    }
}
