using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using soen390_team01.Data.Entities;
using soen390_team01.Data.Exceptions;
using soen390_team01.Data.Queries;
using soen390_team01.Models;
using soen390_team01.Services;

namespace soen390_team01.Controllers
{
    public class InventoryController : Controller
    {
        private readonly InventoryService _invService;
        private InventoryModel _model;

        public InventoryController(InventoryService invService)
        {
            _invService = invService;
            _model = _invService.SetupModel();
        }

        [Authorize]
        [HttpGet]
        public IActionResult Index()
        {
            return View("Index", _model);
        }
        [HttpPost]
        public IActionResult Refresh([FromBody] string selectedTab)
        {
            _model = _invService.SetupModel();
            _model.SelectedTab = selectedTab;

            return PartialView("InventoryBody", _model);
        }

        [HttpPost]
        [FiltersAction]
        public IActionResult FilterProductTable([FromBody] Filters filters)
        {
            if (filters.AnyActive())
            {
                try
                {
                    switch (filters.Table)
                    {
                        case "Bike":
                            _model.BikeList = _invService.GetFilteredProductList<Bike>(filters);
                            _model.BikeFilters = filters;
                            break;
                        case "Part":
                            _model.PartList = _invService.GetFilteredProductList<Part>(filters);
                            _model.PartFilters = filters;
                            break;

                        case "Material":
                            _model.MaterialList = _invService.GetFilteredProductList<Material>(filters);
                            _model.MaterialFilters = filters;
                            break;
                    }

                }
                catch (DataAccessException e)
                {
                    TempData["errorMessage"] = e.ToString();
                }
            }

            _model.SelectedTab = filters.Table;

            return PartialView("InventoryBody", _model);
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
