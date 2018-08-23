namespace VehicleCostsMonitor.Tests.Services.Implementations
{
    using FluentAssertions;
    using System.Linq;
    using System.Threading.Tasks;
    using VehicleCostsMonitor.Data;
    using VehicleCostsMonitor.Models;
    using VehicleCostsMonitor.Services.Implementations;
    using VehicleCostsMonitor.Services.Models.Picture;
    using Xunit;

    public class PictureServiceTests : BaseTest
    {
        private readonly JustMonitorDbContext dbContext;
        private readonly PictureService pictureService;

        public PictureServiceTests()
        {
            this.dbContext = base.DatabaseInstance;
            this.pictureService = new PictureService(this.dbContext);
        }

        #region CreateAsync Tests

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(5000)]
        public async Task CreateAsync_WithUnexistantVehicle_ShouldReturnFalseAndNotAddPictureToDatabase(int vehicleId)
        {
            // Arrange
            const int count = 3;
            for (int i = 1; i <= count; i++)
            {
                this.dbContext.Add(new Vehicle { Id = i });
            }
            this.dbContext.SaveChanges();

            // Act
            var result = await this.pictureService.CreateAsync("path", vehicleId);

            // Assert
            result
                .Should()
                .BeFalse();

            this.dbContext
                .Pictures
                .Should()
                .BeEmpty();
        }

        [Fact]
        public async Task CreateAsync_WithDeletedVehicle_ShouldReturnFalseAndNotAddPictureToDatabase()
        {
            // Arrange
            var vehicleId = 1;
            var vehicle = new Vehicle
            {
                Id = vehicleId,
                IsDeleted = true,
            };
            this.dbContext.Add(vehicle);
            this.dbContext.SaveChanges();

            // Act
            var result = await this.pictureService.CreateAsync("path", vehicleId);

            // Assert
            result
                .Should()
                .BeFalse();

            this.dbContext
                .Pictures
                .Should().BeEmpty();
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public async Task CreateAsync_WithEmptyPath_ShouldReturnFalseAndNotAddPictureToDatabase(string path)
        {
            // Arrange
            const int vehicleId = 1;
            var vehicle = new Vehicle { Id = vehicleId };
            this.dbContext.Add(vehicle);
            this.dbContext.SaveChanges();

            // Act
            var result = await this.pictureService.CreateAsync(path, vehicleId);

            // Assert
            result
                .Should()
                .BeFalse();

            this.dbContext
                .Pictures
                .Should()
                .BeEmpty();
        }

        [Fact]
        public async Task CreateAsync_WithValidData_ShouldReturnTrueAndAddPictureToDatabase()
        {
            // Arrange
            const int vehicleId = 1;
            const string path = "/somedirectory/somefile.jpeg";
            var vehicle = new Vehicle { Id = vehicleId };
            this.dbContext.Add(vehicle);
            this.dbContext.SaveChanges();

            // Act
            var result = await this.pictureService.CreateAsync(path, vehicleId);
            var picture = this.dbContext.Pictures.FirstOrDefault();

            // Assert
            result
                .Should()
                .BeTrue();

            picture
                .Should()
                .NotBeNull();

            picture
                .Should()
                .Match(p => p.As<Picture>().Path == path)
                .And
                .Match(p => p.As<Picture>().VehicleId == vehicleId);
        }

        #endregion

        #region DeleteAsync Tests

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(5000)]
        public async Task DeleteAsync_WithInvalidId_ShouldReturnFalseAndNotDeletePictureFromDatabase(int id)
        {
            // Arrange
            const int picturesCount = 5;
            for (int i = 1; i <= picturesCount; i++)
            {
                this.dbContext.Add(new Picture { Id = i });
            }
            this.dbContext.SaveChanges();

            // Act
            var result = await this.pictureService.DeleteAsync(id);

            // Assert
            result
                .Should()
                .BeFalse();

            this.dbContext
                .Pictures
                .Should()
                .HaveCount(picturesCount);
        }

        [Fact]
        public async Task DeleteAsync_WithValidId_ShouldReturnTrueAndRemovePictureFromDatabase()
        {
            // Arrange
            const int id = 1;
            var picture = new Picture { Id = id };
            this.dbContext.Add(picture);
            this.dbContext.SaveChanges();
            var picturesCountBeforeServiceCalled = this.dbContext.Pictures.Count();

            // Act
            var result = await this.pictureService.DeleteAsync(id);
            var picturesCountAfterServiceCalled = this.dbContext.Pictures.Count();

            // Assert
            result
                .Should()
                .BeTrue();

            picturesCountBeforeServiceCalled
                .Should()
                .Be(1);

            picturesCountAfterServiceCalled
                .Should()
                .Be(0);
        }

        #endregion

        #region GetByVehicleId Tests

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(5000)]
        public async Task GetByVehicleId_WithInvalidVehicleId_ShouldReturnNull(int vehicleId)
        {
            // Arrange
            var vehicle = new Vehicle { Id = 1 };
            var picture = new Picture { VehicleId = 1 };
            this.dbContext.Add(vehicle);
            this.dbContext.Add(picture);
            this.dbContext.SaveChanges();

            // Act
            var result = await this.pictureService.GetByVehicleId(vehicleId);

            // Assert
            result
                .Should()
                .BeNull();
        }

        [Fact]
        public async Task GetByVehicleId_WithIdOfDeletedVehicle_ShouldReturnNull()
        {
            // Arrange
            const int vehicleId = 5;

            var picture = new Picture
            {
                VehicleId = vehicleId,
                Vehicle = new Vehicle
                {
                    Id = vehicleId,
                    IsDeleted = true,
                }
            };

            this.dbContext.Add(picture);
            this.dbContext.SaveChanges();

            // Act
            var result = await this.pictureService.GetByVehicleId(vehicleId);

            // Assert
            result
                .Should()
                .BeNull();
        }

        [Fact]
        public async Task GetByVehicleId_WithValidVehicleId_ShouldReturnCorrectModel()
        {
            // Arrange
            const int vehicleId = 5;

            var picture = new Picture { VehicleId = vehicleId, Vehicle = new Vehicle { Id = vehicleId } };

            this.dbContext.Add(picture);
            this.dbContext.SaveChanges();

            // Act
            var result = await this.pictureService.GetByVehicleId(vehicleId);

            // Assert
            result
                .Should()
                .BeAssignableTo<PictureUpdateServiceModel>()
                .And
                .Match(p => p.As<PictureUpdateServiceModel>().VehicleId == vehicleId);
        }

        #endregion

        #region GetPathAsync Tests

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(5000)]
        public async Task GetPathAsync_WithInvalidId_ShouldReturnNull(int id)
        {
            // Arrange
            const int pictureId = 1;
            const string picturePath = "/dir/file.jpeg";
            var picture = new Picture
            {
                Id = pictureId,
                Path = picturePath
            };
            this.dbContext.Add(picture);
            this.dbContext.SaveChanges();

            // Act
            var result = await this.pictureService.GetPathAsync(id);

            // Assert
            result
                .Should()
                .BeNull();
        }

        [Fact]
        public async Task GetPathAsync_WithValidId_ShouldReturnCorrectValue()
        {
            // Arrange
            const int pictureId = 1;
            const string picturePath = "/dir/file.jpeg";
            var picture = new Picture
            {
                Id = pictureId,
                Path = picturePath,
            };
            this.dbContext.Add(picture);
            this.dbContext.SaveChanges();

            // Act
            var result = await this.pictureService.GetPathAsync(pictureId);

            // Assert
            result
                .Should()
                .NotBeNullOrWhiteSpace()
                .And
                .Match<string>(p => p == picturePath);
        }

        #endregion
    }
}
