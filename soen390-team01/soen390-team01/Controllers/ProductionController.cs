using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using soen390_team01.Data.Entities;
using soen390_team01.Data.Exceptions;
using soen390_team01.Models;
using soen390_team01.Services;

namespace soen390_team01.Controllers
{
    public class ProductionController:  Controller
    {
        private readonly IAssemblyService _model;
        private readonly ILogger<ProductionController> _log;

        public ProductionController(IAssemblyService model, ILogger<ProductionController> log)
        {
            _model = model;
            _log = log;
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
            }
            catch (DataAccessException e)
            {
                TempData["errorMessage"] = e.ToString();
            }
        }

    }
}
