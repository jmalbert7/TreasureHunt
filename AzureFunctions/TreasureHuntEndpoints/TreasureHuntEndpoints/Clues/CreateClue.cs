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

namespace TreasureHunt.API.Clues
{
    public static class CreateClue
    {
        [FunctionName("CreateClue")]
        public static async Task<HttpResponseMessage> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "clues/")] HttpRequest req,
            ILogger log)
        {
            string idQuery = req.Query["huntid"];
            string firstQuery = req.Query["firstflag"];
            string lastQuery = req.Query["lastflag"];
            string lastclueididQuery = req.Query["lastclueid"];
            string locationQuery = req.Query["location"];
            string riddleQuery = req.Query["riddle"];
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            var huntid = idQuery ?? data?.huntid;
            var firstflag = firstQuery ?? data?.firstflag;
            var lastflag = lastQuery ?? data?.lastflag;
            var lastclueid = lastclueididQuery ?? data?.lastclueid;
            var location = locationQuery ?? data?.location;
            var riddle = riddleQuery ??  data?.riddle;

            if (huntid == null || location == null || riddle == null || (firstflag == 1 && lastclueid != null) || (lastclueid == 0 && firstflag != 1 && lastflag != null))
            {
                return new HttpResponseMessage(HttpStatusCode.BadRequest)
                {
                    Content = new StringContent("Usage: huntid, location, riddle, lastclueid needed in request body (unless it is the very first clue).", Encoding.UTF8, "text/plain")
                };
            }

            var connectionString = Environment.GetEnvironmentVariable("sqldb_connection");
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                //SELECT SCOPE_IDENTITY() AS [SCOPE_IDENTITY] returns the id of the inserted row
                var query = $"INSERT Clues (HuntId, FirstFlag, LastFlag, LastClueId, Location, Riddle) VALUES ('{huntid}', '{firstflag}', '{lastflag}', '{lastclueid}', '{location}', '{riddle}')SELECT SCOPE_IDENTITY() AS [SCOPE_IDENTITY];";
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