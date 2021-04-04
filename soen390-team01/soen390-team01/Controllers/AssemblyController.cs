using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
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
            var showModal = false;
            if (ModelState.IsValid)
            {
                try
                {
                    _model.AddNewBike(model.BikeOrder);
                    _log.LogInformation($"Adding Bike {model.BikeOrder.BikeId} to production.");
                }
                catch (MissingPartsException e)
                {
                    TempData["missingParts"] = JsonConvert.SerializeObject(e.MissingParts.Select(mp => new MissingPart(
                            new Part {
                                ItemId = mp.Part.ItemId
                            }, 
                            mp.Quantity, 
                            mp.MissingMaterials.Select(mm => new MissingMaterial(
                                    new Material
                                    {
                                        ItemId = mm.Material.ItemId,
                                    },
                                    mm.Quantity
                                )).ToList()
                            ))
                    );
                }
                catch (InsufficientBikePartsException e)
                {
                    TempData["errorMessage"] = e.Message.ToString();
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

            return RedirectToAction("Index");
        }

        public IActionResult FixProduction(long productionId)
        {
            try
            {
                _model.FixProduction(productionId);
            }
            catch (DataAccessException e)
            {
                TempData["errorMessage"] = e.ToString();
            }
            return RedirectToAction("Index");
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
                
                _model.Productions = mobileFiltersInput.Filters.AnyActive() ? _model.GetFilteredProductionList(mobileFiltersInput.Filters) : _model.GetProductions();
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
