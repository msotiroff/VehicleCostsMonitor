namespace VehicleCostsMonitor.Web.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using VehicleCostsMonitor.Common.Notifications;
    using static VehicleCostsMonitor.Common.Notifications.NotificationConstants;

    public abstract class BaseController : Controller
    {
        protected internal void ShowNotification(string message, NotificationType notificationType = NotificationType.Error)
        {
            this.TempData[NotificationMessageKey] = message;
            this.TempData[NotificationTypeKey] = notificationType.ToString();
        }
    }
}
