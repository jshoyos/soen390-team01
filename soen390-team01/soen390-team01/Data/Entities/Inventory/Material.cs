#region Header

// Author: Tommy Andrews
// File: Material.cs
// Project: soen390-team01
// Created: 02/16/2021
// 

#endregion

using System.Collections.Generic;

#nullable disable

namespace soen390_team01.Data.Entities
{
    public class Material : Item
    {
        public Material()
        {
            PartMaterials = new HashSet<PartMaterial>();
        }

        public virtual ICollection<PartMaterial> PartMaterials { get; set; }
    }
}