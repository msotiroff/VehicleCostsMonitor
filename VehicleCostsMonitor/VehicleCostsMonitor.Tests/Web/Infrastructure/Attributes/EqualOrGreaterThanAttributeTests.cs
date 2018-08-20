namespace VehicleCostsMonitor.Tests.Web.Infrastructure.Attributes
{
    using FluentAssertions;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using VehicleCostsMonitor.Tests.Helpers.ValidationTargets;
    using Xunit;

    public class EqualOrGreaterThanAttributeTests
    {
        [Fact]
        public void IsValid_WithNotNumber_ShouldThrowInvalidCastException()
        {
            // Arrange
            var target = new ValidationTargetInvalidPropertyType();
            target.FirstNumber = 5;
            target.SecondNumber = "SomeString";

            var context = new ValidationContext(target);
            var results = new List<ValidationResult>();

            // Act

            // Assert
            Assert.Throws<InvalidCastException>(() => Validator.TryValidateObject(target, context, results, true));
        }

        [Fact]
        public void IsValid_WithInvalidComparisonPropertyType_ShouldThrowInvalidCastException()
        {
            // Arrange
            var target = new ValidationTargetInvalidComparisonPropertyType();
            target.FirstNumber = "SomePropertyValue";
            target.SecondNumber = 5;

            var context = new ValidationContext(target);
            var results = new List<ValidationResult>();

            // Act

            // Assert
            Assert.Throws<InvalidCastException>(() => Validator.TryValidateObject(target, context, results, true));
        }

        [Fact]
        public void IsValid_WithInvalidComparisonPropertyName_ShouldThrowInvalidOperationException()
        {
            // Arrange
            var target = new ValidationTargetInvalidComparisonPropertyName();
            target.FirstNumber = 5;
            target.SecondNumber = 7;

            var context = new ValidationContext(target);
            var results = new List<ValidationResult>();

            // Act

            // Assert
            Assert.Throws<InvalidOperationException>(() => Validator.TryValidateObject(target, context, results, true));
        }

        [Fact]
        public void IsValid_WithUnfulfilledCondition_ShouldReturnFalseAndSetValidationError()
        {
            // Arrange
            var target = new ValidationTargetValid();
            target.FirstNumber = 5;
            target.SecondNumber = 3;

            var context = new ValidationContext(target);
            var results = new List<ValidationResult>();

            // Act
            var isValid = Validator.TryValidateObject(target, context, results, true);

            // Assert
            isValid
                .Should()
                .BeFalse();

            results
                .Should()
                .HaveCountGreaterThan(0);
        }

        [Fact]
        public void IsValid_WithFulfilledCondition_ShouldReturnTrueAndNotSetValidationError()
        {
            // Arrange
            var target = new ValidationTargetValid();
            target.FirstNumber = 5;
            target.SecondNumber = 6;

            var context = new ValidationContext(target);
            var results = new List<ValidationResult>();

            // Act
            var isValid = Validator.TryValidateObject(target, context, results, true);

            // Assert
            isValid
                .Should()
                .BeTrue();

            results
                .Should()
                .BeEmpty();
        }
    }
}
