using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using soen390_team01.Data;
using soen390_team01.Data.Entities;

namespace soen390_team01.Services
{
    public enum ProductionState
    {
        [Display(Name = "stopped")]
        Stopped,
        [Display(Name = "in_progress")]
        InProgress,
        [Display(Name = "completed")]
        Completed,
    }

    public class AssemblyLineService : IAssemblyLineService
    {
        private readonly ErpDbContext _context;
        private readonly AssemblyInventoryValidator _validator;
        private readonly IProductionReportGenerator _csvGenerator;
        private readonly IProductionReportGenerator _restGenerator;
        private readonly Random _rand;

        public int Interval { get; set; } = 10000; // Default value of 10 seconds

        public AssemblyLineService(ErpDbContext context, AssemblyInventoryValidator validator, Random rand, IEnumerable<IProductionReportGenerator> generators)
        {
            _context = context;
            _validator = validator;
            _rand = rand;
            _csvGenerator = generators.FirstOrDefault(g => g.Name == "Csv");
            _restGenerator = generators.FirstOrDefault(g => g.Name == "Web");
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
                State = ProductionState.InProgress.ToString()
            };

            _context.Productions.Add(production);

            _context.SaveChanges();

            GenerateProductionReport(production, RandomizeProduction(production));
        }

        public void FixStoppedProduction(Production prod)
        {
            prod.State = ProductionState.InProgress.ToString();
            _context.Productions.Update(prod);
            _context.SaveChanges();

            GenerateProductionReport(prod, "good");
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
                state = ProductionState.Completed;
                // 1/5 productions will have bad quality
                quality = _rand.Next(5) == 0 ? "bad" : "good";
            }
            else
            {
                state = ProductionState.Stopped;
            }

            prod.State = state.ToString();

            return quality;
        }

        private void GenerateProductionReport(Production prod, string quality)
        {
            // Waiting the duration of the interval before generating the report
            _ = Task.Delay(Interval);

            // Randomizing which report type is produced
            if (_rand.Next(2) == 1)
            {
                _csvGenerator.Generate(prod, quality);
            }
            else
            {
                _restGenerator.Generate(prod, quality);
            }
        }
    }

    public class MissingPart
    {
        public int Quantity { get; }
        public Part Part { get; }
        public List<MissingMaterial> MissingMaterials { get; }

        public MissingPart(Part part, int quantity)
        {
            Quantity = quantity;
            Part = part;
            MissingMaterials = new List<MissingMaterial>();
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
