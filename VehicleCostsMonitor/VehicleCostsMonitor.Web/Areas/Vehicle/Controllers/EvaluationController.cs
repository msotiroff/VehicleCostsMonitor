namespace VehicleCostsMonitor.Web.Areas.Vehicle.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using System.Threading.Tasks;
    using VehicleCostsMonitor.Services.Interfaces;

    public class EvaluationController : BaseVehicleController
    {
        private readonly IVehicleService vehicles;

        public EvaluationController(IVehicleService vehicles)
        {
            this.vehicles = vehicles;
        }

        [HttpGet]
        public async Task<IActionResult> MostEconomicVehicles()
        {
            var model = await this.vehicles.GetMostEconomicCars();

            return View(model);
        }
    }
}