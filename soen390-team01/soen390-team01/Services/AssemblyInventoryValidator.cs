using System.Collections.Generic;
using System.Linq;
using soen390_team01.Data;
using soen390_team01.Data.Entities;
using soen390_team01.Data.Exceptions;

namespace soen390_team01.Services
{
    public class AssemblyInventoryValidator
    {
        public virtual List<MissingPart> Validate(Bike bike, int bikeQuantity, ErpDbContext context)
        {
            var missingParts = new List<MissingPart>();

            foreach (var bikePart in bike.BikeParts)
            {
                var partInv = context.Inventories.First(i => i.ItemId == bikePart.PartId && i.Type == "part");
                var partQuantity = bikePart.PartQuantity * bikeQuantity;
                var quantityDiff = partInv.Quantity - partQuantity;

                if (quantityDiff >= 0)
                {
                    continue;
                }

                var part = bikePart.Part;
                var missingPart = new MissingPart(part, -quantityDiff);

                foreach (var partMaterial in part.PartMaterials)
                {
                    var materialInv = context.Inventories.First(i => i.ItemId == partMaterial.MaterialId && i.Type == "material");
                    var materialQuantity = partMaterial.MaterialQuantity * missingPart.Quantity;
                    quantityDiff = materialInv.Quantity - materialQuantity;

                    if (quantityDiff >= 0)
                    {
                        continue;
                    }

                    missingPart.MissingMaterials.Add(new MissingMaterial(partMaterial.Material, -quantityDiff));
                }

                missingParts.Add(missingPart);
            }

            if (missingParts.Any(mp => mp.MissingMaterials.Count > 0))
            {
                throw new MissingPartsException(missingParts);
            }

            return missingParts;
        }
    }
}
