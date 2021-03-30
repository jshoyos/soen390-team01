using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using soen390_team01.Data;
using soen390_team01.Data.Entities;
using soen390_team01.Data.Exceptions;
using soen390_team01.Services;

namespace soen390_team01Tests.Unit.Services
{
    class AssemblyLineServiceTest
    {

        private ErpDbContext _context;
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
                            Price = i
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

        [Test, Order(1)]
        public void ProduceBikeWithoutPartBuild()
        {
            var bike = _context.Bikes.First();
            var csvGeneratorMock = new Mock<IProductionReportGenerator>();
            var webGeneratorMock = new Mock<IProductionReportGenerator>();
            _randMock = new Mock<Random>();

            csvGeneratorMock.Setup(g => g.Name).Returns("Csv");
            webGeneratorMock.Setup(g => g.Name).Returns("Web");
            csvGeneratorMock.Setup(g => g.Generate(It.IsAny<Production>(), It.IsAny<string>()));
            _randMock.Setup(r => r.Next(10)).Returns(1); // Results in production completion
            _randMock.Setup(r => r.Next(5)).Returns(1); // Results in good quality
            _randMock.Setup(r => r.Next(2)).Returns(1); // Results in using the csv generator

            _generators = new List<IProductionReportGenerator> { csvGeneratorMock.Object, webGeneratorMock.Object };

            var service = new AssemblyLineService(_context, new AssemblyInventoryValidator(), _randMock.Object, _generators);

            service.ProduceBike(bike, 15);//After this, there will be no built part left to build the first bike

            Assert.AreEqual(1, _context.Productions.Count(p => p.BikeId == bike.ItemId));

            var updatedBike = _context.Bikes.First(b => b.ItemId == bike.ItemId);
            // Parts used to build the bike should have no quantity left in the inventory
            foreach (var bikePart in updatedBike.BikeParts)
            {
                Assert.AreEqual(0, _context.Inventories.First(i => i.ItemId == bikePart.PartId && i.Type == "part").Quantity);
                // Materials' inventory should not have changed
                foreach (var partMaterial in bikePart.Part.PartMaterials)
                {
                    Assert.AreEqual(75, _context.Inventories.First(i => i.ItemId == partMaterial.MaterialId && i.Type == "material").Quantity);
                }
            }
        }

        [Test, Order(2)]
        public void ProduceBikeWithPartBuild()
        {
            var bike = _context.Bikes.First();
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

            var service = new AssemblyLineService(_context, new AssemblyInventoryValidator(), _randMock.Object, _generators);

            service.ProduceBike(bike, 1);

            Assert.AreEqual(2, _context.Productions.Count(p => p.BikeId == bike.ItemId));

            var updatedBike = _context.Bikes.First(b => b.ItemId == bike.ItemId);
            foreach (var bikePart in updatedBike.BikeParts)
            {
                // Parts used to build the bike should still have no quantity left in the inventory
                Assert.AreEqual(0, _context.Inventories.First(i => i.ItemId == bikePart.PartId && i.Type == "part").Quantity);

                // Materials used to build the bike should have reduced inventory (in this case, inventory should be 0)
                foreach (var partMaterial in bikePart.Part.PartMaterials)
                {
                    Assert.AreEqual(0, _context.Inventories.First(i => i.ItemId == partMaterial.MaterialId && i.Type == "material").Quantity);
                }
            }
        }

        [Test, Order(3)]
        public void ProduceBikeMissingMaterials()
        {
            var bike = _context.Bikes.First();
            var csvGeneratorMock = new Mock<IProductionReportGenerator>();
            var webGeneratorMock = new Mock<IProductionReportGenerator>();
            _randMock = new Mock<Random>();
            
            csvGeneratorMock.Setup(g => g.Name).Returns("Csv");
            webGeneratorMock.Setup(g => g.Name).Returns("Web");
            csvGeneratorMock.Setup(g => g.Generate(It.IsAny<Production>(), It.IsAny<string>()));

            _generators = new List<IProductionReportGenerator> { csvGeneratorMock.Object, webGeneratorMock.Object };

            var service = new AssemblyLineService(_context, new AssemblyInventoryValidator(), _randMock.Object, _generators);
            service.FixStoppedProduction(_context.Productions.First());

            try
            {
                service.ProduceBike(bike, 1);
                Assert.Fail("ProduceBike should throw a MissingPartsException");
            }
            catch (MissingPartsException e)
            {
                Assert.AreEqual(3, e.MissingParts.Count);
                Assert.AreEqual(25, e.MissingParts.ElementAt(0).MissingMaterials.ElementAt(0).Quantity);
            }
        }
    }
}
