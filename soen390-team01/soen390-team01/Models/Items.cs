using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace soen390_team01.Models
{
    public abstract class Items
    {
        protected long ItemId;

        protected abstract string Name { get; set; }

        protected abstract double Price { get; set; }

        protected abstract string Grade { get; set; }
    }
}
