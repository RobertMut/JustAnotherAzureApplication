namespace API.AutomatedTests.Implementation.Common.Interfaces;

public interface ISqlCommandExecutor
{
    Task<int> ExecuteWithoutReturn(string query, Dictionary<string, string> parameters = null);
    Task<object?> ExecuteAndReturnData(string query, Dictionary<string, string> parameters = null);
}