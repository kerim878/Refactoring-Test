using System.Configuration;
using System.Data;
using LegacyApp.Extensions;
using Microsoft.Data.SqlClient;

namespace LegacyApp;

public class UserDataAccess : IUserDataAccess
{
    public void AddUser(User user)
    {
        // According to the README.md file, the method and the class should be static. So, I'm not using a constructor to get the connection string.
        var connectionString = ConfigurationManager.ConnectionStrings["appDatabase"].ConnectionString;
        using var connection = new SqlConnection(connectionString);

        try
        {
            var parameters = new Dictionary<string, object>
            {
                { "@Firstname", user.Firstname },
                { "@Surname", user.Surname },
                { "@DateOfBirth", user.DateOfBirth },
                { "@EmailAddress", user.EmailAddress },
                { "@HasCreditLimit", user.HasCreditLimit },
                { "@CreditLimit", user.CreditLimit },
                { "@ClientId", user.Client.Id }
            };

            var command = connection.CreateCommand("uspAddUser", CommandType.StoredProcedure, parameters);
            connection.Open();

            var result = command.ExecuteNonQuery();
            if (result == 0)
            {
                throw new Exception("User not added");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            throw;
        }
        finally
        {
            if (connection.State == ConnectionState.Open)
            {
                connection.Close();
            }
        }
    }
}