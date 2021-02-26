#region Header

// Author: Tommy Andrews
// File: TransfersModel.cs
// Project: soen390-team01
// Created: 02/25/2021
// 

#endregion

using System.Collections.Generic;
using soen390_team01.Data.Entities;

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