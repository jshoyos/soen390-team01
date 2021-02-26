#region Header

// Author: Tommy Andrews
// File: BikePart.cs
// Project: soen390-team01
// Created: 02/16/2021
// 

#endregion

#nullable disable

namespace soen390_team01.Data.Entities
{
    public class BikePart
    {
        public long BikeId { get; set; }
        public long PartId { get; set; }
        public int PartQuantity { get; set; }
        public long BikePartId { get; set; }

        public virtual Bike Bike { get; set; }
        public virtual Part Part { get; set; }
    }
}