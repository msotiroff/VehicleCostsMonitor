namespace VehicleCostsMonitor.Tests.Web.Infrastructure.Filters
{
    using FluentAssertions;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Abstractions;
    using Microsoft.AspNetCore.Mvc.Filters;
    using Microsoft.AspNetCore.Routing;
    using Moq;
    using System.Collections.Generic;
    using VehicleCostsMonitor.Web.Areas.Vehicle.Controllers;
    using VehicleCostsMonitor.Web.Infrastructure.Filters;
    using Xunit;

    public class ValidateModelStateAttributeTests
    {
        private readonly ActionExecutingContext context;
        private readonly ValidateModelStateAttribute attribute;

        public ValidateModelStateAttributeTests()
        {
            this.attribute = new ValidateModelStateAttribute();
            this.context = this.GetActionExecutingContext();
        }

        [Fact]
        public void OnActionExecuting_WithValidModelState_ShouldPass()
        {
            // Arrange

            // Act
            this.attribute.OnActionExecuting(context);

            // Assert
            context
                .ModelState
                .Should()
                .BeEmpty();
        }

        [Fact]
        public void OnActionExecuting_WithInvalidModel_ShouldReturnToGetMethod()
        {
            // Arrange
            context.ActionDescriptor.RouteValues.Add("area", "Vehicle");
            context.ActionDescriptor.RouteValues.Add("controller", "Vehicle");
            context.ActionDescriptor.RouteValues.Add("action", "Create");
            
            context.ModelState.AddModelError("ErrorKey", "ErrorMessage");

            // Act
            this.attribute.OnActionExecuting(context);
            var result = context.Result as RedirectToActionResult;

            // Assert
            result
                .Should()
                .NotBeNull()
                .And
                .Match<RedirectToActionResult>(r => r.ControllerName == "Vehicle")
                .And
                .Match<RedirectToActionResult>(r => r.ActionName == "Create")
                .And
                .Match<RedirectToActionResult>(r => r.RouteValues.ContainsKey("area"))
                .And
                .Match<RedirectToActionResult>(r => r.RouteValues["area"].Equals("Vehicle"));
        }

        private ActionExecutingContext GetActionExecutingContext()
            => new ActionExecutingContext(
                new ActionContext(
                    Mock.Of<HttpContext>(),
                    Mock.Of<RouteData>(),
                    Mock.Of<ActionDescriptor>()),
                new List<IFilterMetadata>(),
                new Dictionary<string, object>(),
                new VehicleController(null, null, null, null, null, null, null, null));
    }
}
