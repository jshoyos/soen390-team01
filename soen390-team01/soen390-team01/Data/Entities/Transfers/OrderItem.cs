#region Header

// Author: Tommy Andrews
// File: OrderItem.cs
// Project: soen390-team01
// Created: 02/16/2021
// 

#endregion

#nullable disable

namespace soen390_team01.Data.Entities
{
    public class OrderItem
    {
        public long OrderId { get; set; }
        public long ItemId { get; set; }
        public int ItemQuantity { get; set; }
        public string Type { get; set; }

        public virtual Order Order { get; set; }
    }
}