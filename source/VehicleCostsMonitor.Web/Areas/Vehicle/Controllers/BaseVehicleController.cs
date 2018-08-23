namespace VehicleCostsMonitor.Web.Areas.Vehicle.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using VehicleCostsMonitor.Web.Controllers;
    using static WebConstants;

    [Area(VehicleArea)]
    public abstract class BaseVehicleController : BaseController
    {
        protected const string FuelTypesCacheKey = "_FuelTypes";
        
        protected IActionResult RedirectToVehicle(int id)
            => RedirectToAction("details", "vehicle", new { id = id, area = "vehicle" });
    }
}