using GtMotive.Estimate.Microservice.Fixture.Database.Extensions;

namespace GtMotive.Estimate.Microservice.Fixture.Database
{
    internal sealed class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("- Running Sql Server container.");
            Console.WriteLine();
            var containerTask = MockDatabase.DeploySqlServerAsync(1434, true);
            containerTask.Wait();
            Console.WriteLine();
            Console.WriteLine($"- Container name: {containerTask.Result.Name}");
            Console.WriteLine($"- Connection string: {containerTask.Result.GetGtMotiveConnectionString()}");
            Console.WriteLine();
            Console.WriteLine($"- Sql Server container running. Press any key for removing it.");
            Console.ReadLine();
        }
    }
}
