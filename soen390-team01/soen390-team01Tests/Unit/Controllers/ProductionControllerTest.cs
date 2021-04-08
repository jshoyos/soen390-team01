using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using soen390_team01.Controllers;
using soen390_team01.Data.Entities;
using soen390_team01.Data.Exceptions;
using soen390_team01.Models;
using soen390_team01.Services;
using System.Collections.Generic;


namespace soen390_team01Tests.Unit.Controllers
{
    class ProductionControllerTest
    {
        Mock<IAssemblyService> _modelMock;
        Mock<ILogger<ProductionController>> _loggerMock;
        Mock<IEmailService> _emailServiceMock;

        [SetUp]
        public void Setup()
        {
            _modelMock = new Mock<IAssemblyService>();
            _loggerMock = new Mock<ILogger<ProductionController>>();
            _emailServiceMock = new Mock<IEmailService>();
        }

        [Test]
        public void ProcessProductionValidTest()
        {
            ProcessProductionInput input = new ProcessProductionInput
            {
                Production = new Production
                {
                    BikeId = 1,
                    ProductionId = 1,
                    Quantity = 1,
                    State = "completed",

                },
                Quality = "good"
            };

            _modelMock.Setup(m => m.UpdateInventory(It.IsAny<Production>())).Returns(new Inventory());
            _modelMock.Setup(m => m.UpdateProduction(It.IsAny<Production>())).Returns(input.Production);

            var controller = new ProductionController(_modelMock.Object, _loggerMock.Object, _emailServiceMock.Object)
            {
                TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>())
            };
            controller.Process(input);
            Assert.Null(controller.TempData["errorMessage"]);
            _modelMock.Verify(m => m.UpdateInventory(input.Production), Times.Once);
            _modelMock.Verify(m => m.UpdateProduction(input.Production), Times.Once);
            _emailServiceMock.Verify(m => m.SendEmail(It.IsAny<string>(), Roles.InventoryManager), Times.Never);
        }

        [Test]
        public void ProcessProductionInvalidTest()
        {
            ProcessProductionInput input = new ProcessProductionInput
            {
                Production = new Production
                {
                    BikeId = 1,
                    ProductionId = 1,
                    Quantity = 1,
                    State = "pending",

                },
                Quality = "bad"
            };

            _modelMock.Setup(m => m.UpdateInventory(It.IsAny<Production>())).Throws(new UnexpectedDataAccessException("some_code"));
            _modelMock.Setup(m => m.UpdateProduction(It.IsAny<Production>())).Throws(new UnexpectedDataAccessException("some_code"));

            var controller = new ProductionController(_modelMock.Object, _loggerMock.Object, _emailServiceMock.Object)
            {
                TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>())
            };
            controller.Process(input);
            Assert.IsNotNull(controller.TempData["errorMessage"]);

        }

        [Test]
        public void ProcessProductionSendEmailTest()
        {
            ProcessProductionInput input = new ProcessProductionInput
            {
                Production = new Production
                {
                    BikeId = 1,
                    ProductionId = 1,
                    Quantity = 1,
                    State = "completed",

                },
                Quality = "bad"
            };

            ProcessProductionInput input2 = new ProcessProductionInput
            {
                Production = new Production
                {
                    BikeId = 1,
                    ProductionId = 1,
                    Quantity = 1,
                    State = "stopped",

                },
                Quality = "none"
            };

            _modelMock.Setup(m => m.UpdateInventory(It.IsAny<Production>())).Returns(new Inventory());
            _modelMock.Setup(m => m.UpdateProduction(It.IsAny<Production>())).Returns(input.Production);


            var controller = new ProductionController(_modelMock.Object, _loggerMock.Object, _emailServiceMock.Object)
            {
                TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>())
            };
            controller.Process(input);
            Assert.Null(controller.TempData["errorMessage"]);
            _modelMock.Verify(m => m.UpdateInventory(input.Production), Times.Once);
            _modelMock.Verify(m => m.UpdateProduction(input.Production), Times.Once);
            _emailServiceMock.Verify(m => m.SendEmail(It.IsAny<string>(), Roles.InventoryManager), Times.Once);

            controller.Process(input2);
            Assert.Null(controller.TempData["errorMessage"]);
            _modelMock.Verify(m => m.UpdateInventory(input2.Production), Times.Once);
            _modelMock.Verify(m => m.UpdateProduction(input2.Production), Times.Once);
            _emailServiceMock.Verify(m => m.SendEmail(It.IsAny<string>(), Roles.InventoryManager), Times.Once);

        }
    }
}
