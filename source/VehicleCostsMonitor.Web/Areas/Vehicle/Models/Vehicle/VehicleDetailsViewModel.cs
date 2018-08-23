namespace VehicleCostsMonitor.Web.Areas.Vehicle.Models
{
    using Common.AutoMapping.Interfaces;
    using Services.Models.Vehicle;
    using System.ComponentModel.DataAnnotations;
    using VehicleCostsMonitor.Services.Models.Entries.Interfaces;
    using VehicleCostsMonitor.Web.Infrastructure.Collections;

    public class VehicleDetailsViewModel : IAutoMapWith<VehicleDetailsServiceModel>
    {
        public int Id { get; set; }

        [Display(Name = "Model name")]
        public string FullModelName { get; set; }

        [Display(Name = "Year of manufacture")]
        public int YearOfManufacture { get; set; }

        [Display(Name = "HP")]
        public int EngineHorsePower { get; set; }

        [Display(Name = "Vehicle type")]
        public string VehicleTypeName { get; set; }

        [Display(Name = "Fuel type")]
        public string FuelTypeName { get; set; }

        [Display(Name = "Gearing type")]
        public string GearingTypeName { get; set; }

        [Display(Name = "Owner")]
        public string OwnerUserName { get; set; }

        [Display(Name = "Currency")]
        public string OwnerDisplayCurrency { get; set; }

        [Display(Name = "Fuel consumption")]
        public double FuelConsumption { get; set; }

        public string PicturePath { get; set; }

        [Display(Name = "Total fuel amount")]
        public double TotalFuelAmount { get; set; }

        [Display(Name = "Total distance")]
        public int TotalDistance { get; set; }

        [Display(Name = "Total costs per 100 km")]
        public decimal TotalCostsPerHundredKm { get; set; }

        [Display(Name = "Total fuel costs")]
        public decimal TotalFuelCosts { get; set; }

        [Display(Name = "Total other costs")]
        public decimal TotalOtherCosts { get; set; }

        [Display(Name = "Total spent money")]
        public decimal TotalSpent { get; set; }

        public Statistics Stats { get; set; }

        public PaginatedList<IEntryModel> Entries { get; set; }
    }
}
