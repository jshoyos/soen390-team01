using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
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
                BaseAddress = new Uri(Environment.GetEnvironmentVariable("HOST")! + "/api/Production/Process/")
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
            try 
            {
                var result =  _client.PostAsync("", data).Result;
            }
            catch(Exception e)
            {

            }
            
        }
    }
}
