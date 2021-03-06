using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace soen390_team01.Models
{
    public class TransferFilter
    {
        public List<string> OrderStatus { get; set; }
        public List<string> ProcurementStatus { get; set; }
        public string Vendor { get; set; }
        public TransferFilter()
        {
            OrderStatus = new List<string>();
            ProcurementStatus = new List<string>();
            Vendor = "clear";
        }
    }
}
