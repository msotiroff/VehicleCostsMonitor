namespace VehicleCostsMonitor.Web.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Models;
    using Models.Home;
    using Services.Interfaces;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading.Tasks;

    public class HomeController : BaseController
    {
        private readonly IManufacturerService manufacturers;

        public HomeController(IManufacturerService manufacturers)
        {
            this.manufacturers = manufacturers;
        }

        public async Task<IActionResult> Index()
        {
            var allManufacturers = await this.manufacturers.AllAsync();

            var model = new IndexViewModel
            {
                AllManufacturers = allManufacturers.Select(m => new SelectListItem(m.Name, m.Id.ToString())),
            };

            return View(model);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
