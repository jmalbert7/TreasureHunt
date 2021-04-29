using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;
using TreasureHunt.API.Models;

namespace TreasureHunt.API
{
    public interface IAzureService
    {
        Task<List<IDataModel>> readResultFromRequest(SqlCommand command, string expectedType);
        Task<List<IDataModel>> executeCommand(string query, string expectedType);
    }
    public class AzureService : IAzureService
    {
        public static IDataModel MapStringToModelType(string str)
        {
            if (str.Equals("UserMo"))
            {
                return new UserMo();
            }
            else if (str.Equals("ClueMo"))
            {
                return new ClueMo();
            }

            else
            {
                return null;
            }
        }
        public async Task<List<IDataModel>> readResultFromRequest(SqlCommand command, string expectedType)
        {
            var reader = await command.ExecuteReaderAsync();
            var rows = new List<IDataModel>();
            IDataModel row = MapStringToModelType(expectedType);
            while (reader.Read())
            {
                row = row.MapToModel(reader);
                rows.Add(row);
            }
            return rows;

        }
        public async Task<List<IDataModel>> executeCommand(string query, string expectedType)
        {
            var connectionString = Environment.GetEnvironmentVariable("sqldb_connection", EnvironmentVariableTarget.Process);
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                try
                {
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        return await readResultFromRequest(command, expectedType);
                    }
                }
                catch
                {
                    return null;
                }
            }
        }
    }
}
