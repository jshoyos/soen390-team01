using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using soen390_team01.Data;

#nullable disable

namespace soen390_team01.Data.Entities
{
    public partial class Bike : Item
    {
        public Bike()
        {
            BikeParts = new HashSet<BikePart>();
        }
        public string Size { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime Added { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime Updated { get; set; }
        public virtual ICollection<BikePart> BikeParts { get; set; }
        public virtual ICollection<Production> Productions { get; set; }

    }
}
