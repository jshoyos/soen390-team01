using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using soen390_team01.Data.Entities;
using soen390_team01.Data.Exceptions;
using soen390_team01.Data.Queries;
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
        [HttpPost]
        public IActionResult FilterTransferTable([FromBody] Filters filters)
        {
            try
            {
                switch (filters.Table)
                {
                    case "procurement":
                        _model.Procurements = true ? _model.GetFilteredProcurementList(filters) : _model.getProcurements();
                        _model.ProcurementFilters = filters;
                        break;
                    case "order":
                        _model.Orders = filters.AnyActive() ? _model.GetFilteredOrderList(filters) : _model.getOrders();
                        _model.OrderFilters = filters;
                        break;
                }
            }
            catch (DataAccessException e)
            {
                TempData["errorMessage"] = e.ToString();
            }

            _model.SelectedTab = filters.Table;

            return PartialView("Filter", _model);
        }
        [HttpPost]
        public IActionResult Refresh([FromBody] string selectedTab)
        {
            switch (selectedTab)
            {
                case "procurement":
                    _model.Procurements = _model.getProcurements();
                    _model.ProcurementFilters = _model.ResetProcurementFilters();
                    break;
                case "order":
                    _model.Orders = _model.getOrders();
                    _model.OrderFilters = _model.ResetOrderFilters();
                    break;
            }

            _model.SelectedTab = selectedTab;

            return PartialView("Filter", _model);
        }

    }
}
