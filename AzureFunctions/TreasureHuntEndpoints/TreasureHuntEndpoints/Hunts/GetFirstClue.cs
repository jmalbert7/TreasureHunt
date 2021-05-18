using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
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
    public static class GetFirstClue
    {
        [FunctionName("GetFirstClue")]
        public static async Task<HttpResponseMessage> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "clues/first/")] HttpRequest req,
            ILogger log)
        {
            string huntidQuery = req.Query["huntid"];
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            var huntid = huntidQuery ?? data?.huntid;

            var azureService = new AzureService();
            try
            {
                var rows = await azureService.executeCommand($"SELECT * FROM Clues WHERE HuntId = '{huntid}' AND FirstFlag = 1;", "ClueMo");
                if (rows != null)
                {
                    var res = JsonConvert.SerializeObject(rows.FirstOrDefault());
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

