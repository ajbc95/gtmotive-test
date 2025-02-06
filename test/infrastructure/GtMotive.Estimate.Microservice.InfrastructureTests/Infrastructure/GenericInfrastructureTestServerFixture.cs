using System;
using System.IO;
using System.Threading.Tasks;
using GtMotive.Estimate.Microservice.Fixture.Database;
using GtMotive.Estimate.Microservice.Fixture.Database.Extensions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Respawn;
using Testcontainers.MsSql;

[assembly: CLSCompliant(false)]

namespace GtMotive.Estimate.Microservice.InfrastructureTests.Infrastructure
{
    public sealed class GenericInfrastructureTestServerFixture : IDisposable
    {
        private readonly MsSqlContainer _sqlServer;
        private readonly Respawner _sqlRespawner;

        public GenericInfrastructureTestServerFixture()
        {
            var hostBuilder = new WebHostBuilder()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseEnvironment("IntegrationTest")
                .UseDefaultServiceProvider(options => { options.ValidateScopes = true; })
                .ConfigureAppConfiguration((context, builder) => { builder.AddEnvironmentVariables(); })
                .UseStartup<Startup>();

            var sqlContainer = MockDatabase.DeploySqlServerAsync();
            sqlContainer.Wait();
            _sqlServer = sqlContainer.Result;
            var respawn = _sqlServer.GetSqlRespawnerAsync();
            respawn.Wait();
            _sqlRespawner = respawn.Result;

            ConnectionString = sqlContainer.Result.GetGtMotiveConnectionString();
            Server = new TestServer(hostBuilder);
        }

        public TestServer Server { get; }

        public string ConnectionString { get; private set; }

        public void DeployInitialTestData() => _sqlServer.DeployTestData();

        public Task RestoreDatabaseAsync() => _sqlRespawner.ResetAsync(_sqlServer.GetGtMotiveConnectionString());

        /// <inheritdoc />
        public void Dispose()
        {
            _sqlServer.StopAsync().Wait();
            _sqlServer.DisposeAsync().AsTask().Wait();
            Server?.Dispose();
        }
    }
}
