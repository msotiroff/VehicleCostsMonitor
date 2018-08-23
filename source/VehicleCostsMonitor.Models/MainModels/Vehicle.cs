namespace VehicleCostsMonitor.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using static Common.ModelConstants;

    public class Vehicle
    {
        public Vehicle()
        {
            this.FuelEntries = new HashSet<FuelEntry>();
            this.CostEntries = new HashSet<CostEntry>();
        }

        [Key]
        public int Id { get; set; }

        [Required]
        public int ManufacturerId { get; set; }

        public Manufacturer Manufacturer { get; set; }

        [Required]
        public int ModelId { get; set; }

        public Model Model { get; set; }

        public string ExactModelname { get; set; }

        [Required]
        [Range(YearOfManufactureMinValue, int.MaxValue)]
        public int YearOfManufacture { get; set; }

        [Required]
        [Range(EngineHorsePowerMinValue, EngineHorsePowerMaxValue)]
        public int EngineHorsePower { get; set; }

        [Required]
        public int VehicleTypeId { get; set; }

        public VehicleType VehicleType { get; set; }

        [Required]
        public int FuelTypeId { get; set; }

        public FuelType FuelType { get; set; }

        [Required]
        public int GearingTypeId { get; set; }
        public GearingType GearingType { get; set; }

        [Required]
        public string UserId { get; set; }

        public User User { get; set; }

        public double FuelConsumption { get; set; }
        
        public double TotalFuelAmount { get; set; }

        public int TotalDistance { get; set; }

        public int? PictureId { get; set; }

        public Picture Picture { get; set; }

        public bool IsDeleted { get; set; }

        public IEnumerable<CostEntry> CostEntries { get; set; }
                                                               
        public IEnumerable<FuelEntry> FuelEntries { get; set; }
    }
}