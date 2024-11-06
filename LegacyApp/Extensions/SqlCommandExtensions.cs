using System.Data;
using Microsoft.Data.SqlClient;

namespace LegacyApp.Extensions;

/// <summary>
///     Extension methods for SqlCommand to simplify parameter handling.
/// </summary>
public static class SqlCommandExtensions
{
    /// <summary>
    ///     Creates a SqlCommand using a dictionary of parameters.
    /// </summary>
    /// <param name="connection">The SqlConnection to use.</param>
    /// <param name="commandText">The SQL command text or stored procedure name.</param>
    /// <param name="commandType">The type of the command (e.g., Text, StoredProcedure).</param>
    /// <param name="parameters">A dictionary of parameter names and values. Can be null or empty.</param>
    /// <returns>A configured SqlCommand.</returns>
    public static SqlCommand CreateCommand(this SqlConnection connection, string commandText, CommandType commandType, IDictionary<string, object> parameters = null)
    {
        var command = new SqlCommand
        {
            Connection = connection,
            CommandType = commandType,
            CommandText = commandText
        };

        if (parameters != null && parameters.Count > 0)
        {
            command.AddParameters(parameters);
        }

        return command;
    }

    /// <summary>
    ///     Adds parameters to a SqlCommand from a dictionary.
    /// </summary>
    /// <param name="command">The SqlCommand to add parameters to.</param>
    /// <param name="parameters">A dictionary of parameter names and values.</param>
    public static void AddParameters(this SqlCommand command, IDictionary<string, object> parameters)
    {
        foreach (var kvp in parameters)
        {
            var sqlDbType = GetSqlDbType(kvp.Value.GetType());
            if (sqlDbType.HasValue)
            {
                command.Parameters.Add(command.CreateParameter(kvp.Key, sqlDbType.Value, kvp.Value));
            }
        }
    }

    /// <summary>
    ///     Creates a SqlParameter.
    /// </summary>
    /// <param name="command">The SqlCommand to add the parameter to.</param>
    /// <param name="name">The name of the parameter.</param>
    /// <param name="type">The SqlDbType of the parameter.</param>
    /// <param name="value">The value of the parameter.</param>
    /// <returns>A configured SqlParameter.</returns>
    public static SqlParameter CreateParameter(this SqlCommand command, string name, SqlDbType type, object value)
    {
        return new SqlParameter(name, type) { Value = value ?? DBNull.Value };
    }

    /// <summary>
    ///     Maps a .NET type to a SqlDbType.
    /// </summary>
    /// <param name="type">The .NET type to map.</param>
    /// <returns>The corresponding SqlDbType, or null if no mapping exists.</returns>
    private static SqlDbType? GetSqlDbType(Type type)
    {
        return type switch
        {
            _ when type == typeof(string) => SqlDbType.VarChar,
            _ when type == typeof(int) => SqlDbType.Int,
            _ when type == typeof(bool) => SqlDbType.Bit,
            _ when type == typeof(DateTime) => SqlDbType.DateTime,
            _ when type == typeof(decimal) => SqlDbType.Decimal,
            _ when type == typeof(double) => SqlDbType.Float,
            _ when type == typeof(Guid) => SqlDbType.UniqueIdentifier,
            _ => null // We can add more mappings as needed
        };
    }
}