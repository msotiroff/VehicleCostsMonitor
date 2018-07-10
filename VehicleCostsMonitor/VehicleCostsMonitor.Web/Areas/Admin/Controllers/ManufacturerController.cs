using Microsoft.AspNetCore.Mvc;

namespace VehicleCostsMonitor.Web.Areas.Admin.Controllers
{
    public class ManufacturerController : BaseAdminController
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}