namespace VehicleCostsMonitor.Tests.Web.Controllers
{
    using Xunit;
    using FluentAssertions;
    using VehicleCostsMonitor.Web.Controllers;
    using VehicleCostsMonitor.Services.Interfaces;
    using Moq;
    using System.Threading.Tasks;
    using System.Collections.Generic;
    using VehicleCostsMonitor.Services.Models.Manufacturer;
    using Microsoft.AspNetCore.Mvc;
    using VehicleCostsMonitor.Web.Models.Home;
    using System.Linq;
    using Microsoft.AspNetCore.Mvc.Rendering;

    public class HomeControllerTests
    {
        private readonly HomeController controller;
        private readonly Mock<IManufacturerService> manufacturerService;

        public HomeControllerTests()
        {
            this.manufacturerService = new Mock<IManufacturerService>();
            this.controller = new HomeController(this.manufacturerService.Object);
        }

        [Fact]
        public async Task Index_ShouldReturnView()
        {
            // Arrange
            this.manufacturerService
                .Setup(s => s.AllAsync())
                .ReturnsAsync(new List<ManufacturerConciseListModel>());

            // Act
            var result = await this.controller.Index();

            // Assert
            result
                .Should()
                .BeAssignableTo<ViewResult>();
        }

        [Fact]
        public async Task Index_ShouldReturnViewWithCorrectModel()
        {
            // Arrange
            this.manufacturerService
                .Setup(s => s.AllAsync())
                .ReturnsAsync(new List<ManufacturerConciseListModel>
                {
                    new ManufacturerConciseListModel(),
                    new ManufacturerConciseListModel(),
                    new ManufacturerConciseListModel(),
                });

            // Act
            var result = await this.controller.Index() as ViewResult;
            var model = result.ViewData.Model;

            // Assert
            model
                .Should()
                .BeAssignableTo<IndexViewModel>()
                .And
                .Match<IndexViewModel>(m => m.AllManufacturers.Count() == 3)
                .And
                .Match<IndexViewModel>(m => m.AllManufacturers
                    .All(manuf => typeof(SelectListItem)
                        .IsAssignableFrom(manuf.GetType())));
        }
    }
}
