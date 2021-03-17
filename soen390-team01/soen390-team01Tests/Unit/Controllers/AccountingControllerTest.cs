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
using PartialViewResult = Microsoft.AspNetCore.Mvc.PartialViewResult;
using ViewResult = Microsoft.AspNetCore.Mvc.ViewResult;

namespace soen390_team01TestsControllers
{
    class AccountingControllerTest
    {
        Mock<IAccountingService> _modelMock;

        [SetUp]
        public void Setup()
        {
            _modelMock = new Mock<IAccountingService>();
        }

        [Test]
        public void IndexTest()
        {
            var paymentList = new List<Payment>();
            for (var i = 1; i <= 3; i++)
            {
                paymentList.Add(new Payment
                {
                    PaymentId = i,
                    Amount = i,
                    State = "pending"
                });
            }
            paymentList.Add(new Payment
            {
                PaymentId = 4,
                Amount = -200,
                State = "canceled"
            });

            _modelMock.Setup(m => m.Payments).Returns(paymentList);

            var controller = new AccountingController(_modelMock.Object);

            var result = controller.Index() as ViewResult;
            Assert.IsNotNull(result);
            Assert.AreEqual(4, (result.Model as IAccountingService).Payments.Count);
        }

        [Test]
        public void FilterPaymentTableTest()
        {
            List<string> list = new List<string>
            {
                "pending",
                "completed",
                "canceled"
            };
            var paymentList = new List<Payment>();
            var filters = new Filters("payment");
            var tabName = "payment";
            filters.Add(new CheckboxFilter("payment", $"State-{tabName}", "state", list) { Values = { "filtered_state" } });

            paymentList.Add(new Payment
            {
                PaymentId = 10,
                Amount = -400,
                State = "pending"
            });

            _modelMock.Setup(m => m.Payments).Returns(paymentList);
            _modelMock.Setup(i => i.FilterSelectedTab(It.IsAny<Filters>()));

            var controller = new AccountingController(_modelMock.Object);
            var result = controller.FilterPaymentTable(filters) as PartialViewResult;
            Assert.IsNotNull(result);
            Assert.AreEqual(1, (result.Model as IAccountingService).Payments.Count);

        }

        [Test]
        public void FilterPaymentTableExceptionTest()
        {
            var paymentList = new List<Payment>();
            var filters = new Filters("payment");
            var tabName = "payment";
            filters.Add(new RangeFilter("payment", $"Amount-{tabName}", "amount"));

            paymentList.Add(new Payment
            {
                PaymentId = 13,
                Amount = 400,
                State = "canceled"
            });

            _modelMock.Setup(i => i.FilterSelectedTab(It.IsAny<Filters>())).Throws(new UnexpectedDataAccessException("some_code"));
            var controller = new AccountingController(_modelMock.Object)
            {
                TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>())
            };
            Assert.IsNotNull(controller.FilterPaymentTable(filters) as PartialViewResult);
            Assert.IsNotNull(controller.TempData["errorMessage"]);
        }

        [Test]
        public void ChangeTabTest()
        {
            var newTab = "payment";
            var controller = new AccountingController(_modelMock.Object);
            var result = controller.ChangeTab(newTab) as PartialViewResult;
            Assert.IsNotNull(result);
        }

        [Test]
        public void RefreshTest()
        {
            _modelMock.Setup(m => m.ResetPayments());
            _modelMock.Setup(m => m.ResetReceivables());
            _modelMock.Setup(m => m.ResetPayables());

            var controller = new AccountingController(_modelMock.Object);

            controller.Refresh("payment");
            _modelMock.Verify(m => m.ResetPayments(), Times.Once());
            controller.Refresh("receivable");
            _modelMock.Verify(m => m.ResetReceivables(), Times.Once());
            controller.Refresh("payable");
            _modelMock.Verify(m => m.ResetPayables(), Times.Once());
        }









    }
}
