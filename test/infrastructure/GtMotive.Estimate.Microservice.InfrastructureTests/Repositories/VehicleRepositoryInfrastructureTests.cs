using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using FluentAssertions;
using GtMotive.Estimate.Microservice.Domain.Models;
using GtMotive.Estimate.Microservice.Infrastructure.Repositories;
using GtMotive.Estimate.Microservice.Infrastructure.SqlServer;
using GtMotive.Estimate.Microservice.Infrastructure.SqlServer.Settings;
using GtMotive.Estimate.Microservice.InfrastructureTests.Infrastructure;
using Microsoft.Extensions.Options;
using Xunit;

namespace GtMotive.Estimate.Microservice.InfrastructureTests.Repositories
{
    [ExcludeFromCodeCoverage]
    public class VehicleRepositoryInfrastructureTests : InfrastructureTestBase
    {
        private readonly VehicleRepository _repository;

        public VehicleRepositoryInfrastructureTests(GenericInfrastructureTestServerFixture fixture)
            : base(fixture)
        {
            ArgumentNullException.ThrowIfNull(fixture);

            var dbSettings = Options.Create(new SqlServerDbSettings { ConnectionString = fixture.ConnectionString });
            _repository = new VehicleRepository(new SqlServerService(dbSettings));
        }

        [Fact]
        public async Task CreateVehicleAsync_Should_InsertVehicle()
        {
            // Arrange
            var vehicle = new Vehicle { Brand = "Toyota", ProductionYear = DateTime.Now.Year };

            // Act
            var result = await _repository.CreateVehicleAsync(vehicle);

            // Assert
            result.Id.Should().BeGreaterThan(0);
            result.Brand.Should().Be(vehicle.Brand);
            result.ProductionYear.Should().Be(vehicle.ProductionYear);
            result.RentedBy.Should().BeNull();
        }

        [Fact]
        public async Task GetAvailableVehiclesAsync_Should_ReturnAvailableVehicles()
        {
            // Arrange
            var vehicle1 = new Vehicle { Brand = "Fiat", ProductionYear = DateTime.Now.Year };
            var vehicle2 = new Vehicle { Brand = "Honda", ProductionYear = DateTime.Now.Year, RentedBy = 1 };
            await _repository.CreateVehicleAsync(vehicle1);
            await _repository.CreateVehicleAsync(vehicle2);

            // Act
            var result = await _repository.GetAvailableVehiclesAsync();

            // Assert
            result.Should().ContainSingle(v => v.Brand == "Fiat");
            result.Should().NotContain(v => v.Brand == "Honda");
        }

        [Fact]
        public async Task GetAsync_Should_ReturnVehicleById()
        {
            // Arrange
            var vehicle = new Vehicle { Brand = "Toyota", ProductionYear = DateTime.Now.Year };
            var createdVehicle = await _repository.CreateVehicleAsync(vehicle);

            // Act
            var result = await _repository.GetAsync(createdVehicle.Id);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(createdVehicle.Id);
            result.Brand.Should().Be(createdVehicle.Brand);
            result.ProductionYear.Should().Be(createdVehicle.ProductionYear);
        }

        [Fact]
        public async Task RentedVehiclesBy_Should_ReturnVehiclesRentedByCustomer()
        {
            // Arrange
            var vehicle1 = new Vehicle { Brand = "Subaru", ProductionYear = DateTime.Now.Year, RentedBy = 4 };
            var vehicle2 = new Vehicle { Brand = "Honda", ProductionYear = DateTime.Now.Year, RentedBy = 2 };
            await _repository.CreateVehicleAsync(vehicle1);
            await _repository.CreateVehicleAsync(vehicle2);

            // Act
            var result = await _repository.RentedVehiclesBy(4);

            // Assert
            result.Should().ContainSingle(v => v.Brand == "Subaru");
            result.Should().NotContain(v => v.Brand == "Honda");
        }

        [Fact]
        public async Task RentVehicleAsync_Should_RentVehicle()
        {
            // Arrange
            var vehicle = new Vehicle { Brand = "Toyota", ProductionYear = DateTime.Now.Year };
            var createdVehicle = await _repository.CreateVehicleAsync(vehicle);

            // Act
            var result = await _repository.RentVehicleAsync(createdVehicle.Id, 1);

            // Assert
            result.Should().BeTrue();
            var rentedVehicle = await _repository.GetAsync(createdVehicle.Id);
            rentedVehicle.RentedBy.Should().Be(1);
        }

        [Fact]
        public async Task ReturnVehicleAsync_Should_ReturnVehicle()
        {
            // Arrange
            var vehicle = new Vehicle { Brand = "Toyota", ProductionYear = DateTime.Now.Year, RentedBy = 1 };
            var createdVehicle = await _repository.CreateVehicleAsync(vehicle);

            // Act
            var result = await _repository.ReturnVehicleAsync(createdVehicle.Id);

            // Assert
            result.Should().BeTrue();
            var returnedVehicle = await _repository.GetAsync(createdVehicle.Id);
            returnedVehicle.RentedBy.Should().BeNull();
        }
    }
}
