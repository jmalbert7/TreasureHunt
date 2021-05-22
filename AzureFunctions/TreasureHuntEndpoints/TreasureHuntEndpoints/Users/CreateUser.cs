using System;
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
            int gameid;
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
                    int userId;
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        var row = await command.ExecuteScalarAsync();

                        if (row != null)
                        {
                            userId = int.Parse(row.ToString());
                        }
                        else
                        {
                            return new HttpResponseMessage(HttpStatusCode.InternalServerError);
                        }
                    }
                    //create Game record
                    query = $"INSERT Games (UserId) VALUES ({userId})SELECT SCOPE_IDENTITY() AS [SCOPE_IDENTITY];";                    
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        var row = await command.ExecuteScalarAsync();
                        if (row != null)
                        {
                            gameid = int.Parse(row.ToString());
                        }
                        else
                        {
                            return new HttpResponseMessage(HttpStatusCode.InternalServerError);
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
            try
            {
                var azureService = new AzureService();
                var rows = await azureService.executeCommand($"SELECT * FROM Games WHERE GameId = {gameid};", "GameMo");
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
                    return new HttpResponseMessage(HttpStatusCode.InternalServerError)
                    {
                        Content = new StringContent("Could not retreive Game", Encoding.UTF8, "application/json")
                    };
                }
            }
            catch (Exception ex)
            {
                return new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent(ex.Message, Encoding.UTF8, "application/json")
                };
            }            
        }
    }
}