using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using soen390_team01.Data.Exceptions;
using soen390_team01.Models;
using soen390_team01.Services;

namespace soen390_team01.Controllers
{
    public class ProductionController : Controller, IProductionProcessor
    {
        private readonly IAssemblyService _model;
        private readonly ILogger<ProductionController> _log;
        private readonly IEmailService _emailService;

        public ProductionController(IAssemblyService model, ILogger<ProductionController> log, IEmailService emailService)
        {
            _model = model;
            _log = log;
            _emailService = emailService;
        }

        [HttpPost]
        [Route("api/Production/Process")]
        public void Process([FromBody] ProcessProductionInput input)
        {
            try
            {
                // TODO: add triggers for bad quality and stopped state here
                var inventory = _model.UpdateInventory(input.Production);
                _log.LogInformation($"Updating inventory for bike {inventory.ItemId} with quantity {inventory.Quantity}");
                input.Production = _model.UpdateProduction(input.Production);
                _log.LogInformation($"Updating production {input.Production.ProductionId} with new state {input.Production.State}");
                if (input.Production.State == "stopped")
                {
                    _emailService.SendEmail($"Problem with production: {input.Production.ProductionId}, it has been stopped.",Roles.InventoryManager);
                }
                else if (input.Quality == "bad")
                {
                    _emailService.SendEmail($"Problem with production: {input.Production.ProductionId}, quality is below standard.", Roles.InventoryManager);
                }


            }
            catch (DataAccessException e)
            {
                TempData["errorMessage"] = e.ToString();
            }

        }

    }
}
