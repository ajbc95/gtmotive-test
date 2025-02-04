using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GtMotive.Estimate.Microservice.Api.Dtos;
using GtMotive.Estimate.Microservice.Domain.Interfaces;
using GtMotive.Estimate.Microservice.Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GtMotive.Estimate.Microservice.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VehiclesController(IVehicleService vehicleService) : ControllerBase
    {
        private readonly IVehicleService _vehicleService = vehicleService ?? throw new ArgumentNullException(nameof(vehicleService));

        [HttpPost]
        [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateVehicle([FromBody] CreateVehicleDto vehicle)
        {
            if (vehicle is null || string.IsNullOrWhiteSpace(vehicle.Brand))
            {
                return BadRequest("Vehicle data is required.");
            }

            var vehicleId = await _vehicleService.CreateAsync(vehicle.Brand, vehicle.ProductionYear);
            return Ok(vehicleId);
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Vehicle>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAvailableVehicles()
        {
            var vehicles = await _vehicleService.ListAvailableAsync();
            return vehicles.Any() ? Ok(vehicles) : NoContent();
        }

        [HttpPost("{vehicleId}/rentBy/{customerId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> RentVehicle(int vehicleId, int customerId)
        {
            var result = await _vehicleService.RentAsync(vehicleId, customerId);
            return !result ? BadRequest("Vehicle not available for rent.") : NoContent();
        }

        [HttpPost("{vehicleId}/return")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ReturnVehicle(int vehicleId)
        {
            var result = await _vehicleService.ReturnAsync(vehicleId);
            return !result ? BadRequest("Vehicle not rented.") : NoContent();
        }
    }
}
