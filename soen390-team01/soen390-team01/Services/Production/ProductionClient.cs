using System;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;
using soen390_team01.Models;

namespace soen390_team01.Services
{
    public class ProductionClient
    {
        private readonly HttpClient _client;

        public ProductionClient()
        {
            _client = new HttpClient
            {
                BaseAddress = new Uri(Environment.GetEnvironmentVariable("HOST")! + "/ProcessProduction/")
            };
        }

        /// <summary>
        /// Sending a production processing request
        /// </summary>
        /// <param name="input"></param>
        public virtual void SendProduction(ProcessProductionInput input)
        {
            var json = JsonConvert.SerializeObject(input);
            var data = new StringContent(json, Encoding.UTF8, "application/json");

            _ = _client.PostAsync("", data);
        }
    }
}
