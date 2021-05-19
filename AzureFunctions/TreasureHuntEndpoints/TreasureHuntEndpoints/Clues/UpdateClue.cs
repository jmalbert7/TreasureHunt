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

namespace TreasureHunt.API.Clues
{
    public static class UpdateClue
    {
        [FunctionName("UpdateClue")]
        public static async Task<HttpResponseMessage> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "clues/update/")] HttpRequest req,
            ILogger log)
        {
            //get all query string params
            string idQuery = req.Query["huntid"];
            string firstQuery = req.Query["firstflag"];
            string lastQuery = req.Query["lastflag"];
            string lastclueididQuery = req.Query["lastclueid"];
            string locationQuery = req.Query["location"];
            string riddleQuery = req.Query["riddle"];
            string clueQuery = req.Query["clueid"];

            //get all req body values
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            string huntid = idQuery ?? data?.huntid;
            string firstflag = firstQuery ?? data?.firstflag;
            string lastflag = lastQuery ?? data?.lastflag;
            string lastclueid = lastclueididQuery ?? data?.lastclueid;
            string location = locationQuery ?? data?.location;
            string riddle = riddleQuery ?? data?.riddle;
            string clueid = clueQuery ?? data?.clueid;

            if(clueid == null)
            {
                return new HttpResponseMessage(HttpStatusCode.BadRequest)
                {
                    Content = new StringContent("Usage: Must supply a ClueId for Update", Encoding.UTF8, "application/json")
                };
            }

            var azureService = new AzureService();
            try
            { 
                //get all existing values for the given clue
                var rows = await azureService.executeCommand($"SELECT * FROM Clues WHERE ClueId = '{clueid}';", "ClueMo");
                if (rows == null || rows.Count == 0)
                {
                    return new HttpResponseMessage(HttpStatusCode.BadRequest)
                    {
                        Content = new StringContent("ClueId does not exist", Encoding.UTF8, "application/json")
                    };
                }
                else 
                {
                    var existingClue = (ClueMo)rows.First();
                    huntid = huntid ?? existingClue.HuntId;
                    if (firstflag == null) firstflag = existingClue.FirstFlag.ToString();
                    if (lastflag == null) lastflag = existingClue.LastFlag.ToString() == "False"? "0" : "1";
                    lastclueid = lastclueid ?? existingClue.LastClueId;
                    location = location ?? existingClue.Location;
                    riddle = riddle ?? existingClue.Riddle;


                    var azureService2 = new AzureService();
                    var updatedClue = await azureService2.executeCommand($"UPDATE[dbo].[Clues] SET HuntId = {huntid},FirstFlag = {firstflag},LastFlag = {lastflag},LastClueId = {lastclueid},Location = '{location}',Riddle = '{riddle}' WHERE ClueId = {clueid}", "ClueMo");
                    if (updatedClue != null)
                    {
                        //var res = JsonConvert.SerializeObject(int.Parse(updatedClue.ToString()));
                        return new HttpResponseMessage(HttpStatusCode.OK)
                        {
                            Content = new StringContent($"Clue {clueid} updated successfully", Encoding.UTF8, "application/json")
                        };
                    }
                }
            }
            catch(Exception ex)
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