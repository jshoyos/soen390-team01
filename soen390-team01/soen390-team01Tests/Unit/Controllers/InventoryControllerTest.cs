using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using soen390_team01.Controllers;
using soen390_team01.Data;
using soen390_team01.Data.Entities;
using soen390_team01.Models;
using soen390_team01.Services;

namespace soen390_team01Tests.Controllers
{
    public class InventoryControllerTest
    {
        [Test]
        public void IndexTest()
        {
            var allList = new List<Inventory>();
            for (var i = 1; i <= 9; i++)
            {
                var type = (9 % i) switch
                {
                    0 => "bike",
                    1 => "part",
                    _ => "material"
                };
                allList.Add(new Inventory
                {
                    ItemId = i,
                    InventoryId = i,
                    Quantity = i,
                    Type = type,
                    Warehouse = "Warehouse " + i
                }
                );
            }

            var inventoryModel = new InventoryModel
            {
                AllList = allList
            };
            var context = new Mock<ErpDbContext>();
            var inventoryServiceMock = new Mock<InventoryService>(context.Object);
            inventoryServiceMock.Setup(i => i.SetupModel()).Returns(inventoryModel);
            var controller = new InventoryController(inventoryServiceMock.Object);

            var result = controller.Index() as ViewResult;
            Assert.IsNotNull(result);
            Assert.AreEqual(9, (result.Model as InventoryModel).AllList.Count);
        }

        [Test]
        public void ChangeQuantityTest()
        {
            var inventory = new Inventory
            {
                ItemId = 1,
                InventoryId = 1,
                Quantity = 1,
                Type = "bike",
                Warehouse = "Warehouse 1"
            };
            var context = new Mock<ErpDbContext>();
            var inventoryServiceMock = new Mock<InventoryService>(context.Object);
            inventoryServiceMock.SetupAllProperties();
            var controller = new InventoryController(inventoryServiceMock.Object);

            var result = controller.ChangeQuantity(inventory) as PartialViewResult;
            Assert.IsNotNull(result);
            Assert.AreEqual(inventory, result.Model);
        }

        [Test]
        public void FilterProductTableBikeTest()
        {
            var bikeList = new List<Bike>();
            var input = new ProductFilterInput
            {
                Type = "Bike",
                Value = "Bike 1",
                Name = "name"
            };

            bikeList.Add(new Bike
            {
                ItemId = 1,
                Grade = "copper",
                Name = "Bike 1",
                Size = "M",
                Price = 1
            }
            );

            var inventoryModel = new InventoryModel
            {
                BikeList = bikeList
            };
            var context = new Mock<ErpDbContext>();
            var inventoryServiceMock = new Mock<InventoryService>(context.Object);
            inventoryServiceMock.Setup(i => i.GetFilteredProductList<Bike>(input)).Returns(bikeList);
            var controller = new InventoryController(inventoryServiceMock.Object);

            var result = controller.FilterProductTable(input) as PartialViewResult;
            Assert.IsNotNull(result);
            Assert.AreEqual(1, (result.Model as List<Bike>).Count);
        }

        [Test]
        public void FilterProductTablePartTest()
        {
            var partList = new List<Part>();
            var input = new ProductFilterInput
            {
                Type = "Part",
                Value = "Part 1",
                Name = "name"
            };

            partList.Add(new Part
            {
                ItemId = 1,
                Grade = "copper",
                Name = "Part 1",
                Size = "L",
                Price = 1
            }
            );

            var inventoryModel = new InventoryModel
            {
                PartList = partList
            };
            var context = new Mock<ErpDbContext>();
            var inventoryServiceMock = new Mock<InventoryService>(context.Object);
            inventoryServiceMock.Setup(i => i.GetFilteredProductList<Part>(input)).Returns(partList);
            var controller = new InventoryController(inventoryServiceMock.Object);

            var result = controller.FilterProductTable(input) as PartialViewResult;
            Assert.IsNotNull(result);
            Assert.AreEqual(1, (result.Model as List<Part>).Count);
        }

        [Test]
        public void FilterProductTableMaterialTest()
        {
            var materialList = new List<Material>();
            var input = new ProductFilterInput
            {
                Type = "Material",
                Value = "Material 1",
                Name = "name"
            };

            materialList.Add(new Material
            {
                ItemId = 1,
                Grade = "copper",
                Name = "Material 1",
                Price = 1
            }
            );

            var inventoryModel = new InventoryModel
            {
                MaterialList = materialList
            };
            var context = new Mock<ErpDbContext>();
            var inventoryServiceMock = new Mock<InventoryService>(context.Object);
            inventoryServiceMock.Setup(i => i.GetFilteredProductList<Material>(input)).Returns(materialList);
            var controller = new InventoryController(inventoryServiceMock.Object);

            var result = controller.FilterProductTable(input) as PartialViewResult;
            Assert.IsNotNull(result);
            Assert.AreEqual(1, (result.Model as List<Material>).Count);
        }

        [Test]
        public void FilterProductTableBikeClearTest()
        {
            var bikeList = new List<Bike>();
            var input = new ProductFilterInput
            {
                Type = "Bike",
                Value = "clear",
                Name = ""
            };
            for (var i = 1; i <= 3; i++)
            {
                bikeList.Add(new Bike
                {
                    ItemId = i,
                    Grade = "Copper " + i,
                    Name = "Bike " + i,
                    Size = "L",
                    Price = i
                }
                );
            }

            var inventoryModel = new InventoryModel
            {
                BikeList = bikeList
            };
            var context = new Mock<ErpDbContext>();
            var inventoryServiceMock = new Mock<InventoryService>(context.Object);
            inventoryServiceMock.Setup(i => i.GetAllBikes()).Returns(bikeList);
            var controller = new InventoryController(inventoryServiceMock.Object);

            var result = controller.FilterProductTable(input) as PartialViewResult;
            Assert.IsNotNull(result);
            Assert.AreEqual(3, (result.Model as List<Bike>).Count);
        }

        [Test]
        public void FilterProductTablePartClearTest()
        {
            var partList = new List<Part>();
            var input = new ProductFilterInput
            {
                Type = "Part",
                Value = "clear",
                Name = ""
            };
            for (var i = 1; i <= 3; i++)
            {
                partList.Add(new Part
                {
                    ItemId = i,
                    Grade = "Copper " + i,
                    Name = "Part " + i,
                    Size = "L",
                    Price = i
                }
                );
            }

            var inventoryModel = new InventoryModel
            {
                PartList = partList
            };
            var context = new Mock<ErpDbContext>();
            var inventoryServiceMock = new Mock<InventoryService>(context.Object);
            inventoryServiceMock.Setup(i => i.GetAllParts()).Returns(partList);
            var controller = new InventoryController(inventoryServiceMock.Object);

            var result = controller.FilterProductTable(input) as PartialViewResult;
            Assert.IsNotNull(result);
            Assert.AreEqual(3, (result.Model as List<Part>).Count);
        }

        [Test]
        public void FilterProductTableMaterialClearTest()
        {
            var materialList = new List<Material>();
            var input = new ProductFilterInput
            {
                Type = "Material",
                Value = "clear",
                Name = ""
            };
            for (var i = 1; i <= 3; i++)
            {
                materialList.Add(new Material
                {
                    ItemId = i,
                    Grade = "Copper " + i,
                    Name = "Material " + i,
                    Price = i
                }
                );
            }

            var inventoryModel = new InventoryModel
            {
                MaterialList = materialList
            };
            var context = new Mock<ErpDbContext>();
            var inventoryServiceMock = new Mock<InventoryService>(context.Object);
            inventoryServiceMock.Setup(i => i.GetAllMaterials()).Returns(materialList);
            var controller = new InventoryController(inventoryServiceMock.Object);

            var result = controller.FilterProductTable(input) as PartialViewResult;
            Assert.IsNotNull(result);
            Assert.AreEqual(3, (result.Model as List<Material>).Count);
        }

        [Test]
        public void RefreshTest()
        {
            var allList = new List<Inventory>();

            string selectedTab = "Bike";
            for (var i = 1; i <= 9; i++)
            {
                var type = (9 % i) switch
                {
                    0 => "bike",
                    1 => "part",
                    _ => "material"
                };
                allList.Add(new Inventory
                {
                    ItemId = i,
                    InventoryId = i,
                    Quantity = i,
                    Type = type,
                    Warehouse = "Warehouse " + i
                }
                );
            }

            var inventoryModel = new InventoryModel
            {
                AllList = allList
            };
            var context = new Mock<ErpDbContext>();
            var inventoryServiceMock = new Mock<InventoryService>(context.Object);
            inventoryServiceMock.Setup(i => i.SetupModel()).Returns(inventoryModel);
            var controller = new InventoryController(inventoryServiceMock.Object);

            var result = controller.Refresh(selectedTab) as PartialViewResult;
            Assert.IsNotNull(result);
            Assert.AreEqual(9, (result.Model as InventoryModel).AllList.Count);
        }
    }
}
