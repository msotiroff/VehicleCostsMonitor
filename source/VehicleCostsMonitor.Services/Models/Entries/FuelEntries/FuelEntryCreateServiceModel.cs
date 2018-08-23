namespace VehicleCostsMonitor.Services.Models.Entries.FuelEntries
{
    using Common.AutoMapping.Interfaces;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using VehicleCostsMonitor.Models;

    public class FuelEntryCreateServiceModel : IAutoMapWith<FuelEntry>
    {
        [Required]
        public DateTime DateCreated { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        public int Odometer { get; set; }

        [Required]
        public int TripOdometer { get; set; }

        [Required]
        public double FuelQuantity { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal Price { get; set; }

        [Required]
        public int CurrencyId { get; set; }

        public string Note { get; set; }

        [Required]
        public int FuelTypeId { get; set; }

        [Required]
        public int FuelEntryTypeId { get; set; }

        public double Average { get; set; }

        [Required]
        public int VehicleId { get; set; }

        public IEnumerable<FuelEntryRouteType> Routes { get; set; }

        public IEnumerable<FuelEntryExtraFuelConsumer> ExtraFuelConsumers { get; set; }
    }
}
