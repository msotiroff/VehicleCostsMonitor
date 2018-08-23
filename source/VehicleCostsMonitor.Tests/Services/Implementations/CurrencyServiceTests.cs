namespace VehicleCostsMonitor.Tests.Services.Implementations
{
    using FluentAssertions;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using VehicleCostsMonitor.Data;
    using VehicleCostsMonitor.Models;
    using VehicleCostsMonitor.Services.Implementations;
    using Xunit;

    public class CurrencyServiceTests : BaseTest
    {
        private readonly CurrencyService currencyService;
        private readonly JustMonitorDbContext dbContext;

        public CurrencyServiceTests()
        {
            this.dbContext = base.DatabaseInstance;
            this.currencyService = new CurrencyService(this.dbContext);
        }

        #region GetAsync Tests

        [Fact]
        public async Task GetAsync_ShouldReturnEmptyCollection()
        {
            // Act
            var result = await this.currencyService.GetAsync();

            // Assert
            result
                .Should()
                .BeEmpty();
        }

        [Theory]
        [InlineData(1)]
        [InlineData(13)]
        [InlineData(100)]
        public async Task GetAsync_ShouldReturnCorrectCountOfEntities(int count)
        {
            // Arrange
            var currencies = new List<Currency>();
            for (int i = 0; i < count; i++)
            {
                currencies.Add(new Currency());
            }
            this.dbContext.AddRange(currencies);
            this.dbContext.SaveChanges();

            // Act
            var result = await this.currencyService.GetAsync();

            // Assert
            result
                .Should()
                .HaveCount(count);
        }

        [Fact]
        public async Task GetAsync_ShouldReturnEntitiesWithCorrectData()
        {
            // Arrange
            var currencies = new List<Currency>();
            const string bgn = "BGN";
            const string euro = "EUR";
            const string usd = "USD";
            currencies.Add(new Currency { DisplayName = bgn });
            currencies.Add(new Currency { DisplayName = euro });
            currencies.Add(new Currency { DisplayName = usd });
            this.dbContext.AddRange(currencies);
            this.dbContext.SaveChanges();

            // Act
            var result = await this.currencyService.GetAsync();

            // Assert
            result
                .Should()
                .Contain(x => x.As<Currency>().DisplayName == bgn)
                .And
                .Contain(x => x.As<Currency>().DisplayName == euro)
                .And
                .Contain(x => x.As<Currency>().DisplayName == usd);
        }

        #endregion
    }
}
