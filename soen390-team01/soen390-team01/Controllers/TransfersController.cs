using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using soen390_team01.Data.Entities;
using soen390_team01.Data.Exceptions;
using soen390_team01.Models;
using soen390_team01.Services;
using System.Linq;

namespace soen390_team01.Controllers
{
    public class TransfersController : Controller
    {
        private readonly TransfersModel _model;

        public TransfersController(TransfersModel model)
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
        public IActionResult AddProcurement(TransfersModel model)
        {
            var showModal = false;
            if (ModelState.IsValid)
            {
                try
                {
                    switch (model.AddProcurement.ItemType)
                    {
                        case "Bike": _model.AddProcurements<Bike>(model.AddProcurement); break;
                        case "Part": _model.AddProcurements<Part>(model.AddProcurement); break;
                        case "Material": _model.AddProcurements<Material>(model.AddProcurement); break;
                    }
                }
                catch (DataAccessException e)
                {
                    TempData["errorMessage"] = e.ToString();
                }
            }
            else
            {
                showModal = true;
            }

            model = _model.SetupModel();
            model.SelectedTab = "Procurement";
            model.ShowModal = showModal;

            return View("Index", model);
        }
        //[HttpPost]
        //public IActionResult FilterTransfers([FromBody] TransferFilter input)
        //{
        //    var isFilterEmpty = input.OrderStatus.Count == 0 && input.ProcurementStatus.Count == 0 && input.Vendor.Equals("clear");
        //    {
        //        try
        //        {
        //            return PartialView("Filter", isFilterEmpty ? _model.SetupModel() : _model.GetFilteredTransferModel(input));
        //        }
        //        catch (DataAccessException e)
        //        {
        //            TempData["errorMessage"] = e.ToString();
        //        }
        //    }
        //    return Index();
        //}

        [HttpPost]
        public IActionResult Refresh([FromBody] string selectedTab)
        {
            switch (selectedTab)
            {
                case "Order": _model.Orders= _model.getOrders(); break;
                case "Procurement": _model.Procurements = _model.getProcurements();  break;
            }
            _model.SelectedTab = selectedTab;

            return PartialView("InventoryBody", _model);
        }
       
    }
}
