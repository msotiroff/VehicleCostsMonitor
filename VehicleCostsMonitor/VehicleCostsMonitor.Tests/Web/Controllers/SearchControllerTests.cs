namespace VehicleCostsMonitor.Tests.Web.Controllers
{

    using FluentAssertions;
    using Microsoft.AspNetCore.Mvc;
    using Moq;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using VehicleCostsMonitor.Models;
    using VehicleCostsMonitor.Services.Interfaces;
    using VehicleCostsMonitor.Services.Models.Manufacturer;
    using VehicleCostsMonitor.Services.Models.Vehicle;
    using VehicleCostsMonitor.Web;
    using VehicleCostsMonitor.Web.Controllers;
    using VehicleCostsMonitor.Web.Infrastructure.Utilities.Interfaces;
    using VehicleCostsMonitor.Web.Models.Search;
    using Xunit;

    public class SearchControllerTests
    {
        private readonly DateTime SampleDateTime = new DateTime(2000, 1, 1);

        private readonly SearchController controller;

        private readonly Mock<IVehicleService> vehicles;
        private readonly Mock<IManufacturerService> manufacturers;
        private readonly Mock<IVehicleElementService> vehicleElements;
        private readonly Mock<IDateTimeProvider> dateTimeProvider;

        public SearchControllerTests()
        {
            this.vehicles = new Mock<IVehicleService>();
            this.manufacturers = new Mock<IManufacturerService>();
            this.vehicleElements = new Mock<IVehicleElementService>();
            this.dateTimeProvider = new Mock<IDateTimeProvider>();

            this.controller = new SearchController(
                this.vehicles.Object,
                this.manufacturers.Object,
                this.vehicleElements.Object,
                this.dateTimeProvider.Object);
        }

        [Fact]
        public void Index_ShouldRedirectToResultWithDefaultRouteValues()
        {
            // Act
            var result = this.controller.Index() as RedirectToActionResult;
            var actionName = result?.ActionName;
            var routeValues = result?.RouteValues;

            // Assert
            result
                .Should()
                .NotBeNull();

            actionName
                .Should()
                .Be(nameof(controller.Result));

            routeValues
                .Should()
                .ContainKey("manufacturerId")
                .WhichValue
                    .Should()
                    .Be(default(int));

            routeValues
                .Should()
                .ContainKey("modelName")
                .WhichValue
                    .Should()
                    .Be(string.Empty);
        }

        [Fact]
        public async Task Result_ShouldReturnView()
        {
            // Arrange
            const int pageIndex = 1;
            this.SetupDependenciesWithDefaultValues();

            // Act
            var result = await this.controller.Result(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), pageIndex);

            // Assert
            result
                .Should()
                .BeAssignableTo<ViewResult>();
        }

        [Fact]
        public async Task Result_ShouldReturnViewWithCorrectPaging()
        {
            // Arrange
            const int pageIndex = 2;
            this.SetupDependenciesWithDefaultValues();

            this.vehicles
                .Setup(v => v.Get(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
                .Returns(this.GetCollectionOf<VehicleSearchServiceModel>(50).AsQueryable());

            // Act
            var result = await this.controller.Result(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), pageIndex);
            var viewModel = (result as ViewResult)?.ViewData?.Model;

            // Assert
            viewModel
                .Should()
                .NotBeNull()
                .And
                .BeAssignableTo<SearchViewModel>()
                .And
                .Match<SearchViewModel>(m => m.Results.PageIndex == pageIndex)
                .And
                .Match<SearchViewModel>(m => m.Results.Count() == WebConstants.SearchResultsPageSize);
        }

        private IEnumerable<TModel> GetCollectionOf<TModel>(int count)
        {
            var modelType = typeof(TModel);
            var collection = new List<TModel>();

            for (int i = 0; i < count; i++)
            {
                collection.Add((TModel)Activator.CreateInstance(modelType));
            }

            return collection;
        }

        private void SetupDependenciesWithDefaultValues()
        {
            this.vehicles
                .Setup(v => v.Get(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
                .Returns(new List<VehicleSearchServiceModel>().AsQueryable());

            this.manufacturers
                .Setup(m => m.AllAsync())
                .ReturnsAsync(new List<ManufacturerConciseListModel>());

            this.vehicleElements
                .Setup(ve => ve.GetGearingTypes())
                .ReturnsAsync(new List<GearingType>());

            this.vehicleElements
                .Setup(ve => ve.GetVehicleTypes())
                .ReturnsAsync(new List<VehicleType>());

            this.vehicleElements
                .Setup(ve => ve.GetFuelTypes())
                .ReturnsAsync(new List<FuelType>());

            this.dateTimeProvider
                .Setup(d => d.GetCurrentDateTime())
                .Returns(SampleDateTime);
        }
    }
}
