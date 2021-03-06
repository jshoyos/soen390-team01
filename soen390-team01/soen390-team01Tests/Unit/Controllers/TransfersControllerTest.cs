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
    public class TransfersControllerTest
    {
        [Test]
        public void IndexTest()
        {
            var transfersModel = CreateModel();
            var transfersServiceMock = new Mock<TransfersModel>(new Mock<ErpDbContext>().Object);
            transfersServiceMock.Setup(i => i.SetupModel()).Returns(transfersModel);
            var controller = new TransfersController(transfersServiceMock.Object);

            var result = controller.Index() as ViewResult;
            Assert.IsNotNull(result);
            Assert.AreEqual(5, (result.Model as TransfersModel).Orders.Count);
            Assert.AreEqual(5, (result.Model as TransfersModel).Procurements.Count);
        }

        [Test]
        public void AddProcurementTest()
        {
            var transfersModel = CreateModel();
            var transfersServiceMock = new Mock<TransfersModel>(new Mock<ErpDbContext>().Object);
            transfersServiceMock.Setup(i => i.AddProcurements<Bike>(It.IsAny<AddProcurementModel>())).Returns(new Procurement());
            transfersServiceMock.Setup(i => i.AddProcurements<Part>(It.IsAny<AddProcurementModel>())).Returns(new Procurement());
            transfersServiceMock.Setup(i => i.AddProcurements<Material>(It.IsAny<AddProcurementModel>())).Returns(new Procurement());
            transfersServiceMock.Setup(i => i.SetupModel()).Returns(transfersModel);
            var controller = new TransfersController(transfersServiceMock.Object);

            var inputModel = new TransfersModel {
                AddProcurement = new AddProcurementModel {
                    ItemId = 0, ItemType = "Bike", ItemQuantity = 1, VendorId = 1
                }
            };

            var resultBike = controller.AddProcurement(inputModel) as ViewResult;
            inputModel.AddProcurement.ItemType = "Part";
            var resultPart = controller.AddProcurement(inputModel) as ViewResult;
            inputModel.AddProcurement.ItemType = "Material";
            var resultMaterial = controller.AddProcurement(inputModel) as ViewResult;

            transfersServiceMock.Verify(t => t.AddProcurements<Bike>(It.IsAny<AddProcurementModel>()), Times.Once());
            transfersServiceMock.Verify(t => t.AddProcurements<Part>(It.IsAny<AddProcurementModel>()), Times.Once());
            transfersServiceMock.Verify(t => t.AddProcurements<Material>(It.IsAny<AddProcurementModel>()), Times.Once());

            Assert.AreEqual("Procurement", (resultBike.Model as TransfersModel).SelectedTab);
            Assert.AreEqual(false, (resultBike.Model as TransfersModel).ShowModal);
            Assert.AreEqual("Procurement", (resultBike.Model as TransfersModel).SelectedTab);
            Assert.AreEqual(false, (resultBike.Model as TransfersModel).ShowModal);
            Assert.AreEqual("Procurement", (resultBike.Model as TransfersModel).SelectedTab);
            Assert.AreEqual(false, (resultBike.Model as TransfersModel).ShowModal);
        }

        private static TransfersModel CreateModel()
        {
            var orders = new List<Order>();
            var procurements = new List<Procurement>();

            var transfersModel = new TransfersModel
            {
                Orders = orders,
                Procurements = procurements
            };

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

            return transfersModel;
        }
    }
}
