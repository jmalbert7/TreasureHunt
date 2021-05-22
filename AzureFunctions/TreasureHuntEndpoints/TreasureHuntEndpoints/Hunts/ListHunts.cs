using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net.Http;
using System.Linq;
using System.Net;
using System.Text;

namespace TreasureHunt.API.Hunts
{
    public static class ListHunts
    {
        [FunctionName("ListHunts")]
        public static async Task<HttpResponseMessage> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "hunts/list/")] HttpRequest req,
            ILogger log)
        {
            var azureService = new AzureService();
            try
            {
                var rows = await azureService.executeCommand($"SELECT * FROM Hunts;", "HuntMo");
                if (rows != null)
                {
                    var res = JsonConvert.SerializeObject(rows);
                    return new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new StringContent(res, Encoding.UTF8, "application/json")
                    };
                }
                else
                {
                    return new HttpResponseMessage(HttpStatusCode.BadRequest)
                    {
                        Content = new StringContent("Could not find Hunts", Encoding.UTF8, "application/json")
                    };
                }
            }
            catch
            {
                return new HttpResponseMessage(HttpStatusCode.InternalServerError);
            }            
        }
    }
}
