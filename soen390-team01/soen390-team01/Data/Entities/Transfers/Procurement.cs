#region Header

// Author: Tommy Andrews
// File: Procurement.cs
// Project: soen390-team01
// Created: 02/16/2021
// 

#endregion

#nullable disable

namespace soen390_team01.Data.Entities
{
    public class Procurement
    {
        public long ProcurementId { get; set; }
        public long ItemId { get; set; }
        public long PaymentId { get; set; }
        public int ItemQuantity { get; set; }
        public string State { get; set; }
        public string Type { get; set; }
        public long VendorId { get; set; }

        public virtual Payment Payment { get; set; }
        public virtual Vendor Vendor { get; set; }
    }
}