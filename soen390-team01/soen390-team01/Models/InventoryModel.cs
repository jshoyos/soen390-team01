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
        public List<Inventory> AllList { get; set; }
        public List<Bike> BikeList { get; set; }
        public List<Part> PartList { get; set; }
        public List<Material> MaterialList { get; set; }

        //List parameter Filters
        public Filters InventoryFilters { get; set; }
        public Filters BikeFilters { get; set; }
        public Filters PartFilters { get; set; }
        public Filters MaterialFilters { get; set; }

        public string SelectedTab { get; set; } = "inventory";
        public bool ShowFilters { get; set; }

        public InventoryModel(ErpDbContext context)
        {
            _context = context;

            AllList = GetInventory();
            BikeList = GetAllBikes();
            PartList = GetAllParts();
            MaterialList = GetAllMaterials();
            InventoryFilters = ResetInventoryFilters();
            BikeFilters = ResetBikeFilters();
            PartFilters = ResetPartFilters();
            MaterialFilters = ResetMaterialFilters();
        }

        /// <summary>
        /// Resets BikeList and its filters
        /// </summary>
        public void ResetInventories()
        {
            AllList = GetInventory();
            InventoryFilters = ResetInventoryFilters();
        }

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
                case "inventory":
                    AllList = filters.AnyActive() ? GetFilteredInventoryList(filters) : GetInventory();
                    InventoryFilters = filters;
                    break;
                case "bike":
                    BikeList = filters.AnyActive() ? GetFilteredProductList<Bike>(filters, "Bike.BikeParts") : GetAllBikes();
                    BikeFilters = filters;
                    break;
                case "part":
                    PartList = filters.AnyActive() ? GetFilteredProductList<Part>(filters, "Part.PartMaterials") : GetAllParts();
                    PartFilters = filters;
                    break;
                case "material":
                    MaterialList = filters.AnyActive() ? GetFilteredProductList<Material>(filters, "") : GetAllMaterials();
                    MaterialFilters = filters;
                    break;
            }
        }

        /// <summary>
        /// Updates an inventory item
        /// </summary>
        /// <param name="inventory">inventory item to update</param>
        public Inventory Update(Inventory inventory)
        {
            try
            {
                var updatedInventory = _context.Inventories.First(i => i.InventoryId == inventory.InventoryId);
                updatedInventory.Quantity = inventory.Quantity;
                _context.Inventories.Update(updatedInventory);
                _context.SaveChanges();
                return updatedInventory;
            }
            catch (DbUpdateException e)
            {
                throw DbAccessExceptionProvider.Provide(e.InnerException as PostgresException);
            }
        }

        public BikePart AddBikePart(BikePart addPart)
        {
            try
            {
                _context.BikeParts.Add(addPart);
                _context.SaveChanges();
                return new BikePart
                {
                    PartId = addPart.PartId,
                    PartQuantity = addPart.PartQuantity,
                    BikeId = addPart.BikeId
                };
            }
            catch (InvalidOperationException)
            {
                throw new NonUniqueValueException("BikePart.PartId");
            }
            catch (DbUpdateException e)
            {
                throw DbAccessExceptionProvider.Provide(e.InnerException as PostgresException);
            }
        }

        public void RemoveBikePart(BikePart removePart)
        {
            try
            {
                var actualBikePart = _context.BikeParts.First(p => p.PartId == removePart.PartId && p.BikeId == removePart.BikeId);
                _context.BikeParts.Remove(actualBikePart);
                _context.SaveChanges();
            }
            catch (InvalidOperationException)
            {
                throw new NotFoundException("BikePart", "PartId", removePart.PartId.ToString());
            }
            catch (DbUpdateException e)
            {
                throw DbAccessExceptionProvider.Provide(e.InnerException as PostgresException);
            }
        }
        public PartMaterial AddPartMaterial(PartMaterial addMat)
        {
            try
            {
                _context.PartMaterials.Add(addMat);
                _context.SaveChanges();
                return new PartMaterial
                {
                    MaterialId = addMat.MaterialId,
                    MaterialQuantity = addMat.MaterialQuantity,
                    PartId = addMat.PartId
                };
            }
            catch (InvalidOperationException)
            {
                throw new NonUniqueValueException("PartMaterial.MaterialId");
            }
            catch (DbUpdateException e)
            {
                throw DbAccessExceptionProvider.Provide(e.InnerException as PostgresException);
            }
        }

        public void RemovePartMaterial(PartMaterial removeMat)
        {
            try
            {
                var actualPartMaterial = _context.PartMaterials.First(m => m.MaterialId == removeMat.MaterialId && m.PartId == removeMat.PartId);
                _context.PartMaterials.Remove(actualPartMaterial);
                _context.SaveChanges();
            }
            catch (InvalidOperationException)
            {
                throw new NotFoundException("PartMaterial", "MaterialId", removeMat.MaterialId.ToString());
            }
            catch (DbUpdateException e)
            {
                throw DbAccessExceptionProvider.Provide(e.InnerException as PostgresException);
            }
        }

        private Filters ResetInventoryFilters()
        {
            var filters = new Filters("inventory");

            filters.Add(new CheckboxFilter("inventory", "Type", "type", _context.Inventories.Select(inv => inv.Type).Distinct().OrderBy(t => t).ToList()));
            filters.Add(new SelectFilter("inventory", "Warehouse", "warehouse", _context.Inventories.Select(inv => inv.Warehouse).Distinct().OrderBy(w => w).ToList()));
            filters.Add(new RangeFilter("inventory", "Quantity", "quantity"));
            filters.Add(new DateRangeFilter("inventory", "Added", "added"));
            filters.Add(new DateRangeFilter("inventory", "Updated", "modified"));
            // Can add bike specific filters

            return filters;
        }

        private Filters ResetBikeFilters()
        {
            var filters = new Filters("bike");

            filters.Add(new StringFilter("bike", "Name", "name"));
            filters.Add(new SelectFilter("bike", "Grade", "grade", _context.Bikes.Select(bike => bike.Grade).Distinct().OrderBy(g => g).ToList()));
            filters.Add(new CheckboxFilter("bike", "Size", "size", _context.Bikes.Select(bike => bike.Size).Distinct().OrderBy(s => s).ToList()));
            filters.Add(new RangeFilter("bike", "Price", "price"));
            filters.Add(new DateRangeFilter("bike", "Added", "added"));
            filters.Add(new DateRangeFilter("bike", "Updated", "modified"));
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
            filters.Add(new DateRangeFilter("part", "Added", "added"));
            filters.Add(new DateRangeFilter("part", "Updated", "modified"));
            // Can add part specific filters

            return filters;
        }

        private Filters ResetMaterialFilters()
        {
            var filters = new Filters("material");

            filters.Add(new StringFilter("material", "Name", "name"));
            filters.Add(new SelectFilter("material", "Grade", "grade", _context.Materials.Select(material => material.Grade).Distinct().OrderBy(g => g).ToList()));
            filters.Add(new RangeFilter("material", "Price", "price"));
            filters.Add(new DateRangeFilter("material", "Added", "added"));
            filters.Add(new DateRangeFilter("material", "Updated", "modified"));
            // Can add material specific filters

            return filters;
        }

        private List<Inventory> GetInventory()
        {
            return _context.Inventories.OrderBy(inv => inv.InventoryId).ToList();
        }

        private List<Bike> GetAllBikes()
        {
            return _context.Bikes.AsNoTracking().Include(bike => bike.BikeParts).OrderBy(bike => bike.ItemId).ToList()
                .ConvertAll(
                    bike =>
                    {
                        bike.BikeParts = bike.BikeParts.ToList().ConvertAll(
                            bp =>
                            {
                                bp.Bike = null;
                                bp.Part = null;
                                return bp;
                            }).ToList(); return bike;
                    });
        }

        private List<Part> GetAllParts()
        {
            return _context.Parts.AsNoTracking().Include(part => part.PartMaterials).OrderBy(part => part.ItemId).ToList()
                .ConvertAll(
                    part =>
                    {
                        part.PartMaterials = part.PartMaterials.ToList().ConvertAll(
                            pm =>
                            {
                                pm.Part = null; return pm;
                            }).ToList();
                        part.BikeParts = part.BikeParts.ToList().ConvertAll(
                            bp =>
                            {
                                bp.Bike = null;
                                bp.Part = null;
                                return bp;
                            }).ToList();
                        return part;
                    });
        }

        private List<Material> GetAllMaterials()
        {
            return _context.Materials.OrderBy(mat => mat.ItemId).ToList();
        }

        private List<Inventory> GetFilteredInventoryList(Filters filters)
        {
            try
            {
                return _context.Inventories.FromSqlRaw(ProductQueryBuilder.FilterProduct(filters)).AsNoTracking().ToList();
            }
            catch (Exception)
            {
                throw new UnexpectedDataAccessException("Could not find: " + filters.Table);
            }
        }

        private List<T> GetFilteredProductList<T>(Filters filters, string includeString) where T : Item
        {
            try
            {
                return _context.Set<T>("soen390_team01.Data.Entities." + CultureInfo.CurrentCulture.TextInfo.ToTitleCase(filters.Table))
                    .FromSqlRaw(ProductQueryBuilder.FilterProduct(filters)).Include(includeString).AsNoTracking().ToList();
            }
            catch (Exception)
            {
                throw new UnexpectedDataAccessException("Could not find: " + filters.Table);
            }
        }
    }
}
