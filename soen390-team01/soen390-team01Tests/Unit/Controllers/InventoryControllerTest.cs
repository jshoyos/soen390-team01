using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using soen390_team01.Controllers;
using soen390_team01.Data;
using soen390_team01.Data.Entities;
using soen390_team01.Models;
using soen390_team01.Services;

namespace soen390_team01Tests.Controllers
{
    public class InventoryControllerTest
    {




        [Test]
        public void IndexTest()
        { 
            var allList = new List<Inventory>();
            for (var i = 1; i <= 9; i++)
            {
                var type = (9 % i) switch
                {
                    0 => "bike",
                    1 => "part",
                    _ => "material"
                };
                allList.Add(new Inventory
                    {
                        ItemId = i,
                        InventoryId = i,
                        Quantity = i,
                        Type = type,
                        Warehouse = "Warehouse " + i
                    }
                );
            }

            var inventoryModel = new InventoryModel
            {
                AllList = allList
            };
            var context = new Mock<ErpDbContext>();
            var inventoryServiceMock = new Mock<InventoryService>(context.Object);
            inventoryServiceMock.Setup(i => i.GetInventoryModel()).Returns(inventoryModel);
            var controller = new InventoryController(inventoryServiceMock.Object);

            var result = controller.Index() as ViewResult;
            Assert.IsNotNull(result);
            Assert.AreEqual(9, (result.Model as InventoryModel).AllList.Count);
        }

        [Test]
        public void ChangeQuantityTest()
        {
            var inventory = new Inventory
            {
                ItemId = 1,
                InventoryId = 1,
                Quantity = 1,
                Type = "bike",
                Warehouse = "Warehouse 1"
            };
            var context = new Mock<ErpDbContext>();
            var inventoryServiceMock = new Mock<InventoryService>(context.Object);
            inventoryServiceMock.SetupAllProperties();
            var controller = new InventoryController(inventoryServiceMock.Object);

            var result = controller.ChangeQuantity(inventory) as PartialViewResult;
            Assert.IsNotNull(result);
            Assert.AreEqual(inventory, result.Model);
        }
    }
}
