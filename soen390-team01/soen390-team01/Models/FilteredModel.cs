using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace soen390_team01.Models
{
    public abstract class FilteredModel : IFilteredModel
    {
        public bool ShowFilters { get; set; }
    }

    public interface IFilteredModel
    {
        public bool ShowFilters { get; set; }
    }
}
