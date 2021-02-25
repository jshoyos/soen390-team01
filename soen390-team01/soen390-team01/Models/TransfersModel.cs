using System;
using soen390_team01.Data.Entities;
using System.Collections.Generic;

namespace soen390_team01.Models
{
    public class TransfersModel
    {
        public List<Order> Orders { get; set; }
        public List<Procurement> Procurements { get; set; }
        public AddProcurementModel AddProcurement { get; set; }

        public string SelectedTab { get; set; } = "Order";
        public bool ShowModal { get; set; } = false;
    }
}
