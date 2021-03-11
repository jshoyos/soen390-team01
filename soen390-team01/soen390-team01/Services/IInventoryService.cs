using System.Collections.Generic;
using soen390_team01.Data.Entities;
using soen390_team01.Data.Queries;

namespace soen390_team01.Services
{
    public interface IInventoryService
    {
        public List<Inventory> AllList { get; set; }
        public List<Bike> BikeList { get; set; }
        public List<Part> PartList { get; set; }
        public List<Material> MaterialList { get; set; }

        //List parameter Filters
        public Filters InventoryFilters { get; set; }
        public Filters BikeFilters { get; set; }
        public Filters PartFilters { get; set; }
        public Filters MaterialFilters { get; set; }
        public string SelectedTab { get; set; }

        public void ResetBikes();

        /// <summary>
        /// Resets PartList and its filters
        /// </summary>
        public void ResetParts();

        /// <summary>
        /// Resets MaterialList and its filters
        /// </summary>
        public void ResetMaterials();

        /// <summary>
        /// Retrieves the filtered list for the selected product type
        /// </summary>
        /// <param name="filters">filters to apply</param>
        public void FilterSelectedTab(Filters filters);

        /// <summary>
        /// Updates an inventory item
        /// </summary>
        /// <param name="inventory">inventory item to update</param>
        public void Update(Inventory inventory);
    }
}
