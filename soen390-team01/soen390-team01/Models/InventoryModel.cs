using System;
using soen390_team01.Data;
using soen390_team01.Data.Entities;
using System.Linq;
using System.Collections.Generic;

namespace soen390_team01.Models
{ 
    public class InventoryModel:Inventory
    {
        public List<Inventory> inventoryList { get; set; }
    }
}
