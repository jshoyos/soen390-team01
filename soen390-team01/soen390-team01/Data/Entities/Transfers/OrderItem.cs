using System;
using System.Collections.Generic;

#nullable disable

namespace soen390_team01.Data.Entities
{
    public partial class OrderItem
    {
        public long OrderId { get; set; }
        public long ItemId { get; set; }
        public int ItemQuantity { get; set; }
        public string Type { get; set; }

        public virtual Order Order { get; set; }
    }
}
