using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace soen390_team01.Data.Entities
{
    public abstract class Item
    {
        public long ItemId { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Grade { get; set; }
    }
}
