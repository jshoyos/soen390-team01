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
        public IActionResult Index()
        {
            _model.ShowFilters = false;
            return View(_model);
        }

    }
}
