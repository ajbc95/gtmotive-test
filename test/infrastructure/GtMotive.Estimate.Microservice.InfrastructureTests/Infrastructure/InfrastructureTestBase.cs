using System.Threading.Tasks;
using Xunit;

namespace GtMotive.Estimate.Microservice.InfrastructureTests.Infrastructure
{
    [Collection(TestCollections.TestServer)]
    public abstract class InfrastructureTestBase(GenericInfrastructureTestServerFixture fixture) : IAsyncLifetime
    {
        protected GenericInfrastructureTestServerFixture Fixture { get; } = fixture;

        public Task DisposeAsync()
        {
            return Task.CompletedTask;
        }

        public async Task InitializeAsync()
        {
            await Fixture.RestoreDatabaseAsync();
        }
    }
}
