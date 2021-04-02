using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using soen390_team01.Controllers;
using soen390_team01.Data.Entities;
using soen390_team01.Data.Exceptions;
using soen390_team01.Services;
using System.Collections.Generic;


namespace soen390_team01Tests.Unit.Controllers
{
    class ProductionControllerTest
    {
        Mock<IAssemblyService> _modelMock;
        Mock<ILogger<ProductionController>> _loggerMock;

        [SetUp]
        public void Setup()
        {
            _modelMock = new Mock<IAssemblyService>();
            _loggerMock = new Mock<ILogger<ProductionController>>();
        }

        [Test]
        public void ProcessProductionInvalidTest()
        {
            var productionList = new List<Production>();

            for (var i = 1; i <= 5; i++)
            {
                productionList.Add(new Production
                {
                    ProductionId = i,
                    BikeId = i,
                    Quantity = i,
                    State = "pending"
                });
            }

            _modelMock.Setup(m => m.Productions).Returns(productionList);

            Production production = new Production
            {
                BikeId = 1,
                ProductionId = 1,
                Quantity = 1,
                State = "pending",

            };

            var inventory = new Inventory
            {
                ItemId = 1,
                InventoryId = 1,
                Quantity = 1,
                Type = "bike",
                Warehouse = "Warehouse 1"
            };

            var controller = new ProductionController(_modelMock.Object, _loggerMock.Object);

            _modelMock.Setup(m => m.UpdateInventory(It.IsAny<Production>())).Throws(new UnexpectedDataAccessException("some_code"));
            _modelMock.Setup(m => m.UpdateProduction(It.IsAny<Production>())).Throws(new UnexpectedDataAccessException("some_code"));

            controller = new ProductionController(_modelMock.Object, _loggerMock.Object)
            {
                TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>())
            };

            Assert.IsNotNull(controller.TempData["errorMessage"]);
        }
    }
}
