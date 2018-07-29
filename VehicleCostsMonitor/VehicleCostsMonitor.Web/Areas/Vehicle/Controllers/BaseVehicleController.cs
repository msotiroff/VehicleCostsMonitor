namespace VehicleCostsMonitor.Web.Areas.Vehicle.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using VehicleCostsMonitor.Web.Controllers;
    using static WebConstants;

    [Area(VehicleArea)]
    public class BaseVehicleController : BaseController
    {
        protected IActionResult RedirectToVehicle(int id)
            => RedirectToAction("details", "vehicle", new { id = id, area = "vehicle" });
    }
}