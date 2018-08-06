namespace VehicleCostsMonitor.Web.Areas.Vehicle.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Caching.Distributed;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using VehicleCostsMonitor.Services.Interfaces;
    using VehicleCostsMonitor.Services.Models.Vehicle;

    public class EvaluationController : BaseVehicleController
    {
        private const string MostEconomicVehiclesCacheKey = "_MostEconomicVehicles-{0}";

        private readonly IDistributedCache cache;
        private readonly IVehicleService vehicles;

        public EvaluationController(IDistributedCache cache, IVehicleService vehicles)
        {
            this.cache = cache;
            this.vehicles = vehicles;
        }

        [HttpGet]
        public async Task<IActionResult> MostEconomicVehicles(string fuelType)
        {
            fuelType = fuelType.ToLower();

            IEnumerable<VehicleStatisticServiceModel> model;
            var modelFromCache = await this.cache.GetStringAsync(string.Format(MostEconomicVehiclesCacheKey, fuelType));
            if (modelFromCache != null)
            {
                model = JsonConvert.DeserializeObject<IEnumerable<VehicleStatisticServiceModel>>(modelFromCache);
            }
            else
            {
                var options = new DistributedCacheEntryOptions();
                options.AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(1);
                model = await this.vehicles.GetMostEconomicCars(fuelType);
                await this.cache.SetStringAsync(string.Format(MostEconomicVehiclesCacheKey, fuelType), JsonConvert.SerializeObject(model), options);
            }
            
            return View(model);
        }
    }
}