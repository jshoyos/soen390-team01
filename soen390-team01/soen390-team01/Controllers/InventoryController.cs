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
        public IActionResult Index(InventoryModel model)
        {
            model.BikeList = _invService.GetAllBikes();
            model.PartList = _invService.GetAllParts();
            model.MaterialList = _invService.GetAllMaterials();
            return View(model);
        }
        /// <summary>
        ///     Action to add item to inventory
        /// </summary>
        /// <param name="model"></param>

        [HttpPost]
        public IActionResult AddItem(InventoryModel model)
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
        ///     Increments the quantity of an item
        /// </summary>
        /// <param name="model"></param>
        
        public IActionResult Increment(Inventory model)
        {
            _invService.Update(new Inventory
            {
                InventoryId = model.InventoryId,
                ItemId = model.ItemId,
                Quantity = model.Quantity + 1,
                Type = model.Type,
                Warehouse = model.Warehouse
            });
 
            return LocalRedirect("/Inventory");
        }
        /// <summary>
        ///     Decrements the quantity of an item
        /// </summary>
        /// <param name="model"></param>
        [HttpPost]
        public IActionResult Decrement(Inventory model)
        {
            _invService.Update(new Inventory
            {
                InventoryId = model.InventoryId,
                ItemId = model.ItemId,
                Quantity = model.Quantity - 1,
                Type = model.Type,
                Warehouse = model.Warehouse
            });
            return View(model);
        }
    }
}
