using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using VehicleCostsMonitor.Web.Models;

namespace VehicleCostsMonitor.Web.Controllers
{
    public class HomeController : BaseController
    {
        public IActionResult Index() => View();

        public IActionResult About() => View();

        public IActionResult Contact() => View();

        public IActionResult Privacy() => View();

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
