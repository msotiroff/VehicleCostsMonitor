namespace VehicleCostsMonitor.Tests.Services.Implementations
{
    using Xunit;
    using FluentAssertions;
    using VehicleCostsMonitor.Services.Implementations;
    using VehicleCostsMonitor.Data;
    using VehicleCostsMonitor.Models;
    using System.Collections.Generic;
    using VehicleCostsMonitor.Services.Models.User;
    using System.Threading.Tasks;

    public class UserServiceTests : BaseTest
    {
        private const string UserEmailTemplate = "User_{0}@example.com";

        private readonly UserService userService;
        private readonly JustMonitorDbContext dbContext;

        public UserServiceTests()
        {
            this.dbContext = base.DatabaseInstance;
            this.userService = new UserService(this.dbContext);
        }

        #region GetAll Tests

        [Theory]
        [InlineData(1)]
        [InlineData(13)]
        [InlineData(124)]
        public void GetAll_ShouldReturnCorrectCountOfUsers(int usersCount)
        {
            // Arrange
            this.SeedUsers(usersCount);

            // Act
            var result = this.userService.GetAll();

            // Assert
            result
                .Should()
                .HaveCount(usersCount);
        }

        [Fact]
        public void GetAll_ShouldReturnCorrectModel()
        {
            // Arrange
            const int count = 5;
            this.SeedUsers(count);

            // Act
            var result = this.userService.GetAll();

            // Assert
            result
                .Should()
                .AllBeAssignableTo<UserListingServiceModel>();
        }

        [Fact]
        public void GetAll_ShouldReturnOrderedCollection()
        {
            // Arrange
            this.SeedUsers(13);

            // Act
            var result = this.userService.GetAll();

            // Assert
            result
                .Should()
                .BeInAscendingOrder(u => u.Email);
        }

        #endregion

        #region GetAsync Tests

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(5000)]
        public async Task GetAsync_WithInvalidId_ShouldReturnNull(int id)
        {
            // Arrange
            this.SeedUsers(5);

            // Act
            var result = await this.userService.GetAsync(id.ToString());

            // Assert
            result
                .Should()
                .BeNull();
        }

        [Fact]
        public async Task GetAsync_WithValidId_ShouldReturnCorrectModel()
        {
            // Arrange
            this.SeedUsers(10);
            const string userId = "1";

            // Act
            var result = await this.userService.GetAsync(userId);

            // Assert
            result
                .Should()
                .BeAssignableTo<UserProfileServiceModel>()
                .And
                .Match<UserProfileServiceModel>(m => m.Id == userId)
                .And
                .Match<UserProfileServiceModel>(m => m.Email == string.Format(UserEmailTemplate, userId));
        }

        #endregion

        #region GetByEmailAsync Tests

        [Theory]
        [InlineData("")]
        [InlineData("  ")]
        [InlineData(null)]
        public async Task GetByEmailAsync_WithEmptyEmail_ShouldReturnNull(string email)
        {
            // Arrange
            this.SeedUsers(5);

            // Act
            var result = await this.userService.GetByEmailAsync(email);

            // Assert
            result
                .Should()
                .BeNull();
        }

        [Fact]
        public async Task GetByemailAsync_WithValidEmail_ShouldReturnCorrectModel()
        {
            // Arrange
            this.SeedUsers(13);
            var email = string.Format(UserEmailTemplate, 1);

            // Act
            var result = await this.userService.GetByEmailAsync(email);

            // Assert
            result
                .Should()
                .BeAssignableTo<UserProfileServiceModel>()
                .And
                .Match<UserProfileServiceModel>(u => u.Email == email);
        }

        #endregion

        private void SeedUsers(int count)
        {
            var users = new List<User>();
            for (int i = 1; i <= count; i++)
            {
                users.Add(new User
                {
                    Id = i.ToString(),
                    Email = string.Format(UserEmailTemplate, i),
                });
            }

            this.dbContext.AddRange(users);
            this.dbContext.SaveChanges();
        }
    }
}
