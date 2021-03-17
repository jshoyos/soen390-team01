using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using soen390_team01.Data.Exceptions;
using soen390_team01.Data.Queries;
using soen390_team01.Models;
using soen390_team01.Services;

namespace soen390_team01.Controllers
{
    [Authorize]
    public class AccountingController : Controller
    {
        private readonly IAccountingService _model;

        public AccountingController(IAccountingService model)
        {
            _model = model;
        }

        [HttpGet]
        public IActionResult Index()
        {
            _model.ShowFilters = false;
            return View(_model);
        }

        [HttpPost]
        public IActionResult Refresh([FromBody] RefreshTabInput refreshTabInput)
        {
            switch (refreshTabInput.SelectedTab)
            {
                case "receivable":
                    _model.ResetReceivables();
                    break;
                case "payable":
                    _model.ResetPayables();
                    break;
                case "payment":
                    _model.ResetPayments();
                    break;
            }

            if (refreshTabInput.Mobile)
            {
                _model.ShowFilters = true;
            }

            _model.SelectedTab = refreshTabInput.SelectedTab;
            return PartialView("AccountingBody", _model);
        }

        [HttpPost]
        [FiltersAction]
        public IActionResult FilterPaymentTable([FromBody] MobileFiltersInput mobileFiltersInput)
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

            return PartialView("AccountingBody", _model);
        }
    }
}
