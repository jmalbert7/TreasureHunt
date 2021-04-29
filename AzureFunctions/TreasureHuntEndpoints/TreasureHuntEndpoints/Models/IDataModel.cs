using System.Data.SqlClient;

namespace TreasureHunt.API.Models
{
    public interface IDataModel
    {
        public abstract IDataModel MapToModel(SqlDataReader reader); 

    }
}
