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
            var model = _transfersService.GetTransfersModel();
            return View(model);
        }

        [HttpPost]
        public IActionResult AddProcurement(TransfersModel model)
        {
            bool showModal = false;
            if (ModelState.IsValid)
            {
                switch (model.AddProcurement.ItemType)
                {
                    case "Bike": _transfersService.AddProcurement<Bike>(model.AddProcurement); break;
                    case "Part": _transfersService.AddProcurement<Part>(model.AddProcurement); break;
                    case "Material": _transfersService.AddProcurement<Material>(model.AddProcurement); break;
                }
            }
            else
            {
                showModal = true;
            }

            model = _transfersService.GetTransfersModel();
            model.SelectedTab = "Procurement";
            model.ShowModal = showModal;

            return View("Index", model);
        }
    }
}
