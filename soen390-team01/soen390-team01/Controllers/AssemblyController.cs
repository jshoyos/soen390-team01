using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

        public AssemblyController(IAssemblyService model)
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
        public IActionResult AddProduction(AssemblyModel model)
        {
            _model.BikeOrder = model.BikeOrder;
            var showModal = false;
            if (ModelState.IsValid)
            {
                try
                {
                    _model.AddNewBike(_model.BikeOrder);
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
    }
}
