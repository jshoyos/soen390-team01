using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using soen390_team01.Data.Entities;
using soen390_team01.Models;
using soen390_team01.Services;

namespace soen390_team01.Controllers
{
    public class TransfersController : Controller
    {
        private readonly TransfersService _transfersService;

        public TransfersController(TransfersService transfersService)
        {
            _transfersService = transfersService;
        }

        [Authorize]
        [HttpGet]
        public IActionResult Index()
        {
            return View(_transfersService.GetTransfersModel());
        }
    }
}
