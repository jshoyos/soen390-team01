﻿using Microsoft.AspNetCore.Authorization;
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
            Bike bike = null;
            try
            {
               bike = _model.AddBikePart(addPart);
            }

            catch (DataAccessException e)
            {
                TempData["errorMessage"] = e.ToString();
            }

            if(bike == null)
            {
                return RedirectToAction("Index");
            }
            return PartialView("BikePartList", bike);
        }

        [HttpPost]
        public IActionResult RemoveBikePart([FromBody] BikePart removePart)
        {
            Bike bike = null;
            try
            {
                bike = _model.RemoveBikePart(removePart);
            }

            catch (DataAccessException e)
            {
                TempData["errorMessage"] = e.ToString();
            }

            if (bike == null)
            {
                return RedirectToAction("Index");
            }
            return PartialView("BikePartList", bike);
        }
    }
}

