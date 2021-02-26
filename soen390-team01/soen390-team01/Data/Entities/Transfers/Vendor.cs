#region Header

// Author: Tommy Andrews
// File: Vendor.cs
// Project: soen390-team01
// Created: 02/16/2021
// 

#endregion

using System.Collections.Generic;

#nullable disable

namespace soen390_team01.Data.Entities
{
    public class Vendor
    {
        public Vendor()
        {
            Procurements = new HashSet<Procurement>();
        }

        public long VendorId { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }

        public virtual ICollection<Procurement> Procurements { get; set; }
    }
}