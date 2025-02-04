using System.ComponentModel.DataAnnotations;

namespace GtMotive.Estimate.Microservice.Api.Dtos
{
    public class CreateVehicleDto
    {
        [Required]
        public string Brand { get; set; }

        [Required]
        [Range(2000, 2100)]
        public int ProductionYear { get; set; }
    }
}
