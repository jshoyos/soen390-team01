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
            return View(_invService.SetupModel());
        }
        [HttpPost]
        public IActionResult Refresh([FromBody] string selectedTab)
        {
            var model = _invService.SetupModel();
            model.SelectedTab = selectedTab;

            return PartialView("InventoryBody",model);
        }

        [HttpPost]
        public IActionResult FilterProductTable([FromBody] ProductFilterInput input)
        {
            bool isFilterEmpty = input.Value.Equals("clear");
            {
                switch (input.Type)
                {
                    case "Bike": return PartialView("BikeTable", isFilterEmpty ? _invService.GetAllBikes() : _invService.GetFilteredProductList<Bike>(input));
                    case "Part": return PartialView("PartTable", isFilterEmpty ? _invService.GetAllParts() : _invService.GetFilteredProductList<Part>(input));
                    case "Material": return PartialView("MaterialTable", isFilterEmpty ? _invService.GetAllMaterials() : _invService.GetFilteredProductList<Material>(input));
                }
            }    
            return View();
        }
        /// <summary>
        /// Changes the quantity of an item
        /// </summary>
        /// <param name="inventory">updated inventory item</param>
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
