namespace VehicleCostsMonitor.Web.Areas.Vehicle.Controllers
{
    using AutoMapper;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Caching.Distributed;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using VehicleCostsMonitor.Services.Interfaces;
    using VehicleCostsMonitor.Services.Models.Vehicle;
    using VehicleCostsMonitor.Web.Areas.Vehicle.Models.Evaluation;
    using VehicleCostsMonitor.Web.Infrastructure.Extensions;
    using VehicleCostsMonitor.Web.Infrastructure.Extensions.ExcelExport.Interfaces;

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
            var model = await this.GetRanking(fuelType);

            return View(model);
        }

        [HttpGet]
        public async Task<IExcelResult> ExportVehicles(string fuelType = "")
        {
            var vehicles = await this.GetRanking(fuelType);
            var model = Mapper.Map<IEnumerable<VehicleStatisticExcelViewModel>>(vehicles);

            var fileName = $"Most_Economic_{fuelType}_Vehicles";

            return Excel(model, fileName);
        }

        private async Task<IEnumerable<VehicleStatisticServiceModel>> GetRanking(string fuelType = "")
        {
            IEnumerable<VehicleStatisticServiceModel> model;
            var modelFromCache = await this.cache.GetStringAsync(string.Format(MostEconomicVehiclesCacheKey, fuelType));
            if (modelFromCache != null)
            {
                model = JsonConvert.DeserializeObject<IEnumerable<VehicleStatisticServiceModel>>(modelFromCache);
            }
            else
            {
                model = await this.vehicles.GetMostEconomicCars(fuelType);
                await this.cache.SetSerializableObject(string.Format(MostEconomicVehiclesCacheKey, fuelType), model, TimeSpan.FromDays(1));
            }

            return model;
        }
    }
}