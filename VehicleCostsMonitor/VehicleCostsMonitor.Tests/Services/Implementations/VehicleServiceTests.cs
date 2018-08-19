namespace VehicleCostsMonitor.Tests.Services.Implementations
{
    using FluentAssertions;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using VehicleCostsMonitor.Data;
    using VehicleCostsMonitor.Models;
    using VehicleCostsMonitor.Services.Implementations;
    using VehicleCostsMonitor.Services.Models.Vehicle;
    using VehicleCostsMonitor.Tests.Helpers;
    using Xunit;

    public class VehicleServiceTests : BaseTest
    {
        private const string SampleManufacturerName = "Mercedes-Benz";
        private const string SampleModelName = "E 500";
        private const string SampleExactModelname = "Chochone";
        private const string SampleUserId = "sample-user-id";
        private const string Diesel = "Diesel";
        private const int SampleEngineHorsePower = 381;
        private const int SampleGearingTypeId = 1;
        private const int SampleFuelTypeId = 1;
        private const int SampleVehicleTypeId = 1;
        private const int SampleYearOfManufacture = 1994;
        private readonly DateTime SampleDate = new DateTime(2000, 1, 1);

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

        #region Get Tests

        [Fact]
        public void Get_ShouldReturnCorrectTypeOfModel()
        {
            // Arrange
            this.SeedManufacturer(1, SampleManufacturerName);
            this.SeedModel(1, SampleModelName);
            this.SeedVehicles(10);

            // Act
            var result = this.vehicleService
                .Get(1, SampleModelName, null, 0, 0, 0, 0, int.MaxValue, int.MinValue, int.MaxValue, 0)
                .ToList();

            // Assert
            result
                .Should()
                .NotBeNull()
                .And
                .AllBeAssignableTo<VehicleSearchServiceModel>();
        }

        [Fact]
        public void Get_ShouldReturnCorrectCountOfVehicles()
        {
            // Arrange
            const int vehiclesCount = 10;
            this.SeedManufacturer(1, SampleManufacturerName);
            this.SeedModel(1, SampleModelName);
            this.SeedVehicles(vehiclesCount);

            // Act
            var result = this.vehicleService
                .Get(1, SampleModelName, null, 0, 0, 0, 0, int.MaxValue, int.MinValue, int.MaxValue, 0)
                .ToList();

            // Assert
            result
                .Should()
                .NotBeNull()
                .And
                .HaveCount(vehiclesCount);
        }

        [Fact]
        public void Get_ShouldFilterManufacturerCorrectly()
        {
            // Arrange
            const int notMatchedVehicleId = 3;
            const int expectedVehiclesCount = 2;
            this.SeedManufacturer(1, SampleManufacturerName);
            this.SeedManufacturer(2, "Some another manufacturer");
            this.SeedModel(1, SampleModelName);
            this.SeedVehicles(expectedVehiclesCount);
            this.dbContext
                .Add(new Vehicle
                {
                    Id = notMatchedVehicleId,
                    ManufacturerId = 2,
                    ModelId = 1,
                });
            this.dbContext.SaveChanges();

            // Act
            var result = this.vehicleService
                .Get(1, SampleModelName, null, 0, 0, 0, 0, int.MaxValue, int.MinValue, int.MaxValue, 0)
                .ToList();

            // Assert
            result
                .Should()
                .NotBeNull()
                .And
                .HaveCount(expectedVehiclesCount)
                .And
                .Match<List<VehicleSearchServiceModel>>(l => l.All(v => v.Id != notMatchedVehicleId));
        }

        [Fact]
        public void Get_ShouldFilterModelCorrectly()
        {
            // Arrange
            const int notMatchedVehicleId = 3;
            const int expectedVehiclesCount = 2;
            this.SeedManufacturer(1, SampleManufacturerName);
            this.SeedModel(1, SampleModelName);
            this.SeedModel(2, "Some another model");
            this.SeedVehicles(expectedVehiclesCount);
            this.dbContext
                .Add(new Vehicle
                {
                    Id = notMatchedVehicleId,
                    ManufacturerId = 1,
                    ModelId = 2,
                });
            this.dbContext.SaveChanges();

            // Act
            var result = this.vehicleService
                .Get(1, SampleModelName, null, 0, 0, 0, 0, int.MaxValue, int.MinValue, int.MaxValue, 0)
                .ToList();

            // Assert
            result
                .Should()
                .NotBeNull()
                .And
                .HaveCount(expectedVehiclesCount)
                .And
                .Match<List<VehicleSearchServiceModel>>(l => l.All(v => v.Id != notMatchedVehicleId));
        }

        [Fact]
        public void Get_ShouldFilterExactModelNameCorrectly()
        {
            // Arrange
            const int notMatchedVehicleId = 3;
            const int expectedVehiclesCount = 2;
            this.SeedManufacturer(1, SampleManufacturerName);
            this.SeedModel(1, SampleModelName);
            this.SeedVehicles(expectedVehiclesCount);
            this.dbContext
                .Add(new Vehicle
                {
                    Id = notMatchedVehicleId,
                    ManufacturerId = 1,
                    ModelId = 1,
                    ExactModelname = "AMG",
                });
            this.dbContext.SaveChanges();

            // Act
            var result = this.vehicleService
                .Get(1, SampleModelName, SampleExactModelname, 0, 0, 0, 0, int.MaxValue, int.MinValue, int.MaxValue, 0)
                .ToList();

            // Assert
            result
                .Should()
                .NotBeNull()
                .And
                .HaveCount(expectedVehiclesCount)
                .And
                .Match<List<VehicleSearchServiceModel>>(l => l.All(v => v.Id != notMatchedVehicleId));
        }

        [Fact]
        public void Get_ShouldFilterEngineHorsePowerCorrectly()
        {
            // Arrange
            const int notMatchedVehicleIdOne = 3;
            const int notMatchedVehicleIdTwo = 4;
            const int expectedVehiclesCount = 2;
            this.SeedManufacturer(1, SampleManufacturerName);
            this.SeedModel(1, SampleModelName);
            this.SeedVehicles(expectedVehiclesCount);
            this.dbContext
                .Add(new Vehicle
                {
                    Id = notMatchedVehicleIdOne,
                    ManufacturerId = 1,
                    ModelId = 1,
                    EngineHorsePower = 150,
                });
            this.dbContext
                .Add(new Vehicle
                {
                    Id = notMatchedVehicleIdTwo,
                    ManufacturerId = 1,
                    ModelId = 1,
                    EngineHorsePower = 400,
                });
            this.dbContext.SaveChanges();

            // Act
            var result = this.vehicleService
                .Get(1, SampleModelName, null, 0, 0, 0, SampleEngineHorsePower, SampleEngineHorsePower, int.MinValue, int.MaxValue, 0)
                .ToList();

            // Assert
            result
                .Should()
                .NotBeNull()
                .And
                .HaveCount(expectedVehiclesCount)
                .And
                .Match<List<VehicleSearchServiceModel>>(l => l.All(v => v.Id != notMatchedVehicleIdOne))
                .And
                .Match<List<VehicleSearchServiceModel>>(l => l.All(v => v.Id != notMatchedVehicleIdTwo));
        }

        [Fact]
        public void Get_ShouldFilterYearOfManufactureCorrectly()
        {
            // Arrange
            const int notMatchedVehicleIdOne = 3;
            const int notMatchedVehicleIdTwo = 4;
            const int expectedVehiclesCount = 2;
            this.SeedManufacturer(1, SampleManufacturerName);
            this.SeedModel(1, SampleModelName);
            this.SeedVehicles(expectedVehiclesCount);
            this.dbContext
                .Add(new Vehicle
                {
                    Id = notMatchedVehicleIdOne,
                    ManufacturerId = 1,
                    ModelId = 1,
                    YearOfManufacture = SampleYearOfManufacture - 1
                });
            this.dbContext
                .Add(new Vehicle
                {
                    Id = notMatchedVehicleIdTwo,
                    ManufacturerId = 1,
                    ModelId = 1,
                    YearOfManufacture = SampleYearOfManufacture + 1,
                });
            this.dbContext.SaveChanges();

            // Act
            var result = this.vehicleService
                .Get(1, SampleModelName, null, 0, 0, 0, int.MinValue, int.MaxValue, SampleYearOfManufacture, SampleYearOfManufacture, 0)
                .ToList();

            // Assert
            result
                .Should()
                .NotBeNull()
                .And
                .HaveCount(expectedVehiclesCount)
                .And
                .Match<List<VehicleSearchServiceModel>>(l => l.All(v => v.Id != notMatchedVehicleIdOne))
                .And
                .Match<List<VehicleSearchServiceModel>>(l => l.All(v => v.Id != notMatchedVehicleIdTwo));
        }

        [Fact]
        public void Get_ShouldFilterMinimumDistanceCorrectly()
        {
            // Arrange
            const int matchedVehicleId = 1;
            const int notMatchedVehicleId = 2;
            const int expectedVehiclesCount = 1;
            const int searchDistance = 1500;

            this.SeedManufacturer(1, SampleManufacturerName);
            this.SeedModel(1, SampleModelName);
            this.dbContext
                .Add(new Vehicle
                {
                    Id = matchedVehicleId,
                    ManufacturerId = 1,
                    ModelId = 1,
                    TotalDistance = searchDistance + 1
                });
            this.dbContext
                .Add(new Vehicle
                {
                    Id = notMatchedVehicleId,
                    ManufacturerId = 1,
                    ModelId = 1,
                    TotalDistance = searchDistance - 1
                });
            this.dbContext.SaveChanges();

            // Act
            var result = this.vehicleService
                .Get(1, SampleModelName, null, 0, 0, 0, int.MinValue, int.MaxValue, int.MinValue, int.MaxValue, searchDistance)
                .ToList();

            // Assert
            result
                .Should()
                .NotBeNull()
                .And
                .HaveCount(expectedVehiclesCount)
                .And
                .Match<List<VehicleSearchServiceModel>>(l => l.All(v => v.Id != notMatchedVehicleId))
                .And
                .Match<List<VehicleSearchServiceModel>>(l => l.Any(v => v.Id == matchedVehicleId));
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
            this.SeedVehicles(10);

            // Act
            var result = await this.vehicleService.GetForUpdateAsync(id);

            // Assert
            result
                .Should()
                .BeNull();
        }

        // Does not work properly with AutoMapper.QueryableExtensions
        //[Fact]
        //public async Task GetForUpdateAsync_WithValidIdShouldReturnCorrectModel()
        //{
        //    // Arrange
        //    const int testedVehicleId = 3;
        //    this.SeedVehicles(5);

        //    // Act
        //    var result = await this.vehicleService.GetForUpdateAsync(testedVehicleId);

        //    // Assert
        //    result
        //        .Should()
        //        .NotBeNull()
        //        .And
        //        .BeAssignableTo<VehicleUpdateServiceModel>()
        //        .And
        //        .Match<VehicleUpdateServiceModel>(m => m.Id == testedVehicleId);
        //}

        #endregion

        #region GetCostEntries Tests

        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(5000)]
        public void GetCostEntries_WithInvalidId_ShouldReturnEmptyQuery(int id)
        {
            // Arrange
            this.SeedVehicles(5);

            // Act
            var result = this.vehicleService.GetCostEntries(id);

            // Assert
            result
                .Should()
                .BeEmpty();
        }

        #endregion

        #region GetMostEconomicCars Tests

        [Theory]
        [InlineData("Bio DieselCHE")]
        [InlineData("Gazchica")]
        [InlineData("Metanche")]
        public async Task GetMostEconomicCars_WithInvalidFuelType_ShouldReturnEmptyCollection(string fuelType)
        {
            // Arrange
            const int vehiclesCount = 10;
            this.SeedVehicles(vehiclesCount);

            // Act
            var result = await this.vehicleService.GetMostEconomicCars(fuelType);

            // Assert
            result
                .Should()
                .BeEmpty();
        }

        [Fact]
        public async Task GetMostEconomicCars_WithValidFuelType_ShouldReturnCorrectModel()
        {
            // Arrange
            const int vehiclesCount = 10;
            this.SeedManufacturer(1, SampleManufacturerName);
            this.SeedModel(1, SampleModelName);
            this.SeedVehicles(vehiclesCount);

            // Act
            var result = await this.vehicleService.GetMostEconomicCars(Diesel);
            var model = result.FirstOrDefault();

            // Assert
            model
                .Should()
                .NotBeNull()
                .And
                .Match<VehicleStatisticServiceModel>(m => m.Count == vehiclesCount)
                .And
                .Match<VehicleStatisticServiceModel>(m => m.FuelType == Diesel)
                .And
                .Match<VehicleStatisticServiceModel>(m => m.ManufacturerId == 1)
                .And
                .Match<VehicleStatisticServiceModel>(m => m.ManufacturerName == SampleManufacturerName)
                .And
                .Match<VehicleStatisticServiceModel>(m => m.ModelName == SampleModelName)
                .And
                .Match<VehicleStatisticServiceModel>(m => m.Average == 0);
        }

        #endregion

        #region UpdateAsync Tests

        [Fact]
        public async Task UpdateAsync_WithoutModel_ShouldReturnFalseAndNotUpdateVehicleInDatabase()
        {
            // Arrange
            this.SeedVehicles(1);
            var vehicleCopy = this.dbContext.Vehicles.First().Clone();

            // Act
            var result = await this.vehicleService.UpdateAsync(null);
            var vehicleAfterServiceCalled = this.dbContext.Vehicles.First();

            // Assert
            result
                .Should()
                .BeFalse();

            vehicleAfterServiceCalled
                .Should()
                .BeEquivalentTo(vehicleCopy);
        }
        
        #endregion
        
        private void SeedCostEntriesForVehicle(int count, int vehicleId, bool randomDate = false)
        {
            var random = new Random();
            var entries = new List<CostEntry>();
            var vehicle = this.dbContext.Vehicles.FirstOrDefault(v => v.Id == vehicleId);

            for (int i = 1; i <= count; i++)
            {
                var entry = new CostEntry(SampleDate, 1, vehicleId)
                {
                    Vehicle = vehicle
                };

                if (randomDate)
                {
                    entry.DateCreated = SampleDate.AddDays(random.Next(1, 1000));
                }
                entries.Add(entry);
            }
            this.dbContext.AddRange(entries);
            this.dbContext.SaveChanges();
        }

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
                    FuelType = new FuelType { Name = Diesel },
                    GearingTypeId = 1,
                    UserId = i.ToString(),
                    VehicleTypeId = 1,
                    CostEntries = new List<CostEntry>(),
                    FuelEntries = new List<FuelEntry>(),
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
