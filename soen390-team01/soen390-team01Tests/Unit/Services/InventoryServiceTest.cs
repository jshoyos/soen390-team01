using System.Linq;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using soen390_team01.Data;
using soen390_team01.Data.Entities;
using soen390_team01.Models;
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
                if (type.Equals("bike"))
                {
                    _context.Bikes.Add(new Bike
                    {
                        ItemId = i,
                        Grade = "copper " + i,
                        Name = "Bike " + i,
                        Size = "M",
                        Price = i
                    });
                }
                else if (type.Equals("part"))
                {
                    _context.Parts.Add(new Part
                    {
                        ItemId = i,
                        Grade = "Copper " + i,
                        Name = "Bike " + i,
                        Size = "L",
                        Price = i
                    });
                }
                else
                {
                    _context.Materials.Add(new Material
                    {
                        ItemId = i,
                        Grade = "Aluminum " + i,
                        Name = "Bike " + i,
                        Price = i
                    });
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
        public void GetInventoryModelTest()
        {
            var inventoryModel = _inventoryService.GetInventoryModel();
            Assert.AreEqual(9, inventoryModel.AllList.Count);
            Assert.AreEqual(3, inventoryModel.BikeList.Count);
            Assert.AreEqual(3, inventoryModel.PartList.Count);
            Assert.AreEqual(3, inventoryModel.MaterialList.Count);
        }

        [Test]
        public void GetFilterTest()
        {   //Tests the bike filters
            Assert.AreEqual(3, _inventoryService.GetFilter("grade", "bike").Count());
            Assert.AreEqual(3, _inventoryService.GetFilter("name", "bike").Count());
            Assert.AreEqual(3, _inventoryService.GetFilter("price", "bike").Count());
            Assert.IsNull(_inventoryService.GetFilter("", "bike"));

            //Tests the part filters
            Assert.AreEqual(3, _inventoryService.GetFilter("grade", "part").Count());
            Assert.AreEqual(3, _inventoryService.GetFilter("name", "part").Count());
            Assert.AreEqual(3, _inventoryService.GetFilter("price", "part").Count());
            Assert.IsNull(_inventoryService.GetFilter("", "part"));

            //Tests the material filters
            Assert.AreEqual(3, _inventoryService.GetFilter("grade", "material").Count());
            Assert.AreEqual(3, _inventoryService.GetFilter("name", "material").Count());
            Assert.AreEqual(3, _inventoryService.GetFilter("price", "material").Count());
            Assert.IsNull(_inventoryService.GetFilter("", "material"));

            Assert.IsNull(_inventoryService.GetFilter("", ""));
        }

        [Test]
        public void UpdateTest()
        {
            Inventory model = _context.Inventories.FirstOrDefault(inv => inv.InventoryId == 1);
            model.Quantity = 10;
            _inventoryService.Update(model);
            Inventory invItem = _context.Inventories.FirstOrDefault(inv => inv.InventoryId == 1);
            Assert.AreEqual(model.Quantity, invItem.Quantity);
        }

        //[Test]
        //public void GetFilteredProductListTest()
        //{
        //    var input = new ProductFilterInput
        //    {
        //        Type = "Material",
        //        Value = "Bike 1",
        //        Name = "Grade"
        //    };

        //    //var test1 = ;
        //    Assert.AreEqual(1, _inventoryService.GetFilteredProductList<Material>(input).Count());
        //}
    }
}
