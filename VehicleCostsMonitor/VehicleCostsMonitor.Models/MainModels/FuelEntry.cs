namespace VehicleCostsMonitor.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class FuelEntry
    {
        public FuelEntry(DateTime dateCreated, int odometer, double fuelQuantity, decimal price, int fuelEntryTypeId, int vehicleId)
        {
            this.DateCreated = dateCreated;
            this.Odometer = odometer;
            this.FuelQuantity = fuelQuantity;
            this.Price = price;
            this.FuelEntryTypeId = fuelEntryTypeId;
            this.VehicleId = vehicleId;

            this.Routes = new HashSet<FuelEntryRouteType>();
            this.ExtraFuelConsumers = new HashSet<FuelEntryExtraFuelConsumer>();
        }

        public FuelEntry(DateTime dateCreated, int odometer, double fuelQuantity, decimal price, int fuelEntryTypeId, int vehicleId, string note = null)
            : this(dateCreated, odometer, fuelQuantity, price, fuelEntryTypeId, vehicleId)
        {
            this.Note = note;
        }


        [Key]
        public int Id { get; set; }

        [Required]
        public DateTime DateCreated { get; set; }
        
        [Required]
        public int Odometer { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        public int TripOdometer { get; set; }

        [Required]
        public double FuelQuantity { get; set; }

        [Required]
        public decimal Price { get; set; }

        public string Note { get; set; }

        [Required]
        public int FuelTypeId { get; set; }

        public FuelType FuelType { get; set; }

        [Required]
        public int FuelEntryTypeId { get; set; }

        public FuelEntryType FuelEntryType { get; set; }

        [Range(0, double.MaxValue)]
        public double Average { get; set; }
        
        [Required]
        public int VehicleId { get; set; }

        public Vehicle Vehicle { get; set; }

        public IEnumerable<FuelEntryRouteType> Routes { get; set; }
        
        public IEnumerable<FuelEntryExtraFuelConsumer> ExtraFuelConsumers { get; set; }
    }
}