namespace VehicleCostsMonitor.Services.Models.Entries.CostEntries
{
    using Common.AutoMapping.Interfaces;
    using System;
    using System.ComponentModel.DataAnnotations;
    using VehicleCostsMonitor.Models;

    public class CostEntryUpdateServiceModel : IAutoMapWith<CostEntry>
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public DateTime DateCreated { get; set; }

        public int? Odometer { get; set; }

        [Required]
        public int CostEntryTypeId { get; set; }

        public decimal Price { get; set; }

        public int CurrencyId { get; set; }

        public string Note { get; set; }

        [Required]
        public int VehicleId { get; set; }
    }
}
