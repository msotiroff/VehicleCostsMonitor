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

    public class VehicleServiceTests : BaseTest
    {
        private const string SampleManufacturerName = "Mercedes-Benz";
        private const string SampleModelName = "E 500";
        private const string SampleExactModelname = "Chochone";
        private const string SampleUserId = "sample-user-id";
        private const int SampleEngineHorsePower = 381;
        private const int SampleGearingTypeId = 1;
        private const int SampleFuelTypeId = 1;
        private const int SampleVehicleTypeId = 1;
        private const int SampleYearOfManufacture = 1994;

        private readonly JustMonitorDbContext dbContext;
        private readonly VehicleService vehicleService;

        public VehicleServiceTests()
        {
            this.dbContext = base.DatabaseInstance;
            this.vehicleService = new VehicleService(this.dbContext);
        }

        #region CreateAsync Tests

        [Fact]
        public async Task CreateAsync_WithoutModel_ShouldReturnZeroAndNotAddVehicleToDatabase()
        {
            // Act
            var result = await this.vehicleService.CreateAsync(null);

            // Assert
            result
                .Should()
                .Be(0);

            this.dbContext
                .Vehicles
                .Should()
                .BeEmpty();
        }

        [Fact]
        public async Task CreateAsync_WithInvalidModelName_ShouldReturnZeroAndNotAddVehicleToDatabase()
        {
            // Arrange
            this.SeedManufacturer(1, SampleManufacturerName);
            var model = new VehicleCreateServiceModel
            {
                ManufacturerId = 1
            };

            // Act
            var result = await this.vehicleService.CreateAsync(model);

            // Assert
            result
                .Should()
                .Be(0);

            this.dbContext
                .Vehicles
                .Should()
                .BeEmpty();
        }

        [Fact]
        public async Task CreateAsync_WithInvalidManufacturerId_ShouldReturnZeroAndNotAddVehicleToDatabase()
        {
            // Arrange
            this.SeedManufacturer(1, SampleManufacturerName);
            this.SeedModel(1, SampleModelName);
            var model = new VehicleCreateServiceModel
            {
                ModelName = SampleModelName
            };

            // Act
            var result = await this.vehicleService.CreateAsync(model);

            // Assert
            result
                .Should()
                .Be(0);

            this.dbContext
                .Vehicles
                .Should()
                .BeEmpty();
        }

        [Fact]
        public async Task CreateAsync_WithInvalidYearOfManufacture_ShouldReturnZeroAndNotAddVehicleToDatabase()
        {
            // Arrange
            this.SeedManufacturer(1, SampleManufacturerName);
            this.SeedModel(1, SampleModelName);
            var createModel = this.InitializeValidVehicleCreateModel();
            createModel.YearOfManufacture = -1;

            // Act
            var result = await this.vehicleService.CreateAsync(createModel);

            // Assert
            result
                .Should()
                .Be(0);

            this.dbContext
                .Vehicles
                .Should()
                .BeEmpty();
        }

        [Fact]
        public async Task CreateAsync_WithInvalidEngineHorsePower_ShouldReturnZeroAndNotAddVehicleToDatabase()
        {
            // Arrange
            this.SeedManufacturer(1, SampleManufacturerName);
            this.SeedModel(1, SampleModelName);
            var createModel = this.InitializeValidVehicleCreateModel();
            createModel.EngineHorsePower = -1;

            // Act
            var result = await this.vehicleService.CreateAsync(createModel);

            // Assert
            result
                .Should()
                .Be(0);

            this.dbContext
                .Vehicles
                .Should()
                .BeEmpty();
        }

        [Fact]
        public async Task CreateAsync_WithMissingUserId_ShouldReturnZeroAndNotAddVehicleToDatabase()
        {
            // Arrange
            this.SeedManufacturer(1, SampleManufacturerName);
            this.SeedModel(1, SampleModelName);
            var createModel = this.InitializeValidVehicleCreateModel();
            createModel.UserId = null;

            // Act
            var result = await this.vehicleService.CreateAsync(createModel);

            // Assert
            result
                .Should()
                .Be(0);

            this.dbContext
                .Vehicles
                .Should()
                .BeEmpty();
        }

        [Fact]
        public async Task CreateAsync_WithValidModel_ShouldReturnCorrectIdAndAddVehicleToDatabase()
        {
            // Arrange
            this.SeedManufacturer(1, SampleManufacturerName);
            this.SeedModel(1, SampleModelName);
            var createModel = this.InitializeValidVehicleCreateModel();

            // Act
            var result = await this.vehicleService.CreateAsync(createModel);
            var newVehicle = this.dbContext.Vehicles.FirstOrDefault();

            // Assert
            newVehicle
                .Should()
                .NotBeNull();

            result
                .Should()
                .Be(newVehicle.Id);
        }

        #endregion

        #region DeleteAsync Tests

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(5000)]
        public async Task DeleteAsync_WithInvalidId_ShouldReturnFalse(int id)
        {
            // Arrange
            const int vehiclesCount = 3;
            this.SeedManufacturer(1, SampleManufacturerName);
            this.SeedModel(1, SampleModelName);
            this.SeedVehicles(vehiclesCount);

            // Act
            var result = await this.vehicleService.DeleteAsync(id);

            // Assert
            result
                .Should()
                .BeFalse();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(5000)]
        public async Task DeleteAsync_WithInvalidId_ShouldNotRemoveVehicleFromDatabase(int id)
        {
            // Arrange
            const int vehiclesCount = 3;
            this.SeedManufacturer(1, SampleManufacturerName);
            this.SeedModel(1, SampleModelName);
            this.SeedVehicles(vehiclesCount);

            // Act
            var result = await this.vehicleService.DeleteAsync(id);

            // Assert
            this.dbContext
                .Vehicles
                .Should()
                .HaveCount(vehiclesCount);
        }

        [Fact]
        public async Task DeleteAsync_WithValidId_ShouldRemoveVehicleFromDatabase()
        {
            // Arrange
            const int vehiclesCount = 1;
            this.SeedManufacturer(1, SampleManufacturerName);
            this.SeedModel(1, SampleModelName);
            this.SeedVehicles(vehiclesCount);

            // Act
            var result = await this.vehicleService.DeleteAsync(1);

            // Assert
            this.dbContext
                .Vehicles
                .First(v => v.Id == 1)
                .Should()
                .Match<Vehicle>(v => v.IsDeleted == true);
        }

        [Fact]
        public async Task DeleteAsync_WithValidId_ShouldReturnTrue()
        {
            // Arrange
            const int vehiclesCount = 1;
            this.SeedManufacturer(1, SampleManufacturerName);
            this.SeedModel(1, SampleModelName);
            this.SeedVehicles(vehiclesCount);

            // Act
            var result = await this.vehicleService.DeleteAsync(1);

            // Assert
            result
                .Should()
                .BeTrue();
        }

        #endregion

        private void SeedVehicles(int vehiclesCount)
        {
            var vehiclesToSeed = new List<Vehicle>();

            for (int i = 1; i <= vehiclesCount; i++)
            {
                vehiclesToSeed.Add(new Vehicle
                {
                    Id = i,
                    Manufacturer = this.dbContext.Manufacturers.FirstOrDefault(),
                    Model = this.dbContext.Models.FirstOrDefault(),
                    ManufacturerId = 1,
                    ModelId = 1,
                    EngineHorsePower = SampleEngineHorsePower,
                    YearOfManufacture = SampleYearOfManufacture,
                    ExactModelname = SampleExactModelname,
                    FuelTypeId = 1,
                    GearingTypeId = 1,
                    UserId = i.ToString(),
                    VehicleTypeId = 1
                });
            }

            this.dbContext.AddRange(vehiclesToSeed);
            this.dbContext.SaveChanges();
        }

        private VehicleCreateServiceModel InitializeValidVehicleCreateModel()
        {
            var model = new VehicleCreateServiceModel
            {
                ManufacturerId = 1,
                ModelName = SampleModelName,
                ExactModelname = SampleExactModelname,
                EngineHorsePower = SampleEngineHorsePower,
                FuelTypeId = SampleFuelTypeId,
                GearingTypeId = SampleGearingTypeId,
                UserId = SampleUserId,
                VehicleTypeId = SampleVehicleTypeId,
                YearOfManufacture = SampleYearOfManufacture
            };

            return model;
        }

        private void SeedModel(int id, string modelName)
        {
            this.dbContext
                .Add(new VehicleCostsMonitor.Models.Model
                {
                    Id = id,
                    Name = modelName,
                    ManufacturerId = 1
                });

            this.dbContext.SaveChanges();
        }

        private void SeedManufacturer(int id, string manufacturerName)
        {
            this.dbContext
                .Add(new Manufacturer
                {
                    Id = id,
                    Name = manufacturerName
                });

            this.dbContext.SaveChanges();
        }
    }
}
