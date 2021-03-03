using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using NUnit.Framework;
using soen390_team01.Controllers;
using soen390_team01.Data;
using soen390_team01.Data.Entities;
using soen390_team01.Data.Exceptions;
using soen390_team01.Data.Queries;
using soen390_team01.Models;
using soen390_team01.Services;

namespace soen390_team01Tests.Controllers
{
    public class InventoryControllerTest
    {
        Mock<InventoryService> inventoryServiceMock;
        [SetUp]
        public void Setup()
        {
            inventoryServiceMock = new Mock<InventoryService>(new Mock<ErpDbContext>().Object);
            inventoryServiceMock.Setup(i => i.ResetBikeFilters()).Returns(new Filters("bike"));
            inventoryServiceMock.Setup(i => i.ResetPartFilters()).Returns(new Filters("part"));
            inventoryServiceMock.Setup(i => i.ResetMaterialFilters()).Returns(new Filters("material"));
        }

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
            
            inventoryServiceMock.Object.Model = inventoryModel;
            var controller = new InventoryController(inventoryServiceMock.Object);

            var result = controller.Index() as ViewResult;
            Assert.IsNotNull(result);
            Assert.AreEqual(9, (result.Model as InventoryModel).AllList.Count);
        }

        [Test]
        public void ChangeQuantityValidTest()
        {
            var inventory = new Inventory
            {
                ItemId = 1,
                InventoryId = 1,
                Quantity = 1,
                Type = "bike",
                Warehouse = "Warehouse 1"
            };
            
            var controller = new InventoryController(inventoryServiceMock.Object);

            var result = controller.ChangeQuantity(inventory) as PartialViewResult;
            Assert.IsNotNull(result);
            Assert.AreEqual(inventory, result.Model);
        }

        [Test]
        public void ChangeQuantityInValidTest()
        {
            var inventory = new Inventory
            {
                ItemId = 1,
                InventoryId = 1,
                Quantity = -5,
                Type = "bike",
                Warehouse = "Warehouse 1"
            };

            var controller = new InventoryController(inventoryServiceMock.Object);

            var result = controller.ChangeQuantity(inventory) as PartialViewResult;
            Assert.IsNotNull(result);
            Assert.AreEqual(0, ((Inventory) result.Model).Quantity, "Quantity should be 0 when the incoming quantity is negative");

            inventoryServiceMock.Setup(i => i.Update(It.IsAny<Inventory>())).Throws(new UnexpectedDataAccessException("some_code"));

            controller = new InventoryController(inventoryServiceMock.Object) {
                TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>())
            };

            Assert.IsNotNull(controller.ChangeQuantity(inventory) as PartialViewResult);
            Assert.IsNotNull(controller.TempData["errorMessage"]);
        }

        [Test]
        public void FilterProductTableBikeTest()
        {
            var bikeList = new List<Bike>();
            var filters = new Filters("bike");
            filters.Add(new StringFilter("bike", "Grade", "grade") { Value = "some_value" });
            bikeList.Add(new Bike {
                ItemId = 1,
                Grade = "copper",
                Name = "Bike 1",
                Size = "M",
                Price = 1
            });

            inventoryServiceMock.Setup(i => i.GetFilteredProductList<Bike>(It.IsAny<Filters>())).Returns(bikeList);

            var controller = new InventoryController(inventoryServiceMock.Object);
            var result = controller.FilterProductTable(filters) as PartialViewResult;
            Assert.IsNotNull(result);
            Assert.AreEqual(1, (result.Model as InventoryModel).BikeList.Count, "With active filters, BikeList should only contain 1 bike");

            bikeList = new List<Bike>();

            var nbInitial = 5;

            for (var i = 0; i < nbInitial; i++)
            {
                bikeList.Add(new Bike
                {
                    ItemId = i,
                    Grade = "copper",
                    Name = "Bike " + i,
                    Size = "M",
                    Price = i
                });
            }

            inventoryServiceMock.Setup(i => i.GetAllBikes()).Returns(bikeList);

            controller = new InventoryController(inventoryServiceMock.Object);
            result = controller.FilterProductTable(new Filters("bike")) as PartialViewResult;
            Assert.IsNotNull(result);
            Assert.AreEqual(nbInitial, (result.Model as InventoryModel).BikeList.Count, "With non-active filters, BikeList should contain the initial number of bikes");
        }

        [Test]
        public void FilterProductTablePartTest()
        {
            var partList = new List<Part>();
            var filters = new Filters("part");
            filters.Add(new StringFilter("part", "Grade", "grade") { Value = "some_value" });

            partList.Add(new Part
            {
                ItemId = 1,
                Grade = "copper",
                Name = "Part 1",
                Size = "L",
                Price = 1
            }
            );

            inventoryServiceMock.Setup(i => i.GetFilteredProductList<Part>(It.IsAny<Filters>())).Returns(partList);
            var controller = new InventoryController(inventoryServiceMock.Object);

            var result = controller.FilterProductTable(filters) as PartialViewResult;
            Assert.IsNotNull(result);
            Assert.AreEqual(1, (result.Model as InventoryModel).PartList.Count, "With active filters, PartList should only contain 1 part");

            partList = new List<Part>();
            var nbInitial = 4;

            for (var i = 0; i < nbInitial; i++)
            {
                partList.Add(new Part
                {
                    ItemId = i,
                    Grade = "copper",
                    Name = "Bike " + i,
                    Size = "M",
                    Price = i
                });
            }

            inventoryServiceMock.Setup(i => i.GetAllParts()).Returns(partList);

            controller = new InventoryController(inventoryServiceMock.Object);
            result = controller.FilterProductTable(new Filters("part")) as PartialViewResult;
            Assert.IsNotNull(result);
            Assert.AreEqual(nbInitial, (result.Model as InventoryModel).PartList.Count, "With non-active filters, BikeList should contain the initial number of bikes");
        }

        [Test]
        public void FilterProductTableMaterialTest()
        {
            var materialList = new List<Material>();
            var filters = new Filters("material");
            filters.Add(new StringFilter("material", "Name", "name") { Value = "some_value" });

            materialList.Add(new Material
            {
                ItemId = 1,
                Grade = "copper",
                Name = "Material 1",
                Price = 1
            });

            inventoryServiceMock.Setup(i => i.GetFilteredProductList<Material>(It.IsAny<Filters>())).Returns(materialList);
            var controller = new InventoryController(inventoryServiceMock.Object);

            var result = controller.FilterProductTable(filters) as PartialViewResult;
            Assert.IsNotNull(result);
            Assert.AreEqual(1, (result.Model as InventoryModel).MaterialList.Count, "With active filters, MaterialList should only contain 1 material");

            materialList = new List<Material>();

            var nbInitial = 3;

            for (var i = 0; i < nbInitial; i++)
            {
                materialList.Add(new Material
                {
                    ItemId = i,
                    Grade = "copper",
                    Name = "Material " + i,
                    Price = i
                });
            }

            inventoryServiceMock.Setup(i => i.GetAllMaterials()).Returns(materialList);

            controller = new InventoryController(inventoryServiceMock.Object);
            result = controller.FilterProductTable(new Filters("material")) as PartialViewResult;
            Assert.IsNotNull(result);
            Assert.AreEqual(nbInitial, (result.Model as InventoryModel).MaterialList.Count, "With non-active filters, MaterialList should contain the initial number of materials");
        }

        [Test]
        public void FilterProductTableExceptionTest()
        {
            var materialList = new List<Material>();
            var filters = new Filters("material");
            filters.Add(new StringFilter("material", "Name", "name") { Value = "some_value" });

            materialList.Add(new Material
            {
                ItemId = 1,
                Grade = "copper",
                Name = "Material 1",
                Price = 1
            });

            inventoryServiceMock.Setup(i => i.GetFilteredProductList<Material>(It.IsAny<Filters>())).Throws(new UnexpectedDataAccessException("some_code") );
            var controller = new InventoryController(inventoryServiceMock.Object) {
                TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>())
            };
            Assert.IsNotNull(controller.FilterProductTable(filters) as PartialViewResult);
            Assert.IsNotNull(controller.TempData["errorMessage"]);
        }

        [Test]
        public void RefreshTest()
        {
            var allList = new List<Inventory>();

            var selectedTab = "bike";
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

            inventoryServiceMock.Object.Model = inventoryModel;
            var controller = new InventoryController(inventoryServiceMock.Object);

            var result = controller.Refresh(selectedTab) as PartialViewResult;
            Assert.IsNotNull(result);
            Assert.AreEqual(9, (result.Model as InventoryModel).AllList.Count);
        }
    }
}
