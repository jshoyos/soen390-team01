using System;
using soen390_team01.Data;
using soen390_team01.Data.Entities;
using System.Linq;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using soen390_team01.Data.Queries;

namespace soen390_team01.Models
{ 
    public class InventoryModel:Inventory
    {
        public List<Inventory> AllList { get; set; }
        public List<Bike> BikeList { get; set; }
        public List<Part> PartList { get; set; }
        public List<Material> MaterialList { get; set; }

        //List parameter Filters
        public Filters BikeFilters { get; set; } =  new Filters("Bike");
        public Filters PartFilters { get; set; } = new Filters("Part");
        public Filters MaterialFilters { get; set; } = new Filters("Material");

        public string SelectedTab { get; set; } = "All";
    }
}
