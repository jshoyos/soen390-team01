using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace soen390_team01.Entities.Models
{
    public abstract class Items
    {
        protected long ItemId { get; set; }
        protected string Name { get; set; }

        protected decimal Price { get; set; }

        protected  string Grade { get; set; }
    }
}
