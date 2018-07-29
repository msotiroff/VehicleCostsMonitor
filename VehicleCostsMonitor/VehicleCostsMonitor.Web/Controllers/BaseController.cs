namespace VehicleCostsMonitor.Web.Controllers
{
    using Common.Notifications;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using VehicleCostsMonitor.Models;
    using VehicleCostsMonitor.Web.Areas.User.Controllers;
    using static VehicleCostsMonitor.Common.Notifications.NotificationConstants;

    public abstract class BaseController : Controller
    {
        protected internal void ShowNotification(string message, NotificationType notificationType = NotificationType.Error)
        {
            this.TempData[NotificationMessageKey] = message;
            this.TempData[NotificationTypeKey] = notificationType.ToString();
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
    }
}
