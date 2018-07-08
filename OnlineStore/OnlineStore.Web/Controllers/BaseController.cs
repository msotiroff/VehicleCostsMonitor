namespace OnlineStore.Web.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using OnlineStore.Common.Notifications;
    using static OnlineStore.Common.Notifications.NotificationConstants;

    public abstract class BaseController : Controller
    {
        protected void ShowNotification(string message, NotificationType type)
        {
            this.TempData[NotificationMessageKey] = message;
            this.TempData[NotificationTypeKey] = type.ToString();
        }
    }
}