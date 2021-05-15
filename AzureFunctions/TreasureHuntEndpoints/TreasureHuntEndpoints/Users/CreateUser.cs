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

namespace TreasureHunt.API.Users
{
    public static class CreateUser
    {
        [FunctionName("CreateUser")]
        public static async Task<HttpResponseMessage> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "users/create/")] HttpRequest req,
            ILogger log)
        {
            //get all query string params
            string usernameQuery = req.Query["username"];
            string hashedPasswordQuery = req.Query["password"];
            
            //get all req body values
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            string username = usernameQuery ?? data?.username;
            string hashedPassword = hashedPasswordQuery ?? data?.password;
            
            if (username == null || hashedPassword == null)
            {
                return new HttpResponseMessage(HttpStatusCode.BadRequest)
                {
                    Content = new StringContent("Usage: username and password must be provided.", Encoding.UTF8, "text/plain")
                };
            }

            var connectionString = Environment.GetEnvironmentVariable("sqldb_connection");
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                //see if user already registered
                var getQuery = $"SELECT * FROM Users WHERE Username = '{username}';";
                try
                {
                    using (SqlCommand command = new SqlCommand(getQuery, connection))
                    {
                        var row = await command.ExecuteScalarAsync();
                        if (row != null)
                        {                            
                            return new HttpResponseMessage(HttpStatusCode.BadRequest)
                            {
                                Content = new StringContent("Username already exists.", Encoding.UTF8, "application/json")
                            };
                        }
                    }
                    //SELECT SCOPE_IDENTITY() AS [SCOPE_IDENTITY] returns the id of the inserted row
                    var query = $"INSERT Users (Username, HashedPassword) VALUES ('{username}', '{hashedPassword}')SELECT SCOPE_IDENTITY() AS [SCOPE_IDENTITY];";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        var row = await command.ExecuteScalarAsync();
                        if (row != null)
                        {
                            var res = JsonConvert.SerializeObject(int.Parse(row.ToString()));
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