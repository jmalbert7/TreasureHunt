using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace TreasureHunt.API.Models
{
    public class HuntMo : IDataModel
    {
        public string HuntId { get; set; }
        public string UserId { get; set; }
        public string HuntName { get; set; }
        public string GeneralLocation { get; set; }

        public IDataModel MapToModel(SqlDataReader reader)
        {
            var huntId = reader["HuntId"].ToString();
            var userId = reader["UserId"].ToString();
            var huntName = reader["HuntName"].ToString();
            var location = reader["GeneralLocation"].ToString();
            var hunt = new HuntMo() { HuntId = huntId, UserId = userId, HuntName = huntName, GeneralLocation = location };
            return hunt;
        }

    }

}
