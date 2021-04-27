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

namespace TreasureHunt.API.Users
{
    public static class GetUserByUsername
    {
        [FunctionName("GetUserByUsername")]
        public static async Task<HttpResponseMessage> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "user/")] HttpRequest req,
            ILogger log)
        {
            string nameQuery = req.Query["username"];
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            var name = nameQuery ?? data?.username;

            var azureService = new AzureService();
            try
            {
                var rows = await azureService.executeCommand($"SELECT * FROM Users WHERE Username = '{name}';", "UserMo");
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

