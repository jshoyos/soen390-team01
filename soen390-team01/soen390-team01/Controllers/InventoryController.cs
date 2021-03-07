using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using soen390_team01.Data.Entities;
using soen390_team01.Data.Exceptions;
using soen390_team01.Data.Queries;
using soen390_team01.Services;

namespace soen390_team01.Controllers
{
    public class InventoryController : Controller
    {
        private readonly IInventoryService _model;

        public InventoryController(IInventoryService model)
        {
            _model = model;
        }

        [Authorize]
        [HttpGet]
        public IActionResult Index()
        {
            return View(_model);
        }

        [HttpPost]
        public IActionResult Refresh([FromBody] string selectedTab)
        {
            switch (selectedTab)
            {
                case "bike":
                    _model.ResetBikes();
                    break;
                case "part":
                    _model.ResetParts();
                    break;
                case "material":
                    _model.ResetMaterials();
                    break;
            }

            _model.SelectedTab = selectedTab;

            return PartialView("InventoryBody", _model);
        }

        [HttpPost]
        [FiltersAction]
        public IActionResult FilterProductTable([FromBody] Filters filters)
        {
            try
            {
                _model.FilterSelectedTab(filters);
            }
            catch (DataAccessException e)
            {
                TempData["errorMessage"] = e.ToString();
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
                    _model.Update(inventory);
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
