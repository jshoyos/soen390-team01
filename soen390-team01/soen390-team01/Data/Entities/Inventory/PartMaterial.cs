#region Header

// Author: Tommy Andrews
// File: PartMaterial.cs
// Project: soen390-team01
// Created: 02/16/2021
// 

#endregion

#nullable disable

namespace soen390_team01.Data.Entities
{
    public class PartMaterial
    {
        public long PartId { get; set; }
        public long MaterialId { get; set; }
        public int MaterialQuantity { get; set; }

        public virtual Material Material { get; set; }
        public virtual Part Part { get; set; }
    }
}