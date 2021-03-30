using System;
using Moq;
using NUnit.Framework;
using soen390_team01.Data.Entities;
using soen390_team01.Models;
using soen390_team01.Services;

namespace soen390_team01Tests.Unit.Services
{
    class WebProductionReportGeneratorTest
    {

        [Test]
        public void GenerateTest()
        {
            Environment.SetEnvironmentVariable("Host", "http://some_host");
            var clientMock = new Mock<ProductionClient>();
            var generator = new WebProductionReportGenerator(clientMock.Object);

            var prod = new Production {
                BikeId = 1,
                Quantity = 5
            };

            generator.Generate(prod, "good");

            clientMock.Verify(c => c.SendProduction(It.IsAny<ProcessProductionInput>()));
        }
    }
}
