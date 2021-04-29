using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace TreasureHunt.API.Models
{
    public class ClueMo : IDataModel
    {
        public string ClueId { get; set; }
        public string HuntId { get; set; }
        public bool FirstFlag { get; set; }
        public bool LastFlag { get; set; }
        public string LastClueId { get; set; }
        public string Location { get; set; }
        public string Riddle { get; set; }

        public IDataModel MapToModel(SqlDataReader reader)
        {
            var id = reader["ClueId"].ToString();
            var huntId = reader["HuntId"].ToString();
            var firstFlag = reader["FirstFlag"].ToString() == "False" ? false : true;
            var lastFlag = reader["LastFlag"].ToString() == "False" ? false : true;
            var lastClueId = reader["LastClueId"].ToString();
            var location = reader["Location"].ToString();
            var riddle = reader["Riddle"].ToString();
            var clue = new ClueMo() { ClueId = id, HuntId = huntId, FirstFlag = firstFlag, LastFlag = lastFlag, LastClueId = lastClueId, Location = location, Riddle = riddle};
            return clue;
        }

    }

}
