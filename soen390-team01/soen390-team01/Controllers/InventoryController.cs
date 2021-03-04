using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using soen390_team01.Data.Entities;
using soen390_team01.Data.Exceptions;
using soen390_team01.Models;
using soen390_team01.Services;

namespace soen390_team01.Controllers
{
    [Authorize]
    public class InventoryController : Controller
    {
        private readonly InventoryService _invService;

        public InventoryController(InventoryService invService)
        {
            _invService = invService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View(_invService.SetupModel());
        }

        [HttpPost]
        [ModulePermissionAttribute(Roles = Role.InventoryManager)]
        public IActionResult Refresh([FromBody] string selectedTab)
        {
            var model = _invService.SetupModel();
            model.SelectedTab = selectedTab;

            return PartialView("InventoryBody",model);
        }

        [HttpPost]
        public IActionResult FilterProductTable([FromBody] ProductFilterInput input)
        {
            var isFilterEmpty = input.Value.Equals("clear");
            {
                try
                {
                    switch (input.Type)
                    {
                        case "Bike": return PartialView("BikeTable", isFilterEmpty ? _invService.GetAllBikes() : _invService.GetFilteredProductList<Bike>(input));
                        case "Part": return PartialView("PartTable", isFilterEmpty ? _invService.GetAllParts() : _invService.GetFilteredProductList<Part>(input));
                        case "Material": return PartialView("MaterialTable", isFilterEmpty ? _invService.GetAllMaterials() : _invService.GetFilteredProductList<Material>(input));
                    }
                }
                catch (DataAccessException e)
                {
                    TempData["errorMessage"] = e.ToString();
                }
            }    
            return Index();
        }
        /// <summary>
        /// Changes the quantity of an item
        /// </summary>
        /// <param name="inventory">updated inventory item</param>
        [HttpPost]
        [ModulePermissionAttribute(Roles = Role.InventoryManager)]
        public IActionResult ChangeQuantity([FromBody] Inventory inventory)
        {
            if (inventory.Quantity >= 0)
            {
                try
                {
                    _invService.Update(inventory);
                }
                catch (DataAccessException e)
                {
                    TempData["errorMessage"] = e.ToString();
                }
            }
            else
            {
                inventory.Quantity = 0;
            }
            
            return PartialView("InventoryItem", inventory);
        }
    }
}
