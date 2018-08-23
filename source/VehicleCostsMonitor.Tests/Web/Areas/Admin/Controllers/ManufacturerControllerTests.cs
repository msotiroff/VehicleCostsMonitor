namespace VehicleCostsMonitor.Tests.Web.Areas.Admin.Controllers
{
    using FluentAssertions;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;
    using Microsoft.EntityFrameworkCore;
    using Moq;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using VehicleCostsMonitor.Common.Notifications;
    using VehicleCostsMonitor.Data;
    using VehicleCostsMonitor.Models;
    using VehicleCostsMonitor.Services.Interfaces;
    using VehicleCostsMonitor.Services.Models.Manufacturer;
    using VehicleCostsMonitor.Services.Models.Vehicle;
    using VehicleCostsMonitor.Web;
    using VehicleCostsMonitor.Web.Areas.Admin.Controllers;
    using Xunit;

    public class ManufacturerControllerTests : BaseTest
    {
        private readonly Mock<IManufacturerService> manufacturerService;
        private readonly ManufacturerController controller;

        public ManufacturerControllerTests()
        {
            this.manufacturerService = new Mock<IManufacturerService>();
            this.controller = new ManufacturerController(this.manufacturerService.Object);
        }

        [Fact]
        public void ManufacturerController_ShouldBeAccessedOnlyByRoleAdministrator()
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
        public async Task Index_ShouldReturnViewWithCorrectModel()
        {
            // Arrange
            this.manufacturerService
                .Setup(s => s.AllAsync())
                .ReturnsAsync(new List<ManufacturerConciseListModel>
                {
                    new ManufacturerConciseListModel(),
                    new ManufacturerConciseListModel(),
                    new ManufacturerConciseListModel(),
                });

            // Act
            var result = await this.controller.Index() as ViewResult;
            var model = result?.ViewData.Model as IEnumerable<ManufacturerConciseListModel>;

            // Assert
            result
                .Should()
                .NotBeNull();

            model
                .Should()
                .NotBeNull()
                .And
                .HaveCount(3)
                .And
                .AllBeAssignableTo<ManufacturerConciseListModel>();
        }

        [Fact]
        public async Task Details_WithInvalidId_ShouldRedirectToIndex()
        {
            // Arrange
            this.InitializeTempData(this.controller);

            this.manufacturerService
                .Setup(s => s.GetDetailedAsync(It.IsAny<int>()))
                .ReturnsAsync(default(ManufacturerDetailsServiceModel));

            // Act
            var result = await this.controller.Details(5) as RedirectToActionResult;

            // Assert
            result
                .Should()
                .NotBeNull()
                .And
                .Match<RedirectToActionResult>(r => r.ActionName == nameof(controller.Index));
        }

        [Fact]
        public async Task Details_WithValidId_ShouldReturnViewWithCorrectModel()
        {
            // Arrange
            const int testedId = 5;
            const string testedName = "Mercedes-Benz";

            this.InitializeTempData(this.controller);

            this.manufacturerService
                .Setup(s => s.GetDetailedAsync(It.IsAny<int>()))
                .ReturnsAsync(new ManufacturerDetailsServiceModel
                {
                    Id = testedId,
                    Name = testedName,
                    Models = new List<ModelConciseServiceModel>
                    {
                        new ModelConciseServiceModel(),
                        new ModelConciseServiceModel(),
                        new ModelConciseServiceModel(),
                    }
                });

            // Act
            var result = await this.controller.Details(testedId) as ViewResult;
            var model = result?.ViewData.Model;

            // Assert
            result
                .Should()
                .NotBeNull();

            model
                .Should()
                .NotBeNull()
                .And
                .Match<ManufacturerDetailsServiceModel>(m => m.Id == testedId)
                .And
                .Match<ManufacturerDetailsServiceModel>(m => m.Name == testedName)
                .And
                .Match<ManufacturerDetailsServiceModel>(m => m.Models.Count() == 3)
                .And
                .Match<ManufacturerDetailsServiceModel>(m => m.Models
                    .All(mod => typeof(ModelConciseServiceModel)
                        .IsAssignableFrom(mod.GetType())));
        }

        [Fact]
        public async Task Create_WithInvalidName_ShouldSetErrorNotification()
        {
            // Arrange
            this.InitializeTempData(this.controller);

            this.manufacturerService
                .Setup(s => s.CreateAsync(It.IsAny<string>()))
                .ReturnsAsync(default(int));

            // Act
            var result = await this.controller.Create("");

            // Assert
            controller.TempData
                .Should()
                .Match<ITempDataDictionary>(td
                    => td[NotificationConstants.NotificationTypeKey].Equals(NotificationType.Error.ToString()));
        }

        [Fact]
        public async Task Create_WithValidName_ShouldSetSuccessNotification()
        {
            // Arrange
            this.InitializeTempData(this.controller);

            this.manufacturerService
                .Setup(s => s.CreateAsync(It.IsAny<string>()))
                .ReturnsAsync(1);

            // Act
            var result = await this.controller.Create("Mercedes-Benz");

            // Assert
            controller.TempData
                .Should()
                .Match<ITempDataDictionary>(td
                    => td[NotificationConstants.NotificationTypeKey].Equals(NotificationType.Success.ToString()));
        }

        [Fact]
        public async Task Create_WithValidName_ShouldRedirectToDetails()
        {
            // Arrange
            const int newVehicleId = 1;
            this.InitializeTempData(this.controller);

            this.manufacturerService
                .Setup(s => s.CreateAsync(It.IsAny<string>()))
                .ReturnsAsync(newVehicleId);

            // Act
            var result = await this.controller.Create("Mercedes-Benz") as RedirectToActionResult;

            // Assert
            result
                .Should()
                .NotBeNull()
                .And
                .Match<RedirectToActionResult>(r => r.RouteValues
                    .Any(rv => rv.Key == "id" && (int)rv.Value == newVehicleId));
        }

        [Fact]
        public async Task Edit_WithInvalidId_ShouldRedirectToIndex()
        {
            // Arrange
            this.InitializeTempData(this.controller);

            this.manufacturerService
                .Setup(s => s.GetForUpdateAsync(It.IsAny<int>()))
                .ReturnsAsync(default(ManufacturerUpdateServiceModel));

            // Act
            var result = await this.controller.Edit(1) as RedirectToActionResult;

            // Assert
            result
                .Should()
                .NotBeNull()
                .And
                .Match<RedirectToActionResult>(r => r.ActionName == nameof(this.controller.Index));
        }

        [Fact]
        public async Task Edit_WithInvalidId_ShouldSetErrorNotification()
        {
            // Arrange
            this.InitializeTempData(this.controller);

            this.manufacturerService
                .Setup(s => s.GetForUpdateAsync(It.IsAny<int>()))
                .ReturnsAsync(default(ManufacturerUpdateServiceModel));

            // Act
            var result = await this.controller.Edit(1) as RedirectToActionResult;
            var tempData = this.controller.TempData;

            // Assert
            tempData
                .Should()
                .Match<ITempDataDictionary>(td 
                    => td[NotificationConstants.NotificationTypeKey].Equals(NotificationType.Error.ToString()));
        }

        [Fact]
        public async Task Edit_WithValidId_ShouldReturnViewWithCorrectModel()
        {
            // Arrange
            this.InitializeTempData(this.controller);
            var expectedModel = new ManufacturerUpdateServiceModel
            {
                Id = 1,
                Name = "Mercedes-Benz"
            };

            this.manufacturerService
                .Setup(s => s.GetForUpdateAsync(It.IsAny<int>()))
                .ReturnsAsync(expectedModel);

            // Act
            var result = await this.controller.Edit(1) as ViewResult;
            var actionModel = result?.ViewData.Model as ManufacturerUpdateServiceModel;

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

        [Fact]
        public async Task Edit_ShouldCallServiceMethodUpdateAsync()
        {
            // Arrange
            this.InitializeTempData(this.controller);

            var called = false;

            this.manufacturerService
                .Setup(s => s.UpdateAsync(It.IsAny<int>(), It.IsAny<string>()))
                .ReturnsAsync(true)
                .Callback(() => called = true);

            // Act
            var result = await this.controller.Edit(new ManufacturerUpdateServiceModel());

            // Assert
            called
                .Should()
                .BeTrue();
        }

        [Fact]
        public async Task Edit_Post_ShouldRedirectToIndex()
        {
            // Arrange
            this.InitializeTempData(this.controller);

            this.manufacturerService
                .Setup(s => s.UpdateAsync(It.IsAny<int>(), It.IsAny<string>()))
                .ReturnsAsync(true);

            // Act
            var result = await this.controller.Edit(new ManufacturerUpdateServiceModel()) as RedirectToActionResult;

            // Assert
            result
                .Should()
                .NotBeNull()
                .And
                .Match<RedirectToActionResult>(r => r.ActionName == nameof(this.controller.Index));
        }


        [Fact]
        public async Task Delete_WithInvalidId_ShouldRedirectToIndex()
        {
            // Arrange
            this.InitializeTempData(this.controller);

            this.manufacturerService
                .Setup(s => s.GetForUpdateAsync(It.IsAny<int>()))
                .ReturnsAsync(default(ManufacturerUpdateServiceModel));

            // Act
            var result = await this.controller.Delete(1) as RedirectToActionResult;

            // Assert
            result
                .Should()
                .NotBeNull()
                .And
                .Match<RedirectToActionResult>(r => r.ActionName == nameof(this.controller.Index));
        }

        [Fact]
        public async Task Delete_WithInvalidId_ShouldSetErrorNotification()
        {
            // Arrange
            this.InitializeTempData(this.controller);

            this.manufacturerService
                .Setup(s => s.GetForUpdateAsync(It.IsAny<int>()))
                .ReturnsAsync(default(ManufacturerUpdateServiceModel));

            // Act
            var result = await this.controller.Delete(1) as RedirectToActionResult;
            var tempData = this.controller.TempData;

            // Assert
            tempData
                .Should()
                .Match<ITempDataDictionary>(td
                    => td[NotificationConstants.NotificationTypeKey].Equals(NotificationType.Error.ToString()));
        }

        [Fact]
        public async Task Delete_WithValidId_ShouldReturnViewWithCorrectModel()
        {
            // Arrange
            this.InitializeTempData(this.controller);
            var expectedModel = new ManufacturerUpdateServiceModel
            {
                Id = 1,
                Name = "Mercedes-Benz"
            };

            this.manufacturerService
                .Setup(s => s.GetForUpdateAsync(It.IsAny<int>()))
                .ReturnsAsync(expectedModel);

            // Act
            var result = await this.controller.Delete(1) as ViewResult;
            var actionModel = result?.ViewData.Model as ManufacturerUpdateServiceModel;

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

        [Fact]
        public async Task Delete_ShouldCallServiceMethodDeleteAsync()
        {
            // Arrange
            this.InitializeTempData(this.controller);

            var called = false;

            this.manufacturerService
                .Setup(s => s.DeleteAsync(It.IsAny<int>()))
                .ReturnsAsync(true)
                .Callback(() => called = true);

            // Act
            var result = await this.controller.Delete(new ManufacturerUpdateServiceModel());

            // Assert
            called
                .Should()
                .BeTrue();
        }

        [Fact]
        public async Task Delete_Post_ShouldRedirectToIndex()
        {
            // Arrange
            this.InitializeTempData(this.controller);

            this.manufacturerService
                .Setup(s => s.DeleteAsync(It.IsAny<int>()))
                .ReturnsAsync(true);

            // Act
            var result = await this.controller.Delete(new ManufacturerUpdateServiceModel()) as RedirectToActionResult;

            // Assert
            result
                .Should()
                .NotBeNull()
                .And
                .Match<RedirectToActionResult>(r => r.ActionName == nameof(this.controller.Index));
        }
    }
}
