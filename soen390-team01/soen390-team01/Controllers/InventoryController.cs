using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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
        private readonly IInventoryService _model;
        private readonly ILogger<InventoryController> _log;

        public InventoryController(IInventoryService model, ILogger<InventoryController> log)
        {
            _model = model;
            _log = log;
        }

        [HttpGet]
        public IActionResult Index()
        {
            _model.BikeList = _model.GetAllBikes();
            _model.PartList = _model.GetAllParts();
            _model.MaterialList = _model.GetAllMaterials();
            return View(_model);
        }

        [HttpPost]
        public IActionResult Refresh([FromBody] RefreshTabInput refreshTabInput)
        {
            switch (refreshTabInput.SelectedTab)
            {

                case "inventory":
                    _model.ResetInventories();
                    break;
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
            _model.SelectedTab = refreshTabInput.SelectedTab;

            return PartialView("InventoryBody", _model);
        }

        [HttpPost]
        [FiltersAction]
        public IActionResult FilterProductTable([FromBody] MobileFiltersInput mobileFiltersInput)
        {
            try
            {
                _model.FilterSelectedTab(mobileFiltersInput.Filters);
                if (mobileFiltersInput.Mobile)
                {
                    _model.ShowFilters = true;
                }
            }
            catch (DataAccessException e)
            {
                TempData["errorMessage"] = e.ToString();
            }

            _model.SelectedTab = mobileFiltersInput.Filters.Tab;

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
                    inventory = _model.Update(inventory);
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
            _log.LogInformation($"Updating inventory item {inventory.ItemId} with quantity {inventory.Quantity}");

            return PartialView("InventoryItem", inventory);
        }

        [HttpPost]
        public IActionResult AddBikePart(BikePart addPart)
        {
            BikePart bp = null;

            try
            {
                if (addPart.PartQuantity <= 0 || addPart.PartId <= 0)
                {
                    throw addPart.PartId <= 0 ? new InvalidValueException("Part Id", addPart.PartId.ToString()) :
                     new InvalidValueException("Part Quantity", addPart.PartQuantity.ToString());
                }
                bp = _model.AddBikePart(addPart);
            }
            catch (InvalidValueException e)
            {
                TempData["errorMessage"] = e.ToString();
                Response.StatusCode = 400;
                return Content("Invalid");
            }

            catch (DataAccessException e)
            {
                TempData["errorMessage"] = e.ToString();
                Response.StatusCode = 400;
                return Content("Invalid");
            }

            return PartialView("BikePartListItem", bp);
        }

        [HttpPost]
        public IActionResult RemoveBikePart([FromBody] BikePart removePart)
        {
            try
            {
                _model.RemoveBikePart(removePart);
            }

            catch (DataAccessException e)
            {
                TempData["errorMessage"] = e.ToString();
            }

            return RedirectToAction("Index");
        }
        
        [HttpPost]
        public IActionResult AddPartMaterial(PartMaterial addMat)
        {
            PartMaterial pm = null;

            try
            {
                if (addMat.MaterialQuantity <= 0 || addMat.MaterialId <= 0)
                {
                    throw addMat.MaterialId <= 0 ? new InvalidValueException("Material Id", addMat.MaterialId.ToString()) :
                     new InvalidValueException("Material Quantity", addMat.MaterialQuantity.ToString());
                }
                pm = _model.AddPartMaterial(addMat);
            }
            catch (InvalidValueException e)
            {
                TempData["errorMessage"] = e.ToString();
                Response.StatusCode = 400;
                return Content("Invalid");
            }

            catch (DataAccessException e)
            {
                TempData["errorMessage"] = e.ToString();
                Response.StatusCode = 400;
                return Content("Invalid");
            }

            return PartialView("PartMaterialListItem", pm);
        }

        [HttpPost]
        public IActionResult RemovePartMaterial([FromBody] PartMaterial removeMat)
        {
            try
            {
                _model.RemovePartMaterial(removeMat);
            }

            catch (DataAccessException e)
            {
                TempData["errorMessage"] = e.ToString();
            }

            return RedirectToAction("Index");
        }
    }
}

