using System.Configuration;
using System.Data;
using LegacyApp.Extensions;
using Microsoft.Data.SqlClient;

namespace LegacyApp;

public class ClientRepository : IClientRepository
{
    private readonly string _connectionString;

    public ClientRepository()
    {
        _connectionString = ConfigurationManager.ConnectionStrings["appDatabase"].ConnectionString;
    }

    public Client GetById(int id)
    {
        using var connection = new SqlConnection(_connectionString);

        try
        {
            var parameters = new Dictionary<string, object>
            {
                { "@clientId", id }
            };

            using var command = connection.CreateCommand("uspGetClientById", CommandType.StoredProcedure, parameters);

            connection.Open();
            using var reader = command.ExecuteReader(CommandBehavior.CloseConnection);

            return reader.Read() ? MapToClient(reader) : null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
            throw;
        }
    }

    private static Client MapToClient(IDataRecord record)
    {
        return new Client
        {
            Id = Convert.ToInt32(record["ClientId"]),
            Name = record["Name"].ToString(),
            ClientStatus = (ClientStatus)Convert.ToInt32(record["ClientStatus"])
        };
    }
}