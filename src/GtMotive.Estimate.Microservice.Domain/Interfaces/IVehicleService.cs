using System.Collections.Generic;
using System.Threading.Tasks;
using GtMotive.Estimate.Microservice.Domain.Models;

namespace GtMotive.Estimate.Microservice.Domain.Interfaces
{
    /// <summary>
    /// Defines vehicles operations.
    /// </summary>
    public interface IVehicleService
    {
        /// <summary>
        /// List non-rented vehicles.
        /// </summary>
        /// <returns>List of available vehicles.</returns>
        public Task<IEnumerable<Vehicle>> ListAvailableAsync();

        /// <summary>
        /// Creates a new vehicle.
        /// </summary>
        /// <param name="brand">Vehicle brand.</param>
        /// <param name="productionYear">Vehicle production year.</param>
        /// <returns>Vehicle identifier.</returns>
        public Task<int> CreateAsync(string brand, int productionYear);

        /// <summary>
        /// Rent a vehicle.
        /// </summary>
        /// <param name="vehicleId">Vehicle identifier.</param>
        /// <param name="customerId">Customer identifier.</param>
        /// <returns>True if renting was correct.</returns>
        public Task<bool> RentAsync(int vehicleId, int customerId);

        /// <summary>
        /// Return a vehicle.
        /// </summary>
        /// <param name="vehicleId">Vehicle identifier.</param>
        /// <returns>True if returning was correct.</returns>
        public Task<bool> ReturnAsync(int vehicleId);
    }
}
