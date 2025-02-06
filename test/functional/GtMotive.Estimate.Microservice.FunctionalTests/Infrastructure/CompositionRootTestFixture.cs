using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using GtMotive.Estimate.Microservice.Api;
using GtMotive.Estimate.Microservice.Fixture.Database;
using GtMotive.Estimate.Microservice.Fixture.Database.Extensions;
using GtMotive.Estimate.Microservice.Host;
using GtMotive.Estimate.Microservice.Infrastructure;
using MediatR;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Respawn;
using Testcontainers.MsSql;
using Xunit;

[assembly: CLSCompliant(false)]

namespace GtMotive.Estimate.Microservice.FunctionalTests.Infrastructure
{
    public sealed class CompositionRootTestFixture : IDisposable, IAsyncLifetime
    {
        private readonly ServiceProvider _serviceProvider;
        private readonly WebApplicationFactory<Program> _factory;
        private MsSqlContainer _sqlServer;
        private Respawner _sqlRespawner;

        public CompositionRootTestFixture()
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.Development.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Development");

            var services = new ServiceCollection();
            Configuration = configuration;
            ConfigureServices(services);
            services.AddSingleton<IConfiguration>(configuration);
            _serviceProvider = services.BuildServiceProvider();

            _factory = new WebApplicationFactory<Program>();
            Client = _factory.CreateClient();
        }

        public IConfiguration Configuration { get; }

        public HttpClient Client { get; }

        public void DeployInitialTestData() => _sqlServer.DeployTestData();

        public Task RestoreDatabaseAsync() => _sqlRespawner.ResetAsync(_sqlServer.GetGtMotiveConnectionString());

        public async Task InitializeAsync()
        {
            _sqlServer = await MockDatabase.DeploySqlServerAsync(1435);
            _sqlRespawner = await _sqlServer.GetSqlRespawnerAsync();
        }

        public async Task DisposeAsync()
        {
            await _sqlServer.StopAsync();
            await _sqlServer.DisposeAsync();
        }

        public async Task UsingHandlerForRequest<TRequest>(Func<IRequestHandler<TRequest, Unit>, Task> handlerAction)
            where TRequest : IRequest
        {
            ArgumentNullException.ThrowIfNull(handlerAction);

            using var scope = _serviceProvider.CreateScope();
            var handler = scope.ServiceProvider.GetRequiredService<IRequestHandler<TRequest, Unit>>();

            await handlerAction.Invoke(handler);
        }

        public async Task UsingHandlerForRequestResponse<TRequest, TResponse>(Func<IRequestHandler<TRequest, TResponse>, Task> handlerAction)
            where TRequest : IRequest<TResponse>
        {
            ArgumentNullException.ThrowIfNull(handlerAction);

            using var scope = _serviceProvider.CreateScope();
            var handler = scope.ServiceProvider.GetRequiredService<IRequestHandler<TRequest, TResponse>>();

            if (handler == null)
            {
                Debug.Fail("The requested handler has not been registered");
            }

            await handlerAction.Invoke(handler);
        }

        public async Task UsingRepository<TRepository>(Func<TRepository, Task> handlerAction)
        {
            ArgumentNullException.ThrowIfNull(handlerAction);

            using var scope = _serviceProvider.CreateScope();
            var handler = scope.ServiceProvider.GetRequiredService<TRepository>();

            if (handler == null)
            {
                Debug.Fail("The requested handler has not been registered");
            }

            await handlerAction.Invoke(handler);
        }

        public void Dispose()
        {
            _serviceProvider.Dispose();
            _factory.Dispose();
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            services.AddApiDependencies();
            services.AddLogging();
            services.AddBaseInfrastructure(true);
        }
    }
}
