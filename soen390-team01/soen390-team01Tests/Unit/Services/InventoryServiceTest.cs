using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using soen390_team01.Data;
using soen390_team01.Data.Entities;
using soen390_team01.Services;
using System.Linq;
using soen390_team01.Data.Queries;

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
                switch (type)
                {
                    case "bike":
                        _context.Bikes.Add(new Bike
                        {
                            ItemId = i,
                            Grade = "copper " + i,
                            Name = "Bike " + i,
                            Size = "M",
                            Price = i
                        });
                        break;
                    case "part":
                        _context.Parts.Add(new Part
                        {
                            ItemId = i,
                            Grade = "copper " + i,
                            Name = "Bike " + i,
                            Size = "L",
                            Price = i
                        });
                        break;
                    default:
                        _context.Materials.Add(new Material
                        {
                            ItemId = i,
                            Grade = "aluminum " + i,
                            Name = "Bike " + i,
                            Price = i
                        });
                        break;
                }

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
        public void UpdateTest()
        {
            var model = _context.Inventories.FirstOrDefault(inv => inv.InventoryId == 1);
            model.Quantity = 10;
            _inventoryService.Update(model);
            var invItem = _context.Inventories.FirstOrDefault(inv => inv.InventoryId == 1);
            Assert.AreEqual(model.Quantity, invItem.Quantity);
        }

        [Test]
        public void ResetFiltersTest()
        {
            Assert.AreEqual(_inventoryService.Model.BikeFilters.List.Count, _inventoryService.ResetBikeFilters().List.Count);
            Assert.AreEqual(_inventoryService.Model.PartFilters.List.Count, _inventoryService.ResetPartFilters().List.Count);
            Assert.AreEqual(_inventoryService.Model.MaterialFilters.List.Count, _inventoryService.ResetMaterialFilters().List.Count);
        }
    }
}
