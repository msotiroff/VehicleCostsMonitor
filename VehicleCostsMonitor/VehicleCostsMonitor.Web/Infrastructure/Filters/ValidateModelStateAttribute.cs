using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Routing;
using System.Collections.Generic;
using System.Linq;
using VehicleCostsMonitor.Web.Controllers;

namespace VehicleCostsMonitor.Web.Infrastructure.Filters
{
    /// <summary>
    /// Validates the ModelState and, if there are any errors, returns the GET method with its arguments
    /// </summary>
    public class ValidateModelStateAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var modelState = context.ModelState;
            if (!modelState.IsValid)
            {
                var routeValues = context.ActionDescriptor.RouteValues;
                routeValues.TryGetValue("action", out string actionName);
                routeValues.TryGetValue("controller", out string controllerName);
                routeValues.TryGetValue("area", out string areaName);

                var model = context.ActionArguments.Values.FirstOrDefault();
                var modelProperties = model.GetType().GetProperties();

                var getMethod = context.Controller
                    .GetType()
                    .GetMethods()
                    .FirstOrDefault(mi => mi.Name == actionName
                        && (mi.CustomAttributes
                            .Any(attr => attr.GetType().Name == nameof(HttpGetAttribute))
                                || mi.CustomAttributes.All(attr => !typeof(HttpMethodAttribute).IsAssignableFrom(attr.GetType()))));
                
                var getMethodArguments = getMethod?
                    .GetParameters()
                    .ToDictionary(
                        pi => pi.Name,
                        pi => modelProperties.
                            FirstOrDefault(prop => prop.Name.ToLower() == pi.Name.ToLower())
                            ?.GetValue(model));

                getMethodArguments.Add("area", areaName);

                this.SetNotificationMessage(context, modelState);
                context.Result = new RedirectToActionResult(actionName, controllerName, getMethodArguments);
            }
        }

        private void SetNotificationMessage(ActionExecutingContext context, ModelStateDictionary modelState)
        {
            var firstOccuredErrorMsg = 
                modelState
                .Values
                .FirstOrDefault(v => v.Errors.Any())
                ?.Errors
                .FirstOrDefault()
                ?.ErrorMessage;

            var baseController = context.Controller as BaseController;

            baseController.ShowNotification(firstOccuredErrorMsg);
        }
    }
}
