using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using soen390_team01.Data.Exceptions;
using soen390_team01.Data.Queries;
using soen390_team01.Models;
using soen390_team01.Services;
using System.Net;
using System.Net.Mail;

namespace soen390_team01.Controllers
{
    [Authorize]
    public class AccountingController : Controller
    {
        private readonly IAccountingService _model;
        private readonly ILogger<AccountingController> _log;

        public AccountingController(IAccountingService model, ILogger<AccountingController> log)
        {
            _model = model;
            _log = log;
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
        public IActionResult update([FromBody] ReceivableUpdateModel receivableUpdateModel )
        {
            var id = receivableUpdateModel.Id;
            var status = receivableUpdateModel.Status;

            var text = "The Receivable payment with id " + id + " has been completed";
            _model.setReceivableState(id, status);



            if (status == "completed")
            {
                var smtpClient = new SmtpClient("smtp.gmail.com")
                {
                    Port = 587,
                    Credentials = new NetworkCredential("soen390Project@gmail.com", "Soen390!"),
                    EnableSsl = true,
                };

                smtpClient.Send("soen390Project@gmail.com", "tigran.kar@hotmail.com", "Payment completed", text);
            }

            _model.SelectedTab = "receivable";
            return PartialView("AccountingBody", _model);
        }
    }
}
