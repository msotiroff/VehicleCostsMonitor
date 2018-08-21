namespace VehicleCostsMonitor.Tests.Services.Implementations
{
    using FluentAssertions;
    using System.Linq;
    using System.Threading.Tasks;
    using VehicleCostsMonitor.Data;
    using VehicleCostsMonitor.Models;
    using VehicleCostsMonitor.Services.Implementations;
    using VehicleCostsMonitor.Services.Models.Manufacturer;
    using Xunit;

    public class ManufacturerServiceTests : BaseTest
    {
        private readonly ManufacturerService manufacturerService;
        private readonly JustMonitorDbContext dbContext;

        public ManufacturerServiceTests()
        {
            this.dbContext = base.DatabaseInstance;
            this.manufacturerService = new ManufacturerService(this.dbContext);
        }

        #region AllAsync Tests

        [Fact]
        public async Task AllAsync_ShouldReturnCollectionWithCorrectModel()
        {
            // Arrange
            this.dbContext.Manufacturers.Add(new Manufacturer());
            this.dbContext.SaveChanges();

            // Act
            var result = await this.manufacturerService.AllAsync();

            // Assert
            result
                .Should()
                .AllBeAssignableTo<ManufacturerConciseListModel>();
        }

        [Fact]
        public async Task AllAsync_ShouldReturnEmptyCollection()
        {
            // Act
            var result = await this.manufacturerService.AllAsync();

            // Assert
            result
                .Should()
                .BeEmpty();
        }

        [Fact]
        public async Task AllAsync_ShouldReturnOrderedByNameCollection()
        {
            // Arrange
            for (int i = 1; i <= 5; i++)
            {
                this.dbContext.Add(new Manufacturer { Id = i, Name = $"Make_{i}" });
            }
            this.dbContext.SaveChanges();

            // Act
            var result = await this.manufacturerService.AllAsync();

            // Assert
            result
                .Should()
                .BeInAscendingOrder(x => x.Name);
        }

        #endregion

        #region CreateAsync Tests

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public async Task CreateAsync_WithInvalidName_ShouldReturnFalseAndNotToAddRowInDatabase(string name)
        {
            // Act
            var result = await this.manufacturerService.CreateAsync(name);

            // Assert
            result
                .Should()
                .Be(default(int));

            this.dbContext
                .Manufacturers
                .Should()
                .BeEmpty();
        }

        [Theory]
        [InlineData("Mercedes-Benz")]
        [InlineData("Audi")]
        [InlineData("BMW")]
        public async Task CreateAsync_WithValidName_ShouldReturnTrueAndShouldAddCorrectEntityInDatabase(string name)
        {
            // Act
            var result = await this.manufacturerService.CreateAsync(name);

            // Assert
            this.dbContext
                .Manufacturers
                .Should()
                .HaveCount(1);

            this.dbContext
                .Manufacturers
                .First()
                .Should()
                .Match(x => x.As<Manufacturer>().Name == name);
        }

        #endregion

        #region DeleteAsync Tests

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(5000)]
        public async Task DeleteAsync_WithInvalidId_ShouldReturnFalseAndNotToDeleteRowFromDatabase(int id)
        {
            // Arrange
            const int count = 3;
            for (int i = 1; i <= count; i++)
            {
                this.dbContext.Add(new Manufacturer { Id = i });
            }
            this.dbContext.SaveChanges();

            // Act
            var result = await this.manufacturerService.DeleteAsync(id);

            // Assert
            result
                .Should()
                .BeFalse();


            this.dbContext
                .Manufacturers
                .Should()
                .HaveCount(count);
        }

        [Fact]
        public async Task DeleteAsync_WithValidId_ShouldReturnTrueAndShouldRemoveEntityFromDatabase()
        {
            // Arrange
            const int count = 3;
            const int id = 1;
            for (int i = 1; i <= count; i++)
            {
                this.dbContext.Add(new Manufacturer { Id = i });
            }
            this.dbContext.SaveChanges();

            // Act
            var result = await this.manufacturerService.DeleteAsync(id);

            // Assert
            result
                .Should()
                .BeTrue();

            this.dbContext
                .Manufacturers
                .Should()
                .HaveCount(count - 1);

            this.dbContext
                .Manufacturers
                .FirstOrDefault(m => m.Id == id)
                .Should()
                .BeNull();
        }

        #endregion

        #region GetForUpdateAsync Tests

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(5000)]
        public async Task GetForUpdateAsync_WithInvalidId_ShouldReturnNull(int id)
        {
            // Arrange
            for (int i = 1; i < 3; i++)
            {
                this.dbContext.Add(new Manufacturer { Id = i });
            }
            this.dbContext.SaveChanges();

            // Act
            var result = await this.manufacturerService.GetForUpdateAsync(id);

            // Assert
            result
                .Should()
                .BeNull();
        }

        [Fact]
        public async Task GetForUpdateAsync_WithValidId_ShouldReturnCorrectModel()
        {
            // Arrange
            const int id = 1;
            this.dbContext.Add(new Manufacturer { Id = id });

            // Act
            var result = await this.manufacturerService.GetForUpdateAsync(id);

            // Assert
            result
                .Should()
                .BeAssignableTo<ManufacturerUpdateServiceModel>();
        }

        [Fact]
        public async Task GetForUpdateAsync_WithValidId_ShouldReturnCorrectEntity()
        {
            // Arrange
            const int count = 3;
            const int testedId = 2;
            for (int i = 1; i <= count; i++)
            {
                this.dbContext.Add(new Manufacturer
                {
                    Id = i,
                    Name = $"Make_{i}"
                });
            }
            this.dbContext.SaveChanges();

            // Act
            var result = await this.manufacturerService.GetForUpdateAsync(testedId);

            // Assert
            result
                .Should()
                .Match(x => x.As<ManufacturerUpdateServiceModel>().Id == testedId)
                .And
                .Match(x => x.As<ManufacturerUpdateServiceModel>().Name == $"Make_{testedId}");
        }

        #endregion

        #region GetDetailedAsync Tests

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(5000)]
        public async Task GetDetailedAsync_WithInvalidId_ShouldReturnNull(int id)
        {
            // Arrange
            const int count = 3;
            for (int i = 1; i <= count; i++)
            {
                this.dbContext.Add(new Manufacturer { Id = i });
            }
            this.dbContext.SaveChanges();

            // Act
            var result = await this.manufacturerService.GetDetailedAsync(id);

            // Assert
            result
                .Should()
                .BeNull();
        }

        [Fact]
        public async Task GetDetailedAsync_WithValidId_ShouldReturnCorrectModel()
        {
            // Arrange
            const int id = 1;
            this.dbContext.Add(new Manufacturer { Id = id });
            this.dbContext.SaveChanges();

            // Act
            var result = await this.manufacturerService.GetDetailedAsync(id);

            // Assert
            result
                .Should()
                .BeAssignableTo<ManufacturerDetailsServiceModel>();
        }

        [Fact]
        public async Task GetDetailedAsync_WithValidId_ShouldReturnCorrectEntity()
        {
            // Arrange
            const int count = 3;
            const int testedId = 2;
            for (int i = 1; i <= count; i++)
            {
                this.dbContext.Add(new Manufacturer
                {
                    Id = i,
                    Name = $"Make_{i}"
                });
            }
            this.dbContext.SaveChanges();

            // Act
            var result = await this.manufacturerService.GetDetailedAsync(testedId);

            // Assert
            result
                .Should()
                .Match(x => x.As<ManufacturerDetailsServiceModel>().Id == testedId)
                .And
                .Match(x => x.As<ManufacturerDetailsServiceModel>().Name == $"Make_{testedId}");
        }

        #endregion

        #region UpdateAsync Tests

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(5000)]
        public async Task UpdateAsync_WithInvalidId_ShouldReturnFalse(int id)
        {
            // Arrange
            const int count = 3;
            const string name = "some name";
            for (int i = 1; i <= count; i++)
            {
                this.dbContext.Add(new Manufacturer { Id = i });
            }
            this.dbContext.SaveChanges();

            // Act
            var result = await this.manufacturerService.UpdateAsync(id, name);

            // Assert
            result
                .Should()
                .BeFalse();


            this.dbContext
                .Manufacturers
                .Should()
                .NotContain(x => x.Name == name);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public async Task UpdateAsync_WithInvalidName_ShouldReturnFalse(string name)
        {
            // Arrange
            const int testedId = 1;
            const string originalName = "Mercedes-Benz";
            this.dbContext.Add(new Manufacturer { Id = testedId, Name = originalName });
            this.dbContext.SaveChanges();

            // Act
            var result = await this.manufacturerService.UpdateAsync(testedId, name);

            // Assert
            result
                .Should()
                .BeFalse();
        }

        [Fact]
        public async Task UpdateAsync_WithValidArguments_ShouldReturnTrueAndUpdateTheEntity()
        {
            // Arrange
            const int testedId = 1;
            const string originalName = "Mercedes";
            const string newName = "Mercedes-Benz";

            this.dbContext.Add(new Manufacturer
            {
                Id = testedId,
                Name = originalName,
            });
            this.dbContext.SaveChanges();

            // Act
            var result = await this.manufacturerService.UpdateAsync(testedId, newName);

            // Assert
            result
                .Should()
                .BeTrue();

            this.dbContext
                .Manufacturers
                .First()
                .Should()
                .Match(x => x.As<Manufacturer>().Name == newName);
        }

        #endregion
    }
}
