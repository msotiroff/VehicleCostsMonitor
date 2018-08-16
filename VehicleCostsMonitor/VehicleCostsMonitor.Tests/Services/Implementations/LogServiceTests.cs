namespace VehicleCostsMonitor.Tests.Services.Implementations
{
    using Xunit;
    using FluentAssertions;
    using VehicleCostsMonitor.Services.Implementations;
    using VehicleCostsMonitor.Services.Models.Log;
    using VehicleCostsMonitor.Data;
    using System;
    using System.Linq;
    using VehicleCostsMonitor.Models;
    using System.Threading.Tasks;

    public class LogServiceTests : BaseTest
    {
        private readonly JustMonitorDbContext dbContext;
        private readonly LogService logService;

        public LogServiceTests()
        {
            this.dbContext = this.DatabaseInstance;
            this.logService = new LogService(this.dbContext);
        }

        #region CreateUserActivityLog Tests

        [Fact]
        public void CreateUserActivityLog_WithValidData_ShouldAddLogToDatabasewithCorrectValues()
        {
            // Arrange
            const string userEmail = "user@monitor.com";
            const string controllerName = "SomeController";
            const string actionName = "SomeAction";
            const string httpMethod = "GET";

            var model = new UserActivityLogCreateModel
            {
                DateTime = new DateTime(2000, 12, 12),
                UserEmail = userEmail,
                ActionName = actionName,
                ControllerName = controllerName,
                HttpMethod = httpMethod
            };

            // Act
            this.logService.CreateUserActivityLog(model);
            var dbLog = this.dbContext
                .UserActivityLogs
                .FirstOrDefault();

            // Assert
            this.dbContext.UserActivityLogs
                .Should()
                .HaveCount(1);

            dbLog
                .Should()
                .BeAssignableTo<UserActivityLog>();

            dbLog
                .Should()
                .Match(x => x.As<UserActivityLog>()
                    .UserEmail == userEmail);

            dbLog
                .Should()
                .Match(x => x.As<UserActivityLog>()
                    .ControllerName == controllerName);

            dbLog
                .Should()
                .Match(x => x.As<UserActivityLog>()
                    .ActionName == actionName);

            dbLog
                .Should()
                .Match(x => x.As<UserActivityLog>()
                    .HttpMethod == httpMethod);
        }

        [Fact]
        public void CreateUserActivityLog_WithMissingUserEmail_ShoulNotInsertEntityToDatabase()
        {
            // Arrange
            const string controllerName = "SomeController";
            const string actionName = "SomeAction";
            const string httpMethod = "GET";

            var model = new UserActivityLogCreateModel
            {
                DateTime = new DateTime(2000, 1, 1),
                ActionName = actionName,
                ControllerName = controllerName,
                HttpMethod = httpMethod
            };

            // Act
            var result = this.logService.CreateUserActivityLog(model);

            // Assert
            result.Should().BeFalse();
            this.dbContext.UserActivityLogs.Should().HaveCount(0);
        }

        [Fact]
        public void CreateUserActivityLog_WithMissingController_ShoulNotInsertEntityToDatabase()
        {
            // Arrange
            const string userEmail = "user@monitor.com";
            const string actionName = "SomeAction";
            const string httpMethod = "GET";

            var model = new UserActivityLogCreateModel
            {
                DateTime = new DateTime(2000, 1, 1),
                UserEmail = userEmail,
                ActionName = actionName,
                HttpMethod = httpMethod
            };

            // Act
            var result = this.logService.CreateUserActivityLog(model);

            // Assert
            result.Should().BeFalse();
            this.dbContext.UserActivityLogs.Should().HaveCount(0);
        }

        [Fact]
        public void CreateUserActivityLog_WithMissingAction_ShoulNotInsertEntityToDatabase()
        {
            // Arrange
            const string userEmail = "user@monitor.com";
            const string controllerName = "SomeController";
            const string httpMethod = "GET";

            var model = new UserActivityLogCreateModel
            {
                DateTime = new DateTime(2000, 1, 1),
                UserEmail = userEmail,
                ControllerName = controllerName,
                HttpMethod = httpMethod
            };

            // Act
            var result = this.logService.CreateUserActivityLog(model);

            // Assert
            result.Should().BeFalse();
            this.dbContext.UserActivityLogs.Should().HaveCount(0);
        }

        [Fact]
        public void CreateUserActivityLog_WithMissingHttpMethod_ShoulNotInsertEntityToDatabase()
        {
            // Arrange
            const string userEmail = "user@monitor.com";
            const string controllerName = "SomeController";
            const string actionName = "SomeAction";

            var model = new UserActivityLogCreateModel
            {
                DateTime = new DateTime(2000, 1, 1),
                UserEmail = userEmail,
                ActionName = actionName,
                ControllerName = controllerName
            };

            // Act
            var result = this.logService.CreateUserActivityLog(model);

            // Assert
            result.Should().BeFalse();
            this.dbContext.UserActivityLogs.Should().HaveCount(0);
        }

        #endregion

        #region GetAll Tests

        [Fact]
        public void GetAll_ShouldReturnQueryWithValidModel()
        {
            // Arrange
            this.dbContext.UserActivityLogs.Add(new UserActivityLog());
            this.dbContext.UserActivityLogs.Add(new UserActivityLog());
            this.dbContext.SaveChanges();

            // Act
            var result = this.logService.GetAll();

            // Assert
            result
                .Should()
                .BeAssignableTo<IQueryable<UserActivityLogConciseServiceModel>>();
        }

        [Fact]
        public void GetAll_ShouldOrderEntitiesByDateDescending()
        {
            // Arrange
            var date = new DateTime(2000, 1, 1);
            for (int i = 1; i <= 3; i++)
            {
                this.dbContext.UserActivityLogs.Add(new UserActivityLog
                {
                    Id = i,
                    DateTime = date.AddDays(i)
                });
            };
            this.dbContext.SaveChanges();

            // Act
            var result = this.logService.GetAll();

            // Assert
            result.Should().HaveCount(3);
            result.Should().BeInDescendingOrder(x => x.DateTime);
        }

        [Fact]
        public void GetAllShouldReturnCorrectCountOfEntities()
        {
            // Arrange
            for (int i = 0; i < 3; i++)
            {
                this.dbContext.UserActivityLogs.Add(new UserActivityLog());
            };
            this.dbContext.SaveChanges();

            // Act
            var result = this.logService.GetAll();

            // Assert
            result.Should().HaveCount(3);
        }

        #endregion

        #region GetAsyncTests

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public async Task GetAsync_WithInvalidId_ShouldReturnNull(int id)
        {
            // Arrange
            for (int i = 1; i < 3; i++)
            {
                this.dbContext.Add(new UserActivityLog() { Id = i });
            }
            this.dbContext.SaveChanges();

            // Act
            var result = await this.logService.GetAsync(id);

            // Assert
            result.Should().BeNull();
        }

        [Theory]
        [InlineData(1)]
        [InlineData(55)]
        [InlineData(101)]
        public async Task GetAsync_WithValidId_ShouldReturnTheCorrectEntity(int id)
        {
            // Arrange
            this.dbContext.Add(new UserActivityLog { Id = id });
            this.dbContext.SaveChanges();

            // Act
            var result = await this.logService.GetAsync(id);

            // Assert
            result.Should().Match(x => x.As<UserActivityLogDetailsServiceModel>().Id == id);
        }

        [Fact]
        public async Task GetAsync_ShouldReturnCorrectModel()
        {
            // Arrange
            this.dbContext.Add(new UserActivityLog() { Id = 1 });
            this.dbContext.SaveChanges();

            // Act
            var result = await this.logService.GetAsync(1);

            // Assert
            result.Should().BeAssignableTo<UserActivityLogDetailsServiceModel>();
        }

        #endregion
    }
}
