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
using TreasureHuntEndpoints.Models;

namespace TreasureHuntEndpoints
{
    public static class GetUserByUsername
    {
        [FunctionName("GetUserByUsername")]
        public static async Task<HttpResponseMessage> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "user/")] HttpRequest req,
            ILogger log)
        {
            string name = req.Query["username"];

            var connectionString = Environment.GetEnvironmentVariable("sqldb_connection");
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var query = $"SELECT * FROM Users WHERE Username = '{name}';";
                try
                {
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        var reader = await command.ExecuteReaderAsync();
                        var rows = new List<UserMo>();
                        while (reader.Read())
                        {
                            var username = reader["Username"].ToString();
                            var hashedPassword = reader["HashedPassword"].ToString();
                            var user = new UserMo() { Username = username, HashedPassword = hashedPassword };
                            rows.Add(user);
                        }
                        if (rows != null)
                        {
                            var res = JsonConvert.SerializeObject(rows);
                            return new HttpResponseMessage(HttpStatusCode.OK)
                            {
                                Content = new StringContent(res, Encoding.UTF8, "application/json")
                            };
                        }
                    }
                }
                catch(Exception ex)
                {
                    return new HttpResponseMessage(HttpStatusCode.BadRequest);
                }

            }
            return new HttpResponseMessage(HttpStatusCode.InternalServerError);
        }
    }
}

