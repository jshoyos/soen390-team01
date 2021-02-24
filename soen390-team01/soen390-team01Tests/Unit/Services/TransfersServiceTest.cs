﻿using System.Linq;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using soen390_team01.Data;
using soen390_team01.Data.Entities;
using soen390_team01.Services;

namespace soen390_team01Tests.Services
{
    public class TransfersServiceTest
    {

        private ErpDbContext _context;
        private TransfersService _transfersService;

        [OneTimeSetUp]
        public void Init()
        {
            var builder = new DbContextOptionsBuilder<ErpDbContext>();
            builder.UseInMemoryDatabase("test_db");
            _context = new ErpDbContext(builder.Options);

            for (var i = 1; i <= 5; i++)
            {
                _context.Orders.Add(new Order
                {
                    OrderId = i,
                    CustomerId = i,
                    State = "pending",
                    PaymentId = i
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

            _transfersService = new TransfersService(_context);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            foreach (var entity in _context.Orders)
            {
                _context.Orders.Remove(entity);
            }
            foreach (var entity in _context.Procurements)
            {
                _context.Procurements.Remove(entity);
            }
            _context.SaveChanges();
        }

        [Test]
        public void GetTransfersModelTest()
        {
            var transfersModel = _transfersService.GetTransfersModel();
            Assert.AreEqual(5, transfersModel.Orders.Count);
            Assert.AreEqual(5, transfersModel.Procurements.Count);
        }

        [Test]
        public void ChangeTransferStateValidTest()
        {
            var transfersModel = _transfersService.GetTransfersModel();

            var orderToChange = transfersModel.Orders.ElementAt(0);

            var changedOrder = _transfersService.ChangeOrderState(orderToChange.OrderId, "completed");

            Assert.NotNull(changedOrder);
            Assert.AreEqual("completed", changedOrder.State);

            var procurementToChange = transfersModel.Procurements.ElementAt(0);
            var changedProcurement = _transfersService.ChangeProcurementState(procurementToChange.ProcurementId, "completed");

            Assert.NotNull(changedProcurement);
            Assert.AreEqual("completed", changedProcurement.State);
        }

        [Test]
        public void ChangeTransferStateInvalidTest()
        {
            var transfersModel = _transfersService.GetTransfersModel();

            var orderToChange = transfersModel.Orders.ElementAt(0);

            Assert.Null(_transfersService.ChangeOrderState(orderToChange.OrderId, "invalid_state"));

            var procurementToChange = transfersModel.Procurements.ElementAt(0);
            Assert.Null(_transfersService.ChangeProcurementState(procurementToChange.ProcurementId, "invalid_state"));
        }
    }
}