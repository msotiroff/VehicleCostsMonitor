namespace VehicleCostsMonitor.Web.Controllers
{
    using Common.Notifications;
    using Microsoft.AspNetCore.Mvc;
    using static VehicleCostsMonitor.Common.Notifications.NotificationConstants;

    public abstract class BaseController : Controller
    {
        protected internal void ShowNotification(string message, NotificationType notificationType = NotificationType.Error)
        {
            this.TempData[NotificationMessageKey] = message;
            this.TempData[NotificationTypeKey] = notificationType.ToString();
        }

        protected IActionResult RedirectToHome() => new RedirectResult("/");
    }
}
