using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace soen390_team01.Data.Entities
{
    public partial class Material: Item
    {
        public Material()
        {
            PartMaterials = new HashSet<PartMaterial>();
        }
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime Added { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime Updated { get; set; }
        public virtual ICollection<PartMaterial> PartMaterials { get; set; }
    }
}
