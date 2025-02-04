using System;
using System.Data;
using System.Reflection;
using System.Threading.Tasks;
using GtMotive.Estimate.Microservice.Infrastructure.SqlServer.Settings;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using Microsoft.SqlServer.Dac;
using Testcontainers.MsSql;

namespace GtMotive.Estimate.Microservice.Infrastructure.SqlServer
{
    /// <summary>
    /// SQL Server service.
    /// </summary>
    public class SqlServerService(IOptions<SqlServerDbSettings> dbSettings)
    {
        public IDbConnection SqlServerConnection { get; private set; } = new SqlConnection(dbSettings?.Value.ConnectionString);

        /// <summary>
        /// Run a SQL Server container with a mock database.
        /// </summary>
        /// <param name="dbName">Database name.</param>
        /// <returns>Mock database connection string.</returns>
        public static async Task<string> RunMockDatabaseAsync(string dbName = "GtMotive")
        {
            var sqlContainer = new MsSqlBuilder()
                .WithName($"gtmotive-sqlserver-{Guid.NewGuid().ToString()[..5]}")
                .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
                .WithPortBinding(1434, 1433)
                .Build();
            await sqlContainer.StartAsync();
            var connString = sqlContainer.GetConnectionString();

            var assembly = Assembly.GetExecutingAssembly();
            var bacpacStream = assembly.GetManifestResourceStream($"{assembly.GetName().Name}.SqlServer.Mock.{dbName}.bacpac");
            var bacpac = BacPackage.Load(bacpacStream);

            var dacServices = new DacServices(connString);
            dacServices.ImportBacpac(bacpac, dbName);

            return connString.Replace("master", dbName, System.StringComparison.InvariantCulture);
        }
    }
}
