using System;
using System.Collections.Generic;

#nullable disable

namespace soen390_team01.Data.Entities
{
    public partial class Inventory
    {
        public long ItemId { get; set; }
        public string Type { get; set; }
        public int Quantity { get; set; }
        public string Warehouse { get; set; }
        public long InventoryId { get; set; }
        public DateTime Added { get; set; }
        public DateTime Updated { get; set; }
    }
}
