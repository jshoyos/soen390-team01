using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using soen390_team01.Data;
using soen390_team01.Models;
using System;
using soen390_team01.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using soen390_team01.Data.Entities;

namespace soen390_team01.Controllers
{
    public class InventoryController : Controller
    {
        private readonly InventoryService _invService;
        
        public InventoryController(InventoryService invService)
        {
            _invService = invService;
        }

        [HttpGet]
        public IActionResult Index(InventoryModel model)
        {
            model.BikeList = _invService.GetAllBikes();
            model.PartList = _invService.GetAllParts();
            model.MaterialList = _invService.GetAllMaterials();
            return View(model);
        }

        //[HttpPost]
        //public IActionResult Index(InventoryModel model)
        //{

        //    return View(model);
        //}

      
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
    }
}
