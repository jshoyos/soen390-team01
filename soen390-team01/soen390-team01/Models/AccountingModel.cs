using System;
using soen390_team01.Data;
using soen390_team01.Data.Entities;
using System.Linq;
using System.Collections.Generic;

namespace soen390_team01.Models
{
    public class AccountingModel
    {
        public List<Inventory> AllList { get; set; }
        public List<Inventory> BikeList { get; set; }
        public List<Inventory> PartList { get; set; }
        public List<Inventory> MaterialList { get; set; }
    }

}
