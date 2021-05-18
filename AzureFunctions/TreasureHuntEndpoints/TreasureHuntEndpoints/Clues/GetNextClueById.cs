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
    public static class GetNextClueById
    {
        [FunctionName("GetNextClueById")]
        public static async Task<HttpResponseMessage> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "clues/next/")] HttpRequest req,
            ILogger log)
        {
            string lastclueQuery = req.Query["lastclueid"];
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            string lastclueid = lastclueQuery ?? data?.lastclueid;

            if(lastclueid == "0" || lastclueid == null)
            {
                return new HttpResponseMessage(HttpStatusCode.BadRequest)
                {
                    Content = new StringContent("Usage: Must supply the lastclueid", Encoding.UTF8, "application/json")
                };
            }

            var azureService = new AzureService();
            try
            {
                var rows = await azureService.executeCommand($"SELECT * FROM Clues WHERE LastClueId = '{lastclueid}';", "ClueMo");
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

