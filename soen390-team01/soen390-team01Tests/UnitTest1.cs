using Microsoft.Extensions.Logging;
using NUnit.Framework;
using soen390_team01.Controllers;

namespace soen390_team01Tests
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Test1()
        {
            HomeController homeController = new HomeController();
            homeController.Index();
            homeController.Privacy();
            Assert.Pass();
        }
    }
}