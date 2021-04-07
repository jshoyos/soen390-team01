using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using soen390_team01.Data;
using soen390_team01.Data.Entities;
using System.Linq;
using MockQueryable.Moq;
using Moq;
using Npgsql;
using soen390_team01.Data.Exceptions;
using soen390_team01.Data.Queries;
using soen390_team01.Models;

namespace soen390_team01Tests.Services
{
    public class InventoryModelTest
    {

        private ErpDbContext _context;
        private InventoryModel _model;

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

            _context.BikeParts.Add(new BikePart { BikeId = 1, PartId = 2, PartQuantity = 1 });
            _context.BikeParts.Add(new BikePart { BikeId = 1, PartId = 4, PartQuantity = 1 });
            _context.BikeParts.Add(new BikePart { BikeId = 3, PartId = 8, PartQuantity = 1 });

            _context.PartMaterials.Add(new PartMaterial { PartId = 2, MaterialId = 5, MaterialQuantity = 1 });
            _context.PartMaterials.Add(new PartMaterial { PartId = 2, MaterialId = 6, MaterialQuantity = 1 });
            _context.PartMaterials.Add(new PartMaterial { PartId = 4, MaterialId = 7, MaterialQuantity = 1 });
            _context.SaveChanges();

            _model = new InventoryModel(_context);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            foreach (var entity in _context.Inventories)
            {
                _context.Inventories.Remove(entity);
            }
            foreach (var entity in _context.BikeParts)
            {
                _context.BikeParts.Remove(entity);
            }
            foreach (var entity in _context.PartMaterials)
            {
                _context.PartMaterials.Remove(entity);
            }
            foreach (var entity in _context.Bikes)
            {
                _context.Bikes.Remove(entity);
            }
            foreach (var entity in _context.Parts)
            {
                _context.Parts.Remove(entity);
            }
            foreach (var entity in _context.Materials)
            {
                _context.Materials.Remove(entity);
            }
            
            _context.SaveChanges();
        }

        [Test]
        public void UpdateValidTest()
        {
            var inventory = _context.Inventories.FirstOrDefault(inv => inv.InventoryId == 1);
            inventory.Quantity = 10;
            _model.Update(inventory);
            var updatedInventory = _context.Inventories.FirstOrDefault(inv => inv.InventoryId == 1);
            Assert.AreEqual(inventory.Quantity, updatedInventory.Quantity);
        }

        [Test]
        public void UpdateInvalidTest()
        {
            var ctx = new Mock<ErpDbContext>();
            var nbInventoriesCall = 0;
            ctx.Setup(c => c.Bikes).Returns(new List<Bike>().AsQueryable().BuildMockDbSet().Object);
            ctx.Setup(c => c.Parts).Returns(new List<Part>().AsQueryable().BuildMockDbSet().Object);
            ctx.Setup(c => c.Materials).Returns(new List<Material>().AsQueryable().BuildMockDbSet().Object);
            ctx.Setup(c => c.Inventories).Returns(new List<Inventory>().AsQueryable().BuildMockDbSet().Object).Callback(() =>
            {
                nbInventoriesCall++;
                // 4th call (the one in Update()) in the update method throws a db exception
                if (nbInventoriesCall == 4)
                    throw new DbUpdateException("error", new PostgresException("", "", "", ""));
            });

            Assert.Throws<UnexpectedDataAccessException>(() => new InventoryModel(ctx.Object).Update(new Inventory()));
        }

        [Test]
        public void FilterSelectedTabInvalidTest()
        {
            var ctx = new Mock<ErpDbContext>();
            ctx.Setup(c => c.Bikes).Returns(new List<Bike>().AsQueryable().BuildMockDbSet().Object);
            ctx.Setup(c => c.Parts).Returns(new List<Part>().AsQueryable().BuildMockDbSet().Object);
            ctx.Setup(c => c.Materials).Returns(new List<Material>().AsQueryable().BuildMockDbSet().Object);
            ctx.Setup(c => c.Inventories).Returns(new List<Inventory>().AsQueryable().BuildMockDbSet().Object);
            var filters = new Filters("bike");
            filters.Add(new StringFilter("bike", "Name", "name") {Value = "filtered_bike"});
            Assert.Throws<UnexpectedDataAccessException>(() => new InventoryModel(ctx.Object).FilterSelectedTab(filters));
        }

        [Test]
        public void ResetProductsTest()
        {
            var initialBikeCount = _model.BikeList.Count;
            var initialBikeFilterCount = _model.BikeFilters.List.Count;
            _model.ResetBikes();
            Assert.AreEqual(initialBikeCount, _model.BikeList.Count);
            Assert.AreEqual(initialBikeFilterCount, _model.BikeFilters.List.Count);

            var initialPartCount = _model.PartList.Count;
            var initialPartFilterCount = _model.PartFilters.List.Count;
            _model.ResetParts();
            Assert.AreEqual(initialPartCount, _model.PartList.Count);
            Assert.AreEqual(initialPartFilterCount, _model.PartFilters.List.Count);

            var initialMaterialCount = _model.MaterialList.Count;
            var initialMaterialFilterCount = _model.MaterialFilters.List.Count;

            _model.ResetMaterials();
            Assert.AreEqual(initialMaterialCount, _model.MaterialList.Count);
            Assert.AreEqual(initialMaterialFilterCount, _model.MaterialFilters.List.Count);

        }

        [Test]
        public void AddBikePartTest()
        {
            var partCount = _context.BikeParts.Count(p => p.BikeId == 1);
            _model.AddBikePart(new BikePart { BikeId = 1, PartId = 8, PartQuantity = 1 });
            var partCountAfterAdd = _context.BikeParts.Count(p => p.BikeId == 1);
            Assert.AreEqual(partCount + 1, partCountAfterAdd);
        }

        [Test]
        public void RemoveBikePartTest()
        {
            var partCount = _context.BikeParts.Count(p => p.BikeId == 1);
            var bp = _context.BikeParts.First(p => p.BikeId == 1 && p.PartId == 2);
            _model.RemoveBikePart(bp);
            var partCountAfterRemove = _context.BikeParts.Count(p => p.BikeId == 1);
            Assert.AreEqual(partCount - 1, partCountAfterRemove);
        }

        [Test]
        public void AddPartMaterialTest()
        {
            var materialCount = _context.PartMaterials.Count(m => m.PartId == 2);
            _model.AddPartMaterial(new PartMaterial { PartId = 2, MaterialId = 7, MaterialQuantity = 1 });
            var materialCountAfterAdd = _context.PartMaterials.Count(m => m.PartId == 2);
            Assert.AreEqual(materialCount + 1, materialCountAfterAdd);
        }

        [Test]
        public void RemovePartMaterialTest()
        {
            var materialCount = _context.PartMaterials.Count(m => m.PartId == 2);
            var mat = _context.PartMaterials.First(m => m.PartId == 2 && m.MaterialId == 5);
            _model.RemovePartMaterial(mat);
            var matCountAfterRemove = _context.PartMaterials.Count(m => m.PartId == 2);
            Assert.AreEqual(materialCount - 1, matCountAfterRemove);
        }
    }
}
