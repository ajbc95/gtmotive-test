using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using FluentAssertions;
using GtMotive.Estimate.Microservice.Domain.Exceptions;
using GtMotive.Estimate.Microservice.Domain.Models;
using GtMotive.Estimate.Microservice.Domain.Repositories;
using GtMotive.Estimate.Microservice.Domain.Services;
using Moq;
using Xunit;

namespace GtMotive.Estimate.Microservice.UnitTests.Domain.Services
{
    /// <summary>
    /// VehicleService tests.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class VehicleServiceTests
    {
        private readonly Mock<IVehicleRepository> _vehicleRepositoryMock;
        private readonly VehicleService _service;

        /// <summary>
        /// Initializes a new instance of the <see cref="VehicleServiceTests"/> class.
        /// Constructor.
        /// </summary>
        public VehicleServiceTests()
        {
            _vehicleRepositoryMock = new Mock<IVehicleRepository>();
            _service = new VehicleService(_vehicleRepositoryMock.Object);
        }

        /// <summary>
        /// CreateAsync should throw a DomainException when the production year is too old.
        /// </summary>
        /// <returns>Task.</returns>
        [Fact]
        public async Task CreateAsync_Should_Throw_DomainException_When_ProductionYearIsTooOld()
        {
            // Arrange
            var brand = "Toyota";
            var productionYear = 1990;

            // Act & Assert
            await Assert.ThrowsAsync<DomainException>(() => _service.CreateAsync(brand, productionYear));
        }

        /// <summary>
        /// RentAsync should throw a DomainException when the vehicle is already rented.
        /// </summary>
        /// <returns>Task.</returns>
        [Fact]
        public async Task RentAsync_Should_Throw_DomainException_When_VehicleAlreadyRented()
        {
            // Arrange
            var vehicleId = 1;
            var customerId = 1;
            var vehicle = new Vehicle { Id = vehicleId, RentedBy = 2 };
            _vehicleRepositoryMock.Setup(repo => repo.GetAsync(vehicleId)).ReturnsAsync(vehicle);

            // Act & Assert
            await Assert.ThrowsAsync<DomainException>(() => _service.RentAsync(vehicleId, customerId));
        }

        /// <summary>
        /// CreateAsync should create a new vehicle.
        /// </summary>
        /// <returns>Task.</returns>
        [Fact]
        public async Task CreateAsync_Should_Create_NewVehicle()
        {
            // Arrange
            var brand = "Toyota";
            var productionYear = DateTime.Now.Year;
            var vehicle = new Vehicle { Id = 1, Brand = brand, ProductionYear = productionYear };
            _vehicleRepositoryMock.Setup(repo => repo.CreateVehicleAsync(It.IsAny<Vehicle>())).ReturnsAsync(vehicle);

            // Act
            var result = await _service.CreateAsync(brand, productionYear);

            // Assert
            Assert.Equal(vehicle.Id, result);
        }

        /// <summary>
        /// ListAvailableAsync should return available vehicles.
        /// </summary>
        /// <returns>Task.</returns>
        [Fact]
        public async Task ListAvailableAsync_Should_Return_AvailableVehicles()
        {
            // Arrange
            var vehicles = new List<Vehicle> { new() { Id = 1, Brand = "Toyota", ProductionYear = DateTime.Now.Year } };
            _vehicleRepositoryMock.Setup(repo => repo.GetAvailableVehiclesAsync()).ReturnsAsync(vehicles);

            // Act
            var result = await _service.ListAvailableAsync();

            // Assert
            Assert.Equal(vehicles, result);
        }

        /// <summary>
        /// RentAsync should throw a DomainException when the customer already rented a vehicle.
        /// </summary>
        /// <returns>Task.</returns>
        [Fact]
        public async Task RentAsync_Should_Throw_DomainException_When_CustomerAlreadyRentedVehicle()
        {
            // Arrange
            var vehicleId = 1;
            var customerId = 1;
            var vehicle = new Vehicle { Id = vehicleId };
            var rentedVehicles = new List<Vehicle> { new() { Id = 2, RentedBy = customerId } };
            _vehicleRepositoryMock.Setup(repo => repo.GetAsync(vehicleId)).ReturnsAsync(vehicle);
            _vehicleRepositoryMock.Setup(repo => repo.RentedVehiclesBy(customerId)).ReturnsAsync(rentedVehicles);

            // Act & Assert
            await Assert.ThrowsAsync<DomainException>(() => _service.RentAsync(vehicleId, customerId));
        }

        /// <summary>
        /// RentAsync should rent a vehicle.
        /// </summary>
        /// <returns>Task.</returns>
        [Fact]
        public async Task RentAsync_Should_RentVehicle()
        {
            // Arrange
            var vehicleId = 1;
            var customerId = 1;
            var vehicle = new Vehicle { Id = vehicleId };
            _vehicleRepositoryMock.Setup(repo => repo.GetAsync(vehicleId)).ReturnsAsync(vehicle);
            _vehicleRepositoryMock.Setup(repo => repo.RentedVehiclesBy(customerId)).ReturnsAsync([]);
            _vehicleRepositoryMock.Setup(repo => repo.RentVehicleAsync(vehicleId, customerId)).ReturnsAsync(true);

            // Act
            var result = await _service.RentAsync(vehicleId, customerId);

            // Assert
            result.Should().BeTrue();
        }

        /// <summary>
        /// ReturnAsync should throw a DomainException when the vehicle is not rented.
        /// </summary>
        /// <returns>Task.</returns>
        [Fact]
        public async Task ReturnAsync_Should_Throw_DomainException_When_VehicleNotRented()
        {
            // Arrange
            var vehicleId = 1;
            var vehicle = new Vehicle { Id = vehicleId };
            _vehicleRepositoryMock.Setup(repo => repo.GetAsync(vehicleId)).ReturnsAsync(vehicle);

            // Act & Assert
            await Assert.ThrowsAsync<DomainException>(() => _service.ReturnAsync(vehicleId));
        }

        /// <summary>
        /// ReturnAsync should return a vehicle.
        /// </summary>
        /// <returns>Task.</returns>
        [Fact]
        public async Task ReturnAsync_Should_ReturnVehicle()
        {
            // Arrange
            var vehicleId = 1;
            var vehicle = new Vehicle { Id = vehicleId, RentedBy = 1 };
            _vehicleRepositoryMock.Setup(repo => repo.GetAsync(vehicleId)).ReturnsAsync(vehicle);
            _vehicleRepositoryMock.Setup(repo => repo.ReturnVehicleAsync(vehicleId)).ReturnsAsync(true);

            // Act
            var result = await _service.ReturnAsync(vehicleId);

            // Assert
            result.Should().BeTrue();
        }
    }
}
