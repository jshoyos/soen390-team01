using System.Linq;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using soen390_team01.Data;
using soen390_team01.Data.Entities;
using soen390_team01.Services;

namespace soen390_team01Tests.Services
{
    public class InventoryServiceTest
    {

        private ErpDbContext _context;
        private InventoryService _inventoryService;

        [OneTimeSetUp]
        public void Init()
        {
            var builder = new DbContextOptionsBuilder<ErpDbContext>();
            builder.UseInMemoryDatabase("test_db");
            _context = new ErpDbContext(builder.Options);

            for (var i = 1; i <= 9; i++)
            {
                var type = (9 % i) switch
                {
                    0 => "bike",
                    1 => "part",
                    _ => "material",
                };
                _context.Inventories.Add(new Inventory
                    {
                        InventoryId = i,
                        ItemId = i,
                        Quantity = i,
                        Type = type,
                        Warehouse = "Warehouse " + i
                    }
                );
                _context.SaveChanges();
            }

            _inventoryService = new InventoryService(_context);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            foreach (var entity in _context.Inventories)
            {
                _context.Inventories.Remove(entity);
            }
            _context.SaveChanges();
        }

        [Test]
        public void GetInventoryTest()
        {
            Assert.AreEqual(9, _inventoryService.GetInventory().Count);
        }

        [Test]
        public void GetAllBikesTest()
        {
            Assert.AreEqual(3, _inventoryService.GetAllBikes().Count);
        }

        [Test]
        public void GetAllPartsTest()
        {
            Assert.AreEqual(3, _inventoryService.GetAllParts().Count);
        }

        [Test]
        public void GetAllMaterialsTest()
        {
            Assert.AreEqual(3, _inventoryService.GetAllMaterials().Count);
        }

        [Test]
        public void GetInventoryModelTest()
        {
            var inventoryModel = _inventoryService.GetInventoryModel();
            Assert.AreEqual(9, inventoryModel.AllList.Count);
            Assert.AreEqual(3, inventoryModel.BikeList.Count);
            Assert.AreEqual(3, inventoryModel.PartList.Count);
            Assert.AreEqual(3, inventoryModel.MaterialList.Count);
        }
    }
}
