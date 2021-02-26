#region Header

// Author: Tommy Andrews
// File: Part.cs
// Project: soen390-team01
// Created: 02/16/2021
// 

#endregion

using System.Collections.Generic;

#nullable disable

namespace soen390_team01.Data.Entities
{
    public class Part : Item
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