using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
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

    public class AssemblyLineService
    {
        private const int INTERVAL = 1;
        private readonly ErpDbContext _context;
        private readonly AssemblyInventoryValidator _validator;
        private readonly IProductionReportGenerator _csvGenerator;
        private readonly IProductionReportGenerator _restGenerator;
        private readonly Random _rand;

        public AssemblyLineService(ErpDbContext context, AssemblyInventoryValidator validator, ProductionReportGeneratorResolver resolver)
        {
            _context = context;
            _validator = validator;
            _csvGenerator = resolver.Resolve("Csv");
            _restGenerator = resolver.Resolve("Rest");
            _rand = new Random();
        }

        public void ProduceBike(Bike bike, int quantity)
        {
            var partsToBuild = _validator.Validate(bike, quantity, _context);
            BuildMissingParts(partsToBuild);

            var production = new Production
            {
                BikeId = bike.ItemId,
                Quantity = quantity,
                State = ProductionState.InProgress.ToString()
            };

            _context.Productions.Add(production);

            _context.SaveChanges();

            // Waiting the duration of the interval before generating the report
            _ = Task.Delay(INTERVAL);

            GenerateProductionReport(production, RandomizeProduction(production));

            Debug.WriteLine("AFTER INTERVAL");
        }

        public void FixStoppedProduction(Production prod)
        {
            prod.State = ProductionState.InProgress.ToString();
            GenerateProductionReport(prod, "good");
        }

        private void BuildMissingParts(IEnumerable<MissingPart> missingParts)
        {
            foreach (var missingPart in missingParts)
            {
                foreach (var partMaterial in missingPart.Part.PartMaterials)
                {
                    var materialInv = _context.Inventories.First(i => i.ItemId == partMaterial.MaterialId && i.Type == "material");
                    materialInv.Quantity -= partMaterial.MaterialQuantity * missingPart.Quantity;
                    _context.Inventories.Update(materialInv);
                }
            }
        }

        private string RandomizeProduction(Production prod)
        {
            ProductionState state;
            var quality = "none";

            // 9/10 productions will be completed. The rest will be stopped
            if (_rand.Next(0, 10) > 0)
            {
                state = ProductionState.Completed;
                // 1/5 productions will have bad quality
                quality = _rand.Next(0, 5) == 0 ? "bad" : "good";
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
            // Randomizing which report type is produced
            if (_rand.Next(0, 2) == 1)
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
