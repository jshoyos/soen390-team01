using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

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
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime Added { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime Updated { get; set; }
    }
}
