namespace VehicleCostsMonitor.Tests.Web.Areas.Admin.Controllers
{
    using FluentAssertions;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;
    using Moq;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using VehicleCostsMonitor.Common.Notifications;
    using VehicleCostsMonitor.Services.Interfaces;
    using VehicleCostsMonitor.Services.Models.Log;
    using VehicleCostsMonitor.Web;
    using VehicleCostsMonitor.Web.Areas.Admin.Controllers;
    using VehicleCostsMonitor.Web.Areas.Admin.Models.Enums;
    using VehicleCostsMonitor.Web.Areas.Admin.Models.Log;
    using VehicleCostsMonitor.Web.Infrastructure.Collections;
    using Xunit;

    public class LogControllerTests : BaseTest
    {
        private readonly DateTime SampleDate = new DateTime(2000, 1, 1);
        private const string UserEmailTemplate = "User_{0}@example.com";

        private readonly LogController controller;
        private readonly Mock<ILogService> logService;

        public LogControllerTests()
        {
            this.logService = new Mock<ILogService>();
            this.controller = new LogController(this.logService.Object);
        }

        [Fact]
        public void LogController_ShouldBeAccessedOnlyByRoleAdministrator()
        {
            // Arrange
            var authorizeAttributes = this.controller
                .GetType()
                .GetCustomAttributes(true)
                .Where(attr => typeof(AuthorizeAttribute)
                    .IsAssignableFrom(attr.GetType()))
                .Cast<AuthorizeAttribute>()
                .ToList();

            // Assert
            authorizeAttributes
                .Should()
                .Contain(attr => attr.Roles.Contains(WebConstants.AdministratorRole));
        }

        [Fact]
        public void Index_WithNoSearchTerms_ShouldReturnViewWithCorrectModel()
        {
            // Arrange
            var logsCount = WebConstants.LogsListPageSize * 3;
            var logs = this.GetCollectionOfLogs(logsCount, true).AsQueryable();
            this.logService
                .Setup(s => s.GetAll())
                .Returns(logs);

            // Act
            var result = this.controller.Index(null, null) as ViewResult;
            var model = result?.ViewData.Model as UserActivityLogListViewModel;
            var modelLogs = model?.Logs;

            // Assert
            result
                .Should()
                .NotBeNull();

            model
                .Should()
                .NotBeNull()
                .And
                .Match<UserActivityLogListViewModel>(m => m.Logs.PageIndex == 1)
                .And
                .Match<UserActivityLogListViewModel>(m => m.Logs.TotalPages == 3)
                .And
                .Match<UserActivityLogListViewModel>(m => m.Logs.HasNextPage == true)
                .And
                .Match<UserActivityLogListViewModel>(m => m.Logs.HasPreviousPage == false);

            modelLogs
                .Should()
                .NotBeNull()
                .And
                .BeInDescendingOrder(pl => pl.DateTime);
        }

        [Fact]
        public void Index_WithSearchTerms_ShouldReturnViewWithCorrectModel()
        {
            // Arrange
            var logsCount = WebConstants.LogsListPageSize * 3;
            var searchCriteria = LogSearchCriteria.Http_Method.ToString();
            var searchTerm = "get";
            var logs = this.GetCollectionOfLogs(logsCount, true).AsQueryable();
            this.logService
                .Setup(s => s.GetAll())
                .Returns(logs);

            // Act
            var result = this.controller.Index(searchTerm, searchCriteria) as ViewResult;
            var model = result?.ViewData.Model as UserActivityLogListViewModel;
            var modelLogs = model?.Logs;

            // Assert
            result
                .Should()
                .NotBeNull();

            modelLogs
                .Should()
                .NotBeNull()
                .And
                .BeInDescendingOrder(pl => pl.DateTime)
                .And
                .Match<PaginatedList<UserActivityLogConciseServiceModel>>(pl => pl
                    .All(x => x.HttpMethod.ToLower()
                        .Contains(searchTerm.ToLower())));
        }

        [Fact]
        public void Index_WithPageIndex_ShouldReturnModelWithCorrectPage()
        {
            // Arrange
            var expectedPageIndex = 2;
            var logsCount = WebConstants.LogsListPageSize * 3;
            var logs = this.GetCollectionOfLogs(logsCount, true).AsQueryable();
            this.logService
                .Setup(s => s.GetAll())
                .Returns(logs);

            // Act
            var result = this.controller.Index(null, null, expectedPageIndex) as ViewResult;
            var model = result?.ViewData.Model as UserActivityLogListViewModel;
            var modelLogs = model?.Logs;

            // Assert
            model
                .Should()
                .NotBeNull()
                .And
                .Match<UserActivityLogListViewModel>(m => m.Logs.PageIndex == expectedPageIndex)
                .And
                .Match<UserActivityLogListViewModel>(m => m.Logs.TotalPages == 3)
                .And
                .Match<UserActivityLogListViewModel>(m => m.Logs.HasNextPage == true)
                .And
                .Match<UserActivityLogListViewModel>(m => m.Logs.HasPreviousPage == true);
        }

        [Fact]
        public async Task Details_WithInvalidId_ShouldRedirectToIndex()
        {
            // Arrange
            this.InitializeTempData(this.controller);

            this.logService
                .Setup(s => s.GetAsync(It.IsAny<int>()))
                .ReturnsAsync(default(UserActivityLogDetailsServiceModel));

            // Act
            var result = await this.controller.Details(1) as RedirectToActionResult;

            // Assert
            result
                .Should()
                .NotBeNull()
                .And
                .Match<RedirectToActionResult>(r => r.ActionName == nameof(this.controller.Index));
        }

        [Fact]
        public async Task Details_WithInvalidId_ShouldSetErrorNotification()
        {
            // Arrange
            this.InitializeTempData(this.controller);

            this.logService
                .Setup(s => s.GetAsync(It.IsAny<int>()))
                .ReturnsAsync(default(UserActivityLogDetailsServiceModel));

            // Act
            var result = await this.controller.Details(1);
            var tempData = this.controller.TempData;

            // Assert
            tempData
                .Should()
                .Match<ITempDataDictionary>(td 
                    => td[NotificationConstants.NotificationTypeKey].Equals(NotificationType.Error.ToString()));
        }

        [Fact]
        public async Task Details_WithValidId_ShouldReturnViewWithCorrectModel()
        {
            // Arrange
            this.InitializeTempData(this.controller);
            var expectedModel = new UserActivityLogDetailsServiceModel
            {
                Id = 1,
                DateTime = new DateTime(2000, 1, 1),
                UserEmail = "some-user@example.com"
            };

            this.logService
                .Setup(s => s.GetAsync(It.IsAny<int>()))
                .ReturnsAsync(expectedModel);

            // Act
            var result = await this.controller.Details(1) as ViewResult;
            var actionModel = result?.ViewData.Model as UserActivityLogDetailsServiceModel;

            // Assert
            result
                .Should()
                .NotBeNull();

            actionModel
                .Should()
                .NotBeNull()
                .And
                .BeEquivalentTo(expectedModel);
        }

        private IEnumerable<UserActivityLogConciseServiceModel> GetCollectionOfLogs(int count, bool randomDate = false)
        {
            var random = new Random();
            var verbs = new string[] { "GET", "POST" };
            var logs = new List<UserActivityLogConciseServiceModel>();

            for (int i = 0; i < count; i++)
            {
                var log = new UserActivityLogConciseServiceModel
                {
                    DateTime = randomDate ? SampleDate.AddDays(random.Next(1, 1000)) : SampleDate,
                    UserEmail = string.Format(UserEmailTemplate, i),
                    HttpMethod = verbs[random.Next(0, verbs.Length)],
                };

                logs.Add(log);
            }

            return logs.OrderByDescending(l => l.DateTime);
        }
    }
}
