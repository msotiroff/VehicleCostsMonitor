namespace VehicleCostsMonitor.Web.Areas.Admin.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using VehicleCostsMonitor.Web.Controllers;
    using VehicleCostsMonitor.Web.Infrastructure.Filters;
    using static WebConstants;

    [Area(AdministratorArea)]
    [Authorize(Roles = AdministratorRole)]
    [LogActivity]
    public abstract class BaseAdminController : BaseController
    {
    }
}