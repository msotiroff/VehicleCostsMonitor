using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace VehicleCostsMonitor.Web.Areas.Admin.Controllers
{
    [Area(WebConstants.AdministratorRole)]
    [Authorize(Roles = WebConstants.AdministratorRole)]
    public abstract class BaseAdminController : Controller
    {
    }
}