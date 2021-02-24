using System;
using soen390_team01.Data;
using soen390_team01.Data.Entities;
using System.Linq;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace soen390_team01.Models
{ 
    public class InventoryModel:Inventory
    {
        public List<Inventory> AllList { get; set; }
        public List<Bike> BikeList { get; set; }
        public List<Part> PartList { get; set; }
        public List<Material> MaterialList { get; set; }

        //List parameter Filters
        public SelectFilters BikeFilters = new SelectFilters("Bike");
        public SelectFilters PartFilters = new SelectFilters("Part");
        public SelectFilters MaterialFilters = new SelectFilters("Material");

        public string SelectedTab { get; set; } = "All";
    }
}
