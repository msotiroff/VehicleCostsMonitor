namespace VehicleCostsMonitor.Web.Infrastructure.Filters
{
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;
    using Microsoft.EntityFrameworkCore;
    using System.Linq;
    using System.Reflection;
    using VehicleCostsMonitor.Data;
    using VehicleCostsMonitor.Models;
    using VehicleCostsMonitor.Web.Areas.Vehicle.Controllers;

    /// <summary>
    /// This filter ensures, that the current logged in user has permisions to make changes over the entity
    /// </summary>
    public class EnsureOwnershipAttribute : ActionFilterAttribute
    {
        private const string AccessDeniedUrl = "/identity/account/accessdenied";

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var httpContext = context.HttpContext;

            var database = httpContext.RequestServices.GetService(typeof(JustMonitorDbContext)) as JustMonitorDbContext;
            var userManager = httpContext.RequestServices.GetService(typeof(UserManager<User>)) as UserManager<User>;

            var userId = userManager.GetUserId(context.HttpContext.User);
            var vehicleId = this.GetVehicleId(database, context);

            var dbVehicle = database.Vehicles.AsNoTracking().FirstOrDefault(v => v.Id == vehicleId);
            if (dbVehicle == null || dbVehicle.UserId != userId)
            {
                context.Result = new LocalRedirectResult(AccessDeniedUrl);
            }

            base.OnActionExecuting(context);
        }

        private int GetVehicleId(JustMonitorDbContext database, ActionExecutingContext context)
        {
            var actionArguments = context.ActionArguments;
            var controllerTypeName = context.Controller.GetType().Name;
            
            if (actionArguments.ContainsKey("vehicleId"))
            {
                return (int)actionArguments["vehicleId"];
            }

            if (actionArguments.ContainsKey("id"))
            {
                var entityId = actionArguments["id"] as int?;
                
                switch (controllerTypeName)
                {
                    case nameof(VehicleController):
                        return entityId ?? default(int);

                    case nameof(CostEntryController):
                        return database.CostEntries.AsNoTracking().FirstOrDefault(ce => ce.Id == entityId)?.VehicleId ?? default(int);

                    case nameof(FuelEntryController):
                        return database.FuelEntries.AsNoTracking().FirstOrDefault(ce => ce.Id == entityId)?.VehicleId ?? default(int);

                    default:
                        return default(int);
                }
            }
            else
            {
                var referenceTypes = actionArguments.Where(aa => !aa.GetType().IsPrimitive && aa.GetType() != typeof(string)).Select(aa => aa.Value);
                foreach (var model in referenceTypes)
                {
                    PropertyInfo vehicleIdProperty;
                    if (controllerTypeName == nameof(VehicleController))
                    {
                        vehicleIdProperty = model.GetType().GetProperties().FirstOrDefault(pi => pi.Name.ToLower() == "id");
                    }
                    else
                    {
                        vehicleIdProperty = model.GetType().GetProperties().FirstOrDefault(pi => pi.Name.ToLower() == "vehicleid");
                    }

                    if (vehicleIdProperty != null)
                    {
                        if (int.TryParse(vehicleIdProperty.GetValue(model).ToString(), out int vehicleId))
                        {
                            return vehicleId;
                        }
                    }
                }

            }

            return default(int);
        }
    }
}
