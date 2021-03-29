using soen390_team01.Data.Entities;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace soen390_team01.Services
{
    public class CsvProductionReportGenerator : IProductionReportGenerator
    {
        public void Generate(Production prod, string quality)
        {
            var sb = new StringBuilder();
            const string DELIMITER = ",";

            sb.Append(prod.ProductionId + DELIMITER);
            sb.Append(prod.BikeId + DELIMITER);
            sb.Append(prod.State + DELIMITER);
            sb.Append(prod.Quantity + DELIMITER);
            sb.Append(prod.Added + DELIMITER);
            sb.Append(prod.Modified + DELIMITER);
            sb.Append(quality);

            var fileName = $"productions/{prod.ProductionId}.csv";

            try
            {
                // Removing old file if its name is already used
                if (File.Exists(fileName))
                {
                    File.Delete(fileName);
                }

                // Create the new report
                using var fs = File.Create(fileName);
                fs.Write(new UTF8Encoding(true).GetBytes(sb.ToString()));
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
            }
        }
    }
}
