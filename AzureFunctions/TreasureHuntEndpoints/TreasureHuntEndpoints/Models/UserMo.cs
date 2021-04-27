using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace TreasureHunt.API.Models
{
    public interface IDataModel
    {
        public abstract IDataModel MapToModel(SqlDataReader reader); 

    }
    public class UserMo: IDataModel
    {
        public string UserId { get; set; }
        public string Username { get; set; }
        public string HashedPassword { get; set; }

        public IDataModel MapToModel(SqlDataReader reader)
        {
            var id = reader["UserId"].ToString();
            var username = reader["Username"].ToString();
            var hashedPassword = reader["HashedPassword"].ToString();
            var user = new UserMo() { Username = username, HashedPassword = hashedPassword, UserId = id };
            return user;
        }
    }
}
