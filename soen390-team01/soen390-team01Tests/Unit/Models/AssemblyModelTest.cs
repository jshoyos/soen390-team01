using Microsoft.EntityFrameworkCore;
using MockQueryable.Moq;
using Moq;
using Npgsql;
using NUnit.Framework;
using soen390_team01.Data;
using soen390_team01.Data.Entities;
using soen390_team01.Data.Exceptions;
using soen390_team01.Data.Queries;
using soen390_team01.Models;
using soen390_team01.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace soen390_team01Tests.Unit.Models
{
    class AssemblyModelTest
    {
        private ErpDbContext _context;
        private AssemblyModel _model;
        private ProductionService _service;
        private Mock<Random> _randMock;
        private List<IProductionReportGenerator> _generators;

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
                            Price = i,               
                        });
                        break;
                    case "part":
                        _context.Parts.Add(new Part
                        {
                            ItemId = i,
                            Grade = "copper " + i,
                            Name = "Part " + i,
                            Size = "L",
                            Price = i
                        });
                        break;
                    default:
                        _context.Materials.Add(new Material
                        {
                            ItemId = i,
                            Grade = "copper " + i,
                            Name = "Material " + i,
                            Price = i
                        });
                        break;
                }

                _context.Inventories.Add(new Inventory
                {
                    ItemId = i,
                    Quantity = 75,
                    Type = type,
                    Warehouse = "Warehouse " + i
                }
                );
            }

            _context.SaveChanges();

            var goodBike = _context.Bikes.First();

            for (var i = 0; i < 3; i++)
            {
                var bikePart = new BikePart
                {
                    BikeId = goodBike.ItemId,
                    PartId = _context.Parts.ToList().ElementAt(i).ItemId,
                    PartQuantity = 5
                };
                _context.BikeParts.Add(bikePart);
                _context.SaveChanges();

                for (var j = 0; j < 3; j++)
                {
                    var partMaterial = new PartMaterial
                    {
                        PartId = bikePart.PartId,
                        MaterialId = _context.Materials.ToList().ElementAt(j).ItemId,
                        MaterialQuantity = 5
                    };
                    _context.PartMaterials.Add(partMaterial);
                }
                _context.SaveChanges();
            }
            for (var i = 1; i <= 5; i++)
            {
                _context.Productions.Add(new Production
                {
                    BikeId = i,
                    Quantity = i,
                    State = "inProgress"
                });

                _context.SaveChanges();
            }
           

            var csvGeneratorMock = new Mock<IProductionReportGenerator>();
            var webGeneratorMock = new Mock<IProductionReportGenerator>();
            _randMock = new Mock<Random>();

            csvGeneratorMock.Setup(g => g.Name).Returns("Csv");
            webGeneratorMock.Setup(g => g.Name).Returns("Web");
            webGeneratorMock.Setup(g => g.Generate(It.IsAny<Production>(), It.IsAny<string>()));
            _randMock.Setup(r => r.Next(10)).Returns(1); // Results in production completion
            _randMock.Setup(r => r.Next(5)).Returns(0); // Results in bad quality
            _randMock.Setup(r => r.Next(2)).Returns(0); // Results in using the web generator

            _generators = new List<IProductionReportGenerator> { csvGeneratorMock.Object, webGeneratorMock.Object };

            _service = new ProductionService(_context, new ProductionInventoryValidator(), _randMock.Object, _generators);
            _model = new AssemblyModel(_context, _service);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            foreach (var entity in _context.Productions)
            {
                _context.Productions.Remove(entity);
            }
            foreach (var entity in _context.BikeParts)
            {
                _context.BikeParts.Remove(entity);
            }
            foreach (var entity in _context.PartMaterials)
            {
                _context.PartMaterials.Remove(entity);
            }
            foreach (var entity in _context.Inventories)
            {
                _context.Inventories.Remove(entity);
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
            _context.Database.EnsureDeleted();
        }

        [Test]
        public void GetFilteredProductionListInvalidTest()
        {
            List<string> list = new List<string>
            {
                "stopped",
                "inProgress",
                "completed"
            };

            var ctx = new Mock<ErpDbContext>();
            ctx.Setup(c => c.Productions).Returns(new List<Production>().AsQueryable().BuildMockDbSet().Object);

            var filters = new Filters("production");
            filters.Add(new CheckboxFilter("production", "State", "state", list) { Values = { "filtered_state" } });
            Assert.Throws<UnexpectedDataAccessException>(() => new AssemblyModel(ctx.Object, _service).GetFilteredProductionList(filters));

        }

        [Test]
        public void ResetTest()
        {
            var initialProductionCount = _model.Productions.Count;
            var initialProductionFilterCount = _model.ProductionFilters.List.Count;
            _model.ResetProductionFilters();
            Assert.AreEqual(initialProductionCount, _model.Productions.Count);
            Assert.AreEqual(initialProductionFilterCount, _model.ProductionFilters.List.Count);
        }

        [Test]
        public void AddNewBikeInsufficientTest()
        {
            var bikeOrder = new BikeOrder
            {
                BikeId = 1,
                ItemQuantity = 1,
            };
            Assert.Throws<InsufficientBikePartsException>(() => _model.AddNewBike(bikeOrder));
        }

        [Test]
        public void AddNewBikeInvalidTest()
        {
            var ctx = new Mock<ErpDbContext>();
            var nbProductionCall = 0;
            ctx.Setup(c => c.Productions).Returns(new List<Production>().AsQueryable().BuildMockDbSet().Object).Callback(() =>
            {
                nbProductionCall++;
                if (nbProductionCall == 1)
                    throw new DbUpdateException("error", new PostgresException("", "", "", ""));
            });
            Assert.Throws<DbUpdateException>(() => new AssemblyModel(ctx.Object, _service).AddNewBike(new BikeOrder()));
        }

        [Test]
        public void UpdateInventoryValidTest()
        {
            Production production = new Production
            {

                BikeId = 1,
                Quantity = 10,
                ProductionId = 1,
                State = "inProgress"
            };

            _model.UpdateInventory(production);
            var updatedInventory = _context.Inventories.FirstOrDefault(inv => inv.ItemId == 1);
            Assert.AreEqual(production.Quantity + 75, updatedInventory.Quantity);
        }

        [Test]
        public void UpdateInventoryNullTest()
        {
            Production production = new Production
            {
                BikeId = 10,
                Quantity = 10,
                ProductionId = 1,
                State = "inProgress"
            };

            _model.UpdateInventory(production);
            var updatedInventory = _context.Inventories.FirstOrDefault(inv => inv.ItemId == production.BikeId);
            Assert.AreEqual(production.Quantity, updatedInventory.Quantity);
        }

        [Test]
        public void UpdateInventoryInvalidTest()
        {
            var ctx = new Mock<ErpDbContext>();
            var nbInventoriesCall = 0;
            ctx.Setup(c => c.Bikes).Returns(new List<Bike>().AsQueryable().BuildMockDbSet().Object);
            ctx.Setup(c => c.Productions).Returns(new List<Production>().AsQueryable().BuildMockDbSet().Object);
            ctx.Setup(c => c.Inventories).Returns(new List<Inventory>().AsQueryable().BuildMockDbSet().Object).Callback(() =>
            {
                nbInventoriesCall++;

                if (nbInventoriesCall == 2)
                    throw new DbUpdateException("error", new PostgresException("", "", "", ""));
            });

            Assert.Throws<UnexpectedDataAccessException>(() => new AssemblyModel(ctx.Object, _service).UpdateInventory(new Production()));
        }

        [Test]
        public void UpdateProductionStateValidTest()
        {
            var production = _context.Productions.FirstOrDefault(p => p.ProductionId == 1);
            _model.UpdateProduction(production);
            var updatedProduction = _context.Productions.FirstOrDefault(p => p.ProductionId == 1);
            Assert.AreEqual("inProgress", updatedProduction.State);
        }

        [Test]
        public void UpdateProductionStateInvalidTest()
        {
            var ctx = new Mock<ErpDbContext>();
            var nbProductionCall = 0;
            ctx.Setup(c => c.Productions).Returns(new List<Production>().AsQueryable().BuildMockDbSet().Object).Callback(() =>
            {
                nbProductionCall++;
                if (nbProductionCall == 2)
                    throw new DbUpdateException("error", new PostgresException("", "", "", ""));
            });
            Assert.Throws<UnexpectedDataAccessException>(() => new AssemblyModel(ctx.Object, _service).UpdateProduction(new Production()));
        }


    }
}
