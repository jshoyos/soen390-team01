using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using soen390_team01.Data.Entities;
using soen390_team01.Data.Exceptions;
using soen390_team01.Data.Queries;
using soen390_team01.Models;
using soen390_team01.Services;

namespace soen390_team01.Controllers
{
    [Authorize]
    public class InventoryController : Controller
    {
        private readonly InventoryService _invService;
        private readonly InventoryModel _model;

        public InventoryController(InventoryService invService)
        {
            _invService = invService;
            _model = _invService.Model;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View(_model);
        }

        [HttpPost]
        [ModulePermission(Roles = Role.InventoryManager)]
        public IActionResult Refresh([FromBody] string selectedTab)
        {
            switch (selectedTab)
            {
                case "bike":
                    _model.BikeList = _invService.GetAllBikes();
                    _model.BikeFilters = _invService.ResetBikeFilters();
                    break;
                case "part":
                    _model.PartList = _invService.GetAllParts();
                    _model.PartFilters = _invService.ResetPartFilters();
                    break;
                case "material":
                    _model.MaterialList = _invService.GetAllMaterials();
                    _model.MaterialFilters = _invService.ResetMaterialFilters();
                    break;
            }
            _model.SelectedTab = selectedTab;
            // Workaround until we put logic in models
            _invService.Model = _model;
            return PartialView("InventoryBody", _model);
        }

        [HttpPost]
        [FiltersAction]
        public IActionResult FilterProductTable([FromBody] Filters filters)
        {
            try
            {
                switch (filters.Table)
                {
                    case "bike":
                        _model.BikeList = filters.AnyActive() ? _invService.GetFilteredProductList<Bike>(filters) : _invService.GetAllBikes();
                        _model.BikeFilters = filters;
                        break;
                    case "part":
                        _model.PartList = filters.AnyActive() ? _invService.GetFilteredProductList<Part>(filters) : _invService.GetAllParts();
                        _model.PartFilters = filters;
                        break;
                    case "material":
                        _model.MaterialList = filters.AnyActive() ? _invService.GetFilteredProductList<Material>(filters) : _invService.GetAllMaterials();
                        _model.MaterialFilters = filters;
                        break;
                }
            }
            catch (DataAccessException e)
            {
                TempData["errorMessage"] = e.ToString();
            }

            _model.SelectedTab = filters.Table;
            // Workaround until we put logic in models
            _invService.Model = _model;

            return PartialView("InventoryBody", _model);
        }

        /// <summary>
        /// Changes the quantity of an item
        /// </summary>
        /// <param name="inventory">updated inventory item</param>
        [HttpPost]
        [ModulePermission(Roles = Role.InventoryManager)]
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
