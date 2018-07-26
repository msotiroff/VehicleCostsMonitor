namespace VehicleCostsMonitor.Web.Infrastructure.Filters
{
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;
    using System.Collections.Generic;
    using System.Linq;
    using VehicleCostsMonitor.Common;
    using VehicleCostsMonitor.Data;
    using VehicleCostsMonitor.Models;
    using VehicleCostsMonitor.Web.Controllers;

    public class AuthorizeOwnerAttribute : ActionFilterAttribute
    {
        private const string AccessDeniedRedirectUrl = "/Identity/Account/AccessDenied";

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var requestServices = context.HttpContext.RequestServices;

            var actionArguments = context.ActionArguments;
            if (!actionArguments.Any())
            {
                context.Result = new BadRequestObjectResult(WebConstants.BadRequestErrorMsg);
                return;
            }

            int vehicleId = this.GetVehicleId(actionArguments);

            var database = requestServices.GetRequiredService<JustMonitorDbContext>();
            var userManager = requestServices.GetRequiredService<UserManager<User>>();
            var controller = context.Controller as BaseController;
            var currentLoggedUserId = userManager.GetUserId(controller?.User);

            var vehicle = database.Vehicles.AsNoTracking().FirstOrDefault(v => v.Id == vehicleId);

            if (vehicle?.UserId != currentLoggedUserId)
            {
                context.Result = new LocalRedirectResult(AccessDeniedRedirectUrl);
            }

            base.OnActionExecuting(context);
        }

        private int GetVehicleId(IDictionary<string, object> actionArguments)
        {
            int vehicleId = default(int);
            if (actionArguments.ContainsKey("id"))
            {
                vehicleId = (int)actionArguments["id"];
            }
            else if (actionArguments.ContainsKey("vehicleId"))
            {
                vehicleId = (int)actionArguments["vehicleId"];
            }
            else
            {
                var model = actionArguments.Values.FirstOrDefault(aa => !aa.GetType().IsPrimitive && aa.GetType() != typeof(string));
                var modelIdProperty = model?.GetType().GetProperty("Id");
                if (modelIdProperty != null)
                {
                    vehicleId = (int)modelIdProperty.GetValue(model);
                }
            }

            return vehicleId;
        }
    }
}
