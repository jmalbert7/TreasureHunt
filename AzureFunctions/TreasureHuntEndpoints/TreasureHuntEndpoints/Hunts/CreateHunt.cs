using System;
using System.Data.SqlClient;
using System.IO;
using System.Net;
using System.Net.Http;
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
            [HttpTrigger(AuthorizationLevel.Anonymous,"post", Route = "hunts/")] HttpRequest req,
            ILogger log)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            var userid =  data?.userid;
            var name = data?.name;
            var location = data?.location;

            if (String.IsNullOrEmpty(userid) || String.IsNullOrEmpty(name) || String.IsNullOrEmpty(location))
            {
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }

            var connectionString = Environment.GetEnvironmentVariable("sqldb_connection");
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var query = $"INSERT Hunts (UserId, HuntName, GeneralLocation) VALUES ('{userid}', '{name}', '{location}');";
                try
                {
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        var rowsAffected = await command.ExecuteNonQueryAsync();

                    }
                }
                catch (Exception ex)
                {
                    return new HttpResponseMessage(HttpStatusCode.BadRequest);
                }

            }
            return new HttpResponseMessage(HttpStatusCode.InternalServerError);
        }
    }
}

