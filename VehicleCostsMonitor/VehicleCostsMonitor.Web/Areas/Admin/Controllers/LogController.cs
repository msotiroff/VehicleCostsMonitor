namespace VehicleCostsMonitor.Web.Areas.Admin.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using VehicleCostsMonitor.Common.Notifications;
    using VehicleCostsMonitor.Services.Interfaces;
    using VehicleCostsMonitor.Services.Models.Log;
    using VehicleCostsMonitor.Web.Areas.Admin.Models.Log;
    using VehicleCostsMonitor.Web.Infrastructure.Collections;
    using static WebConstants;

    public class LogController : BaseAdminController
    {
        private readonly ILogService logService;

        public LogController(ILogService logService)
        {
            this.logService = logService;
        }

        public IActionResult Index(string searchTerm, int page = 1)
        {
            page = Math.Max(1, page);
            var allLogs = this.logService.GetAll();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                allLogs = allLogs.Where(l => l.UserEmail.ToLower().Contains(searchTerm.ToLower()));
            }

            var totalPages = (int)Math.Ceiling(allLogs.Count() / (double)LogsListPageSize);
            page = Math.Min(page, totalPages);

            var model = new UserActivityLogListViewModel
            {
                SearchTerm = searchTerm,
                Logs = new PaginatedList<UserActivityLogConciseServiceModel>(allLogs, page, totalPages)
            };

            return View(model);
        }

        public async Task<IActionResult> Details(int id)
        {
            var logModel = await this.logService.GetAsync(id);
            if (logModel == null)
            {
                this.ShowNotification(string.Format(NotificationMessages.LogDoesNotExist, id));
                return RedirectToAction(nameof(Index));
            }

            return View(logModel);
        }
    }
}