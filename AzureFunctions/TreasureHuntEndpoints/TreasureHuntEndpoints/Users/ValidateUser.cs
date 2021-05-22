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

namespace TreasureHunt.API.Users
{
    public static class ValidateUser
    {
        [FunctionName("ValidateUser")]
        public static async Task<HttpResponseMessage> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "users/validate/")] HttpRequest req,
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
            var azureService = new AzureService();
            try
            {
                var row = (await azureService.executeCommand($"SELECT * FROM Users WHERE Username = '{username}';", "UserMo")).FirstOrDefault();
                if (row != null)
                {
                    UserMo user = (UserMo)row;
                    var storedPassword = user.HashedPassword;
                    if (storedPassword.Equals(hashedPassword))
                    {
                        var azureService2 = new AzureService();
                        var game = await azureService2.executeCommand($"SELECT * FROM Games WHERE UserId = '{user.UserId}'", "GameMo");
                        if (game != null)
                        {
                            var result = JsonConvert.SerializeObject(game.FirstOrDefault());
                            return new HttpResponseMessage(HttpStatusCode.OK)
                            {
                                Content = new StringContent(result, Encoding.UTF8, "application/json")
                            };
                        }
                    }
                    return new HttpResponseMessage(HttpStatusCode.BadRequest)
                    {
                        Content = new StringContent("Invalid Password", Encoding.UTF8, "application/json")
                    };
                }
                else
                {
                    return new HttpResponseMessage(HttpStatusCode.BadRequest)
                    {
                        Content = new StringContent("Username doesn't exist", Encoding.UTF8, "application/json")
                    };
                }
            }
            catch
            {
                return new HttpResponseMessage(HttpStatusCode.InternalServerError);
            }            
        }
    }
}

