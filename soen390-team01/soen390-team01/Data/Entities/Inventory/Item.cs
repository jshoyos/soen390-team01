#region Header

// Author: Tommy Andrews
// File: Item.cs
// Project: soen390-team01
// Created: 02/16/2021
// 

#endregion

namespace soen390_team01.Data.Entities
{
    public abstract class Item
    {
        public long ItemId { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Grade { get; set; }
    }
}