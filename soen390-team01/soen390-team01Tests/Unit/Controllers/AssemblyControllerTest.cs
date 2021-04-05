using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging;
using MockQueryable.Moq;
using Moq;
using NUnit.Framework;
using soen390_team01.Controllers;
using soen390_team01.Data;
using soen390_team01.Data.Entities;
using soen390_team01.Data.Exceptions;
using soen390_team01.Data.Queries;
using soen390_team01.Models;
using soen390_team01.Services;
using System.Collections.Generic;
using System.Linq;


namespace soen390_team01Tests.Unit.Controllers
{
    class AssemblyControllerTest
    {
        Mock<IProductionService> _serviceMock;
        Mock<IAssemblyService> _modelMock;
        Mock<ILogger<AssemblyController>> _loggerMock;

        [SetUp]
        public void Setup()
        {
            _serviceMock = new Mock<IProductionService>();
            _modelMock = new Mock<IAssemblyService>();
            _loggerMock = new Mock<ILogger<AssemblyController>>();
        }

        [Test]
        public void IndexTest()
        {
            var productionList = new List<Production>();

            for (var i = 1; i <= 5; i++)
            {
                productionList.Add(new Production
                {
                    ProductionId = i,
                    BikeId = i,
                    Quantity = i,
                    State = "inProgress"
                });
            }

            _modelMock.Setup(m => m.Productions).Returns(productionList);

            var controller = new AssemblyController(_modelMock.Object, _loggerMock.Object);

            var result = controller.Index() as ViewResult;
            Assert.IsNotNull(result);
            Assert.AreEqual(5, (result.Model as IAssemblyService).Productions.Count);

        }

        [Test]
        public void AddProductionTest()
        {
            var ctx = new Mock<ErpDbContext>();
            ctx.Setup(c => c.Productions).Returns(new List<Production>().AsQueryable().BuildMockDbSet().Object);
            ctx.Setup(c => c.Parts).Returns(new List<Part>().AsQueryable().BuildMockDbSet().Object);
            ctx.Setup(c => c.BikeParts).Returns(new List<BikePart>().AsQueryable().BuildMockDbSet().Object);
            ctx.Setup(c => c.Bikes).Returns(new List<Bike>().AsQueryable().BuildMockDbSet().Object);


            var assemblyModel = CreateModel(false);
            var controller = new AssemblyController(assemblyModel, _loggerMock.Object);

            var inputModel = new AssemblyModel(ctx.Object, _serviceMock.Object)
            {
                BikeOrder = new BikeOrder
                {
                    BikeId = 1,
                    ItemQuantity = 1
                }
            };

            var resultProduction = controller.AddProduction(inputModel) as RedirectToActionResult;
            Assert.IsNotNull(resultProduction);
            Assert.AreEqual("Index", resultProduction.ActionName);
        }
        [Test]
        public void AddProductionInsufficientTest()
        {
            var ctx = new Mock<ErpDbContext>();
            ctx.Setup(c => c.Productions).Returns(new List<Production>().AsQueryable().BuildMockDbSet().Object);
            ctx.Setup(c => c.Parts).Returns(new List<Part>().AsQueryable().BuildMockDbSet().Object);
            ctx.Setup(c => c.BikeParts).Returns(new List<BikePart>().AsQueryable().BuildMockDbSet().Object);
            ctx.Setup(c => c.Bikes).Returns(new List<Bike>().AsQueryable().BuildMockDbSet().Object);


            var assemblyModel = CreateModel(true);
            var controller = new AssemblyController(assemblyModel, _loggerMock.Object)
            {
                TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>())
            };

            var inputModel = new AssemblyModel(ctx.Object, _serviceMock.Object)
            {
                BikeOrder = new BikeOrder
                {
                    BikeId = 1,
                    ItemQuantity = 1
                }
            };

            var resultProduction = controller.AddProduction(inputModel) as RedirectToActionResult;
            Assert.IsNotNull(resultProduction);
            Assert.AreEqual("Index", resultProduction.ActionName);
        }
        private AssemblyModel CreateModel(bool lessBikeParts)
        {
            var productions = new List<Production>();
            var bikes = new List<Bike>();
            var bikeParts = new List<BikePart>();
            var parts = new List<Part>();

            for (var i = 1; i <= 5; i++)
            {
                productions.Add(new Production
                {
                    ProductionId = i,
                    BikeId = i,
                    Quantity = i,
                    State = "inProgress"
                });
                bikes.Add(new Bike
                {
                    ItemId = i,
                    Grade = "copper",
                    Name = "Bike" + i,
                    Size = "M",
                    Price = i + 10,
                    BikeParts = bikeParts
                });
                parts.Add(new Part
                {
                    ItemId = i+5,
                    Grade = "copper",
                    Name = "Part" + (i + 4),
                    Size = "M",
                    Price = i + 10

                });
                if (lessBikeParts && i == 4)
                {
                    continue;
                }
                bikeParts.Add(new BikePart
                {
                    BikeId = 1,
                    PartId = i + 4,
                    PartQuantity = 1
                });
            }

            var ctx = new Mock<ErpDbContext>();
            ctx.Setup(c => c.Productions).Returns(productions.AsQueryable().BuildMockDbSet().Object);
            ctx.Setup(c => c.Parts).Returns(parts.AsQueryable().BuildMockDbSet().Object);
            ctx.Setup(c => c.BikeParts).Returns(bikeParts.AsQueryable().BuildMockDbSet().Object);
            ctx.Setup(c => c.Bikes).Returns(bikes.AsQueryable().BuildMockDbSet().Object);


            var assemblyModel = new AssemblyModel(ctx.Object, _serviceMock.Object)
            {
                Productions = productions
            };

            return assemblyModel;
        }

        [Test]
        public void FilterAssemblyTableTest()
        {
            List<string> list = new List<string>
            {
                "inProgress",
                "stopped",
                "completed"
            };

            var productionList = new List<Production>();
            var filters = new Filters("production");
            filters.Add(new CheckboxFilter("production", "State", "state", list) { Values = { "some_value" } });

            productionList.Add(new Production
            {
                ProductionId = 1,
                BikeId = 1,
                Quantity = 1,
                State = "inProgress"
            });

            _modelMock.Setup(m => m.Productions).Returns(productionList);
            _modelMock.Setup(i => i.GetFilteredProductionList(It.IsAny<Filters>()));

            var controller = new AssemblyController(_modelMock.Object, _loggerMock.Object);
            var result = controller.FilterAssemblyTable(new MobileFiltersInput { Filters = filters, Mobile = false }) as PartialViewResult;
            Assert.IsNotNull(result);
            Assert.AreEqual(1, (result.Model as IAssemblyService).Productions.Count);
        }

        [Test]
        public void FilterTransferTableExceptionTest()
        {
            List<string> list = new List<string>
            {
                 "inProgress",
                "stopped",
                "completed"
            };
            var productionList = new List<Production>();
            var filters = new Filters("production");
            filters.Add(new CheckboxFilter("production", "State", "state", list) { Values = { "some_value" } });

            productionList.Add(new Production
            {
                BikeId = 1,
                ProductionId = 1,
                Quantity = 1,
                State = "inProgress",

            });

            _modelMock.Setup(i => i.GetFilteredProductionList(It.IsAny<Filters>())).Throws(new UnexpectedDataAccessException("some_code"));
            var controller = new AssemblyController(_modelMock.Object, _loggerMock.Object)
            {
                TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>())
            };
            Assert.IsNotNull(controller.FilterAssemblyTable(new MobileFiltersInput { Filters = filters, Mobile = true }) as PartialViewResult);
            Assert.IsNotNull(controller.TempData["errorMessage"]);
        }

        [Test]
        public void RefreshTest()
        {
            _modelMock.Setup(m => m.GetProductions());
            _modelMock.Setup(m => m.ResetProductionFilters());

            var controller = new AssemblyController(_modelMock.Object, _loggerMock.Object);

            controller.Refresh(new RefreshTabInput { SelectedTab = "production", Mobile = true });
            _modelMock.Verify(m => m.GetProductions(), Times.Once());
            _modelMock.Verify(m => m.ResetProductionFilters(), Times.Once());
        }

        [Test]
        public void FixProductionValidTest()
        {
            _modelMock.Setup(m => m.FixProduction(It.IsAny<long>()));

            var controller = new AssemblyController(_modelMock.Object, _loggerMock.Object);

            controller.FixProduction(1);
            _modelMock.Verify(m => m.FixProduction(1), Times.Once());
        }

        [Test]
        public void FixProductionInvalidTest()
        {
            _modelMock.Setup(m => m.FixProduction(It.IsAny<long>())).Throws(new NotFoundException("", "", ""));

            var controller = new AssemblyController(_modelMock.Object, _loggerMock.Object)
            {
                TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>())
            };

            Assert.IsNotNull(controller.FixProduction(1));
            Assert.IsNotNull(controller.TempData["errorMessage"]);
        }
    }
}
