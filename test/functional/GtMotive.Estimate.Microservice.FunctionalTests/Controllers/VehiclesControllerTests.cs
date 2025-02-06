using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using FluentAssertions;
using GtMotive.Estimate.Microservice.Api.Dtos;
using GtMotive.Estimate.Microservice.Domain.Models;
using GtMotive.Estimate.Microservice.FunctionalTests.Infrastructure;
using Xunit;

namespace GtMotive.Estimate.Microservice.FunctionalTests.Controllers
{
    public class VehiclesControllerTests(CompositionRootTestFixture fixture) : FunctionalTestBase(fixture)
    {
        [Fact]
        public async Task CreateVehicle_Should_ReturnVehicleId()
        {
            // Arrange
            var vehicleDto = new CreateVehicleDto { Brand = "Toyota", ProductionYear = DateTime.Now.Year };

            // Act
            var response = await Fixture.Client.PostAsJsonAsync("/api/vehicles", vehicleDto);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var vehicleId = await response.Content.ReadFromJsonAsync<int>();
            vehicleId.Should().BeGreaterThan(0);
        }

        [Fact]
        public async Task GetAvailableVehicles_Should_ReturnAvailableVehicles()
        {
            // Arrange
            var vehicleDto = new CreateVehicleDto { Brand = "Fiat", ProductionYear = DateTime.Now.Year };
            await Fixture.Client.PostAsJsonAsync("/api/vehicles", vehicleDto);

            // Act
            var response = await Fixture.Client.GetAsync("/api/vehicles/available");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var vehicles = await response.Content.ReadFromJsonAsync<IEnumerable<Vehicle>>();
            vehicles.Should().ContainSingle(v => v.Brand == "Fiat");
        }
    }
}
