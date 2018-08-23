namespace VehicleCostsMonitor.Tests.Web.Areas.Admin.Controllers
{
    using AutoMapper;
    using AutoMapper.QueryableExtensions;
    using FluentAssertions;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;
    using Moq;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using VehicleCostsMonitor.Common.Notifications;
    using VehicleCostsMonitor.Models;
    using VehicleCostsMonitor.Services.Interfaces;
    using VehicleCostsMonitor.Services.Models.User;
    using VehicleCostsMonitor.Tests.Helpers;
    using VehicleCostsMonitor.Web;
    using VehicleCostsMonitor.Web.Areas.Admin.Controllers;
    using VehicleCostsMonitor.Web.Areas.Admin.Models.User;
    using Xunit;

    public class UserControllerTests : BaseTest
    {
        private const string SampleUserEmail = "user@example.com";
        private const string UserEmailTemplate = "User_{0}@example.com";
        private const string UserUsernameTemplate = "User_{0}";

        private readonly Mock<RoleManager<IdentityRole>> roleManager;
        private readonly Mock<UserManager<User>> userManager;
        private readonly Mock<IUserService> userService;
        private readonly UserController controller;

        public UserControllerTests()
        {
            this.roleManager = MockGenerator.RoleManagerMock;
            this.userManager = MockGenerator.UserManagerMock;
            this.userService = new Mock<IUserService>();

            this.controller = new UserController(this.roleManager.Object, this.userManager.Object, this.userService.Object);
        }

        [Fact]
        public void UsersController_ShouldBeAccessedOnlyByRoleAdministrator()
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

        #region Index Tests

        [Fact]
        public async Task Index_WithSearchTerm_ShouldReturnViewWithCorrectModel()
        {
            // Arrange
            const int usersCount = 30;
            const string searchTerm = "USER_1";
            var users = this.GetCollectionOfUsers(usersCount).AsQueryable();
            this.SetupDependencies(users);

            // Act
            var result = await this.controller.Index(searchTerm) as ViewResult;
            var model = result?.ViewData.Model as UserListingViewModel;

            // Assert
            model
                .Should()
                .NotBeNull()
                .And
                .Match<UserListingViewModel>(m => m.Users
                    .All(u => u.Email.ToLower().Contains(searchTerm.ToLower())));
        }

        [Fact]
        public async Task Index_WithoutSearchTerm_ShouldReturnModelWithCorrectPageIndex()
        {
            // Arrange
            const int pageIndex = 2;
            var usersCount = WebConstants.UsersListPageSize * 3;
            var users = this.GetCollectionOfUsers(usersCount).AsQueryable();
            this.SetupDependencies(users);

            // Act
            var result = await this.controller.Index(null, pageIndex) as ViewResult;
            var model = result?.ViewData.Model as UserListingViewModel;

            // Assert
            result
                .Should()
                .NotBeNull();

            model
                .Should()
                .NotBeNull()
                .And
                .Match<UserListingViewModel>(m => m.Users.PageIndex == pageIndex);
        }

        [Fact]
        public async Task Index_WithoutSearchTerm_ShouldReturnViewWithCorrectModel()
        {
            // Arrange
            const int usersCount = 30;
            var users = this.GetCollectionOfUsers(usersCount).AsQueryable();
            this.SetupDependencies(users);

            // Act
            var result = await this.controller.Index(null) as ViewResult;
            var model = result?.ViewData.Model as UserListingViewModel;

            // Assert
            result
                .Should()
                .NotBeNull();

            model
                .Should()
                .NotBeNull()
                .And
                .Match<UserListingViewModel>(m => m.Users
                    .Count() == WebConstants.UsersListPageSize)
                .And
                .Match<UserListingViewModel>(m => m.Users
                    .All(u => typeof(UserListingServiceModel).IsAssignableFrom(u.GetType())));
        }

        #endregion

        #region AddToRole Tests

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public async Task AddToRole_WithoutRole_ShouldReturnToIndex(string invalidRole)
        {
            // Arrange
            this.InitializeTempData(this.controller);

            // Act
            var result = await this.controller.AddToRole(SampleUserEmail, invalidRole) as RedirectToActionResult;

            // Assert
            result
                .Should()
                .NotBeNull()
                .And
                .Match<RedirectToActionResult>(r => r.ActionName == nameof(this.controller.Index));
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public async Task AddToRole_WithoutRole_ShouldSetErrorNotification(string invalidRole)
        {
            // Arrange
            this.InitializeTempData(this.controller);

            // Act
            var result = await this.controller.AddToRole(SampleUserEmail, invalidRole) as RedirectToActionResult;
            var tempData = this.controller.TempData;

            // Assert
            tempData
                .Should()
                .NotBeNull()
                .And
                .Match<ITempDataDictionary>(td
                    => td[NotificationConstants.NotificationTypeKey].Equals(NotificationType.Error.ToString()));
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public async Task AddToRole_WithoutEmail_ShouldReturnToIndex(string invalidEmail)
        {
            // Arrange
            this.InitializeTempData(this.controller);

            // Act
            var result = await this.controller.AddToRole(invalidEmail, WebConstants.AdministratorRole) as RedirectToActionResult;

            // Assert
            result
                .Should()
                .NotBeNull()
                .And
                .Match<RedirectToActionResult>(r => r.ActionName == nameof(this.controller.Index));
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public async Task AddToRole_WithoutEmail_ShouldSetErrorNotification(string invalidEmail)
        {
            // Arrange
            this.InitializeTempData(this.controller);

            // Act
            var result = await this.controller.AddToRole(invalidEmail, WebConstants.AdministratorRole) as RedirectToActionResult;
            var tempData = this.controller.TempData;

            // Assert
            tempData
                .Should()
                .NotBeNull()
                .And
                .Match<ITempDataDictionary>(td
                    => td[NotificationConstants.NotificationTypeKey].Equals(NotificationType.Error.ToString()));
        }

        [Fact]
        public async Task AddToRole_WhenNotSuccess_ShouldSetErrorNotification()
        {
            // Arrange
            this.InitializeTempData(this.controller);
            const int usersCount = 30;
            var users = this.GetCollectionOfUsers(usersCount).AsQueryable();
            this.SetupDependencies(users);
            this.userManager
                .Setup(um => um.AddToRoleAsync(It.IsAny<User>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Failed(new IdentityError()));


            // Act
            var result = await this.controller.AddToRole(SampleUserEmail, WebConstants.AdministratorRole) as RedirectToActionResult;
            var tempData = this.controller.TempData;

            // Assert
            tempData
                .Should()
                .NotBeNull()
                .And
                .Match<ITempDataDictionary>(td
                    => td[NotificationConstants.NotificationTypeKey].Equals(NotificationType.Error.ToString()));
        }

        [Fact]
        public async Task AddToRole_WhenSuccess_ShouldSetSuccessNotification()
        {
            // Arrange
            this.InitializeTempData(this.controller);
            const int usersCount = 30;
            var users = this.GetCollectionOfUsers(usersCount).AsQueryable();
            this.SetupDependencies(users);
            this.userManager
                .Setup(um => um.AddToRoleAsync(It.IsAny<User>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);


            // Act
            var result = await this.controller.AddToRole(SampleUserEmail, WebConstants.AdministratorRole) as RedirectToActionResult;
            var tempData = this.controller.TempData;

            // Assert
            tempData
                .Should()
                .NotBeNull()
                .And
                .Match<ITempDataDictionary>(td
                    => td[NotificationConstants.NotificationTypeKey].Equals(NotificationType.Success.ToString()));
        }

        #endregion


        #region RemoveFromRole Tests

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public async Task RemoveFromRole_WithoutRole_ShouldReturnToIndex(string invalidRole)
        {
            // Arrange
            this.InitializeTempData(this.controller);

            // Act
            var result = await this.controller.RemoveFromRole(SampleUserEmail, invalidRole) as RedirectToActionResult;

            // Assert
            result
                .Should()
                .NotBeNull()
                .And
                .Match<RedirectToActionResult>(r => r.ActionName == nameof(this.controller.Index));
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public async Task RemoveFromRole_WithoutRole_ShouldSetErrorNotification(string invalidRole)
        {
            // Arrange
            this.InitializeTempData(this.controller);

            // Act
            var result = await this.controller.RemoveFromRole(SampleUserEmail, invalidRole) as RedirectToActionResult;
            var tempData = this.controller.TempData;

            // Assert
            tempData
                .Should()
                .NotBeNull()
                .And
                .Match<ITempDataDictionary>(td
                    => td[NotificationConstants.NotificationTypeKey].Equals(NotificationType.Error.ToString()));
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public async Task RemoveFromRole_WithoutEmail_ShouldReturnToIndex(string invalidEmail)
        {
            // Arrange
            this.InitializeTempData(this.controller);

            // Act
            var result = await this.controller.RemoveFromRole(invalidEmail, WebConstants.AdministratorRole) as RedirectToActionResult;

            // Assert
            result
                .Should()
                .NotBeNull()
                .And
                .Match<RedirectToActionResult>(r => r.ActionName == nameof(this.controller.Index));
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public async Task RemoveFromRole_WithoutEmail_ShouldSetErrorNotification(string invalidEmail)
        {
            // Arrange
            this.InitializeTempData(this.controller);

            // Act
            var result = await this.controller.RemoveFromRole(invalidEmail, WebConstants.AdministratorRole) as RedirectToActionResult;
            var tempData = this.controller.TempData;

            // Assert
            tempData
                .Should()
                .NotBeNull()
                .And
                .Match<ITempDataDictionary>(td
                    => td[NotificationConstants.NotificationTypeKey].Equals(NotificationType.Error.ToString()));
        }

        [Fact]
        public async Task RemoveFromRole_WhenNotSuccess_ShouldSetErrorNotification()
        {
            // Arrange
            this.AddClaimsPrincipal(this.controller, "SomeUserName");
            this.InitializeTempData(this.controller);
            const int usersCount = 30;
            var users = this.GetCollectionOfUsers(usersCount).AsQueryable();
            this.SetupDependencies(users);

            this.userManager
                .Setup(um => um.GetUserAsync(this.controller.User))
                .ReturnsAsync(new User { Email = "Some@Email.com" });

            this.userManager
                .Setup(um => um.RemoveFromRoleAsync(It.IsAny<User>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Failed(new IdentityError()));


            // Act
            var result = await this.controller.RemoveFromRole(SampleUserEmail, WebConstants.AdministratorRole) as RedirectToActionResult;
            var tempData = this.controller.TempData;

            // Assert
            tempData
                .Should()
                .NotBeNull()
                .And
                .Match<ITempDataDictionary>(td
                    => td[NotificationConstants.NotificationTypeKey].Equals(NotificationType.Error.ToString()));
        }

        [Fact]
        public async Task RemoveFromRole_WhenSuccess_ShouldSetSuccessNotification()
        {
            // Arrange
            this.AddClaimsPrincipal(this.controller, "SomeUserName");
            this.InitializeTempData(this.controller);
            const int usersCount = 30;
            var users = this.GetCollectionOfUsers(usersCount).AsQueryable();
            this.SetupDependencies(users);

            this.userManager
                .Setup(um => um.GetUserAsync(this.controller.User))
                .ReturnsAsync(new User { Email = "Some@Email.com" });

            this.userManager
                .Setup(um => um.RemoveFromRoleAsync(It.IsAny<User>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);


            // Act
            var result = await this.controller.RemoveFromRole(SampleUserEmail, WebConstants.AdministratorRole) as RedirectToActionResult;
            var tempData = this.controller.TempData;

            // Assert
            tempData
                .Should()
                .NotBeNull()
                .And
                .Match<ITempDataDictionary>(td
                    => td[NotificationConstants.NotificationTypeKey].Equals(NotificationType.Success.ToString()));
        }

        #endregion

        private void SetupDependencies(IQueryable<UserListingServiceModel> users)
        {
            var roles = new IdentityRole[]
                        {
                            new IdentityRole(WebConstants.AdministratorRole),
                        };

            this.userService
                .Setup(s => s.GetAll())
                .Returns(users);

            this.roleManager
                .Setup(rm => rm.Roles)
                .Returns(roles.AsQueryable());

            this.userManager
                .Setup(um => um.FindByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(new User());

            this.userManager
                .Setup(um => um.GetRolesAsync(It.IsAny<User>()))
                .ReturnsAsync(new List<string> { WebConstants.AdministratorRole });
        }

        private IEnumerable<UserListingServiceModel> GetCollectionOfUsers(int count)
        {
            var users = new List<UserListingServiceModel>();

            for (int i = 0; i < count; i++)
            {
                var user = new UserListingServiceModel
                {
                    Email = string.Format(UserEmailTemplate, i),
                    UserName = string.Format(UserUsernameTemplate, i),
                };

                users.Add(user);
            }

            return users;
        }
    }
}
