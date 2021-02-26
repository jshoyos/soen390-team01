#region Header

// Author: Tommy Andrews
// File: Payment.cs
// Project: soen390-team01
// Created: 02/16/2021
// 

#endregion

using System.Collections.Generic;

#nullable disable

namespace soen390_team01.Data.Entities
{
    public class Payment
    {
        public Payment()
        {
            Orders = new HashSet<Order>();
            Procurements = new HashSet<Procurement>();
        }

        public long PaymentId { get; set; }
        public decimal Amount { get; set; }
        public string State { get; set; }

        public virtual ICollection<Order> Orders { get; set; }
        public virtual ICollection<Procurement> Procurements { get; set; }
    }
}