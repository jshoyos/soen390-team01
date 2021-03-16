using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using MockQueryable.Moq;
using Moq;
using NUnit.Framework;
using soen390_team01.Data;
using soen390_team01.Data.Entities;
using soen390_team01.Data.Exceptions;
using soen390_team01.Data.Queries;
using soen390_team01.Models;

namespace soen390_team01Tests.Services
{
    public class TransfersModelTest
    {

        private ErpDbContext _context;
        private TransfersModel _model;

        [OneTimeSetUp]
        public void Init()
        {
            var builder = new DbContextOptionsBuilder<ErpDbContext>();
            builder.UseInMemoryDatabase("test_db");
            _context = new ErpDbContext(builder.Options);

            for (var i = 1; i <= 5; i++)
            {
                _context.Payments.Add(new Payment
                {
                    PaymentId = i,
                    Amount = i,
                    State = "pending"
                });
                _context.Customers.Add(new Customer
                {
                    CustomerId = i,
                    Name = "name" + i,
                    Address = "address" + i,
                    PhoneNumber = "1234567890"
                });
                _context.OrderItems.Add(new OrderItem
                {
                    OrderId = i,
                    ItemId = i,
                    ItemQuantity = i,
                    Type = "bike"
                });
                _context.Orders.Add(new Order
                {
                    OrderId = i,
                    CustomerId = i,
                    State = "pending",
                    PaymentId = i
                });
                _context.Vendors.Add(new Vendor
                {
                    VendorId = i,
                    Name = "name" + i,
                    Address = "address" + i,
                    PhoneNumber = "1234567890"
                });
                _context.Procurements.Add(new Procurement
                {
                    ProcurementId = i,
                    ItemId = i,
                    PaymentId = i,
                    ItemQuantity = i,
                    State = "pending",
                    Type = "bike",
                    VendorId = i
                });
                
                _context.SaveChanges();
            }

            _model = new TransfersModel(_context);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            foreach (var entity in _context.OrderItems)
            {
                _context.OrderItems.Remove(entity);
            }
            foreach (var entity in _context.Orders)
            {
                _context.Orders.Remove(entity);
            }
            foreach (var entity in _context.Procurements)
            {
                _context.Procurements.Remove(entity);
            }
            foreach (var entity in _context.Customers)
            {
                _context.Customers.Remove(entity);
            }
            foreach (var entity in _context.Vendors)
            {
                _context.Vendors.Remove(entity);
            }
            foreach (var entity in _context.Payments)
            {
                _context.Payments.Remove(entity);
            }
            _context.SaveChanges();
        }

        [Test]
        public void GetFilteredListInvalidTest()
        {
            List<string> list = new List<string>
            {
                "Vendor1",
                "Vendor2",
                "Vendor3"
            };
            var ctx = new Mock<ErpDbContext>();
            ctx.Setup(c => c.Procurements).Returns(new List<Procurement>().AsQueryable().BuildMockDbSet().Object);
            ctx.Setup(c => c.Vendors).Returns(new List<Vendor>().AsQueryable().BuildMockDbSet().Object);
            ctx.Setup(c => c.Payments).Returns(new List<Payment>().AsQueryable().BuildMockDbSet().Object);
            ctx.Setup(c => c.Orders).Returns(new List<Order>().AsQueryable().BuildMockDbSet().Object);
            ctx.Setup(c => c.Customers).Returns(new List<Customer>().AsQueryable().BuildMockDbSet().Object);

            var filters = new Filters("procurement");
            filters.Add(new SelectFilter("procurement", "Vendor", "vendor", list) { Value = "filtered_vendor" });
            Assert.Throws<UnexpectedDataAccessException>(() => new TransfersModel(ctx.Object).GetFilteredProcurementList(filters));

        }

        [Test]
        public void GetFilteredOrderListInvalidTest()
        {
            List<string> list = new List<string>
            {
                "pending",
                "completed",
                "canceled"
            };
            var ctx = new Mock<ErpDbContext>();
            ctx.Setup(c => c.Procurements).Returns(new List<Procurement>().AsQueryable().BuildMockDbSet().Object);
            ctx.Setup(c => c.Vendors).Returns(new List<Vendor>().AsQueryable().BuildMockDbSet().Object);
            ctx.Setup(c => c.Payments).Returns(new List<Payment>().AsQueryable().BuildMockDbSet().Object);
            ctx.Setup(c => c.Orders).Returns(new List<Order>().AsQueryable().BuildMockDbSet().Object);
            ctx.Setup(c => c.Customers).Returns(new List<Customer>().AsQueryable().BuildMockDbSet().Object);

            var filters = new Filters("order");
            filters.Add(new CheckboxFilter("order", "State", "state", list) { Values = { "filtered_state" } });
            Assert.Throws<UnexpectedDataAccessException>(() => new TransfersModel(ctx.Object).GetFilteredOrderList(filters));
        }

        [Test]
        public void ChangeTransferStateValidTest()
        {
            var orderToChange = _model.Orders.ElementAt(0);

            var changedOrder = _model.ChangeOrderState(orderToChange.OrderId, "completed");

            Assert.NotNull(changedOrder);
            Assert.AreEqual("completed", changedOrder.State);

            var procurementToChange = _model.Procurements.ElementAt(0);
            var changedProcurement = _model.ChangeProcurementState(procurementToChange.ProcurementId, "completed");

            Assert.NotNull(changedProcurement);
            Assert.AreEqual("completed", changedProcurement.State);
        }

        [Test]
        public void ChangeTransferStateInvalidTest()
        {
            const int INVALID_ID = 12345;
            var orderToChange = _model.Orders.ElementAt(0);
            Assert.Throws<InvalidValueException>(() => _model.ChangeOrderState(orderToChange.OrderId, "invalid_state"));
            Assert.Throws<NotFoundException>(() => _model.ChangeOrderState(INVALID_ID, "pending"));

            var procurementToChange = _model.Procurements.ElementAt(0);
            Assert.Throws<InvalidValueException>(() => _model.ChangeProcurementState(procurementToChange.ProcurementId, "invalid_state"));
            Assert.Throws<NotFoundException>(() => _model.ChangeProcurementState(INVALID_ID, "pending"));
        }

        [Test]
        public void ResetTest()
        {
            var initialProcurementCount = _model.Procurements.Count;
            var initialProcurementFilterCount = _model.ProcurementFilters.List.Count;
            _model.ResetProcurementFilters();
            Assert.AreEqual(initialProcurementCount, _model.Procurements.Count);
            Assert.AreEqual(initialProcurementFilterCount, _model.ProcurementFilters.List.Count);

            var initialOrderCount = _model.Orders.Count;
            var initialOrderFilterCount = _model.OrderFilters.List.Count;
            _model.ResetOrderFilters();
            Assert.AreEqual(initialOrderCount, _model.Orders.Count);
            Assert.AreEqual(initialOrderFilterCount, _model.OrderFilters.List.Count);

        }
    }
}
