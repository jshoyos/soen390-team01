#region Header

// Author: Tommy Andrews
// File: Order.cs
// Project: soen390-team01
// Created: 02/16/2021
// 

#endregion

using System.Collections.Generic;

#nullable disable

namespace soen390_team01.Data.Entities
{
    public class Order
    {
        public Order()
        {
            OrderItems = new HashSet<OrderItem>();
        }

        public long OrderId { get; set; }
        public long CustomerId { get; set; }
        public string State { get; set; }
        public long PaymentId { get; set; }

        public virtual Customer Customer { get; set; }
        public virtual Payment Payment { get; set; }
        public virtual ICollection<OrderItem> OrderItems { get; set; }
    }
}