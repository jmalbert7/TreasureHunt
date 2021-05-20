using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace TreasureHunt.API.Models
{
    public class GameMo : IDataModel
    {
        public string  GameId { get; set; }
        public string ClueId { get; set; }
        public string UserId { get; set; }

        public IDataModel MapToModel(SqlDataReader reader)
        {
            var gameid = reader["GameId"].ToString();
            var clueid = reader["ClueId"].ToString();
            var userid = reader["UserId"].ToString();
            var game = new GameMo() { GameId = gameid, ClueId = clueid, UserId = userid};
            return game;
        }

    }

}
