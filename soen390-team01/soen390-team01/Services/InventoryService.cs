using System;
using soen390_team01.Data;
using soen390_team01.Data.Entities;
using System.Collections.Generic;
using System.Linq;
using soen390_team01.Models;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using soen390_team01.Data.Exceptions;
using soen390_team01.Data.Queries;

namespace soen390_team01.Services
{
    public class InventoryService : DisposableService
    {
        private readonly ErpDbContext _context;

        public InventoryService(ErpDbContext context)
        {
            _context = context;
        }

        public virtual List<T> GetFilteredProductList<T>(Filters filters) where T : Item
        {
            try
            {
                return _context.Set<T>("soen390_team01.Data.Entities." + filters.Table).FromSqlRaw(ProductQueryBuilder.FilterProduct(filters)).ToList();
            }
            catch(Exception)
            {
                throw new UnexpectedDataAccessException("Could not find: " + filters.Table);
            }
        }
        public virtual InventoryModel SetupModel()
        {
            var model = GetInventoryModel();
            model.BikeFilters.Add(new StringFilter("bike", "Name", "name"));
            model.BikeFilters.Add(new SelectFilter("bike", "Grade", "grade", _context.Bikes.Select(bike => bike.Grade).Distinct().OrderBy(g => g).ToList()));
            model.BikeFilters.Add(new SelectFilter("bike", "Size", "size", _context.Bikes.Select(bike => bike.Size).Distinct().OrderBy(s => s).ToList()));
            model.PartFilters.Add(new StringFilter("part", "Name", "name"));
            model.PartFilters.Add(new SelectFilter("part", "Grade", "grade", _context.Parts.Select(part => part.Grade).Distinct().OrderBy(g => g).ToList()));
            model.PartFilters.Add(new SelectFilter("part", "Size", "size", _context.Parts.Select(part => part.Size).Distinct().OrderBy(s => s).ToList()));
            model.MaterialFilters.Add(new StringFilter("material", "Name", "name"));
            model.MaterialFilters.Add(new SelectFilter("material", "Grade", "grade", _context.Materials.Select(item => item.Grade).Distinct().OrderBy(g => g).ToList()));
            //model.BikeFilters.Add("Price", GetFilter("price", "bike"));
            //model.PartFilters.Add("Price", GetFilter("price", "part"));
            //model.MaterialFilters.Add("Price", GetFilter("price", "material"));
            return model;
        }

        /// <summary>
        /// Queries all the items in the inventory and splits the into an InventoryModel
        /// </summary>
        /// <returns>InventoryModel</returns>
        public virtual InventoryModel GetInventoryModel()
        {

            var model = new InventoryModel();
            var all = GetInventory();
            model.AllList = all;
            model.BikeList = GetAllBikes();
            model.PartList = GetAllParts();
            model.MaterialList = GetAllMaterials();
            return model;
        }

        /// <summary>
        /// Queries all the items in the inventory
        /// </summary>
        /// <returns>List of inventory items</returns>
        public virtual List<Inventory> GetInventory()
        {
            return _context.Inventories.OrderBy(inv => inv.InventoryId).ToList();
        }
        /// <summary>
        /// Queries all the bikes in the inventory
        /// </summary>
        /// <returns>List of inventory items</returns>
        public virtual List<Bike> GetAllBikes()
        {
            return _context.Bikes.OrderBy(bike => bike.ItemId).ToList();
        }
        /// <summary>
        /// Queries all the parts in the inventory
        /// </summary>
        /// <returns>List of inventory items</returns>
        public virtual List<Part> GetAllParts()
        {
            return _context.Parts.OrderBy(part => part.ItemId).ToList();
        }
        /// <summary>
        /// Queries all the materials in the inventory
        /// </summary>
        /// <returns>List of inventory items</returns>
        public virtual List<Material> GetAllMaterials()
        {
            return _context.Materials.OrderBy(mat => mat.ItemId).ToList();
        }
        /// <summary>
        /// Updates an inventory item
        /// </summary>
        /// <param name="inventory">inventory item to update</param>
        public virtual void Update(Inventory inventory)
        {
            try
            {
                var updatedInventory = _context.Inventories.First(i => i.InventoryId == inventory.InventoryId);
                updatedInventory.Quantity = inventory.Quantity;
                _context.Inventories.Update(updatedInventory);
                _context.SaveChanges();
            }
            catch (DbUpdateException e)
            {
                throw DbAccessExceptionProvider.Provide(e.InnerException as PostgresException);
            }

        }
    }
}
