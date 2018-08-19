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
    using VehicleCostsMonitor.Services.Models.Entries.CostEntries;
    using Xunit;

    public class CostEntryServiceTests : BaseTest
    {
        private readonly DateTime SampleDateCreated = new DateTime(2000, 1, 1);
        private const int SampleCostEntryTypeId = 1;
        private const int SampleVehicleId = 13;
        private const decimal SamplePrice = 150m;
        private const int SampleCurrencyId = 2;
        private const string SampleNote = "Lorem ipsum dolor sit amet";
        private const int SampleOdometer = 100_000;

        private readonly JustMonitorDbContext dbContext;
        private readonly CostEntryService costEntryService;

        public CostEntryServiceTests()
        {
            this.dbContext = base.DatabaseInstance;
            this.costEntryService = new CostEntryService(this.dbContext);
        }

        #region CreateAsync Tests

        [Fact]
        public async Task CreateAsync_WithCorrectData_ShouldReturnTrueAndAddEntryToDatabase()
        {
            // Act
            var result = await this.costEntryService.CreateAsync(SampleDateCreated, SampleCostEntryTypeId, SampleVehicleId, SamplePrice, SampleCurrencyId, SampleNote, SampleOdometer);

            var dbEntry = this.dbContext.CostEntries.FirstOrDefault();

            // Assert
            result
                .Should()
                .BeTrue();

            dbEntry
                .Should()
                .NotBeNull()
                .And
                .Match<CostEntry>(ce => ce.DateCreated == SampleDateCreated)
                .And
                .Match<CostEntry>(ce => ce.CostEntryTypeId == SampleCostEntryTypeId)
                .And
                .Match<CostEntry>(ce => ce.VehicleId == SampleVehicleId)
                .And
                .Match<CostEntry>(ce => ce.Price == SamplePrice)
                .And
                .Match<CostEntry>(ce => ce.CurrencyId == SampleCurrencyId)
                .And
                .Match<CostEntry>(ce => ce.Note == SampleNote)
                .And
                .Match<CostEntry>(ce => ce.Odometer == SampleOdometer);
        }

        [Fact]
        public async Task CreateAsync_WithNegativePrice_ShouldReturnFalseAndNotAddEntryToDatabase()
        {
            // Act
            var result = await this.costEntryService.CreateAsync(SampleDateCreated, SampleCostEntryTypeId, SampleVehicleId, -100, SampleCurrencyId, SampleNote, SampleOdometer);

            // Assert
            result
                .Should()
                .BeFalse();

            this.dbContext
                .CostEntries
                .Should()
                .BeEmpty();
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
            const int entriesCount = 10;
            this.SeedCostEntries(entriesCount);

            // Act
            var result = await this.costEntryService.DeleteAsync(id);

            // Assert
            result
                .Should()
                .BeFalse();

            this.dbContext
                .CostEntries
                .Should()
                .HaveCount(entriesCount);
        }

        [Fact]
        public async Task DeleteAsync_WithValidId_ShouldReturnTrueAndRemoveEntryFromDatabase()
        {
            // Arrange
            const int sampleId = 1;
            const int entriesToSeedCount = 10;
            this.SeedCostEntries(entriesToSeedCount);

            // Act
            var countOfEntriesBeforeServiceCalled = this.dbContext.CostEntries.Count();
            var result = await this.costEntryService.DeleteAsync(sampleId);
            var countOfEntriesAfterServiceCalled = this.dbContext.CostEntries.Count();

            // Assert
            result
                .Should()
                .BeTrue();

            countOfEntriesBeforeServiceCalled
                .Should()
                .Be(entriesToSeedCount);

            countOfEntriesAfterServiceCalled
                .Should()
                .Be(entriesToSeedCount - 1);

            this.dbContext
                .CostEntries
                .Should()
                .NotContain(ce => ce.Id == sampleId);
        }

        #endregion

        #region GetEntryTypesAsync Tests

        [Theory]
        [InlineData(1)]
        [InlineData(13)]
        [InlineData(124)]
        public async Task GetEntryTypesAsync_ShouldReturnCorrectCountOfEntryTypes(int count)
        {
            // Arrange
            var types = new List<CostEntryType>();
            for (int i = 0; i < count; i++)
            {
                types.Add(new CostEntryType());
            }
            this.dbContext.AddRange(types);
            this.dbContext.SaveChanges();

            // Act
            var result = await this.costEntryService.GetEntryTypesAsync();

            // Assert
            result
                .Should()
                .HaveCount(count);
        }

        [Fact]
        public async Task GetEntryTypesAsync_ShouldReturnCorrectModelType()
        {
            // Arrange
            this.dbContext.Add(new CostEntryType());
            this.dbContext.Add(new CostEntryType());
            this.dbContext.Add(new CostEntryType());
            this.dbContext.SaveChanges();

            // Act
            var result = await this.costEntryService.GetEntryTypesAsync();

            // Assert
            result
                .Should()
                .AllBeAssignableTo<CostEntryType>();
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
            this.SeedCostEntries(10);

            // Act
            var result = await this.costEntryService.GetForDeleteAsync(id);

            // Assert
            result
                .Should()
                .BeNull();
        }

        // Does not work properly with AutoMapper.QueryableExtensions
        //[Fact]
        //public async Task GetForDelete_WithValidId_ShouldReturnCorrectModel()
        //{
        //    // Arrange
        //    const int sampleId = 4;
        //    this.SeedCostEntries(10);

        //    // Act
        //    var result = await this.costEntryService.GetForDeleteAsync(sampleId);

        //    // Assert
        //    result
        //        .Should()
        //        .BeAssignableTo<CostEntryDeleteServiceModel>()
        //        .And
        //        .Match<CostEntryDeleteServiceModel>(ce => ce.Id == sampleId);
        //}

        #endregion

        #region GetForUpdateAsync Tests

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(5000)]
        public async Task GetForUpdateAsync_WithInvalidId_ShouldReturnNull(int id)
        {
            // Arrange
            this.SeedCostEntries(10);

            // Act
            var result = await this.costEntryService.GetForUpdateAsync(id);

            // Assert
            result
                .Should()
                .BeNull();
        }

        [Fact]
        public async Task GetForUpdate_WithValidId_ShouldReturnCorrectModel()
        {
            // Arrange
            const int sampleId = 4;
            this.SeedCostEntries(10);

            // Act
            var result = await this.costEntryService.GetForUpdateAsync(sampleId);

            // Assert
            result
                .Should()
                .BeAssignableTo<CostEntryUpdateServiceModel>()
                .And
                .Match<CostEntryUpdateServiceModel>(ce => ce.Id == sampleId);
        }

        #endregion

        #region UpdateAsync Tests

        [Fact]
        public async Task UpdateAsync_WithInvalidId_ShouldReturnFalse()
        {
            // Arrange
            this.SeedCostEntries(5);

            // Act
            var result = await this.costEntryService
                .UpdateAsync(500, SampleDateCreated, SampleCostEntryTypeId, SamplePrice, SampleCurrencyId, SampleNote, null);

            // Assert
            result
                .Should()
                .BeFalse();
        }

        [Fact]
        public async Task UpdateAsync_WithNegativePrice_ShouldReturnFalse()
        {
            // Arrange
            this.SeedCostEntries(5);

            // Act
            var result = await this.costEntryService
                .UpdateAsync(1, SampleDateCreated, SampleCostEntryTypeId, -1, SampleCurrencyId, SampleNote, SampleOdometer);

            // Assert
            result
                .Should()
                .BeFalse();

            Assert.True(this.dbContext.CostEntries.All(ce => ce.Price == default(decimal)));
        }

        [Fact]
        public async Task UpdateAsync_WithValidData_ShouldUpdateEntryCorrectly()
        {
            // Arrange
            this.SeedCostEntries(1, true);

            var costEntryBeforeServiceCalled = this.dbContext.CostEntries.FirstOrDefault();

            var dateCreatedBeforeServiceCalled = costEntryBeforeServiceCalled.DateCreated;
            var entryTypeIdBeforeServiceCalled = costEntryBeforeServiceCalled.CostEntryTypeId;
            var vehicleIdBeforeServiceCalled = costEntryBeforeServiceCalled.VehicleId;
            var priceBeforeServiceCalled = costEntryBeforeServiceCalled.Price;
            var currencyIdBeforeServiceCalled = costEntryBeforeServiceCalled.CurrencyId;
            var noteBeforeServiceCalled = costEntryBeforeServiceCalled.Note;
            var odometerBeforeServiceCalled = costEntryBeforeServiceCalled.Odometer;

            var newEntryTypeId = 13;

            // Act
            var result = await this.costEntryService
                .UpdateAsync(1, SampleDateCreated, newEntryTypeId, SamplePrice, SampleCurrencyId, SampleNote, SampleOdometer);
            var costEntryAfterServiceCalled = this.dbContext.CostEntries.FirstOrDefault();

            // Assert
            result
                .Should()
                .BeTrue();

            dateCreatedBeforeServiceCalled.Should().NotBe(SampleDateCreated);
            entryTypeIdBeforeServiceCalled.Should().Be(SampleCostEntryTypeId);
            vehicleIdBeforeServiceCalled.Should().Be(SampleVehicleId);
            priceBeforeServiceCalled.Should().Be(default(decimal));
            currencyIdBeforeServiceCalled.Should().Be(default(int));
            noteBeforeServiceCalled.Should().Be(default(string));
            odometerBeforeServiceCalled.Should().Be(null);

            costEntryAfterServiceCalled
                .Should()
                .Match<CostEntry>(ce => ce.Id == 1)
                .And
                .Match<CostEntry>(ce => ce.DateCreated == SampleDateCreated)
                .And
                .Match<CostEntry>(ce => ce.CostEntryTypeId == newEntryTypeId)
                .And
                .Match<CostEntry>(ce => ce.VehicleId == SampleVehicleId)
                .And
                .Match<CostEntry>(ce => ce.Price == SamplePrice)
                .And
                .Match<CostEntry>(ce => ce.CurrencyId == SampleCurrencyId)
                .And
                .Match<CostEntry>(ce => ce.Note == SampleNote)
                .And
                .Match<CostEntry>(ce => ce.Odometer == SampleOdometer);
        }

        #endregion

        private void SeedCostEntries(int count, bool randomDate = false)
        {
            var random = new Random();

            var entriesToSeed = new List<CostEntry>();

            for (int i = 1; i <= count; i++)
            {
                var sampleCostEntry = new CostEntry(SampleDateCreated, SampleCostEntryTypeId, SampleVehicleId)
                {
                    Id = i
                };

                if (randomDate)
                {
                    sampleCostEntry.DateCreated = SampleDateCreated.AddDays(random.Next(1, 1000));
                }

                entriesToSeed.Add(sampleCostEntry);
            }

            this.dbContext.AddRange(entriesToSeed);
            this.dbContext.SaveChanges();
        }
    }
}
