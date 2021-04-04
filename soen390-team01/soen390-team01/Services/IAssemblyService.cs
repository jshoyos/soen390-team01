using soen390_team01.Data.Entities;
using soen390_team01.Data.Queries;
using soen390_team01.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace soen390_team01.Services
{
    public interface IAssemblyService : IFilteredModel
    {
        public List<Production> Productions { get; set; }
        public Filters ProductionFilters { get; set; }
        public BikeOrder BikeOrder { get; set; }
        public string SelectedTab { get; set; }
        public bool ShowModal { get; set; }
        /// <summary>
        /// Get production list
        /// </summary>
        /// <returns></returns>
        public List<Production> GetProductions();
        /// <summary>
        /// Resets production filters
        /// </summary>
        /// <returns></returns>
        public Filters ResetProductionFilters();
        /// <summary>
        /// Gets filters production list
        /// </summary>
        /// <param name="filters"></param>
        /// <returns></returns>
        public List<Production> GetFilteredProductionList(Filters filters);
        /// <summary>
        /// Updates the inventory with new bike orders
        /// </summary>
        /// <param name="order"></param>
        public void AddNewBike(BikeOrder order);
        /// <summary>
        /// Fixes a production's state
        /// </summary>
        /// <param name="productionId"></param>
        public void FixProduction(long productionId);
        /// <summary>
        /// Updates the inventory with new bike order
        /// </summary>
        /// <param name="production"></param>
        /// <returns></returns>
        public Inventory UpdateInventory(Production production);
        /// <summary>
        /// Updates the production table
        /// </summary>
        /// <param name="production"></param>
        /// <returns></returns>
        public Production UpdateProduction(Production production);
    }
}
