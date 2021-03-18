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
            _model.ShowFilters = false;
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

            _model.SelectedTab = "procurement";
            _model.ShowModal = showModal;

            return View("Index", _model);
        }
        [HttpPost]
        [FiltersAction]
        public IActionResult FilterTransferTable([FromBody] MobileFiltersInput mobileFiltersInput)
        {
            try
            {
                if (mobileFiltersInput.Mobile)
                {
                    _model.ShowFilters = true;
                }
                switch (mobileFiltersInput.Filters.Table)
                {
                    case "procurement":
                        _model.Procurements = true ? _model.GetFilteredProcurementList(mobileFiltersInput.Filters) : _model.GetProcurements();
                        _model.ProcurementFilters = mobileFiltersInput.Filters;
                        break;
                    case "order":
                        _model.Orders = mobileFiltersInput.Filters.AnyActive() ? _model.GetFilteredOrderList(mobileFiltersInput.Filters) : _model.GetOrders();
                        _model.OrderFilters = mobileFiltersInput.Filters;
                        break;
                }
            }
            catch (DataAccessException e)
            {
                TempData["errorMessage"] = e.ToString();
            }

            _model.SelectedTab = mobileFiltersInput.Filters.Table;

            return PartialView("TransferBody", _model);
        }
        [HttpPost]
        public IActionResult Refresh([FromBody] RefreshTabInput refreshTabInput)
        {
            switch (refreshTabInput.SelectedTab)
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

            if (refreshTabInput.Mobile)
            {
                _model.ShowFilters = true;
            }

            _model.SelectedTab = refreshTabInput.SelectedTab.ToLower();

            return PartialView("TransferBody", _model);
        }

    }
}
