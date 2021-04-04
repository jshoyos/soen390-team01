using System;
using System.Globalization;
using System.IO;
using CsvHelper;
using soen390_team01.Data.Entities;
using soen390_team01.Models;

namespace soen390_team01.Services
{
    public class CsvProductionProcessor
    {
        private readonly string _directory;
        private readonly IProductionProcessor _productionController;

        public CsvProductionProcessor(string directory, IProductionProcessor productionController)
        {
            _directory = directory;
            _productionController = productionController;
        }

        public void Process()
        {
            string[] files = Directory.GetFiles(_directory, "*.csv");

            Array.Sort(files, StringComparer.Ordinal);

            if (files.Length <= 0)
            {
                return;
            }

            using (var streamReader = File.OpenText(files[0]))
            {
                using var reader = new CsvReader(streamReader, CultureInfo.CurrentCulture);
                reader.Read();
                var record = reader.GetRecord<CsvInput>();
                _productionController.Process(new ProcessProductionInput
                {
                    Production = new Production
                    {
                        ProductionId = record.ProductionId,
                        BikeId = record.BikeId,
                        State = record.State,
                        Quantity = record.Quantity,
                        Added = record.Added,
                        Modified = record.Modified
                    },
                    Quality = record.Quality
                });
            }

            File.Delete(files[0]);
        }
    }

    internal class CsvInput
    {
        public long ProductionId { get; set; }
        public long BikeId { get; set; }
        public string State { get; set; }
        public int Quantity { get; set; }
        public DateTime Added { get; set; }
        public DateTime Modified { get; set; }
                public string Quality { get; set; }

    }
}
