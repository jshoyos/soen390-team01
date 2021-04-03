using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using soen390_team01.Data;
using soen390_team01.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace soen390_team01.Services
{
    public enum ProductionState
    {
        stopped,
        inProgress,
        completed,
    }

    public class ProductionService : IProductionService
    {
        private readonly ErpDbContext _context;
        private readonly ProductionInventoryValidator _validator;
        private readonly IProductionReportGenerator _csvGenerator;
        private readonly IProductionReportGenerator _webGenerator;
        private readonly Random _rand;

        public int Interval { get; set; } = 10000; // Default value of 10 seconds

        public ProductionService(ErpDbContext context, ProductionInventoryValidator validator, Random rand, IEnumerable<IProductionReportGenerator> generators)
        {
            _context = context;
            _validator = validator;
            _rand = rand;
            _csvGenerator = generators.FirstOrDefault(g => g.Name == "Csv");
            _webGenerator = generators.FirstOrDefault(g => g.Name == "Web");
        }

        public void ProduceBike(Bike bike, int quantity)
        {
            var partsToBuild = _validator.Validate(bike, quantity, _context);
            BuildMissingParts(partsToBuild);

            // Removing required parts' inventory
            foreach (var bikePart in bike.BikeParts)
            {
                var partInv = _context.Inventories.First(inv => inv.ItemId == bikePart.Part.ItemId && inv.Type == "part");
                partInv.Quantity -= bikePart.PartQuantity * quantity;
                _context.Inventories.Update(partInv);
            }

            var production = new Production
            {
                BikeId = bike.ItemId,
                Quantity = quantity,
                State = ProductionState.inProgress.ToString()
            };

            _context.Productions.Add(production);

            _context.SaveChanges();
            production = _context.Productions.AsNoTracking().First(p => p.ProductionId == production.ProductionId);

            _= GenerateProductionReport(production, RandomizeProduction(production));
        }

        public void FixStoppedProduction(Production prod)
        {
            // Change state to in progress in the database
            prod.State = ProductionState.inProgress.ToString();
            _context.Productions.Update(prod);
            _context.SaveChanges();

            // Make the production completed in the report
            prod.State = ProductionState.completed.ToString();

            _= GenerateProductionReport(prod, "good");
        }

        private void BuildMissingParts(IEnumerable<MissingPart> missingParts)
        {
            foreach (var missingPart in missingParts)
            {
                // Removing required materials' inventory
                foreach (var partMaterial in missingPart.Part.PartMaterials)
                {
                    var materialInv = _context.Inventories.First(i => i.ItemId == partMaterial.MaterialId && i.Type == "material");
                    materialInv.Quantity -= partMaterial.MaterialQuantity * missingPart.Quantity;
                    _context.Inventories.Update(materialInv);
                }
                // Adding newly built part's inventory
                var partInv = _context.Inventories.First(i => i.ItemId == missingPart.Part.ItemId && i.Type == "part");
                partInv.Quantity += missingPart.Quantity;
                _context.Inventories.Update(partInv);
            }
        }

        private string RandomizeProduction(Production prod)
        {
            ProductionState state;
            var quality = "none";

            // 9/10 productions will be completed. The rest will be stopped
            if (_rand.Next(10) > 0)
            {
                state = ProductionState.completed;
                // 1/5 productions will have bad quality
                quality = _rand.Next(5) == 0 ? "bad" : "good";
            }
            else
            {
                state = ProductionState.stopped;
            }

            prod.State = state.ToString(); 

            return quality;
        }

        private async Task GenerateProductionReport(Production prod, string quality)
        {
            // Waiting the duration of the interval before generating the report
            await Task.Delay(Interval);

            // Randomizing which report type is produced
            if (_rand.Next(2) == 1)
            {
                _csvGenerator.Generate(prod, quality);
            }
            else
            {
                _webGenerator.Generate(prod, quality);
            }
        }
    }

    public class MissingPart
    {
        public int Quantity { get; }
        public Part Part { get; }
        public List<MissingMaterial> MissingMaterials { get; }

        public MissingPart(Part part, int quantity, List<MissingMaterial> missingMaterials = null)
        {
            Quantity = quantity;
            Part = part;
            MissingMaterials = missingMaterials ?? new List<MissingMaterial>();
        }
    }

    public class MissingMaterial
    {
        public int Quantity { get; }
        public Material Material { get; }

        public MissingMaterial(Material material, int quantity)
        {
            Quantity = quantity;
            Material = material;
        }
    }
}
