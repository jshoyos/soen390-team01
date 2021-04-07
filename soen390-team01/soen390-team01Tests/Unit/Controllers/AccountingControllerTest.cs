using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using soen390_team01.Controllers;
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
        Mock<ILogger<AccountingController>> _loggerMock;
        Mock<IEmailService> _emailServiceMock;

        [SetUp]
        public void Setup()
        {
            _modelMock = new Mock<IAccountingService>();
            _loggerMock = new Mock<ILogger<AccountingController>>();
            _emailServiceMock = new Mock<IEmailService>();
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

            var controller = new AccountingController(_modelMock.Object, _loggerMock.Object, _emailServiceMock.Object);

            var result = controller.Index() as ViewResult;
            Assert.IsNotNull(result);
            Assert.AreEqual(4, (result.Model as IAccountingService).Payments.Count);
        }

        [Test]
        public void FilterPaymentTableTest()
        {
            var list = new List<string>
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

            var controller = new AccountingController(_modelMock.Object, _loggerMock.Object, _emailServiceMock.Object);
            var result = controller.FilterPaymentTable(new MobileFiltersInput { Filters = filters, Mobile = false}) as PartialViewResult;
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
            var controller = new AccountingController(_modelMock.Object, _loggerMock.Object, _emailServiceMock.Object)
            {
                TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>())
            };
            Assert.IsNotNull(controller.FilterPaymentTable(new MobileFiltersInput { Filters = filters, Mobile = true }) as PartialViewResult);
            Assert.IsNotNull(controller.TempData["errorMessage"]);
        }

        [Test]
        public void RefreshTest()
        {
            _modelMock.Setup(m => m.ResetPayments());
            _modelMock.Setup(m => m.ResetReceivables());
            _modelMock.Setup(m => m.ResetPayables());

            var controller = new AccountingController(_modelMock.Object, _loggerMock.Object, _emailServiceMock.Object);

            controller.Refresh(new RefreshTabInput { SelectedTab = "payment", Mobile = true });
            _modelMock.Verify(m => m.ResetPayments(), Times.Once());
            controller.Refresh(new RefreshTabInput { SelectedTab = "receivable", Mobile = false });
            _modelMock.Verify(m => m.ResetReceivables(), Times.Once());
            controller.Refresh(new RefreshTabInput { SelectedTab = "payable", Mobile = false });
            _modelMock.Verify(m => m.ResetPayables(), Times.Once());
        }

        public void UpdateStatusCompleted()
        {
            ReceivableUpdateModel input = new ReceivableUpdateModel
            {
                Id = 1,
                Status="completed"
                
            };

            var controller = new AccountingController(_modelMock.Object, _loggerMock.Object, _emailServiceMock.Object);

            controller.Update(input);
            _emailServiceMock.Verify(m => m.SendEmail(It.IsAny<string>(), Roles.Accountant), Times.Once);

        }
    }
}
