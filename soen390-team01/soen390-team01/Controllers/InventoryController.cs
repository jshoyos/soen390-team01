using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using soen390_team01.Data.Entities;
using soen390_team01.Models;
using soen390_team01.Services;

namespace soen390_team01.Controllers
{
    public class InventoryController : Controller
    {
        private readonly InventoryService _invService;

        public InventoryController(InventoryService invService)
        {
            _invService = invService;
        }
        [Authorize]
        [HttpGet]
        public IActionResult Index()
        {
            return View(_invService.GetInventoryModel());
        }
        /// <summary>
        ///     Action to add item to inventory
        /// </summary>
        /// <param name="model"></param>

        [HttpPost]
        public IActionResult AddItem([FromBody] InventoryModel model)
        {
            _invService.AddItem(new Inventory
            {
                InventoryId = model.InventoryId,
                ItemId = model.ItemId,
                Quantity = model.Quantity,
                Type = model.Type,
                Warehouse = model.Warehouse
            });
            return View(model);
        }
        /// <summary>
        ///     Changes the quantity of an item
        /// </summary>
        /// <param name="inventory"></param>
        [HttpPost]
        public IActionResult ChangeQuantity([FromBody] Inventory inventory)
        {
            if (inventory.Quantity >= 0)
            {
                _invService.Update(inventory);
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Quantity below 0");
            }

            return PartialView("InventoryItem", inventory);
        }
    }
}
