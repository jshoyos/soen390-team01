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
    public class AssemblyController : Controller
    {
        private readonly IAssemblyService _model;
        private readonly ILogger<AssemblyController> _log;

        public AssemblyController(IAssemblyService model, ILogger<AssemblyController> log)
        {
            _model = model;
            _log = log;
        }

        [HttpGet]
        [ModulePermission(Roles = Role.Accountant + "," + Role.SalesRep + "," + Role.Admin)]
        public IActionResult Index()
        {
            _model.ShowFilters = false;
            return View(_model);
        }

        [HttpPost]
        public IActionResult AddProduction(AssemblyModel model)
        {
            _model.BikeOrder = model.BikeOrder;
            var showModal = false;
            if (ModelState.IsValid)
            {
                try
                {
                    _model.AddNewBike(_model.BikeOrder);
                    _log.LogInformation($"Adding Production {_model.BikeOrder.BikeId}");
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

            _model.SelectedTab = "production";
            _model.ShowModal = showModal;

          

            return View("Index", _model);
        }

        [HttpPost]
        [FiltersAction]
        public IActionResult FilterAssemblyTable([FromBody] MobileFiltersInput mobileFiltersInput)
        {
            try
            {
                if (mobileFiltersInput.Mobile)
                {
                    _model.ShowFilters = true;
                }
                
                _model.Productions = true ? _model.GetFilteredProductionList(mobileFiltersInput.Filters) : _model.GetProductions();
                _model.ProductionFilters = mobileFiltersInput.Filters;
                     
            }
            catch (DataAccessException e)
            {
                TempData["errorMessage"] = e.ToString();
            }

            _model.SelectedTab = mobileFiltersInput.Filters.Table;

            return PartialView("AssemblyBody", _model);
        }
        [HttpPost]
        public IActionResult Refresh([FromBody] RefreshTabInput refreshTabInput)
        {
            _model.Productions = _model.GetProductions();
            _model.ProductionFilters = _model.ResetProductionFilters();
                
            if (refreshTabInput.Mobile)
            {
                _model.ShowFilters = true;
            }

            _model.SelectedTab = refreshTabInput.SelectedTab.ToLower();

            return PartialView("AssemblyBody", _model);
        }

        [HttpPost]

        public IActionResult ProcessProduction(Production production)
        {
            Inventory inventory = null;
            try
            {
                inventory = _model.UpdateInventory(production);
                _log.LogInformation($"Updating inventory with new bike {inventory.ItemId} with quantity {inventory.Quantity}");
                production = _model.UpdateProductionState(production);
                _log.LogInformation($"Updating production {production.ProductionId} with new state {production.State}");
            }
            catch (DataAccessException e)
            {
                TempData["errorMessage"] = e.ToString();
            }        
            return View("Index", _model);
        }
    }
}
