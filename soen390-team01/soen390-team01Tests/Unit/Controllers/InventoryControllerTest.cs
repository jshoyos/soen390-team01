using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using NUnit.Framework;
using soen390_team01.Controllers;
using soen390_team01.Data.Entities;
using soen390_team01.Data.Exceptions;
using soen390_team01.Data.Queries;
using soen390_team01.Models;
using soen390_team01.Services;

namespace soen390_team01Tests.Controllers
{
    public class InventoryControllerTest
    {
        Mock<IInventoryService> _modelMock;

        [SetUp]
        public void Setup()
        {
            _modelMock = new Mock<IInventoryService>();
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

            _modelMock.Setup(m => m.AllList).Returns(allList);

            var controller = new InventoryController(_modelMock.Object);

            var result = controller.Index() as ViewResult;
            Assert.IsNotNull(result);
            Assert.AreEqual(9, (result.Model as IInventoryService).AllList.Count);
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

            _modelMock.Setup(m => m.Update(It.IsAny<Inventory>())).Returns(inventory);

            var controller = new InventoryController(_modelMock.Object);

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

            var controller = new InventoryController(_modelMock.Object);

            var result = controller.ChangeQuantity(inventory) as PartialViewResult;
            Assert.IsNotNull(result);
            Assert.AreEqual(0, ((Inventory) result.Model).Quantity, "Quantity should be 0 when the incoming quantity is negative");

            _modelMock.Setup(i => i.Update(It.IsAny<Inventory>())).Throws(new UnexpectedDataAccessException("some_code"));

            controller = new InventoryController(_modelMock.Object) {
                TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>())
            };

            Assert.IsNotNull(controller.ChangeQuantity(inventory) as PartialViewResult);
            Assert.IsNotNull(controller.TempData["errorMessage"]);
        }

        [Test]
        public void FilterProductTableTest()
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

            _modelMock.Setup(m => m.BikeList).Returns(bikeList);
            _modelMock.Setup(i => i.FilterSelectedTab(It.IsAny<Filters>()));

            var controller = new InventoryController(_modelMock.Object);
            var result = controller.FilterProductTable(new MobileFiltersInput { Filters = filters, Mobile = false }) as PartialViewResult;
            Assert.IsNotNull(result);
            Assert.AreEqual(1, (result.Model as IInventoryService).BikeList.Count);
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

            _modelMock.Setup(i => i.FilterSelectedTab(It.IsAny<Filters>())).Throws(new UnexpectedDataAccessException("some_code") );
            var controller = new InventoryController(_modelMock.Object) {
                TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>())
            };
            Assert.IsNotNull(controller.FilterProductTable(new MobileFiltersInput { Filters = filters, Mobile = true }) as PartialViewResult);
            Assert.IsNotNull(controller.TempData["errorMessage"]);
        }

        [Test]
        public void RefreshTest()
        {
            _modelMock.Setup(m => m.ResetBikes());
            _modelMock.Setup(m => m.ResetParts());
            _modelMock.Setup(m => m.ResetMaterials());

            var controller = new InventoryController(_modelMock.Object);

            controller.Refresh(new RefreshTabInput { SelectedTab = "bike", Mobile = true});
            _modelMock.Verify(m => m.ResetBikes(), Times.Once());
            controller.Refresh(new RefreshTabInput { SelectedTab = "part", Mobile = false });
            _modelMock.Verify(m => m.ResetParts(), Times.Once());
            controller.Refresh(new RefreshTabInput { SelectedTab = "material", Mobile = false });
            _modelMock.Verify(m => m.ResetMaterials(), Times.Once());
        }
    }
}
