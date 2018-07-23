namespace VehicleCostsMonitor.Web.Areas.User.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using System.Threading.Tasks;
    using VehicleCostsMonitor.Models;
    using VehicleCostsMonitor.Services.Interfaces;
    using VehicleCostsMonitor.Web.Controllers;
    using static WebConstants;

    [Area(UserArea)]
    public class ProfileController : BaseController
    {
        private readonly IUserService userService;
        private readonly UserManager<User> userManager;

        public ProfileController(IUserService userService, UserManager<User> userManager)
        {
            this.userService = userService;
            this.userManager = userManager;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Index()
        {
            var model = await this.userService
                .GetAsync(this.userManager.GetUserId(User));

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Details(string id)
        {
            var model = await this.userService.GetAsync(id);
            return View(model);
        }
    }
}