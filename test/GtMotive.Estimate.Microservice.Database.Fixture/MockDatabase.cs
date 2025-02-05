using System.Reflection;
using DotNet.Testcontainers.Containers;
using Microsoft.SqlServer.Dac;
using Testcontainers.MsSql;

namespace GtMotive.Estimate.Microservice.Fixture.Database
{
    /// <summary>
    /// Mock database.
    /// </summary>
    public static class MockDatabase
    {
        /// <summary>
        /// Deploy a SQL Server container with a mock database.
        /// </summary>
        /// <param name="hostPort">Host port.</param>
        /// <param name="includeBacpac">Include bacpac with test data.</param>
        /// <returns>The DB container instante.</returns>
        public static async Task<IDatabaseContainer> DeploySqlServerAsync(int? hostPort = null, bool includeBacpac = true)
        {
            var containerName = $"gtmotive-sqlserver-{Guid.NewGuid().ToString()[..5]}";
            var containerImage = "mcr.microsoft.com/mssql/server:2022-latest";
            var sqlServerPort = 1433;

            var sqlContainer = new MsSqlBuilder().WithName(containerName).WithImage(containerImage).WithPortBinding(sqlServerPort, true).Build();

            if (hostPort.HasValue)
            {
                sqlContainer = new MsSqlBuilder().WithName(containerName).WithImage(containerImage).WithPortBinding(hostPort.Value, sqlServerPort).Build();
            }

            await sqlContainer.StartAsync();

            if (includeBacpac)
            {
                var assembly = Assembly.GetExecutingAssembly();
                var bacpacStream = assembly.GetManifestResourceStream($"{assembly.GetName().Name}.SqlServer.{Constants.SqlServerDatabaseName}.bacpac");
                var bacpac = BacPackage.Load(bacpacStream);

                var dacServices = new DacServices(sqlContainer.GetConnectionString());
                dacServices.ImportBacpac(bacpac, Constants.SqlServerDatabaseName);
            }

            return sqlContainer;
        }
    }
}
