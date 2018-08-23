namespace VehicleCostsMonitor.Web.Areas.Admin.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using VehicleCostsMonitor.Common.Notifications;
    using VehicleCostsMonitor.Services.Interfaces;
    using VehicleCostsMonitor.Services.Models.Log;
    using VehicleCostsMonitor.Web.Areas.Admin.Models.Enums;
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

        public IActionResult Index(string searchTerm, string criteria, int page = 1)
        {
            page = Math.Max(1, page);
            var allLogs = this.logService.GetAll();

            Enum.TryParse(criteria, out LogSearchCriteria logCriteria);

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                switch (logCriteria)
                {
                    case LogSearchCriteria.Email:
                        allLogs = allLogs.Where(l => l.UserEmail.ToLower().Contains(searchTerm.ToLower()));
                        break;
                    case LogSearchCriteria.Controller:
                        allLogs = allLogs.Where(l => l.ControllerName.ToLower().Contains(searchTerm.ToLower()));
                        break;
                    case LogSearchCriteria.Action:
                        allLogs = allLogs.Where(l => l.ActionName.ToLower().Contains(searchTerm.ToLower()));
                        break;
                    case LogSearchCriteria.Http_Method:
                        allLogs = allLogs.Where(l => l.HttpMethod.ToLower().Contains(searchTerm.ToLower()));
                        break;
                    default:
                        break;
                }
            }

            var totalPages = (int)(Math.Ceiling(allLogs.Count() / (double)LogsListPageSize));
            page = Math.Min(page, Math.Max(1, totalPages));

            var logsToShow = allLogs
                .Skip((page - 1) * LogsListPageSize)
                .Take(LogsListPageSize)
                .ToList();

            var model = new UserActivityLogListViewModel
            {
                SearchTerm = searchTerm,
                AllSearchCriterias = Enum.GetNames(typeof(LogSearchCriteria)).Select(c => new SelectListItem(c, c)),
                SearchCriteria = criteria,
                Logs = new PaginatedList<UserActivityLogConciseServiceModel>(logsToShow, page, totalPages)
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