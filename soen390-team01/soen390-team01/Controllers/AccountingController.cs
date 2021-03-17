using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using soen390_team01.Data.Entities;
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
        public IActionResult Refresh([FromBody] string selectedTab)
        {
            switch (selectedTab)
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
            _model.ShowFilters = true;
            return PartialView("AccountingBody", _model);
        }

        [HttpPost]
        public IActionResult ChangeTab([FromBody] string selectedTab)
        {
            _model.SelectedTab = selectedTab;
            return PartialView("AccountingBody", _model);
        }

        [HttpPost]
        [FiltersAction]
        public IActionResult FilterPaymentTable([FromBody] Filters filters)
        {
            try
            {
                _model.FilterSelectedTab(filters);
                _model.ShowFilters = true;
            }
            catch (DataAccessException e)
            {
                TempData["errorMessage"] = e.ToString();
            }

            return PartialView("AccountingBody", _model);
        }
    }
}
