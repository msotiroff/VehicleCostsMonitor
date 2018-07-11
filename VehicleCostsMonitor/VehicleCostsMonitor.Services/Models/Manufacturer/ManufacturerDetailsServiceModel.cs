﻿namespace VehicleCostsMonitor.Services.Models.Manufacturer
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using VehicleCostsMonitor.Common.AutoMapping;
    using VehicleCostsMonitor.Models;
    using VehicleModel;

    public class ManufacturerDetailsServiceModel : IAutoMapWith<Manufacturer>
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public IEnumerable<ModelConciseServiceModel> Models { get; set; }
    }
}