using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using VehicleCostsMonitor.Services.Interfaces;
using VehicleCostsMonitor.Services.Models.Manufacturer;

namespace VehicleCostsMonitor.Web.Areas.Admin.Controllers
{
    public class ManufacturerController : BaseAdminController
    {
        private readonly IManufacturerService manufacturers;

        public ManufacturerController(IManufacturerService manufacturers)
        {
            this.manufacturers = manufacturers;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var model = await this.manufacturers.All();

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var model = await this.manufacturers.GetDetailed(id);

            return View(model);
        }
        
        [HttpPost]
        public async Task<IActionResult> Create(string name)
        {
            var success = await this.manufacturers.CreateAsync(name);
            if (!success)
            {
                return BadRequest();
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var model = await this.manufacturers.Get(id);
            if (model == null)
            {
                return BadRequest(id);
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(ManufacturerUpdateServiceModel model)
        {
            var success = await this.manufacturers.UpdateAsync(model.Id, model.Name);
            if (!success)
            {
                return BadRequest();
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var model = await this.manufacturers.Get(id);
            if (model == null)
            {
                return BadRequest(id);
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(ManufacturerUpdateServiceModel model)
        {
            var success = await this.manufacturers.DeleteAsync(model.Id);
            if (!success)
            {
                return BadRequest();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}