using soen390_team01.Data.Entities;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;
using soen390_team01.Models;

namespace soen390_team01.Services
{
    public class WebProductionReportGenerator : IProductionReportGenerator
    { 
        public string Name { get; } = "Web";

        private readonly ProductionClient _client;

        public WebProductionReportGenerator(ProductionClient client)
        {
            _client = client;
        }

        public void Generate(Production prod, string quality)
        {
            var input = new ProcessProductionInput
            {
                Production = prod,
                Quality = quality
            };
            
            _client.SendProduction(input);
        }
    }
}
