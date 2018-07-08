using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using static VehicleCostsMonitor.Models.Common.ModelConstants;

namespace VehicleCostsMonitor.Models
{
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
        public int ManufactureId { get; set; }

        public Manufacturer Manufacturer { get; set; }

        [Required]
        public int ModelId { get; set; }

        public Model Model { get; set; }

        public string ExactModelname { get; set; }

        [Required]
        public int YearOfManufacture { get; set; }

        [Required]
        [Range(EngineHorsePowerMinValue, EngineHorsePowerMaxValue)]
        public int EngineHorsePower { get; set; }

        [Required]
        public VehicleType VehicleType { get; set; }

        [Required]
        public FuelType FuelType { get; set; }

        [Required]
        public GearingType GearingType { get; set; }

        [Required]
        public string UserId { get; set; }

        public User User { get; set; }

        public int? PictureId { get; set; }

        public Picture Picture { get; set; }

        public bool IsDeleted { get; set; }

        public IEnumerable<CostEntry> CostEntries { get; set; }
                                                               
        public IEnumerable<FuelEntry> FuelEntries { get; set; }
    }
}