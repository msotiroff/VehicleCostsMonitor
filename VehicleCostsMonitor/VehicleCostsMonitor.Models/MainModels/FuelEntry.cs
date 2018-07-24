namespace VehicleCostsMonitor.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;

    public class FuelEntry
    {
        #region Fields
        private int tripOdometer;
        private double average;
        #endregion

        public FuelEntry()
        {
            this.Routes = new HashSet<FuelEntryRouteType>();
            this.ExtraFuelConsumers = new HashSet<FuelEntryExtraFuelConsumer>();
        }

        #region Properties
        [Key]
        public int Id { get; set; }

        [Required]
        public DateTime DateCreated { get; set; }
        
        [Required]
        public int Odometer { get; set; }

        [Required]
        public int TripOdometer
        {
            get => this.tripOdometer;
            set => this.tripOdometer = this.CalculateDistance();
        }

        [Required]
        public double FuelQuantity { get; set; }

        [Required]
        public decimal Price { get; set; }

        public string Note { get; set; }
        
        [Required]
        public int FuelEntryTypeId { get; set; }

        public FuelEntryType FuelEntryType { get; set; }
        
        public double? Average
        {
            get => this.average;
            set
            {
                if (this.tripOdometer == 0)
                {
                    this.average = 0;
                }
                else
                {
                    this.average = (this.FuelQuantity / this.tripOdometer) * 100.0;
                }
            }
        }

        [Required]
        public int VehicleId { get; set; }

        public Vehicle Vehicle { get; set; }

        public IEnumerable<FuelEntryRouteType> Routes { get; set; }
        
        public IEnumerable<FuelEntryExtraFuelConsumer> ExtraFuelConsumers { get; set; }
        #endregion

        #region Methods
        private int CalculateDistance()
        {
            var lastFuelingEntry = this.Vehicle.FuelEntries.LastOrDefault();

            if (lastFuelingEntry == null)
            {
                return 0;
            }

            var distance = this.Odometer - lastFuelingEntry.Odometer;
            if (distance < 0)
            {
                throw new InvalidOperationException("Odometer value should be greater or equal to the last entered odometer value!");
            }

            return distance;
        }
        #endregion
    }
}