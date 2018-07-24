namespace VehicleCostsMonitor.Services.Models.Entries
{
    using System;
    using Common.AutoMapping;
    using Interfaces;
    using VehicleCostsMonitor.Models;

    public class FuelEntryDetailsModel : IEntryModel, IAutoMapWith<FuelEntry>
    {
        public int Id { get; set; }
        
        public DateTime DateCreated { get; set; }
        
        public int Odometer { get; set; }
        
        public int TripOdometer { get; set; }
        
        public double FuelQuantity { get; set; }
        
        public decimal Price { get; set; }
        
        public double? Average { get; set; }
        
        public int VehicleId { get; set; }
    }
}
