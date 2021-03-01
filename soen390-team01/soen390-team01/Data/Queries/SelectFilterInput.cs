using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace soen390_team01.Data.Queries
{
    public class SelectFilterInput
    {
        public string SelectValue { get; set; }
        public List<string> PossibleValues { get; set; }
    }
}
