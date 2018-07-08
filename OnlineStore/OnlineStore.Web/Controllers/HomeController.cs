namespace OnlineStore.Web.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using OnlineStore.Web.Models;
    using System.Diagnostics;

    public class HomeController : ApiClientController
    {
        public IActionResult Index() 
            => this.RedirectToAction(nameof(CategoryController.Index), "Category");

        public IActionResult About() 
            => View();

        public IActionResult Contact() 
            => View();

        public IActionResult Error()
            => View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
