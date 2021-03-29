using soen390_team01.Data.Entities;
using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Text;

namespace soen390_team01.Services
{
    public class RestProductionReportGenerator : IProductionReportGenerator
    {
        private HttpClient _client;

        public RestProductionReportGenerator()
        {
            _client = new HttpClient();
            _client.BaseAddress = new Uri(Environment.GetEnvironmentVariable("HOST")!);
        }

        public void Generate(Production prod, string quality)
        {

        }
    }
}
