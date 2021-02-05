using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using soen390_team01.Controllers;
using soen390_team01.Data;
using soen390_team01.Data.Entities;
using soen390_team01.Services;

namespace soen390_team01Tests.Services
{
    public class InventoryControllerTest
    {
        [Test]
        public void GetAllBikesTest()
        {
            var inventoryServiceMock = new Mock<InventoryService>();

            var controller = new InventoryController(inventoryServiceMock.Object);
        }
    }
}
