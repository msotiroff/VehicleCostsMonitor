namespace VehicleCostsMonitor.Tests.Services.Implementations
{
    using FluentAssertions;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using VehicleCostsMonitor.Data;
    using VehicleCostsMonitor.Models;
    using VehicleCostsMonitor.Services.Implementations;
    using VehicleCostsMonitor.Services.Models.Vehicle;
    using Xunit;

    public class VehicleModelServiceTests : BaseTest
    {
        private const string ModelNameTemplate = "Model_{0}";

        private readonly JustMonitorDbContext dbContext;
        private readonly VehicleModelService modelService;

        public VehicleModelServiceTests()
        {
            this.dbContext = base.DatabaseInstance;
            this.modelService = new VehicleModelService(this.dbContext);
        }

        #region CreateAsync Tests

        [Fact]
        public async Task CreateAsync_WithDuplicatedName_ShouldReturnFalseAndNotAddModelToDatabase()
        {
            // Arrange
            const int manufacturerId = 1;
            const string modelName = "E 500";
            var model = new Model
            {
                ManufacturerId = manufacturerId,
                Name = modelName
            };
            this.dbContext.Add(model);
            this.dbContext.SaveChanges();

            // Act
            var result = await this.modelService.CreateAsync(modelName, manufacturerId);

            // Assert
            result
                .Should()
                .BeFalse();

            this.dbContext
                .Models
                .Should()
                .HaveCount(1);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public async Task CreateAsync_WithEmptyModelName_ShouldReturnFalseAndNotAddModelToDatabase(string modelName)
        {
            // Act
            var result = await this.modelService.CreateAsync(modelName, 1);

            // Assert
            result
                .Should()
                .BeFalse();

            this.dbContext
                .Models
                .Should()
                .BeEmpty();
        }

        [Fact]
        public async Task CreateAsync_WithValidName_ShouoldReturnTrueAndAddCorrectModelToDatabase()
        {
            // Arrange
            const string modelName = "E 500";
            const int manufacturerId = 1;

            // Act
            var result = await this.modelService.CreateAsync(modelName, manufacturerId);

            // Assert
            result
                .Should()
                .BeTrue();

            this.dbContext
                .Models
                .Should()
                .HaveCount(1);

            this.dbContext
                .Models
                .First()
                .Should()
                .Match<Model>(m => m.Name == modelName)
                .And
                .Match<Model>(m => m.ManufacturerId == manufacturerId);
        }

        #endregion

        #region DeleteAsync Tests

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(5000)]
        public async Task DeleteAsync_WithInvalidId_ShouldReturnFalseAndNotRemoveModelFromDatabase(int id)
        {
            // Arrange
            const int modelsCount = 10;
            this.SeedModels(modelsCount);

            // Act
            var result = await this.modelService.DeleteAsync(id);

            // Assert
            result
                .Should()
                .BeFalse();

            this.dbContext
                .Models
                .Should()
                .HaveCount(modelsCount);
        }

        [Fact]
        public async Task DeleteAsync_WithValidId_ShouldReturnTrueAndRemoveModelFromDatabase()
        {
            // Arrange
            const int modelsCount = 3;
            this.SeedModels(modelsCount);
            var countBeforeServiceCalled = this.dbContext.Models.Count();

            // Act
            var result = await this.modelService.DeleteAsync(1);
            var countAfterServiceCalled = this.dbContext.Models.Count();

            // Assert
            result
                .Should()
                .BeTrue();

            countBeforeServiceCalled
                .Should()
                .Be(modelsCount);

            countAfterServiceCalled
                .Should()
                .Be(modelsCount - 1);
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
            this.SeedModels(10);

            // Act
            var result = await this.modelService.GetAsync(id);

            // Assert
            result
                .Should()
                .BeNull();
        }

        [Fact]
        public async Task GetAsync_WithValidId_ShouldReturnCorrectModel()
        {
            // Arrange
            const int id = 1;
            this.SeedModels(10);

            // Act
            var result = await this.modelService.GetAsync(id);

            // Assert
            result
                .Should()
                .Match<ModelConciseServiceModel>(m => m.Id == id);
        }

        #endregion

        #region GetByManufacturerIdAsync Tests

        [Fact]
        public async Task GetByManufacturerIdAsync_WithInvalidManufacturerId_ShouldReturnEmptyCollection()
        {
            // Arrange
            const int modelsCount = 10;
            const int manufacturerId = 1;
            this.SeedModels(modelsCount, manufacturerId);

            // Act
            var result = await this.modelService.GetByManufacturerIdAsync(2);

            // Assert
            result
                .Should()
                .BeEmpty();
        }

        [Fact]
        public async Task GetByManufacturerId_WithValidId_ShouldReturnCorrectCount()
        {
            // Arrange
            const int count = 5;
            const int manufacturerId = 1;
            this.SeedModels(count, manufacturerId);

            // Act
            var result = await this.modelService.GetByManufacturerIdAsync(manufacturerId);

            // Assert
            result
                .Should()
                .HaveCount(count);

            result
                .Should()
                .AllBeAssignableTo<string>();
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
            this.SeedModels(10);

            // Act
            var result = await this.modelService.UpdateAsync(id, "E 500");

            // Assert
            result
                .Should()
                .BeFalse();
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public async Task UpdateAsync_WithEmptyName_ShouldReturnFalseAndNotUpdateEntity(string name)
        {
            // Arrange
            const int modelId = 1;
            const string modelName = "E 500";
            this.dbContext.Add(new Model
            {
                Id = modelId,
                Name = modelName
            });
            this.dbContext.SaveChanges();

            // Act
            var result = await this.modelService.UpdateAsync(modelId, name);

            // Assert
            result
                .Should()
                .BeFalse();
        }

        #endregion

        private void SeedModels(int count)
        {
            var models = new List<Model>();
            for (int i = 1; i <= count; i++)
            {
                var model = new Model
                {
                    Id = i,
                    Name = string.Format(ModelNameTemplate, i),
                };
                models.Add(model);
            }
            this.dbContext.AddRange(models);
            this.dbContext.SaveChanges();
        }

        private void SeedModels(int count, int manufacturerId)
        {
            var models = new List<Model>();
            for (int i = 1; i <= count; i++)
            {
                var model = new Model
                {
                    Id = i,
                    Name = string.Format(ModelNameTemplate, i),
                    ManufacturerId = manufacturerId
                };

                models.Add(model);
            }

            this.dbContext.AddRange(models);
            this.dbContext.SaveChanges();
        }
    }
}
