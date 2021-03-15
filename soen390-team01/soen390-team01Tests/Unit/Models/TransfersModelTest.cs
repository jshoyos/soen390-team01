using System.Linq;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using soen390_team01.Data;
using soen390_team01.Data.Entities;
using soen390_team01.Data.Exceptions;
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
        public void GetTransfersModelTest()
        {
            Assert.AreEqual(5, _model.Orders.Count);
            Assert.AreEqual(5, _model.Procurements.Count);
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



    }
}
