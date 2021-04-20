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

namespace TreasureHuntEndpoints
{
    public static class GetUsers
    {
        [FunctionName("GetUsers")]
        public static async Task<HttpResponseMessage> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "users/")] HttpRequest req,
            ILogger log)
        {
            var connectionString = Environment.GetEnvironmentVariable("sqldb_connection");
            using(SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var query = "SELECT * FROM Users;";
                using(SqlCommand command = new SqlCommand(query, connection))
                {
                    var reader = await command.ExecuteReaderAsync();
                    var rows = new List<string>();
                    while (reader.Read())
                    {
                        var user = reader["Username"].ToString();
                        rows.Add(user);
                    }
                    if(rows != null)
                    {
                        var res = JsonConvert.SerializeObject(rows);
                        return new HttpResponseMessage(HttpStatusCode.OK)
                        {
                            Content = new StringContent(res, Encoding.UTF8, "application/json")
                        };
                    }
                }
            }
            return new HttpResponseMessage(HttpStatusCode.InternalServerError);

            //log.LogInformation("C# HTTP trigger function processed a request.");

            //string name = req.Query["name"];

            //string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            //dynamic data = JsonConvert.DeserializeObject(requestBody);
            //name = name ?? data?.name;

            //string responseMessage = string.IsNullOrEmpty(name)
            //    ? "This HTTP triggered function executed successfully. Pass a name in the query string or in the request body for a personalized response."
            //    : $"Hello, {name}. This HTTP triggered function executed successfully.";


        }
    }
}

