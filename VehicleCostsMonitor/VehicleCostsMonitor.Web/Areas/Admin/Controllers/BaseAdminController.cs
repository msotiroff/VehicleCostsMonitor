namespace VehicleCostsMonitor.Web.Areas.Admin.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using VehicleCostsMonitor.Web.Controllers;
    using static WebConstants;

    [Area(AdministratorRole)]
    [Authorize(Roles = AdministratorRole)]
    public abstract class BaseAdminController : BaseController
    {
    }
}