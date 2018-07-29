namespace VehicleCostsMonitor.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class CostEntry
    {
        public CostEntry(DateTime dateCreated, int costEntryTypeId, int vehicleId, decimal price, string note, int? odometer = null)
            : this(dateCreated, costEntryTypeId, vehicleId)
        {
            this.Price = price;
            this.Note = note;
            this.Odometer = odometer;
        }
        
        public CostEntry(DateTime dateCreated, int costEntryTypeId, int vehicleId)
        {
            this.DateCreated = dateCreated;
            this.CostEntryTypeId = costEntryTypeId;
            this.VehicleId = vehicleId;
        }
        
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