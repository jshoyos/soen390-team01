#region Header

// Author: Tommy Andrews
// File: InventoryModel.cs
// Project: soen390-team01
// Created: 02/24/2021
// 

#endregion

using System.Collections.Generic;
using soen390_team01.Data.Entities;

namespace soen390_team01.Models
{
    public class InventoryModel : Inventory
    {
        //List parameter Filters
        public SelectFilters BikeFilters = new SelectFilters("Bike");
        public SelectFilters MaterialFilters = new SelectFilters("Material");
        public SelectFilters PartFilters = new SelectFilters("Part");
        public List<Inventory> AllList { get; set; }
        public List<Bike> BikeList { get; set; }
        public List<Part> PartList { get; set; }
        public List<Material> MaterialList { get; set; }

        public string SelectedTab { get; set; } = "All";
    }
}