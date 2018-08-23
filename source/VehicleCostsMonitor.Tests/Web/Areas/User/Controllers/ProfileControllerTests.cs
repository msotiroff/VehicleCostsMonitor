namespace VehicleCostsMonitor.Tests.Web.Areas.User.Controllers
{
    using Microsoft.AspNetCore.Identity;
    using VehicleCostsMonitor.Services.Interfaces;
    using VehicleCostsMonitor.Models;
    using VehicleCostsMonitor.Web.Areas.User.Controllers;
    using Moq;
    using VehicleCostsMonitor.Tests.Helpers;
    using Xunit;
    using System.Threading.Tasks;
    using VehicleCostsMonitor.Services.Models.User;
    using FluentAssertions;
    using Microsoft.AspNetCore.Mvc;
    using System.Collections.Generic;
    using VehicleCostsMonitor.Services.Models.Vehicle;
    using System.Linq;

    public class ProfileControllerTests : BaseTest
    {
        private readonly Mock<IUserService> userService;
        private readonly Mock<UserManager<User>> userManager;
        private readonly ProfileController controller;

        public ProfileControllerTests()
        {
            this.userService = new Mock<IUserService>();
            this.userManager = MockGenerator.UserManagerMock;

            this.controller = new ProfileController(this.userService.Object, this.userManager.Object);
        }

        [Fact]
        public async Task Index_WithInvalidUserId_ShouldRedirectToHomePage()
        {
            // Arrange
            const string loggedUserName = "Mollier";
            this.AddClaimsPrincipal(this.controller, loggedUserName);

            this.userService
                .Setup(us => us.GetAsync(It.IsAny<string>()))
                .ReturnsAsync(default(UserProfileServiceModel));

            // Act
            var result = await this.controller.Index() as RedirectResult;

            // Assert
            result
                .Should()
                .NotBeNull()
                .And
                .Match<RedirectResult>(r => r.Url == "/" || r.Url == "/home/index");
        }

        [Fact]
        public async Task Index_WithWrongId_ShouldRedirectToHomePage()
        {
            // Arrange
            const string loggedUserName = "Mollier";
            this.AddClaimsPrincipal(this.controller, loggedUserName);

            this.userService
                .Setup(us => us.GetAsync(It.IsAny<string>()))
                .ReturnsAsync(default(UserProfileServiceModel));

            // Act
            var result = await this.controller.Index() as RedirectResult;

            // Assert
            result
                .Should()
                .NotBeNull()
                .And
                .Match<RedirectResult>(r => r.Url == "/" || r.Url == "/home/index");
        }

        [Fact]
        public async Task Index_ValidId_ShouldReturViewWithCorrectModel()
        {
            // Arrange
            const string loggedUserId = "MollierId";
            const string loggedUserName = "Mollier";
            const string loggedUserEmail = "mollier@tartuffe.com";
            this.AddClaimsPrincipal(this.controller, loggedUserName);

            this.userService
                .Setup(us => us.GetAsync(It.IsAny<string>()))
                .ReturnsAsync(new UserProfileServiceModel
                {
                    Id = loggedUserId,
                    Email = loggedUserEmail,
                    UserName = loggedUserName,
                    Vehicles = new List<VehicleConciseServiceModel>
                    {
                        new VehicleConciseServiceModel(),
                        new VehicleConciseServiceModel(),
                        new VehicleConciseServiceModel(),
                    }
                });

            // Act
            var result = await this.controller.Index() as ViewResult;
            var model = result?.ViewData.Model;

            // Assert
            result
                .Should()
                .NotBeNull();

            model
                .Should()
                .NotBeNull()
                .And
                .Match<UserProfileServiceModel>(m => m.Id == loggedUserId)
                .And
                .Match<UserProfileServiceModel>(m => m.Email == loggedUserEmail)
                .And
                .Match<UserProfileServiceModel>(m => m.UserName == loggedUserName)
                .And
                .Match<UserProfileServiceModel>(m => m.Vehicles.Count() == 3);
        }
    }
}
