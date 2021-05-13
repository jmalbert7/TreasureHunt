using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using TreasureHunt.API.Models;

namespace TreasureHunt.API.Clues
{
    public static class GetClueById
    {
        [FunctionName("GetClueById")]
        public static async Task<HttpResponseMessage> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "clues/")] HttpRequest req,
            ILogger log)
        {
            string clueQuery = req.Query["lastclueid"];
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            var clueid = clueQuery ?? data?.lastclueid;

            var azureService = new AzureService();
            try
            {
                var rows = await azureService.executeCommand($"SELECT * FROM Clues WHERE LastClueId = '{clueid}';", "ClueMo");
                if (rows != null)
                {
                    var res = JsonConvert.SerializeObject(rows);
                    return new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new StringContent(res, Encoding.UTF8, "application/json")
                    };
                }
            }
            catch
            {
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }
            return new HttpResponseMessage(HttpStatusCode.InternalServerError);
        }
    }
}

