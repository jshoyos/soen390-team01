using Microsoft.EntityFrameworkCore;
using MockQueryable.Moq;
using Moq;
using Npgsql;
using NUnit.Framework;
using soen390_team01.Data;
using soen390_team01.Data.Entities;
using soen390_team01.Data.Exceptions;
using soen390_team01.Data.Queries;
using soen390_team01.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace soen390_team01Tests.Unit.Models
{
    class AssemblyModelTest
    {
        private ErpDbContext _context;
        private AssemblyModel _model;

        [OneTimeSetUp]
        public void Init()
        {
            var builder = new DbContextOptionsBuilder<ErpDbContext>();
            builder.UseInMemoryDatabase("test_db");
            _context = new ErpDbContext(builder.Options);

            for (var i = 1; i <= 5; i++)
            {
                _context.Productions.Add(new Production
                {
                    BikeId = i,
                    Quantity = i,
                    State = "pending"
                });

                _context.SaveChanges();
            }

            _model = new AssemblyModel(_context);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            foreach (var entity in _context.Productions)
            {
                _context.Productions.Remove(entity);
            }
            _context.SaveChanges();
        }

        [Test]
        public void GetFilteredProductionListInvalidTest()
        {
            List<string> list = new List<string>
            {
                "pending",
                "completed",
                "canceled"
            };

            var ctx = new Mock<ErpDbContext>();
            ctx.Setup(c => c.Productions).Returns(new List<Production>().AsQueryable().BuildMockDbSet().Object);

            var filters = new Filters("production");
            filters.Add(new CheckboxFilter("production", "State", "state", list) { Values = { "filtered_state" } });
            Assert.Throws<UnexpectedDataAccessException>(() => new AssemblyModel(ctx.Object).GetFilteredProductionList(filters));

        }

        [Test]
        public void ResetTest()
        {
            var initialProductionCount = _model.Productions.Count;
            var initialProductionFilterCount = _model.ProductionFilters.List.Count;
            _model.ResetProductionFilters();
            Assert.AreEqual(initialProductionCount, _model.Productions.Count);
            Assert.AreEqual(initialProductionFilterCount, _model.ProductionFilters.List.Count);
        }

        [Test]
        public void AddNewBikeTest()
        {
            var initialProductionCount = _model.Productions.Count;
            var bikeOrder = new BikeOrder
            {
                BikeId = 1,
                ItemQuantity = 1,
            };

            _model.AddNewBike(bikeOrder);
            Assert.AreEqual(initialProductionCount + 1, _model.Productions.Count);
        }

        [Test]
        public void AddNewBikeInvalidTest()
        {
            var ctx = new Mock<ErpDbContext>();
            var nbProductionCall = 0;
            ctx.Setup(c => c.Productions).Returns(new List<Production>().AsQueryable().BuildMockDbSet().Object).Callback(() =>
            {
                nbProductionCall++;
                if (nbProductionCall == 1)
                    throw new DbUpdateException("error", new PostgresException("", "", "", ""));
            });
            Assert.Throws<DbUpdateException>(() => new AssemblyModel(ctx.Object).AddNewBike(new BikeOrder()));
        }
    }
}
