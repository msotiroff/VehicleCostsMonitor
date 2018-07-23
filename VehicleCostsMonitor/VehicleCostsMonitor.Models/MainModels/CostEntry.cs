namespace VehicleCostsMonitor.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class CostEntry
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public DateTime DateCreated { get; set; }

        public int? Odometer { get; set; }

        [Required]
        public int CostEntryTypeId { get; set; }

        public CostEntryType CostEntryType { get; set; }

        public decimal Price { get; set; }

        public string Note { get; set; }

        [Required]
        public int VehicleId { get; set; }

        public Vehicle Vehicle { get; set; }
    }
}