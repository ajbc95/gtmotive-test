using GtMotive.Estimate.Microservice.Fixture.Database.Extensions;

namespace GtMotive.Estimate.Microservice.Fixture.Database
{
    internal sealed class Program
    {
        public static async Task Main(string[] args)
        {
            Console.WriteLine("- Running Sql Server container.");
            Console.WriteLine();
            var container = await MockDatabase.DeploySqlServerAsync(1434, true);
            var customers = await container.QuerySqlAsync<dynamic>("SELECT * FROM app.Customer");
            Console.WriteLine();

            Console.WriteLine($"- Container name: {container.Name}");
            Console.WriteLine($"- Connection string: {container.GetGtMotiveConnectionString()}");
            Console.WriteLine($"- Sample customers Ids: {string.Join(", ", customers.Select(_ => _.id).AsEnumerable())}");
            Console.WriteLine();
            Console.WriteLine($"- Sql Server container running. Press any key for removing it...");
            Console.ReadLine();

            Console.WriteLine($"- Stopping Sql Server container.");
            await container.StopAsync();
            await container.DisposeAsync();
        }
    }
}
