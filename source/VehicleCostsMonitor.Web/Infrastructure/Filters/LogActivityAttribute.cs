﻿namespace VehicleCostsMonitor.Web.Infrastructure.Filters
{
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc.Filters;
    using Newtonsoft.Json;
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using VehicleCostsMonitor.Models;
    using VehicleCostsMonitor.Services.Interfaces;
    using VehicleCostsMonitor.Services.Models.Log;
    using VehicleCostsMonitor.Web.Infrastructure.Utilities.Interfaces;

    /// <summary>
    /// Logs the request of the current singed in user and write it to the database
    /// </summary>
    public class LogActivityAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var httpContext = context.HttpContext;

            var userManager = httpContext.RequestServices.GetService(typeof(UserManager<User>)) as UserManager<User>;
            var dateTimeProvider = httpContext.RequestServices.GetService(typeof(IDateTimeProvider)) as IDateTimeProvider;
            var logService = httpContext.RequestServices.GetService(typeof(ILogService)) as ILogService;

            var userClaims = httpContext.User;
            string userEmail = string.Empty;

            Task.Run(async () =>
            {
                var user = await userManager.GetUserAsync(userClaims);
                userEmail = await userManager.GetEmailAsync(user);
            })
            .GetAwaiter()
            .GetResult();
            
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

            var logModel = new UserActivityLogCreateModel
            {
                DateTime = dateTimeProvider.GetCurrentDateTime(),
                UserEmail = userEmail,
                HttpMethod = httpMethod,
                ControllerName = controllerName,
                ActionName = actionName,
                AreaName = areaName,
                Url = url,
                QueryString = queryString,
                ActionArguments = actionArguments
            };

            logService.CreateUserActivityLog(logModel);
        }
    }
}
