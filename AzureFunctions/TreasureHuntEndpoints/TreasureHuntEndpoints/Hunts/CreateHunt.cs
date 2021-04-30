using System;
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

namespace TreasureHunt.API.Hunts
{
    public static class CreateHunt
    {
        [FunctionName("CreateHunt")]
        public static async Task<HttpResponseMessage> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "hunts/")] HttpRequest req,
            ILogger log)
        {
            string idQuery = req.Query["userid"];
            string nameQuery = req.Query["name"];
            string locationQuery = req.Query["location"];
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            var userid = idQuery ?? data?.userid;
            var name = nameQuery ?? data?.name;
            var location = locationQuery ?? data?.location;

            if (userid == null || name == null || location == null)
            {
                return new HttpResponseMessage(HttpStatusCode.BadRequest)
                {
                    Content = new StringContent("Usage: userid, name, location needed in request body", Encoding.UTF8, "text/plain")
                };
            }

            var connectionString = Environment.GetEnvironmentVariable("sqldb_connection");
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                //SELECT SCOPE_IDENTITY() AS [SCOPE_IDENTITY] returns the id of the inserted row
                var query = $"INSERT Hunts (UserId, HuntName, GeneralLocation) VALUES ('{userid}', '{name}', '{location}')SELECT SCOPE_IDENTITY() AS [SCOPE_IDENTITY];";
                try
                {
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        var row = await command.ExecuteScalarAsync();
                        if (row != null)
                        {
                            var res = JsonConvert.SerializeObject(Int32.Parse(row.ToString()));
                            return new HttpResponseMessage(HttpStatusCode.OK)
                            {
                                Content = new StringContent(res, Encoding.UTF8, "application/json")
                            };
                        }
                    }
                }
                catch (Exception ex)
                {
                    return new HttpResponseMessage(HttpStatusCode.BadRequest)
                    {
                        Content = new StringContent(ex.Message, Encoding.UTF8, "text/plain")
                    };
                }
            }
            return new HttpResponseMessage(HttpStatusCode.InternalServerError);
        }
    }
}