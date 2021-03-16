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
    [Authorize]
    public class TransfersController : Controller
    {
        private readonly ITransferService _model;

        public TransfersController(ITransferService model)
        {
            _model = model;
        }
        [HttpGet]
        [ModulePermission(Roles = Role.Accountant + "," + Role.SalesRep + "," + Role.Admin)]
        public IActionResult Index()
        {
            return View(_model);
        }

        [HttpPost]
        public IActionResult AddProcurement(TransfersModel model)
        {
            _model.AddProcurement = model.AddProcurement;
            var showModal = false;
            if (ModelState.IsValid)
            {
                try
                {
                    switch (_model.AddProcurement.ItemType)
                    {
                        case "bike": _model.AddProcurements<Bike>(_model.AddProcurement); break;
                        case "part": _model.AddProcurements<Part>(_model.AddProcurement); break;
                        case "material": _model.AddProcurements<Material>(_model.AddProcurement); break;
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

            _model.SelectedTab = "Procurement";
            _model.ShowModal = showModal;

            return View("Index", _model);
        }
        [HttpPost]
        [FiltersAction]
        public IActionResult FilterTransferTable([FromBody] Filters filters)
        {
            try
            {
                switch (filters.Table)
                {
                    case "procurement":
                        _model.Procurements = true ? _model.GetFilteredProcurementList(filters) : _model.GetProcurements();
                        _model.ProcurementFilters = filters;
                        break;
                    case "order":
                        _model.Orders = filters.AnyActive() ? _model.GetFilteredOrderList(filters) : _model.GetOrders();
                        _model.OrderFilters = filters;
                        break;
                }
            }
            catch (DataAccessException e)
            {
                TempData["errorMessage"] = e.ToString();
            }

            _model.SelectedTab = filters.Table;

            return PartialView("TransferBody", _model);
        }
        [HttpPost]
        public IActionResult Refresh([FromBody] string selectedTab)
        {
            switch (selectedTab)
            {
                case "procurement":
                    _model.Procurements = _model.GetProcurements();
                    _model.ProcurementFilters = _model.ResetProcurementFilters();
                    break;
                case "order":
                    _model.Orders = _model.GetOrders();
                    _model.OrderFilters = _model.ResetOrderFilters();
                    break;
            }

            _model.SelectedTab = selectedTab;

            return PartialView("TransferBody", _model);
        }

    }
}
