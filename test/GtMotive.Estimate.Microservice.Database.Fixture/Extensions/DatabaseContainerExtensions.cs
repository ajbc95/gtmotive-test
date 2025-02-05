using DotNet.Testcontainers.Containers;

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
    }
}
