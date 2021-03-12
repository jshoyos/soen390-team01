using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

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
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime Added { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime Updated { get; set; }
        public virtual ICollection<BikePart> BikeParts { get; set; }
        public virtual ICollection<PartMaterial> PartMaterials { get; set; }
    }
}
