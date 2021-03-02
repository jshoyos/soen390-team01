using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using soen390_team01.Controllers;
using soen390_team01.Data;
using soen390_team01.Data.Entities;
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
            
            var controller = new InventoryController(inventoryServiceMock.Object);

            var result = controller.ChangeQuantity(inventory) as PartialViewResult;
            Assert.IsNotNull(result);
            Assert.AreEqual(inventory, result.Model);
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

            inventoryServiceMock.Setup(i => i.GetFilteredProductList<Bike>(filters)).Returns(bikeList);
            var controller = new InventoryController(inventoryServiceMock.Object);

            var result = controller.FilterProductTable(filters) as PartialViewResult;
            Assert.IsNotNull(result);
            Assert.AreEqual(1, (result.Model as InventoryModel).BikeList.Count);
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

            var inventoryModel = new InventoryModel
            {
                PartList = partList
            };
            
            inventoryServiceMock.Setup(i => i.GetFilteredProductList<Part>(filters)).Returns(partList);
            var controller = new InventoryController(inventoryServiceMock.Object);

            var result = controller.FilterProductTable(filters) as PartialViewResult;
            Assert.IsNotNull(result);
            Assert.AreEqual(1, (result.Model as InventoryModel).PartList.Count);
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

            inventoryServiceMock.Setup(i => i.GetFilteredProductList<Material>(filters)).Returns(materialList);
            var controller = new InventoryController(inventoryServiceMock.Object);

            var result = controller.FilterProductTable(filters) as PartialViewResult;
            Assert.IsNotNull(result);
            Assert.AreEqual(1, (result.Model as InventoryModel).MaterialList.Count);
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
