using System;
using System.IO;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using soen390_team01.Data.Entities;
using soen390_team01.Services;

namespace soen390_team01Tests.Unit.Services
{
    class CsvProductionReportGeneratorTest
    {

        [OneTimeSetUp]
        public void Init()
        {
            Directory.CreateDirectory("productions");
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            var di = new DirectoryInfo("productions");

            foreach (var file in di.GetFiles())
            {
                file.Delete();
            }
            Directory.Delete("productions");
        }

        [Test, Order(1)]
        public void GenerateNoExistingFileTest()
        {
            var production = new Production
            {
                ProductionId = 5,
                BikeId = 2,
                State = "inProgress",
                Quantity = 10,
                Added = new DateTime(2021, 1,1),
                Modified = new DateTime(2021, 1,2)
            };

            var quality = "good";
            var generator = new CsvProductionReportGenerator(new Mock<ILogger<CsvProductionReportGenerator>>().Object);

            generator.Generate(production, quality);

            var path = $"productions/{production.ProductionId}.csv";
            Assert.True(File.Exists(path));
        }

        [Test, Order(2)]
        public void GenerateAlreadyExistingFileTest()
        {
            var production = new Production
            {
                ProductionId = 5,
                BikeId = 2,
                State = "inProgress",
                Quantity = 10,
                Added = new DateTime(2021, 1, 1),
                Modified = new DateTime(2021, 1, 2)
            };

            var quality = "good";
            var generator = new CsvProductionReportGenerator(new Mock<ILogger<CsvProductionReportGenerator>>().Object);

            generator.Generate(production, quality);

            var path = $"productions/{production.ProductionId}.csv";
            Assert.True(File.Exists(path));
        }
    }
}
