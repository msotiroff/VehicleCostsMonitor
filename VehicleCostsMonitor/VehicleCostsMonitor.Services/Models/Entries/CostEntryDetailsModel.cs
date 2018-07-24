namespace VehicleCostsMonitor.Services.Models.Entries
{
    using Common.AutoMapping;
    using Interfaces;
    using System;
    using VehicleCostsMonitor.Models;

    public class CostEntryDetailsModel : IEntryModel, IAutoMapWith<CostEntry>
    {
        public int Id { get; set; }
        
        public DateTime DateCreated { get; set; }

        public int? Odometer { get; set; }
        
        public string CostEntryTypeName { get; set; }

        public decimal Price { get; set; }
        
        public int VehicleId { get; set; }
    }
}
