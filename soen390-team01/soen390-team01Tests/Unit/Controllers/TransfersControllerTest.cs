using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
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
using PartialViewResult = Microsoft.AspNetCore.Mvc.PartialViewResult;
using ViewResult = Microsoft.AspNetCore.Mvc.ViewResult;

namespace soen390_team01Tests.Controllers
{
    public class TransfersControllerTest
    {
        Mock<ITransferService> _modelMock;
        Mock<ILogger<TransfersController>> _loggerMock;

        [SetUp]
        public void Setup()
        {
            _modelMock = new Mock<ITransferService>();
            _loggerMock = new Mock<ILogger<TransfersController>>();
        }

        [Test]
        public void IndexTest()
        {
            var orderList = new List<Order>();
            var procurementList = new List<Procurement>();
            for (var i = 1; i <= 5; i++)
            {
               
                orderList.Add(new Order
                {
                    OrderId = i,
                    CustomerId = i,
                    State = "pending",
                    PaymentId = i
                });

                procurementList.Add(new Procurement
                {
                    ProcurementId = i,
                    ItemId = i,
                    PaymentId = i,
                    ItemQuantity = i,
                    State = "pending",
                    Type = "bike",
                    VendorId = i
                });

            }

            _modelMock.Setup(m => m.Orders).Returns(orderList);
            _modelMock.Setup(m => m.Procurements).Returns(procurementList);

            var controller = new TransfersController(_modelMock.Object, _loggerMock.Object);

            var result = controller.Index() as ViewResult;
            Assert.IsNotNull(result);
            Assert.AreEqual(5, (result.Model as ITransferService).Orders.Count);
            Assert.AreEqual(5, (result.Model as ITransferService).Procurements.Count);

        }

        [Test]
        public void AddProcurementTest()
        {
            var ctx = new Mock<ErpDbContext>();
            ctx.Setup(c => c.Procurements).Returns(new List<Procurement>().AsQueryable().BuildMockDbSet().Object);
            ctx.Setup(c => c.Vendors).Returns(new List<Vendor>().AsQueryable().BuildMockDbSet().Object);
            ctx.Setup(c => c.Payments).Returns(new List<Payment>().AsQueryable().BuildMockDbSet().Object);
            ctx.Setup(c => c.Orders).Returns(new List<Order>().AsQueryable().BuildMockDbSet().Object);
            ctx.Setup(c => c.Customers).Returns(new List<Customer>().AsQueryable().BuildMockDbSet().Object);

            var transfersModel = CreateModel();
            var controller = new TransfersController(transfersModel, _loggerMock.Object);

            var inputModel = new TransfersModel(ctx.Object)
            {
                AddProcurement = new AddProcurementModel
                {
                    ItemId = 0,
                    ItemType = "Bike",
                    ItemQuantity = 1,
                    VendorId = 1
                }
            };

            var resultBike = controller.AddProcurement(inputModel) as ViewResult;
            inputModel.AddProcurement.ItemType = "Part";
            var resultPart = controller.AddProcurement(inputModel) as ViewResult;
            inputModel.AddProcurement.ItemType = "Material";
            var resultMaterial = controller.AddProcurement(inputModel) as ViewResult;

            Assert.AreEqual("procurement", (resultBike.Model as TransfersModel).SelectedTab);
            Assert.AreEqual(false, (resultBike.Model as TransfersModel).ShowModal);
            Assert.AreEqual("procurement", (resultPart.Model as TransfersModel).SelectedTab);
            Assert.AreEqual(false, (resultPart.Model as TransfersModel).ShowModal);
            Assert.AreEqual("procurement", (resultMaterial.Model as TransfersModel).SelectedTab);
            Assert.AreEqual(false, (resultMaterial.Model as TransfersModel).ShowModal);
        }

        private static TransfersModel CreateModel()
        {
            var orders = new List<Order>();
            var procurements = new List<Procurement>();
            var ctx = new Mock<ErpDbContext>();
            ctx.Setup(c => c.Procurements).Returns(new List<Procurement>().AsQueryable().BuildMockDbSet().Object);
            ctx.Setup(c => c.Vendors).Returns(new List<Vendor>().AsQueryable().BuildMockDbSet().Object);
            ctx.Setup(c => c.Payments).Returns(new List<Payment>().AsQueryable().BuildMockDbSet().Object);
            ctx.Setup(c => c.Orders).Returns(new List<Order>().AsQueryable().BuildMockDbSet().Object);
            ctx.Setup(c => c.Customers).Returns(new List<Customer>().AsQueryable().BuildMockDbSet().Object);

            for (var i = 1; i <= 5; i++)
            {
                orders.Add(new Order
                {
                    OrderId = i,
                    CustomerId = i,
                    State = "pending",
                    PaymentId = i
                });
                procurements.Add(new Procurement
                {
                    ProcurementId = i,
                    ItemId = i,
                    PaymentId = i,
                    ItemQuantity = i,
                    State = "pending",
                    Type = "bike",
                    VendorId = i
                });
            }

            var transfersModel = new TransfersModel(ctx.Object)
            {
                Orders = orders,
                Procurements = procurements
            };
       
            return transfersModel;
        }

        [Test]
        public void EmptyFilterTransferTableTest()
        {
            List<string> list = new List<string>
            {
                "pending",
                "completed",
                "canceled"
            };

            var orderList = new List<Order>();
            var procurementList = new List<Procurement>();
            var filters = new Filters("order");
            var filters2 = new Filters("procurement");
            filters.Add(new CheckboxFilter("order", "State", "state", list) );
            filters2.Add(new CheckboxFilter("procurement", "State", "state", list) );
            orderList.Add(new Order
            {
                OrderId = 1,
                CustomerId = 1,
                State = "pending",
                PaymentId = 1
            });
            procurementList.Add(new Procurement
            {
                ProcurementId = 1,
                ItemId = 1,
                PaymentId = 1,
                ItemQuantity = 1,
                State = "pending",
                Type = "bike",
                VendorId = 1
            });

            _modelMock.Setup(m => m.Orders).Returns(orderList);
            _modelMock.Setup(i => i.GetOrders());
            _modelMock.Setup(m => m.Procurements).Returns(procurementList);
            _modelMock.Setup(i => i.GetProcurements());

            var controller = new TransfersController(_modelMock.Object, _loggerMock.Object);
            var result = controller.FilterTransferTable(new MobileFiltersInput { Filters = filters, Mobile = false }) as PartialViewResult;
            Assert.IsNotNull(result);
            Assert.AreEqual(1, (result.Model as ITransferService).Orders.Count);
            Assert.AreEqual(1, (result.Model as ITransferService).Procurements.Count);
        }


        [Test]
        public void FilterTransferTableTest()
        {
            List<string> list = new List<string>
            {
                "pending",
                "completed",
                "canceled"
            };

            var orderList = new List<Order>();
            var procurementList = new List<Procurement>();
            var filters = new Filters("order");
            var filters2 = new Filters("procurement");
            filters.Add(new CheckboxFilter("order", "State", "state", list) { Values = { "some_value" } });
            filters2.Add(new CheckboxFilter("procurement", "State", "state", list) { Values = { "some_value" } });

            orderList.Add(new Order
            {
                OrderId = 1,
                CustomerId = 1,
                State = "pending",
                PaymentId = 1
            });
            procurementList.Add(new Procurement
            {
                ProcurementId = 1,
                ItemId = 1,
                PaymentId = 1,
                ItemQuantity = 1,
                State = "pending",
                Type = "bike",
                VendorId = 1
            });

            _modelMock.Setup(m => m.Orders).Returns(orderList);
            _modelMock.Setup(i => i.GetFilteredOrderList(It.IsAny<Filters>()));
            _modelMock.Setup(m => m.Procurements).Returns(procurementList);
            _modelMock.Setup(i => i.GetFilteredProcurementList(It.IsAny<Filters>()));

            var controller = new TransfersController(_modelMock.Object, _loggerMock.Object);
            var result = controller.FilterTransferTable(new MobileFiltersInput { Filters = filters, Mobile = false }) as PartialViewResult;
            Assert.IsNotNull(result);
            Assert.AreEqual(1, (result.Model as ITransferService).Orders.Count);
            Assert.AreEqual(1, (result.Model as ITransferService).Procurements.Count);
        }


        [Test]
        public void FilterTransferTableExceptionTest()
        {
            List<string> list = new List<string>
            {
                "Vendor1",
                "Vendor2",
                "Vendor3"
            };
            var procurementList = new List<Procurement>();
            var filters = new Filters("procurement");
            filters.Add(new SelectFilter("procurement", "Vendor", "vendor", list) { Value = "some_value" });

            procurementList.Add(new Procurement
            {
                ProcurementId = 1,
                ItemId = 1,
                PaymentId = 1,
                ItemQuantity = 1,
                State = "pending",
                Type = "bike",
                VendorId = 1
            });

            _modelMock.Setup(i => i.GetFilteredProcurementList(It.IsAny<Filters>())).Throws(new UnexpectedDataAccessException("some_code"));
            var controller = new TransfersController(_modelMock.Object, _loggerMock.Object)
            {
                TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>())
            };
            Assert.IsNotNull(controller.FilterTransferTable(new MobileFiltersInput { Filters = filters, Mobile = true }) as PartialViewResult);
            Assert.IsNotNull(controller.TempData["errorMessage"]);

        }

        [Test]
        public void RefreshTest()
        {
            _modelMock.Setup(m => m.GetProcurements());
            _modelMock.Setup(m => m.GetOrders());
            _modelMock.Setup(m => m.ResetProcurementFilters());
            _modelMock.Setup(m => m.ResetOrderFilters());

            var controller = new TransfersController(_modelMock.Object, _loggerMock.Object);

            controller.Refresh(new RefreshTabInput { SelectedTab = "procurement", Mobile = true });
            _modelMock.Verify(m => m.GetProcurements(), Times.Once());
            _modelMock.Verify(m => m.ResetProcurementFilters(), Times.Once());
            controller.Refresh(new RefreshTabInput { SelectedTab = "order", Mobile = false });
            _modelMock.Verify(m => m.GetOrders(), Times.Once());
            _modelMock.Verify(m => m.ResetOrderFilters(), Times.Once());
        }
    }
}
