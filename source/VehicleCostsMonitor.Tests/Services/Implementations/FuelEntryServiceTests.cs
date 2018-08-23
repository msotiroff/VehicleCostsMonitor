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
    using VehicleCostsMonitor.Services.Models.Entries.FuelEntries;
    using Xunit;

    public class FuelEntryServiceTests : BaseTest
    {
        private readonly DateTime SampleDateCreated = new DateTime(2000, 1, 1);
        private const int SampleFuelEntryTypeId = 1;
        private const int SampleFuelTypeId = 2;
        private const int SampleVehicleId = 13;
        private const decimal SamplePrice = 150m;
        private const double SampleFuelQuantity = 30;
        private const int SampleCurrencyId = 2;
        private const string SampleNote = "Lorem ipsum dolor sit amet";
        private const int SampleOdometer = 100_000;

        private readonly JustMonitorDbContext dbContext;
        private readonly FuelEntryService fuelEntryService;

        public FuelEntryServiceTests()
        {
            this.dbContext = base.DatabaseInstance;
            this.fuelEntryService = new FuelEntryService(this.dbContext);
        }

        #region CreateAsync Tests

        [Fact]
        public async Task CreateAsync_WithoutModel_ShouldReturnFalseAndNotAddEntryToDatabase()
        {
            // Act
            var result = await this.fuelEntryService.CreateAsync(null);

            // Assert
            result
                .Should()
                .BeFalse();

            this.dbContext
                .FuelEntries
                .Should()
                .BeEmpty();
        }

        [Fact]
        public async Task CreateAsync_WithUnExistantVehicle_ShouldReturnFalseAndNotAddEntryToDatabase()
        {
            // Arrange
            var model = this.GetFuelEntryCreateModel();

            // Act
            var result = await this.fuelEntryService.CreateAsync(model);

            // Assert
            result
                .Should()
                .BeFalse();

            this.dbContext
                .FuelEntries
                .Should()
                .BeEmpty();
        }

        [Fact]
        public async Task CreateAsync_WithNegativeOdometer_ShouldReturnFalseAndNotAddEntryToDatabase()
        {
            // Arrange
            this.SeedFuelEntryTypes();
            this.AddVehicleToDatabase(SampleVehicleId);
            var model = this.GetFuelEntryCreateModel();
            model.Odometer = -5;

            // Act
            var result = await this.fuelEntryService.CreateAsync(model);

            // Assert
            result
                .Should()
                .BeFalse();

            this.dbContext
                .FuelEntries
                .Should()
                .BeEmpty();
        }

        [Fact]
        public async Task CreateAsync_WithNegativePrice_ShouldReturnFalseAndNotAddEntryToDatabase()
        {
            // Arrange
            this.SeedFuelEntryTypes();
            this.AddVehicleToDatabase(SampleVehicleId);
            var model = this.GetFuelEntryCreateModel();
            model.Price = -5;

            // Act
            var result = await this.fuelEntryService.CreateAsync(model);

            // Assert
            result
                .Should()
                .BeFalse();

            this.dbContext
                .FuelEntries
                .Should()
                .BeEmpty();
        }

        [Fact]
        public async Task CreateAsync_WithValidModel_ShouldReturnTrue()
        {
            // Arrange
            var model = this.GetFuelEntryCreateModel();
            this.AddVehicleToDatabase(SampleVehicleId);
            this.SeedFuelEntryTypes();

            // Act
            var result = await this.fuelEntryService.CreateAsync(model);

            // Assert
            result
                .Should()
                .BeTrue();
        }

        [Fact]
        public async Task CreateAsync_WithValidModel_ShouldAddEntryToDatabase()
        {
            // Arrange
            var model = this.GetFuelEntryCreateModel();
            this.AddVehicleToDatabase(SampleVehicleId);
            this.SeedFuelEntryTypes();

            // Act
            var result = await this.fuelEntryService.CreateAsync(model);

            // Assert
            this.dbContext
                .FuelEntries
                .Should()
                .HaveCount(1);
        }

        [Fact]
        public async Task CreateAsync_WithValidModel_ShouldSetCorrectValues()
        {
            // Arrange
            var model = this.GetFuelEntryCreateModel();
            this.AddVehicleToDatabase(SampleVehicleId);
            this.SeedFuelEntryTypes();

            // Act
            var result = await this.fuelEntryService.CreateAsync(model);
            var entry = this.dbContext.FuelEntries.FirstOrDefault();

            // Assert
            entry
                .Should()
                .NotBeNull()
                .And
                .Match<FuelEntry>(fe => fe.DateCreated == SampleDateCreated)
                .And
                .Match<FuelEntry>(fe => fe.VehicleId == SampleVehicleId)
                .And
                .Match<FuelEntry>(fe => fe.TripOdometer == 0)
                .And
                .Match<FuelEntry>(fe => fe.Price == SamplePrice)
                .And
                .Match<FuelEntry>(fe => fe.Odometer == SampleOdometer)
                .And
                .Match<FuelEntry>(fe => fe.Note == SampleNote)
                .And
                .Match<FuelEntry>(fe => fe.FuelTypeId == SampleFuelTypeId)
                .And
                .Match<FuelEntry>(fe => fe.FuelQuantity == SampleFuelQuantity)
                .And
                .Match<FuelEntry>(fe => fe.FuelEntryTypeId == SampleFuelEntryTypeId)
                .And
                .Match<FuelEntry>(fe => fe.CurrencyId == SampleCurrencyId)
                .And
                .Match<FuelEntry>(fe => fe.Average == 0);
        }

        [Fact]
        public async Task CreateAsync_WithValidModel_ShouldSetCorrectTripOdodmeter()
        {
            // Arrange
            const int firstFuelingOdometer = 1000;
            const int secondFuelingOdometer = 1500;
            var expectedTripOdodmeter = secondFuelingOdometer - firstFuelingOdometer;

            this.AddVehicleToDatabase(SampleVehicleId);
            this.SeedFuelEntryTypes();
            var fuelEntries = new List<FuelEntry>()
            {
                new FuelEntry
                {
                    Odometer = firstFuelingOdometer,
                    DateCreated = SampleDateCreated,
                }
            };


            var vehicle = this.dbContext.Vehicles.First();
            vehicle.FuelEntries = fuelEntries;
            this.dbContext.FuelEntries.Add(fuelEntries.First());
            this.dbContext.SaveChanges();

            var model = this.GetFuelEntryCreateModel();
            model.Odometer = secondFuelingOdometer;
            model.DateCreated = SampleDateCreated.AddDays(1);


            // Act
            var result = await this.fuelEntryService.CreateAsync(model);
            var entry = this.dbContext.FuelEntries.OrderBy(fe => fe.DateCreated).LastOrDefault();

            // Assert
            entry
                .Should()
                .NotBeNull()
                .And
                .Match<FuelEntry>(fe => fe.TripOdometer == expectedTripOdodmeter);
        }

        [Fact]
        public async Task CreateAsync_WithValidModel_ShouldSetCorrectAverage()
        {
            // Arrange
            const int firstFuelingOdometer = 1000;
            const int secondFuelingOdometer = 1500;
            var tripOdodmeter = secondFuelingOdometer - firstFuelingOdometer;

            const double modelFuelQuantity = 25;
            var expectedAverage = modelFuelQuantity / tripOdodmeter * 100;

            this.AddVehicleToDatabase(SampleVehicleId);
            this.SeedFuelEntryTypes();
            var fuelEntries = new List<FuelEntry>()
            {
                new FuelEntry
                {
                    Odometer = firstFuelingOdometer,
                    DateCreated = SampleDateCreated,
                    FuelEntryTypeId = this.dbContext.FuelEntryTypes.FirstOrDefault(fet => fet.Name == "Full").Id
                }
            };


            var vehicle = this.dbContext.Vehicles.First();
            vehicle.FuelEntries = fuelEntries;
            this.dbContext.FuelEntries.Add(fuelEntries.First());
            this.dbContext.SaveChanges();

            var model = this.GetFuelEntryCreateModel();
            model.FuelQuantity = modelFuelQuantity;
            model.Odometer = secondFuelingOdometer;
            model.DateCreated = SampleDateCreated.AddDays(1);


            // Act
            var result = await this.fuelEntryService.CreateAsync(model);
            var entry = this.dbContext.FuelEntries.OrderBy(fe => fe.DateCreated).LastOrDefault();

            // Assert
            entry
                .Should()
                .NotBeNull()
                .And
                .Match<FuelEntry>(fe => fe.Average == expectedAverage);
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
            this.SeedFuelEntries(10);

            // Act
            var result = await this.fuelEntryService.GetAsync(id);

            // Assert
            result
                .Should()
                .BeNull();
        }

        [Fact]
        public async Task GetAsync_WithValidId_ShouldReturnEntryWithCorrectData()
        {
            // Arrange
            const int testedEntryId = 3;
            this.SeedFuelEntries(10);
            var testedEntry = this.dbContext.FuelEntries.First(fe => fe.Id == testedEntryId);
            testedEntry.ExtraFuelConsumers = new List<FuelEntryExtraFuelConsumer>
            {
                new FuelEntryExtraFuelConsumer(),
                new FuelEntryExtraFuelConsumer(),
                new FuelEntryExtraFuelConsumer(),
            };
            testedEntry.Routes = new List<FuelEntryRouteType>
            {
                new FuelEntryRouteType(),
                new FuelEntryRouteType(),
                new FuelEntryRouteType(),
            };
            this.dbContext.SaveChanges();

            // Act
            var result = await this.fuelEntryService.GetAsync(testedEntryId);

            // Assert
            result
                .Should()
                .NotBeNull()
                .And
                .Match<FuelEntry>(fe => fe.ExtraFuelConsumers.Count() == 3)
                .And
                .Match<FuelEntry>(fe => fe.Routes.Count() == 3);
        }

        #endregion

        #region GetEntryTypes Tests

        [Fact]
        public async Task GetEntryTypes_ShouldReturnEmptyCollection()
        {
            // Act
            var result = await this.fuelEntryService.GetEntryTypes();

            // Assert
            result
                .Should()
                .BeEmpty();
        }

        [Fact]
        public async Task GetEntryTypes_ShouldReturnCorrectCount()
        {
            // Arrange
            this.SeedFuelEntryTypes();

            // Act
            var result = await this.fuelEntryService.GetEntryTypes();

            // Assert
            result
                .Should()
                .HaveCount(2);
        }

        [Fact]
        public async Task GetEntryTypes_ShouldReturnCorrectModels()
        {
            // Arrange
            this.SeedFuelEntryTypes();

            // Act
            var result = await this.fuelEntryService.GetEntryTypes();

            // Assert
            result
                .Should()
                .AllBeAssignableTo<FuelEntryType>();
        }

        #endregion

        #region GetExtraFuelConsumers Tests

        [Fact]
        public async Task GetExtraFuelConsumers_ShouldReturnEmptyCollection()
        {
            // Act
            var result = await this.fuelEntryService.GetExtraFuelConsumers();

            // Assert
            result
                .Should()
                .BeEmpty();
        }

        [Fact]
        public async Task GetExtraFuelConsumers_ShouldReturnCorrectCount()
        {
            // Arrange
            this.SeedExtraFuelConsumers();

            // Act
            var result = await this.fuelEntryService.GetExtraFuelConsumers();

            // Assert
            result
                .Should()
                .HaveCount(2);
        }

        [Fact]
        public async Task GetExtraFuelConsumers_ShouldReturnCorrectModels()
        {
            // Arrange
            this.SeedExtraFuelConsumers();

            // Act
            var result = await this.fuelEntryService.GetExtraFuelConsumers();

            // Assert
            result
                .Should()
                .AllBeAssignableTo<ExtraFuelConsumer>();
        }

        #endregion

        #region GetFuelTypes Tests

        [Fact]
        public async Task GetFuelTypes_ShouldReturnEmptyCollection()
        {
            // Act
            var result = await this.fuelEntryService.GetFuelTypes();

            // Assert
            result
                .Should()
                .BeEmpty();
        }

        [Fact]
        public async Task GetFuelTypes_ShouldReturnCorrectCount()
        {
            // Arrange
            this.SeedFuelTypes();

            // Act
            var result = await this.fuelEntryService.GetFuelTypes();

            // Assert
            result
                .Should()
                .HaveCount(2);
        }

        [Fact]
        public async Task GetFuelTypes_ShouldReturnCorrectModels()
        {
            // Arrange
            this.SeedFuelTypes();

            // Act
            var result = await this.fuelEntryService.GetFuelTypes();

            // Assert
            result
                .Should()
                .AllBeAssignableTo<FuelType>();
        }

        #endregion

        #region GetRouteTypes Tests

        [Fact]
        public async Task GetRouteTypes_ShouldReturnEmptyCollection()
        {
            // Act
            var result = await this.fuelEntryService.GetRouteTypes();

            // Assert
            result
                .Should()
                .BeEmpty();
        }

        [Fact]
        public async Task GetRouteTypes_ShouldReturnCorrectCount()
        {
            // Arrange
            this.SeedRouteTypes();

            // Act
            var result = await this.fuelEntryService.GetRouteTypes();

            // Assert
            result
                .Should()
                .HaveCount(2);
        }

        [Fact]
        public async Task GetRouteTypes_ShouldReturnCorrectModels()
        {
            // Arrange
            this.SeedRouteTypes();

            // Act
            var result = await this.fuelEntryService.GetRouteTypes();

            // Assert
            result
                .Should()
                .AllBeAssignableTo<RouteType>();
        }

        #endregion

        #region GetForDeleteAsync Tests

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(5000)]
        public async Task GetForDeleteAsync_WithInvalidId_ShouldReturnNull(int id)
        {
            // Arrange
            this.SeedFuelEntries(10, true);

            // Act
            var result = await this.fuelEntryService.GetForDeleteAsync(id);

            // Assert
            result
                .Should()
                .BeNull();
        }

        // Does not work properly with AutoMapper.QueryableExtensions
        //[Fact]
        //public async Task GetForDeleteAsync_WithValidId_ShouldReturnCorrectEntry()
        //{
        //    // Arrange
        //    const int testedId = 3;
        //    this.SeedFuelEntries(10, true);

        //    // Act
        //    var result = await this.fuelEntryService.GetForDeleteAsync(testedId);

        //    // Assert
        //    result
        //        .Should()
        //        .Match<FuelEntryDeleteServiceModel>(fe => fe.Id == testedId);
        //}

        #endregion

        #region GetPreviousOdometerValue Tests

        [Fact]
        public async Task GetPreviousOdometerValue_WithUnexistantVehicleId_ShouldReturnZero()
        {
            // Act
            var result = await this.fuelEntryService.GetPreviousOdometerValue(1, SampleDateCreated);

            // Assert
            result
                .Should()
                .Be(0);
        }

        [Fact]
        public async Task GetPreviousOdometerValue_WithDeletedVehicle_ShouldReturnZero()
        {
            // Arrange
            this.AddVehicleToDatabase(1);
            var vehicle = this.dbContext.Vehicles.First();
            vehicle.IsDeleted = true;

            // Act
            var result = await this.fuelEntryService.GetPreviousOdometerValue(1, SampleDateCreated);

            // Assert
            result
                .Should()
                .Be(0);
        }

        [Fact]
        public async Task GetPreviousOdometerValue_WithVehicleWithoutFuelings_ShouldReturnZero()
        {
            // Arrange
            this.AddVehicleToDatabase(1);
            var vehicle = this.dbContext.Vehicles.First();
            vehicle.FuelEntries = new List<FuelEntry>();

            // Act
            var result = await this.fuelEntryService.GetPreviousOdometerValue(1, SampleDateCreated);

            // Assert
            result
                .Should()
                .Be(0);
        }

        [Fact]
        public async Task GetPreviousOdometerValue_WithCorrectData_ShouldReturnCorrectValue()
        {
            // Arrange
            this.AddVehicleToDatabase(1);
            var vehicle = this.dbContext.Vehicles.First();
            var fuelEntries = new List<FuelEntry>
            {
                new FuelEntry
                {
                    DateCreated = SampleDateCreated,
                    Odometer = SampleOdometer
                }
            };

            vehicle.FuelEntries = fuelEntries;
            this.dbContext.FuelEntries.AddRange(fuelEntries);
            this.dbContext.SaveChanges();

            // Act
            var result = await this.fuelEntryService.GetPreviousOdometerValue(1, SampleDateCreated.AddDays(1));

            // Assert
            result
                .Should()
                .Be(SampleOdometer);
        }

        [Fact]
        public async Task GetPreviousOdometerValue_WithCorrectDataAndMiddleDate_ShouldReturnCorrectValue()
        {
            // Arrange
            this.AddVehicleToDatabase(1);
            var vehicle = this.dbContext.Vehicles.First();
            var fuelEntries = new List<FuelEntry>
            {
                new FuelEntry
                {
                    DateCreated = SampleDateCreated,
                    Odometer = SampleOdometer
                },
                new FuelEntry
                {
                    DateCreated = SampleDateCreated.AddDays(2),
                    Odometer = SampleOdometer + 1000
                }
            };

            vehicle.FuelEntries = fuelEntries;
            this.dbContext.FuelEntries.AddRange(fuelEntries);
            this.dbContext.SaveChanges();

            // Act
            var result = await this.fuelEntryService.GetPreviousOdometerValue(1, SampleDateCreated.AddDays(1));

            // Assert
            result
                .Should()
                .Be(SampleOdometer);
        }

        #endregion

        #region UpdateAsync Tests

        [Fact]
        public async Task UpdateAsync_WithoutModel_ShouldReturnFalse()
        {
            // Act
            var result = await this.fuelEntryService.UpdateAsync(null);

            // Assert
            result
                .Should()
                .BeFalse();
        }

        [Fact]
        public async Task UpdateAsync_WithMissingVehicle_ShouldReturnFalse()
        {
            // Arrange
            this.SeedFuelEntries(1);
            var fuelEntry = this.dbContext.FuelEntries.First();

            // Act
            var result = await this.fuelEntryService.UpdateAsync(fuelEntry);

            // Assert
            result
                .Should()
                .BeFalse();
        }

        [Fact]
        public async Task UpdateAsync_WithDeletedVehicle_ShouldReturnFalse()
        {
            // Arrange
            const int vehicleId = 5;
            this.SeedFuelEntries(1);
            this.AddVehicleToDatabase(vehicleId);
            var fuelEntry = this.dbContext.FuelEntries.First();
            fuelEntry.VehicleId = vehicleId;
            var vehicle = this.dbContext.Vehicles.First(v => v.Id == vehicleId);
            vehicle.IsDeleted = true;
            this.dbContext.SaveChanges();

            // Act
            var result = await this.fuelEntryService.UpdateAsync(fuelEntry);

            // Assert
            result
                .Should()
                .BeFalse();
        }

        [Fact]
        public async Task UpdateAsync_WithNegativeOdometer_ShouldReturnFalseAndNotUpdateEntryInDatabase()
        {
            // Arrange
            this.SeedFuelEntryTypes();
            this.AddVehicleToDatabase(SampleVehicleId);
            this.SeedFuelEntries(1);
            var fuelEntry = this.dbContext.FuelEntries.First();
            fuelEntry.Odometer = -5;

            // Act
            var result = await this.fuelEntryService.UpdateAsync(fuelEntry);

            // Assert
            result
                .Should()
                .BeFalse();

            this.dbContext
                .FuelEntries
                .Should()
                .HaveCount(1);
        }

        [Fact]
        public async Task UpdateAsync_WithNegativePrice_ShouldReturnFalseAndNotUpdateEntryInDatabase()
        {
            // Arrange
            this.SeedFuelEntryTypes();
            this.AddVehicleToDatabase(SampleVehicleId);
            this.SeedFuelEntries(1);
            var fuelEntry = this.dbContext.FuelEntries.First();
            fuelEntry.Price = -5;

            // Act
            var result = await this.fuelEntryService.UpdateAsync(fuelEntry);

            // Assert
            result
                .Should()
                .BeFalse();

            this.dbContext
                .FuelEntries
                .Should()
                .HaveCount(1);
        }

        [Fact]
        public async Task UpdateAsync_WithValidModel_ShouldReturnTrue()
        {
            // Arrange
            this.SeedFuelEntries(1);
            this.AddVehicleToDatabase(SampleVehicleId);
            this.SeedFuelEntryTypes();
            var entry = this.dbContext.FuelEntries.First();
            entry.Odometer += 1;

            // Act
            var result = await this.fuelEntryService.UpdateAsync(entry);

            // Assert
            result
                .Should()
                .BeTrue();
        }

        [Fact]
        public async Task UpdateAsync_WithValidModel_ShouldUpdateEntryInDatabase()
        {
            // Arrange
            const int odometerIncreaseValue = 50;
            this.SeedFuelEntries(1);
            this.AddVehicleToDatabase(SampleVehicleId);
            this.SeedFuelEntryTypes();
            var dbEntry = this.dbContext.FuelEntries.First();
            dbEntry.Odometer = SampleOdometer;
            var odometerBeforeServiceCalled = dbEntry.Odometer;
            dbEntry.Odometer += odometerIncreaseValue;

            // Act
            var result = await this.fuelEntryService.UpdateAsync(dbEntry);

            // Assert
            var updated = this.dbContext.FuelEntries.First();
            updated
                .Should()
                .Match<FuelEntry>(fe => fe.Odometer == odometerBeforeServiceCalled + odometerIncreaseValue);
        }
        
        [Fact]
        public async Task UpdateAsync_WithValidModel_ShouldSetCorrectTripOdodmeter()
        {
            // Arrange
            const int firstFuelingOdometer = 1000;
            const int secondFuelingOdometer = 1500;
            var expectedTripOdodmeter = secondFuelingOdometer - firstFuelingOdometer;

            this.AddVehicleToDatabase(SampleVehicleId);
            this.SeedFuelEntryTypes();
            var fuelEntries = new List<FuelEntry>()
            {
                new FuelEntry
                {
                    Odometer = firstFuelingOdometer,
                    DateCreated = SampleDateCreated,
                }
            };


            var vehicle = this.dbContext.Vehicles.First();
            vehicle.FuelEntries = fuelEntries;
            this.dbContext.FuelEntries.Add(fuelEntries.First());
            this.dbContext.SaveChanges();

            var fuelEntry = this.dbContext.FuelEntries.First();
            fuelEntry.Odometer = secondFuelingOdometer;
            fuelEntry.DateCreated = SampleDateCreated.AddDays(1);


            // Act
            var result = await this.fuelEntryService.UpdateAsync(fuelEntry);
            var entry = this.dbContext.FuelEntries.OrderBy(fe => fe.DateCreated).LastOrDefault();

            // Assert
            entry
                .Should()
                .NotBeNull()
                .And
                .Match<FuelEntry>(fe => fe.TripOdometer == expectedTripOdodmeter);
        }

        [Fact]
        public async Task UpdateAsync_WithValidModel_ShouldSetCorrectAverage()
        {
            // Arrange
            const int firstFuelingOdometer = 1000;
            const int secondFuelingOdometer = 1500;
            var tripOdodmeter = secondFuelingOdometer - firstFuelingOdometer;

            const double modelFuelQuantity = 25;
            var expectedAverage = modelFuelQuantity / tripOdodmeter * 100;

            this.AddVehicleToDatabase(SampleVehicleId);
            this.SeedFuelEntryTypes();
            var fuelEntries = new List<FuelEntry>()
            {
                new FuelEntry
                {
                    Odometer = firstFuelingOdometer,
                    DateCreated = SampleDateCreated,
                    FuelEntryTypeId = this.dbContext.FuelEntryTypes.FirstOrDefault(fet => fet.Name == "Full").Id
                }
            };


            var vehicle = this.dbContext.Vehicles.First();
            vehicle.FuelEntries = fuelEntries;
            this.dbContext.FuelEntries.Add(fuelEntries.First());
            this.dbContext.SaveChanges();

            var fuelEntry = this.dbContext.FuelEntries.First();
            fuelEntry.FuelQuantity = modelFuelQuantity;
            fuelEntry.Odometer = secondFuelingOdometer;
            fuelEntry.DateCreated = SampleDateCreated.AddDays(1);


            // Act
            var result = await this.fuelEntryService.UpdateAsync(fuelEntry);
            var entry = this.dbContext.FuelEntries.OrderBy(fe => fe.DateCreated).LastOrDefault();

            // Assert
            entry
                .Should()
                .NotBeNull()
                .And
                .Match<FuelEntry>(fe => fe.Average == expectedAverage);
        }

        #endregion

        #region DeleteAsync Tests

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(5000)]
        public async Task DeleteAsync_WithInvalidId_ShouldReturnFalseAndNotRemoveEntryFromDatabase(int id)
        {
            // Arrange
            const int fuelEntriesCount = 10;
            this.SeedFuelEntries(fuelEntriesCount);

            // Act
            var result = await this.fuelEntryService.DeleteAsync(id);

            // Assert
            result
                .Should()
                .BeFalse();

            this.dbContext
                .FuelEntries
                .Should()
                .HaveCount(fuelEntriesCount);
        }

        [Fact]
        public async Task DeleteAsync_WithValidId_ShouldReturnTrue()
        {
            // Arrange
            const int fuelEntryToBeDeletedId = 3;
            const int fuelEntriesCount = 10;
            this.AddVehicleToDatabase(SampleVehicleId);
            this.SeedFuelEntries(fuelEntriesCount, includeFuelEntryTypes: true);
            var dbEntry = this.dbContext.FuelEntries.First(fe => fe.Id == fuelEntryToBeDeletedId);
            var vehicle = this.dbContext.Vehicles.First();

            // Act
            var result = await this.fuelEntryService.DeleteAsync(fuelEntryToBeDeletedId);

            // Assert
            result
                .Should()
                .BeTrue();
        }

        [Fact]
        public async Task DeleteAsync_WithValidId_ShouldRemoveEntryFromDatabase()
        {
            // Arrange
            const int fuelEntryToBeDeletedId = 3;
            const int fuelEntriesCount = 10;
            this.SeedFuelEntries(fuelEntriesCount);

            // Act
            var result = await this.fuelEntryService.DeleteAsync(fuelEntryToBeDeletedId);

            // Assert
            this.dbContext
                .FuelEntries
                .Should()
                .HaveCount(fuelEntriesCount - 1);

            this.dbContext
                .FuelEntries
                .Should()
                .NotContain(fe => fe.Id == fuelEntryToBeDeletedId);
        }

        #endregion

        private void SeedExtraFuelConsumers()
        {
            var trailer = new ExtraFuelConsumer
            {
                Id = 1,
                Name = "Trailer"
            };
            var airConditioner = new ExtraFuelConsumer
            {
                Id = 2,
                Name = "AC"
            };

            this.dbContext.ExtraFuelConsumers.Add(trailer);
            this.dbContext.ExtraFuelConsumers.Add(airConditioner);
            this.dbContext.SaveChanges();
        }

        private void SeedRouteTypes()
        {
            var motorWay = new RouteType
            {
                Id = 1,
                Name = "Motor Way"
            };
            var city = new RouteType
            {
                Id = 2,
                Name = "City"
            };

            this.dbContext.RouteTypes.Add(motorWay);
            this.dbContext.RouteTypes.Add(city);
            this.dbContext.SaveChanges();
        }

        private void SeedFuelTypes()
        {
            var diesel = new FuelType
            {
                Id = 1,
                Name = "Diesel"
            };
            var gasoline = new FuelType
            {
                Id = 2,
                Name = "Gasoline"
            };

            this.dbContext.FuelTypes.Add(diesel);
            this.dbContext.FuelTypes.Add(gasoline);
            this.dbContext.SaveChanges();
        }

        private void AddVehicleToDatabase(int id)
        {
            this.dbContext
                .Vehicles
                .Add(new Vehicle
                {
                    Id = id,
                    FuelEntries = new List<FuelEntry>()
                });

            this.dbContext.SaveChanges();
        }

        private FuelEntryCreateServiceModel GetFuelEntryCreateModel()
        {
            return new FuelEntryCreateServiceModel
            {
                DateCreated = SampleDateCreated,
                FuelEntryTypeId = SampleFuelEntryTypeId,
                FuelTypeId = SampleFuelTypeId,
                CurrencyId = SampleCurrencyId,
                VehicleId = SampleVehicleId,
                Note = SampleNote,
                FuelQuantity = SampleFuelQuantity,
                Odometer = SampleOdometer,
                Price = SamplePrice
            };
        }

        private void SeedFuelEntries(int count, bool randomDate = false, bool includeFuelEntryTypes = false)
        {
            var random = new Random();
            FuelEntryType fuelEntryType = null;
            if (includeFuelEntryTypes)
            {
                this.SeedFuelEntryTypes();
                fuelEntryType = this.dbContext.FuelEntryTypes.First(fet => fet.Id == SampleFuelEntryTypeId);
            }

            var entriesToSeed = new List<FuelEntry>();

            for (int i = 1; i <= count; i++)
            {
                var sampleFuelEntry = new FuelEntry
                {
                    Id = i,
                    DateCreated = SampleDateCreated,
                    FuelEntryTypeId = SampleFuelEntryTypeId,
                    FuelEntryType = fuelEntryType,
                    VehicleId = SampleVehicleId
                };

                if (randomDate)
                {
                    sampleFuelEntry.DateCreated = SampleDateCreated.AddDays(random.Next(1, 1000));
                }

                entriesToSeed.Add(sampleFuelEntry);
            }

            this.dbContext.AddRange(entriesToSeed);
            this.dbContext.SaveChanges();
        }

        private void SeedFuelEntryTypes()
        {
            var firstFueling = new FuelEntryType
            {
                Id = 1,
                Name = "First fueling"
            };
            var full = new FuelEntryType
            {
                Id = 2,
                Name = "Full"
            };
            this.dbContext.Add(firstFueling);
            this.dbContext.Add(full);
            this.dbContext.SaveChanges();
        }
    }
}
