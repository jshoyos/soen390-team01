using System;
using soen390_team01.Data;
using soen390_team01.Data.Entities;
using System.Linq;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using soen390_team01.Data.Exceptions;
using soen390_team01.Data.Queries;
using soen390_team01.Services;

namespace soen390_team01.Models
{ 
    public class InventoryModel : Inventory, IInventoryService
    {

        private readonly ErpDbContext _context;

        public InventoryModel(ErpDbContext context)
        {
            _context = context;

            AllList = GetInventory();
            BikeList = GetAllBikes();
            PartList = GetAllParts();
            MaterialList = GetAllMaterials();
            BikeFilters = ResetBikeFilters();
            PartFilters = ResetPartFilters();
            MaterialFilters = ResetMaterialFilters();
        }

        public List<Inventory> AllList { get; set; }
        public List<Bike> BikeList { get; set; }
        public List<Part> PartList { get; set; }
        public List<Material> MaterialList { get; set; }

        //List parameter Filters
        public Filters BikeFilters { get; set; }
        public Filters PartFilters { get; set; }
        public Filters MaterialFilters { get; set; }

        public string SelectedTab { get; set; } = "All";

        /// <summary>
        /// Resets BikeList and its filters
        /// </summary>
        public void ResetBikes()
        {
            BikeList = GetAllBikes();
            BikeFilters = ResetBikeFilters();
        }

        /// <summary>
        /// Resets PartList and its filters
        /// </summary>
        public void ResetParts()
        {
            PartList = GetAllParts();
            PartFilters = ResetPartFilters();
        }

        /// <summary>
        /// Resets MaterialList and its filters
        /// </summary>
        public void ResetMaterials()
        {
            MaterialList = GetAllMaterials();
            MaterialFilters = ResetMaterialFilters();
        }

        /// <summary>
        /// Retrieves the filtered list for the selected product type
        /// </summary>
        /// <param name="filters">filters to apply</param>
        public void FilterSelectedTab(Filters filters)
        {
            switch (filters.Table)
            {
                case "bike":
                    BikeList = filters.AnyActive() ? GetFilteredProductList<Bike>(filters) : GetAllBikes();
                    BikeFilters = filters;
                    break;
                case "part":
                    PartList = filters.AnyActive() ? GetFilteredProductList<Part>(filters) : GetAllParts();
                    PartFilters = filters;
                    break;
                case "material":
                    MaterialList = filters.AnyActive() ? GetFilteredProductList<Material>(filters) : GetAllMaterials();
                    MaterialFilters = filters;
                    break;
            }
        }

        /// <summary>
        /// Updates an inventory item
        /// </summary>
        /// <param name="inventory">inventory item to update</param>
        public void Update(Inventory inventory)
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

        private Filters ResetBikeFilters()
        {
            var filters = new Filters("bike");

            filters.Add(new StringFilter("bike", "Name", "name"));
            filters.Add(new SelectFilter("bike", "Grade", "grade", _context.Bikes.Select(bike => bike.Grade).Distinct().OrderBy(g => g).ToList()));
            filters.Add(new CheckboxFilter("bike", "Size", "size", _context.Bikes.Select(bike => bike.Size).Distinct().OrderBy(s => s).ToList()));
            filters.Add(new RangeFilter("bike", "Price", "price"));
            // Can add bike specific filters

            return filters;
        }

        private Filters ResetPartFilters()
        {
            var filters = new Filters("part");

            filters.Add(new StringFilter("part", "Name", "name"));
            filters.Add(new SelectFilter("part", "Grade", "grade", _context.Parts.Select(part => part.Grade).Distinct().OrderBy(g => g).ToList()));
            filters.Add(new CheckboxFilter("part", "Size", "size", _context.Parts.Select(part => part.Size).Distinct().OrderBy(s => s).ToList()));
            filters.Add(new RangeFilter("part", "Price", "price"));
            // Can add part specific filters

            return filters;
        }

        private Filters ResetMaterialFilters()
        {
            var filters = new Filters("material");

            filters.Add(new StringFilter("material", "Name", "name"));
            filters.Add(new SelectFilter("material", "Grade", "grade", _context.Materials.Select(material => material.Grade).Distinct().OrderBy(g => g).ToList()));
            filters.Add(new RangeFilter("material", "Price", "price"));
            // Can add material specific filters

            return filters;
        }

        private List<Inventory> GetInventory()
        {
            return _context.Inventories.OrderBy(inv => inv.InventoryId).ToList();
        }

        private List<Bike> GetAllBikes()
        {
            return _context.Bikes.OrderBy(bike => bike.ItemId).ToList();
        }

        private List<Part> GetAllParts()
        {
            return _context.Parts.OrderBy(part => part.ItemId).ToList();
        }

        private List<Material> GetAllMaterials()
        {
            return _context.Materials.OrderBy(mat => mat.ItemId).ToList();
        }

        private List<T> GetFilteredProductList<T>(Filters filters) where T : Item
        {
            try
            {
                return _context.Set<T>("soen390_team01.Data.Entities." + CultureInfo.CurrentCulture.TextInfo.ToTitleCase(filters.Table))
                    .FromSqlRaw(ProductQueryBuilder.FilterProduct(filters)).ToList();
            }
            catch (Exception)
            {
                throw new UnexpectedDataAccessException("Could not find: " + filters.Table);
            }
        }
    }
}
