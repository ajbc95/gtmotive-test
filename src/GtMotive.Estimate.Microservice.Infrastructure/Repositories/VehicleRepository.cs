using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using GtMotive.Estimate.Microservice.Domain.Models;
using GtMotive.Estimate.Microservice.Domain.Repositories;
using GtMotive.Estimate.Microservice.Infrastructure.SqlServer;

namespace GtMotive.Estimate.Microservice.Infrastructure.Repositories
{
    public class VehicleRepository(SqlServerService sqlServerService) : IVehicleRepository
    {
        private readonly SqlServerService _sqlServerService = sqlServerService;

        /// <inheritdoc/>
        public async Task<Vehicle> CreateVehicleAsync(Vehicle vehicle)
        {
            System.ArgumentNullException.ThrowIfNull(vehicle);

            var query = "INSERT INTO [app].[vehicle] ([brand], [productionYear], [rentedBy]) VALUES (@Brand, @ProductionYear, @rentedBy); SELECT @@IDENTITY";
            vehicle.Id = await _sqlServerService.SqlServerConnection.QuerySingleAsync<int>(query, new { vehicle.Brand, vehicle.ProductionYear, vehicle.RentedBy });

            return vehicle;
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<Vehicle>> GetAvailableVehiclesAsync()
        {
            var query = "SELECT * FROM [app].[vehicle] WHERE [rentedBy] IS NULL";

            return await _sqlServerService.SqlServerConnection.QueryAsync<Vehicle>(query);
        }

        /// <inheritdoc/>
        public async Task<Vehicle> GetAsync(int id)
        {
            var query = "SELECT * FROM [app].[vehicle] WHERE [id] = @id";

            return await _sqlServerService.SqlServerConnection.QueryFirstOrDefaultAsync<Vehicle>(query, new { id });
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<Vehicle>> RentedVehiclesBy(int customerId)
        {
            var query = "SELECT * FROM [app].[vehicle] WHERE [rentedBy] = @customerId";
            return await _sqlServerService.SqlServerConnection.QueryAsync<Vehicle>(query, new { customerId });
        }

        /// <inheritdoc/>
        public async Task<bool> RentVehicleAsync(int vehicleId, int customerId)
        {
            var query = "UPDATE [app].[vehicle] SET [rentedBy] = @customerId WHERE Id = @vehicleId";

            var rowsAffected = await _sqlServerService.SqlServerConnection.ExecuteAsync(query, new { vehicleId, customerId });
            return rowsAffected > 0;
        }

        /// <inheritdoc/>
        public async Task<bool> ReturnVehicleAsync(int vehicleId)
        {
            var query = "UPDATE [app].[vehicle] SET [rentedBy] = NULL WHERE Id = @vehicleId";

            var rowsAffected = await _sqlServerService.SqlServerConnection.ExecuteAsync(query, new { vehicleId });
            return rowsAffected > 0;
        }
    }
}
