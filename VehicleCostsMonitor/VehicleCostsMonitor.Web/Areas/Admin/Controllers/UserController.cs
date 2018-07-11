namespace VehicleCostsMonitor.Web.Areas.Admin.Controllers
{
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using System;
    using System.Threading.Tasks;
    using VehicleCostsMonitor.Models;

    public class UserController : BaseAdminController
    {
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly UserManager<User> userManager;

        public UserController(
            RoleManager<IdentityRole> roleManager, 
            UserManager<User> userManager)
        {
            this.roleManager = roleManager;
            this.userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            throw new NotImplementedException();
        }

        public async Task<IActionResult> Details(string id)
        {
            throw new NotImplementedException();
        }

        public async Task<IActionResult> Vehicles(string id)
        {
            throw new NotImplementedException();
        }

        [HttpPost]
        public async Task<IActionResult> AddToRole(string userId, string role)
        {
            throw new NotImplementedException();
        }

        [HttpPost]
        public async Task<IActionResult> RemoveFromRole(string userId, string role)
        {
            throw new NotImplementedException();
        }
    }
}