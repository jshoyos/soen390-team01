using System;
using soen390_team01.Data;
using soen390_team01.Data.Entities;
using System.Collections.Generic;
using System.Globalization;
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
        public InventoryModel Model { get; set; }

        public InventoryService(ErpDbContext context)
        {
            _context = context;
            SetupModel();
        }

        public virtual List<T> GetFilteredProductList<T>(Filters filters) where T : Item
        {
            try
            {
                return _context.Set<T>("soen390_team01.Data.Entities." + CultureInfo.CurrentCulture.TextInfo.ToTitleCase(filters.Table))
                    .FromSqlRaw(ProductQueryBuilder.FilterProduct(filters)).ToList();
            }
            catch(Exception)
            {
                throw new UnexpectedDataAccessException("Could not find: " + filters.Table);
            }
        }

        private void SetupModel()
        {
            Model = new InventoryModel();
            var all = GetInventory();
            Model.AllList = all;
            Model.BikeList = GetAllBikes();
            Model.PartList = GetAllParts();
            Model.MaterialList = GetAllMaterials();
            Model.BikeFilters = ResetBikeFilters();
            Model.PartFilters = ResetPartFilters();
            Model.MaterialFilters = ResetMaterialFilters();
        }

        public virtual Filters ResetBikeFilters()
        {
            var filters = new Filters("bike");

            filters.Add(new StringFilter("bike", "Name", "name"));
            filters.Add(new SelectFilter("bike", "Grade", "grade", _context.Bikes.Select(bike => bike.Grade).Distinct().OrderBy(g => g).ToList()));
            filters.Add(new CheckboxFilter("bike", "Size", "size", _context.Bikes.Select(bike => bike.Size).Distinct().OrderBy(s => s).ToList()));
            filters.Add(new RangeFilter("bike", "Price", "price"));
            // Can add bike specific filters

            return filters;
        }

        public virtual Filters ResetPartFilters()
        {
            var filters = new Filters("part");

            filters.Add(new StringFilter("part", "Name", "name"));
            filters.Add(new SelectFilter("part", "Grade", "grade", _context.Parts.Select(part => part.Grade).Distinct().OrderBy(g => g).ToList()));
            filters.Add(new CheckboxFilter("part", "Size", "size", _context.Parts.Select(part => part.Size).Distinct().OrderBy(s => s).ToList()));
            filters.Add(new RangeFilter("part", "Price", "price"));
            // Can add part specific filters

            return filters;
        }

        public virtual Filters ResetMaterialFilters()
        {
            var filters = new Filters("material");

            filters.Add(new StringFilter("material", "Name", "name"));
            filters.Add(new SelectFilter("material", "Grade", "grade", _context.Materials.Select(material => material.Grade).Distinct().OrderBy(g => g).ToList()));
            filters.Add(new RangeFilter("material", "Price", "price"));
            // Can add material specific filters

            return filters;
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
