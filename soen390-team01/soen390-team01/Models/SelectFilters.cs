using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace soen390_team01.Models
{
    public class SelectFilters: Dictionary<string, SelectList>
    {
        public string Type { get; set; }
        

        public SelectFilters(string type)
        {
            this.Type = type;
        }
    }
}
