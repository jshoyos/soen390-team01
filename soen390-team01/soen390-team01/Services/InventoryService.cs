using soen390_team01.Data;
using soen390_team01.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using soen390_team01.Models;

namespace soen390_team01.Services
{
    public class InventoryService : IDisposable
    {
        private readonly ErpDbContext _context;
        private bool _disposed;

        public InventoryService(ErpDbContext context)
        {
            _context = context;
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
            model.BikeList = all.Where(inv => inv.Type.Equals("bike")).OrderBy(inv => inv.InventoryId).ToList();
            model.PartList = all.Where(inv => inv.Type.Equals("part")).OrderBy(inv => inv.InventoryId).ToList();
            model.MaterialList = all.Where(inv => inv.Type.Equals("material")).OrderBy(inv => inv.InventoryId).ToList();
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
        public virtual List<Inventory> GetAllBikes()
        {
            return _context.Inventories
                                .Where(inv => inv.Type.Equals("bike"))
                                .OrderBy(inv => inv.InventoryId)
                                .ToList();
        }
        /// <summary>
        /// Queries all the parts in the inventory
        /// </summary>
        /// <returns>List of inventory items</returns>
        public virtual List<Inventory> GetAllParts()
        {
            return _context.Inventories
                                .Where(inv => inv.Type.Equals("part"))
                                .OrderBy(inv => inv.InventoryId)
                                .ToList();
        }
        /// <summary>
        /// Queries all the materials in the inventory
        /// </summary>
        /// <returns>List of inventory items</returns>
        public virtual List<Inventory> GetAllMaterials()
        {
            return _context.Inventories
                                .Where(inv => inv.Type.Equals("material"))
                                .OrderBy(inv => inv.InventoryId)
                                .ToList();
        }
        /// <summary>
        /// Adds an inventory item to the Inventory table
        /// </summary>
        /// <param name="item"></param>
        public virtual void AddItem(Inventory item)
        {
            _context.Inventories.Add(item);
            _context.SaveChanges();
        }
        /// <summary>
        /// Updates an inventory item
        /// </summary>
        /// <param name="updatedInventory">inventory item to update</param>
        public virtual void Update(Inventory updatedInventory)
        {
            _context.Inventories.Update(updatedInventory);
            _context.SaveChanges();
        }

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }
            _disposed = true;
        }
    }
}
