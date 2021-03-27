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
        Mock<IAssemblyService> _modelMock;
        Mock<ILogger<AssemblyController>> _loggerMock;

        [SetUp]
        public void Setup()
        {
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
                    State = "pending"
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

            var assemblyModel = CreateModel();
            var controller = new AssemblyController(assemblyModel, _loggerMock.Object);

            var inputModel = new AssemblyModel(ctx.Object)
            {
                BikeOrder = new BikeOrder
                {
                    BikeId = 1,
                    ItemQuantity = 1
                }
            };

            var resultProduction = controller.AddProduction(inputModel) as ViewResult;
           
            Assert.AreEqual("production", (resultProduction.Model as AssemblyModel).SelectedTab);
            Assert.AreEqual(false, (resultProduction.Model as AssemblyModel).ShowModal); 
        }
        private static AssemblyModel CreateModel()
        {
            var productions = new List<Production>();
            var ctx = new Mock<ErpDbContext>();
            ctx.Setup(c => c.Productions).Returns(new List<Production>().AsQueryable().BuildMockDbSet().Object);

            for (var i = 1; i <= 5; i++)
            {
                productions.Add(new Production
                {
                    ProductionId = i,
                    BikeId = i,
                    Quantity = i,
                    State = "pending"
                });
            }

            var assemblyModel = new AssemblyModel(ctx.Object)
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
                "pending",
                "completed",
                "canceled"
            };

            var productionList = new List<Production>();
            var filters = new Filters("production");
            filters.Add(new CheckboxFilter("production", "State", "state", list) { Values = { "some_value" } });

            productionList.Add(new Production
            {
                ProductionId = 1,
                BikeId = 1,
                Quantity = 1,
                State = "pending"
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
                "pending",
                "completed",
                "canceled"
            };
            var productionList = new List<Production>();
            var filters = new Filters("production");
            filters.Add(new CheckboxFilter("production", "State", "state", list) { Values = { "some_value" } });

            productionList.Add(new Production
            {
               BikeId = 1,
               ProductionId = 1,
               Quantity = 1,
               State = "pending",
               
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
    }
}
