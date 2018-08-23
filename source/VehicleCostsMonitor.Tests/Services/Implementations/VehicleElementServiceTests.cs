namespace VehicleCostsMonitor.Tests.Services.Implementations
{
    using Xunit;
    using FluentAssertions;
    using VehicleCostsMonitor.Data;
    using VehicleCostsMonitor.Services.Implementations;
    using VehicleCostsMonitor.Models;
    using System.Threading.Tasks;
    using System;
    using System.Collections.Generic;

    public class VehicleElementServiceTests : BaseTest
    {
        private readonly JustMonitorDbContext dbContext;
        private readonly VehicleElementService elementService;

        public VehicleElementServiceTests()
        {
            this.dbContext = base.DatabaseInstance;
            this.elementService = new VehicleElementService(this.dbContext);
        }

        #region GetFuelTypes Tests

        [Fact]
        public async Task GetFuelTypes_ShouldReturnCorrectEntityType()
        {
            // Arrange
            this.dbContext.Add(new FuelType());

            // Act
            var result = await this.elementService.GetFuelTypes();

            // Assert
            result
                .Should()
                .AllBeAssignableTo<FuelType>();
        }

        [Theory]
        [InlineData(1)]
        [InlineData(13)]
        [InlineData(124)]
        public async Task GetFuelTypes_ShouldReturnCorrectCount(int expectedCount)
        {
            // Arrange
            this.SeedBaseTypes<FuelType>(expectedCount);

            // Act
            var result = await this.elementService.GetFuelTypes();

            // Assert
            result
                .Should()
                .HaveCount(expectedCount);
        }

        #endregion

        #region GetGearingTypes Tests

        [Fact]
        public async Task GetGearingTypes_ShouldReturnCorrectEntityType()
        {
            // Arrange
            this.dbContext.Add(new GearingType());

            // Act
            var result = await this.elementService.GetGearingTypes();

            // Assert
            result
                .Should()
                .AllBeAssignableTo<GearingType>();
        }

        [Theory]
        [InlineData(1)]
        [InlineData(13)]
        [InlineData(124)]
        public async Task GetGearingTypes_ShouldReturnCorrectCount(int expectedCount)
        {
            // Arrange
            this.SeedBaseTypes<GearingType>(expectedCount);

            // Act
            var result = await this.elementService.GetGearingTypes();

            // Assert
            result
                .Should()
                .HaveCount(expectedCount);
        }

        #endregion

        #region GetVehicleTypes Tests

        [Fact]
        public async Task GetVehicleTypes_ShouldReturnCorrectEntityType()
        {
            // Arrange
            this.dbContext.Add(new VehicleType());

            // Act
            var result = await this.elementService.GetVehicleTypes();

            // Assert
            result
                .Should()
                .AllBeAssignableTo<VehicleType>();
        }

        [Theory]
        [InlineData(1)]
        [InlineData(13)]
        [InlineData(124)]
        public async Task GetVehicleTypes_ShouldReturnCorrectCount(int expectedCount)
        {
            // Arrange
            this.SeedBaseTypes<VehicleType>(expectedCount);

            // Act
            var result = await this.elementService.GetVehicleTypes();

            // Assert
            result
                .Should()
                .HaveCount(expectedCount);
        }

        #endregion

        private void SeedBaseTypes<T>(int count) where T : class
        {
            var collection = new List<T>();
            for (int i = 0; i < count; i++)
            {
                var instance = Activator.CreateInstance(typeof(T)) as T;
                collection.Add(instance);
            }

            this.dbContext.Set<T>().AddRange(collection);
            this.dbContext.SaveChanges();
        }
    }
}
