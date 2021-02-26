#region Header

// Author: Tommy Andrews
// File: Inventory.cs
// Project: soen390-team01
// Created: 02/16/2021
// 

#endregion

#nullable disable

namespace soen390_team01.Data.Entities
{
    public class Inventory
    {
        public long ItemId { get; set; }
        public string Type { get; set; }
        public int Quantity { get; set; }
        public string Warehouse { get; set; }
        public long InventoryId { get; set; }
    }
}