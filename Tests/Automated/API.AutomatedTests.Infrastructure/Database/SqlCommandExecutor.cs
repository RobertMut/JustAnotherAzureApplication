using System.Data;
using API.AutomatedTests.Implementation.Common.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace API.AutomatedTests.Infrastructure.Database;

public class SqlCommandExecutor : ISqlCommandExecutor
{
    private readonly SqlConnection connection;

    public SqlCommandExecutor(IConfiguration configuration)
    {
        string connectionString = configuration.GetConnectionString("JAAADatabase");
        connection = new SqlConnection(connectionString);
    }

    public async Task<int> ExecuteWithoutReturn(string query, Dictionary<string, string> parameters = null)
    {
        int rowsAffected;
        
        using (var command = new SqlCommand(query, connection))
        {
            
            if (parameters != null)
            {
                command.Parameters.AddRange(GetParameter(parameters));
            }
            
            try
            {
                await command.Connection.OpenAsync();
                rowsAffected = await command.ExecuteNonQueryAsync();
            }
            catch (SqlException e)
            {
                throw new Exception(e.ToString(), e.InnerException);
            }
            finally
            {
                await command.Connection.CloseAsync();
            }
        }

        return rowsAffected;
    }

    public async Task<object?> ExecuteAndReturnData(string query, Dictionary<string, string> parameters = null)
    {
        object? data;
        using (var command = new SqlCommand(query, connection))
        {
            if (parameters != null)
            {
                command.Parameters.AddRange(GetParameter(parameters));
            }

            try
            {
                
                await command.Connection.OpenAsync();
                data = await command.ExecuteScalarAsync();
            }
            catch (SqlException e)
            {
                throw new Exception(e.ToString(), e.InnerException);
            }
            finally
            {
                await command.Connection.CloseAsync();
            }
        }

        return data;
    }

    private SqlParameter[] GetParameter(IDictionary<string, string> parameters) =>
        parameters.Select(pair =>
            new SqlParameter
            {
                ParameterName = pair.Key,
                Direction = ParameterDirection.Input,
                Value = pair.Value
            }).ToArray();
}