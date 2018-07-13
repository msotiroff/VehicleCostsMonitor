namespace VehicleCostsMonitor.Web.Infrastructure.Filters
{
    using Microsoft.AspNetCore.Mvc.Filters;
    using Newtonsoft.Json;
    using System;
    using System.Linq;
    using VehicleCostsMonitor.Services.Interfaces;
    using VehicleCostsMonitor.Services.Models.Log;
    using VehicleCostsMonitor.Web.Infrastructure.Providers.Interfaces;

    /// <summary>
    /// Logs the request of the current singed in user and write it to the database
    /// </summary>
    public class LogActivityAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var httpContext = context.HttpContext;
            
            var user = httpContext.User.Identity.Name;
            var url = httpContext.Request.Path;
            var queryString = httpContext.Request.QueryString.ToString();
            var httpMethod = httpContext.Request.Method;

            var routeValues = context.ActionDescriptor.RouteValues;
            routeValues.TryGetValue("action", out string actionName);
            routeValues.TryGetValue("controller", out string controllerName);
            routeValues.TryGetValue("area", out string areaName);
            
            var actionArguments = string
                .Join(Environment.NewLine, context
                    .ActionArguments
                    .Select(arg => $"{arg.Key}: {JsonConvert.SerializeObject(arg.Value)}"));

            var dateTimeService = httpContext.RequestServices.GetService(typeof(IDateTimeProvider)) as IDateTimeProvider;

            var logModel = new UserActivityLogCreateModel
            {
                DateTime = dateTimeService.GetCurrentDateTime(),
                UserEmail = user,
                HttpMethod = httpMethod,
                ControllerName = controllerName,
                ActionName = actionName,
                AreaName = areaName,
                Url = url,
                QueryString = queryString,
                ActionArguments = actionArguments
            };

            var logService = httpContext.RequestServices.GetService(typeof(ILogService)) as ILogService;

            logService.CreateUserActivityLog(logModel);
        }
    }
}
