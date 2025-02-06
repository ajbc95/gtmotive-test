using System.Collections.Generic;
using System.Threading.Tasks;
using GtMotive.Estimate.Microservice.Domain.Models;

namespace GtMotive.Estimate.Microservice.Domain.Repositories
{
    /// <summary>
    /// Vehicle repository.
    /// </summary>
    public interface IVehicleRepository
    {
        /// <summary>
        /// Creates a new vehicle.
        /// </summary>
        /// <param name="vehicle">Vehicle metadata.</param>
        /// <returns>Vehicle identifier.</returns>
        Task<Vehicle> CreateVehicleAsync(Vehicle vehicle);

        /// <summary>
        /// List non-rented vehicles.
        /// </summary>
        /// <returns>List of available vehicles.</returns>
        Task<IEnumerable<Vehicle>> GetAvailableVehiclesAsync();

        /// <summary>
        /// Get a vehicle by identifier.
        /// </summary>
        /// <param name="id">Vehicle identifier.</param>
        /// <returns>Vehicle found, null otherwhise.</returns>
        Task<Vehicle> GetAsync(int id);

        /// <summary>
        /// Rent a vehicle.
        /// </summary>
        /// <param name="vehicleId">Vehicle identifier.</param>
        /// <param name="customerId">Customer identifier.</param>
        /// <returns>True if renting was correct.</returns>
        Task<bool> RentVehicleAsync(int vehicleId, int customerId);

        /// <summary>
        /// Return a vehicle.
        /// </summary>
        /// <param name="vehicleId">Vehicle identifier.</param>
        /// <returns>True if returning was correct.</returns>
        Task<bool> ReturnVehicleAsync(int vehicleId);

        /// <summary>
        /// Get vehicles rented by a customer.
        /// </summary>
        /// <param name="customerId">Customer identifier.</param>
        /// <returns>Rented vehicles by customer.</returns>
        Task<IEnumerable<Vehicle>> RentedVehiclesBy(int customerId);
    }
}
