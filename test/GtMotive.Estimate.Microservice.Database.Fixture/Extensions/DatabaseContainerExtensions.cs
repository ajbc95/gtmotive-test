using System.Data;
using System.Reflection;
using Dapper;
using DotNet.Testcontainers.Containers;
using Microsoft.Data.SqlClient;
using Microsoft.SqlServer.Dac;
using Respawn;
using Testcontainers.MsSql;

namespace GtMotive.Estimate.Microservice.Fixture.Database.Extensions
{
    /// <summary>
    /// Database container extensions.
    /// </summary>
    public static class DatabaseContainerExtensions
    {
        /// <summary>
        /// Get GtMotive connection string.
        /// </summary>
        /// <param name="container">Container instance.</param>
        /// <returns>Connection string.</returns>
        public static string GetGtMotiveConnectionString(this IDatabaseContainer container)
        {
            ArgumentNullException.ThrowIfNull(container);

            return container.GetConnectionString().Replace("master", Constants.SqlServerDatabaseName, StringComparison.InvariantCulture);
        }

        /// <summary>
        /// Execute SQL command.
        /// </summary>
        /// <typeparam name="T">Return type.</typeparam>
        /// <param name="sqlContainer">Sql Server container.</param>
        /// <param name="sqlCommand">Sql command.</param>
        /// <returns>Queried data.</returns>
        public static async Task<IEnumerable<T>> QuerySqlAsync<T>(this MsSqlContainer sqlContainer, string sqlCommand)
        {
            using IDbConnection db = new SqlConnection(sqlContainer.GetGtMotiveConnectionString());
            return await db.QueryAsync<T>(sqlCommand, commandType: CommandType.Text);
        }

        /// <summary>
        /// Deploy test data.
        /// </summary>
        /// <param name="sqlContainer">Sql Server container.</param>
        public static void DeployTestData(this MsSqlContainer sqlContainer)
        {
            ArgumentNullException.ThrowIfNull(sqlContainer);

            var assembly = Assembly.GetExecutingAssembly();
            var bacpacStream = assembly.GetManifestResourceStream($"{assembly.GetName().Name}.SqlServer.{Constants.SqlServerDatabaseName}.bacpac");
            var bacpac = BacPackage.Load(bacpacStream);

            var dacServices = new DacServices(sqlContainer.GetConnectionString());
            dacServices.ImportBacpac(bacpac, Constants.SqlServerDatabaseName);
        }

        /// <summary>
        /// Get SQL respawner.
        /// </summary>
        /// <param name="sqlContainer">Sql Server container.</param>
        /// <returns>Respawner instance.</returns>
        public static Task<Respawner> GetSqlRespawnerAsync(this MsSqlContainer sqlContainer) => Respawner.CreateAsync(sqlContainer.GetGtMotiveConnectionString(), new RespawnerOptions()
        {
            CheckTemporalTables = true,
            SchemasToInclude = ["app"],
            TablesToIgnore =
            [
                new Respawn.Graph.Table("app", "customer")
            ]
        });
    }
}
