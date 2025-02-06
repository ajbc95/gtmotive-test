using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GtMotive.Estimate.Microservice.Domain.Exceptions;
using GtMotive.Estimate.Microservice.Domain.Interfaces;
using GtMotive.Estimate.Microservice.Domain.Models;
using GtMotive.Estimate.Microservice.Domain.Repositories;

namespace GtMotive.Estimate.Microservice.Domain.Services
{
    /// <summary>
    /// Vehicles operations implementation.
    /// </summary>
    public class VehicleService(IVehicleRepository vehicleRepository) : IVehicleService
    {
        private readonly IVehicleRepository _vehicleRepository = vehicleRepository ?? throw new ArgumentNullException(nameof(vehicleRepository));

        /// <inheritdoc/>
        public async Task<int> CreateAsync(string brand, int productionYear)
        {
            if ((DateTime.Now.Year - productionYear) > Constants.VehicleMaxProductionYear)
            {
                throw new DomainException("Vehicle production year is too old.");
            }

            var newVehicle = await _vehicleRepository.CreateVehicleAsync(new()
            {
                Brand = brand,
                ProductionYear = productionYear,
            });

            return newVehicle.Id;
        }

        /// <inheritdoc/>
        public Task<IEnumerable<Vehicle>> ListAvailableAsync() => _vehicleRepository.GetAvailableVehiclesAsync();

        /// <inheritdoc/>
        public async Task<bool> RentAsync(int vehicleId, int customerId)
        {
            var vehicle = await Get(vehicleId);
            if (vehicle.RentedBy.HasValue)
            {
                throw new DomainException("Vehicle already rented.");
            }

            var vehiclesRentedByCustomer = await _vehicleRepository.RentedVehiclesBy(customerId);

            return vehiclesRentedByCustomer.Any()
                ? throw new DomainException("Customer already rented a vehicle.")
                : await _vehicleRepository.RentVehicleAsync(vehicleId, customerId);
        }

        /// <inheritdoc/>
        public async Task<bool> ReturnAsync(int vehicleId)
        {
            var vehicle = await Get(vehicleId);

            return !vehicle.RentedBy.HasValue
                ? throw new DomainException("Vehicle not rented.")
                : await _vehicleRepository.ReturnVehicleAsync(vehicleId);
        }

        private async Task<Vehicle> Get(int vehicleId, bool throwErrorIfNotFound = true)
        {
            var vehicle = await _vehicleRepository.GetAsync(vehicleId);
            return vehicle is null && throwErrorIfNotFound ? throw new KeyNotFoundException($"Vehicle '{vehicleId}' not found.") : vehicle;
        }
    }
}
