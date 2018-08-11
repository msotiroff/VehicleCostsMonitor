namespace VehicleCostsMonitor.Web.Controllers
{
    using Common.Notifications;
    using Microsoft.AspNetCore.Mvc;
    using System.Collections.Generic;
    using System.Linq;
    using VehicleCostsMonitor.Web.Areas.User.Controllers;
    using VehicleCostsMonitor.Web.Infrastructure.Extensions.ExcelExport.Implementations;
    using static VehicleCostsMonitor.Common.Notifications.NotificationConstants;

    public abstract class BaseController : Controller
    {
        protected internal void ShowNotification(string message, NotificationType notificationType = NotificationType.Error)
        {
            this.TempData[NotificationMessageKey] = message;
            this.TempData[NotificationTypeKey] = notificationType.ToString();
        }

        protected internal void ShowModelStateError()
        {
            var firstOccuredErrorMsg = ModelState
                .Values
                .FirstOrDefault(v => v.Errors.Any())
                ?.Errors
                .FirstOrDefault()
                ?.ErrorMessage;

            if (firstOccuredErrorMsg != null)
            {
                this.ShowNotification(firstOccuredErrorMsg);
            }
        }

        protected IActionResult RedirectToHome() => new RedirectResult("/");

        protected IActionResult RedirectToProfile()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction(nameof(ProfileController.Index), "Profile", new { area = "user" });
            }

            return this.RedirectToHome();
        }

        protected ExcelResult<T> Excel<T>(IEnumerable<T> data) where T : class
            => new ExcelResult<T>(data);

        protected ExcelResult<T> Excel<T>(IEnumerable<T> data, string fileName) where T : class
            => new ExcelResult<T>(data, fileName);

        protected ExcelResult<T> Excel<T>(IEnumerable<T> data, string fileName, string sheetName) where T : class
            => new ExcelResult<T>(data, fileName, sheetName);
    }
}
