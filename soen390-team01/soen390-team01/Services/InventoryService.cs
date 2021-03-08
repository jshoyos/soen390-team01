using System;
using soen390_team01.Data;
using soen390_team01.Data.Entities;
using System.Collections.Generic;
using System.Linq;
using soen390_team01.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
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

        public virtual List<T> GetFilteredProductList<T>(ProductFilterInput input) where T : Item
        {
            try
            {
                return _context.Set<T>("soen390_team01.Data.Entities." + input.Type).FromSqlRaw(ProductQueryBuilder.FilterProduct(input)).ToList();
            }
            catch(Exception)
            {
                throw new NotFoundException(input.Type, input.Name, input.Value);
            }
        }
        public virtual InventoryModel SetupModel()
        {
            var model = GetInventoryModel();
            model.BikeFilters.Add("Name", GetFilter("name","bike"));
            model.BikeFilters.Add("Grade", GetFilter("grade", "bike"));
            model.BikeFilters.Add("Price", GetFilter("price", "bike"));
            model.PartFilters.Add("Name", GetFilter("name", "part"));
            model.PartFilters.Add("Grade", GetFilter("grade", "part"));
            model.PartFilters.Add("Price", GetFilter("price", "part"));
            model.MaterialFilters.Add("Name", GetFilter("name", "material"));
            model.MaterialFilters.Add("Grade", GetFilter("grade", "material"));
            model.MaterialFilters.Add("Price", GetFilter("price", "material"));
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


        //TODO: refactor
        public virtual SelectList GetFilter(string param,string table)
        {
            switch (table)
            {
                case "bike":
                    switch (param)
                    {
                        case "grade": return new SelectList(_context.Bikes.Select(bike => bike.Grade).Distinct().ToList());
                        case "name": return new SelectList(_context.Bikes.Select(bike => bike.Name).Distinct().ToList());
                        case "price":
                            List<string> stringList = new List<string>();
                            foreach (double d in _context.Bikes.Select(bike => bike.Price).Distinct().ToList())
                            {
                                stringList.Add(d.ToString());
                            }
                            return new SelectList(stringList);
                        default: return null;
                    }
                case "part":
                    switch (param)
                    {
                        case "grade": return new SelectList(_context.Parts.Select(bike => bike.Grade).Distinct().ToList());
                        case "name": return new SelectList(_context.Parts.Select(bike => bike.Name).Distinct().ToList());
                        case "price":
                            List<string> stringList = new List<string>();
                            foreach (double d in _context.Parts.Select(bike => bike.Price).Distinct().ToList())
                            {
                                stringList.Add(d.ToString());
                            }
                            return new SelectList(stringList);
                        default: return null;
                    }
                case "material":
                    switch (param)
                    {
                        case "grade": return new SelectList(_context.Materials.Select(bike => bike.Grade).Distinct().ToList());
                        case "name": return new SelectList(_context.Materials.Select(bike => bike.Name).Distinct().ToList());
                        case "price":
                            List<string> stringList = new List<string>();
                            foreach (double d in _context.Materials.Select(bike => bike.Price).Distinct().ToList())
                            {
                                stringList.Add(d.ToString());
                            }
                            return new SelectList(stringList);
                        default: return null;
                    }
                default: return null;
            }     
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
        /// <param name="updatedInventory">inventory item to update</param>
        public virtual void Update(Inventory updatedInventory)
        {
            try
            {
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
