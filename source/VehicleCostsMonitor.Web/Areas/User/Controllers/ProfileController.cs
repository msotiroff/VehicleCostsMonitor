namespace VehicleCostsMonitor.Web.Areas.User.Controllers
{
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using System.Threading.Tasks;
    using VehicleCostsMonitor.Models;
    using VehicleCostsMonitor.Services.Interfaces;
    using VehicleCostsMonitor.Services.Models.User;
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
        [Route("[area]/[controller]/{userName?}/{id?}")]
        public async Task<IActionResult> Index(string id = null, string userName = null)
        {
            UserProfileServiceModel model = null;

            if (id == null)
            {
                if (this.User.Identity.IsAuthenticated)
                {
                    model = await this.userService.GetAsync(this.userManager.GetUserId(User));
                }
            }
            else
            {
                model = await this.userService.GetAsync(id);
            }

            return model == null ? this.RedirectToHome() : this.View(model);
        }
    }
}