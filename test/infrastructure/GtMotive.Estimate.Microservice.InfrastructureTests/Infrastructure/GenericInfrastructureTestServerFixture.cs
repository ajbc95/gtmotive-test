using System;
using System.IO;
using GtMotive.Estimate.Microservice.Fixture.Database;
using GtMotive.Estimate.Microservice.Fixture.Database.Extensions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;

[assembly: CLSCompliant(false)]

namespace GtMotive.Estimate.Microservice.InfrastructureTests.Infrastructure
{
    public sealed class GenericInfrastructureTestServerFixture : IDisposable
    {
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

            ConnectionString = sqlContainer.Result.GetGtMotiveConnectionString();
            Server = new TestServer(hostBuilder);
        }

        public TestServer Server { get; }

        public string ConnectionString { get; private set; }

        /// <inheritdoc />
        public void Dispose()
        {
            Server?.Dispose();
        }
    }
}
