namespace GtMotive.Estimate.Microservice.Domain.Models
{
    /// <summary>
    /// Vehicle model.
    /// </summary>
    public class Vehicle
    {
        /// <summary>
        /// Gets or sets the vehicle identifier.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the vehicle brand.
        /// </summary>
        public string Brand { get; set; }

        /// <summary>
        /// Gets or sets the vehicle production year.
        /// </summary>
        public int ProductionYear { get; set; }

        /// <summary>
        /// Gets or sets a value indicating who rented the vehicle.
        /// </summary>
        public int? RentedBy { get; set; }
    }
}
