using System;
using System.Collections.Generic;

#nullable disable

namespace soen390_team01.Data.Entities
{
    public partial class Material: Item
    {
        public Material()
        {
            PartMaterials = new HashSet<PartMaterial>();
        }
        public DateTime Added { get; set; }
        public virtual ICollection<PartMaterial> PartMaterials { get; set; }
    }
}
