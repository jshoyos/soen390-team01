using Firebase.Auth;
using soen390_team01.Data;
using soen390_team01.Data.Entities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

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
        ///     Queries all the bikes in the inventory
        /// </summary>
        /// <returns>List of inventory items</returns>
        public List<Inventory> GetAllBikes()
        {
            return _context.Inventories
                                .Where(inv => inv.Type.Equals("bike"))
                                .OrderBy(inv => inv.InventoryId)
                                .ToList();
        }
        /// <summary>
        ///     Queries all the parts in the inventory
        /// </summary>
        /// <returns>List of inventory items</returns>
        public List<Inventory> GetAllParts()
        {
            return _context.Inventories
                                .Where(inv => inv.Type.Equals("part"))
                                .OrderBy(inv => inv.InventoryId)
                                .ToList();
        }
        /// <summary>
        ///     Queries all the materials in the inventory
        /// </summary>
        /// <returns>List of inventory items</returns>
        public List<Inventory> GetAllMaterials()
        {
            return _context.Inventories
                                .Where(inv => inv.Type.Equals("material"))
                                .OrderBy(inv => inv.InventoryId)
                                .ToList();
        }
        /// <summary>
        ///     adds an item to the respective table
        /// </summary>
        /// <param name="item"></param>

        public void AddItem(Inventory item)
        {
            _context.Inventories.Add(item);
        }
        public void Update(Inventory updatedInventory)
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
