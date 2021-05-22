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

namespace TreasureHunt.API.Games
{
    public static class UpdateGame
    {
        [FunctionName("UpdateGame")]
        public static async Task<HttpResponseMessage> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "games/update/")] HttpRequest req,
            ILogger log)
        {
            //get all query string params
            string userQuery = req.Query["userid"];
            string clueQuery = req.Query["clueid"];

            //get all req body values
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            string userid = userQuery ?? data?.userid;
            string clueid = clueQuery ?? data?.clueid;

            if (clueid == null || userid == null)
            {
                return new HttpResponseMessage(HttpStatusCode.BadRequest)
                {
                    Content = new StringContent($"Usage: Must supply a ClueId and UserId for Update", Encoding.UTF8, "application/json")
                };
            }

            var azureService = new AzureService();
            try
            {
                //get all existing values for the given clue
                var rows = await azureService.executeCommand($"SELECT * FROM Games WHERE UserId = '{userid}';", "GameMo");
                if (rows == null || rows.Count == 0)
                {
                    return new HttpResponseMessage(HttpStatusCode.BadRequest)
                    {
                        Content = new StringContent("UserId does not have a Game started", Encoding.UTF8, "application/json")
                    };
                }
                else
                {
                    var existingGame = (GameMo)rows.First();
                    clueid = clueid ?? existingGame.ClueId;

                    var azureService2 = new AzureService();
                    var updatedGame = await azureService2.executeCommand($"UPDATE[dbo].[Games] SET ClueId = {clueid} WHERE UserId = {userid}", "GameMo");
                    if (updatedGame != null)
                    {
                        //var res = JsonConvert.SerializeObject(int.Parse(updatedClue.ToString()));
                        return new HttpResponseMessage(HttpStatusCode.OK)
                        {
                            Content = new StringContent($"Game for User {userid} updated successfully", Encoding.UTF8, "application/json")
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                return new HttpResponseMessage(HttpStatusCode.BadRequest)
                {
                    Content = new StringContent(ex.Message.ToString(), Encoding.UTF8, "application/json")
                };
            }
            return new HttpResponseMessage(HttpStatusCode.InternalServerError);
        }
    }
}