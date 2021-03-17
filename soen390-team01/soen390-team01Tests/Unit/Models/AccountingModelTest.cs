
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using soen390_team01.Data;
using soen390_team01.Data.Entities;
using System.Linq;
using MockQueryable.Moq;
using Moq;
using Npgsql;
using soen390_team01.Data.Exceptions;
using soen390_team01.Data.Queries;
using soen390_team01.Models;

namespace soen390_team01Tests.Services
{
    class AccountingModelTest
    {
        private ErpDbContext _context;
        private AccountingModel _model;

        [OneTimeSetUp]
        public void Init()
        {
            var builder = new DbContextOptionsBuilder<ErpDbContext>();
            builder.UseInMemoryDatabase("test_db");
            _context = new ErpDbContext(builder.Options);

            for (var i = 1; i <= 3; i++)
            {
                _context.Payments.Add(new Payment
                {
                    PaymentId = i,
                    Amount = i,
                    State = "pending"
                });
            }
            _context.Payments.Add(new Payment
            {
                PaymentId = 4,
                Amount = -200,
                State = "canceled"
            });

            _context.SaveChanges();

            _model = new AccountingModel(_context);
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            foreach (var entity in _context.Payments)
            {
                _context.Payments.Remove(entity);
            }
            _context.SaveChanges();
        }


        [Test]
        public void FilterSelectedTabInvalidTest()
        {
            List<string> list = new List<string>
            {
                "pending",
                "completed",
                "canceled"
            };
            var ctx = new Mock<ErpDbContext>();
            ctx.Setup(c => c.Payments).Returns(new List<Payment>().AsQueryable().BuildMockDbSet().Object);

            var filters = new Filters("payment");
            var tabName = "payment";
            filters.Add(new CheckboxFilter("payment", $"State-{tabName}", "state", list) { Values = { "filtered_state" } });
            Assert.Throws<UnexpectedDataAccessException>(() => new AccountingModel(ctx.Object).FilterSelectedTab(filters));
        }

        [Test]
        public void FilterSelectedTabTest()
        {
            _model.
        }

        [Test]
        public void ResetPaymentsTest()
        {
            var initialPaymentCount = _model.Payments.Count;
            _model.ResetPayments();
            Assert.AreEqual(initialPaymentCount, _model.Payments.Count);


            var initialReceivableCount = _model.Receivables.Count;
            _model.ResetReceivables();
            Assert.AreEqual(initialReceivableCount, _model.Receivables.Count);

            var initialPayableCount = _model.Payables.Count;
            _model.ResetPayables();
            Assert.AreEqual(initialPayableCount, _model.Payables.Count);
        }

        [Test]
        public void ResetFiltersTest()
        {
            var initialPaymentFilterCount = _model.PaymentFilters.List.Count;
            _model.ResetPayments();
            Assert.AreEqual(initialPaymentFilterCount, _model.PaymentFilters.List.Count);

            var initialReceivableFilterCount = _model.ReceivableFilters.List.Count;
            _model.ResetReceivables();
            Assert.AreEqual(initialReceivableFilterCount, _model.ReceivableFilters.List.Count);

            var initialPayableFilterCount = _model.PayableFilters.List.Count;
            _model.ResetPayables();
            Assert.AreEqual(initialPayableFilterCount, _model.PayableFilters.List.Count);
        }
    }
}
