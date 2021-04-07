using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<AccountingController> _log;
        private readonly IEmailService _emailService;

        public AccountingController(IAccountingService model, ILogger<AccountingController> log, IEmailService emailService)
        {
            _model = model;
            _log = log;
            _emailService = emailService;
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

        [HttpPost]
        public IActionResult Update([FromBody] ReceivableUpdateModel receivableUpdateModel )
        {
            var id = receivableUpdateModel.Id;
            var status = receivableUpdateModel.Status;
                      
            try
            {
               string name = _model.SetReceivableState(id, status);
                if (status == "completed")
                {
                    var text = "The receivable payment with id " + id + " from "+ name + " has been completed.";
                    _emailService.SendEmail(text, Roles.Accountant);
                }
            }
            catch (DataAccessException e)
            {
                TempData["errorMessage"] = e.ToString();
            }
                       
            _model.SelectedTab = "receivable";
            return PartialView("AccountingBody", _model);
        }
    }
}
