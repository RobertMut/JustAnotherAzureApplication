using Domain.Constants.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
namespace Infrastructure.Persistence;

/// <summary>
/// Abstract DesignTimeDbContextFactoryBase
/// </summary>
/// <typeparam name="TContext">DbContext</typeparam>
public abstract class DesignTimeDbContextFactoryBase<TContext> : IDesignTimeDbContextFactory<TContext> where TContext : DbContext
{
    private const string AspNetCoreEvironment = "ASPNETCORE_ENVIRONMENT";

    /// <summary>
    /// Create DbContext
    /// </summary>
    /// <param name="args"></param>
    /// <returns>Context</returns>
    public TContext CreateDbContext(string[] args)
    {
        var basePath = Directory.GetCurrentDirectory() + String.Format("{0}..{0}API", Path.DirectorySeparatorChar);

        return Create(basePath, Environment.GetEnvironmentVariable(AspNetCoreEvironment));
    }

    protected abstract TContext CreateNewInstance(DbContextOptions<TContext> options);

    /// <summary>
    /// Creates instance based on conenction string
    /// </summary>
    /// <param name="basePath">current directory</param>
    /// <param name="environmentName">Environment name</param>
    /// <returns>Dbcontext</returns>
    private TContext Create(string basePath, string environmentName)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.json")
            .AddJsonFile($"appsettings.Local.json", optional: true)
            .AddJsonFile($"appsettings.{environmentName}.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        string connectionString = configuration.GetConnectionString(Database.ConnectionStringName);

        if (string.IsNullOrEmpty(connectionString))
        {
            throw new Exception("Connection string is empty");
        }
            
        return Create(connectionString);
    }

    /// <summary>
    /// Gets instance of dbcontext from connection string
    /// </summary>
    /// <param name="connectionString">Connection string</param>
    /// <returns>DbContext</returns>
    /// <exception cref="ArgumentException">If connection string is null or empty</exception>
    private TContext Create(string connectionString)
    {
        if (string.IsNullOrEmpty(connectionString))
        {
            throw new ArgumentException($"Connection string '{Database.ConnectionStringName}' is null or empty", nameof(connectionString));
        }

        var optionsBuilder = new DbContextOptionsBuilder<TContext>();

        optionsBuilder.UseSqlServer(connectionString);

        return CreateNewInstance(optionsBuilder.Options);
    }
}