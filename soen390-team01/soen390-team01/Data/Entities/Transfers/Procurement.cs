using System;
using System.Collections.Generic;

#nullable disable

namespace soen390_team01.Data.Entities
{
    public partial class Procurement
    {
        public long ProcurementId { get; set; }
        public long ItemId { get; set; }
        public long PaymentId { get; set; }
        public int ItemQuantity { get; set; }
        public string State { get; set; }
        public string Type { get; set; }
        public long VendorId { get; set; }
        public DateTime Added { get; set; }
        public DateTime Updated { get; set; }
        public virtual Payment Payment { get; set; }
        public virtual Vendor Vendor { get; set; }
    }
}
