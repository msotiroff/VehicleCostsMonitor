namespace VehicleCostsMonitor.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class CostEntry
    {
        public CostEntry(DateTime dateCreated, int costEntryTypeId, int vehicleId, decimal price, int currencyId, string note, int? odometer = null)
            : this(dateCreated, costEntryTypeId, vehicleId)
        {
            this.Price = price;
            this.Note = note;
            this.Odometer = odometer;
            this.CurrencyId = currencyId;
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

        [Range(0, double.MaxValue)]
        public decimal Price { get; set; }

        public int CurrencyId { get; set; }

        public Currency Currency { get; set; }
        
        public string Note { get; set; }

        [Required]
        public int VehicleId { get; set; }

        public Vehicle Vehicle { get; set; }
    }
}