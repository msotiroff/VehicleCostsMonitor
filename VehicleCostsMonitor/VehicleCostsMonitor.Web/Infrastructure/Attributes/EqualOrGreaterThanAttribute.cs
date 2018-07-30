namespace VehicleCostsMonitor.Web.Infrastructure.Attributes
{
    using System;
    using System.ComponentModel.DataAnnotations;

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class EqualOrGreaterThanAttribute : ValidationAttribute
    {
        private const string FailedValidationErrorMsg = "{0}'s value must be equal or greater than {1}'s value ({2})!";
        private const string FailedObjectTypeErrorMsg = "GreaterOrEqualOf attribute can be used only for numeric types";
        private const string PropertyNotFoundErrorMsg = "Property with name {0} not found!";

        private readonly string comparisonProperty;

        public EqualOrGreaterThanAttribute(string comparisonProperty)
        {
            this.comparisonProperty = comparisonProperty;
        }
        
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            double.TryParse(value.ToString(), out double corePropertyValue);
            
            if (corePropertyValue.ToString() != value.ToString())
            {
                throw new InvalidCastException(FailedObjectTypeErrorMsg);
            }

            var comparedProperty = validationContext.ObjectType.GetProperty(this.comparisonProperty);
            if (comparedProperty == null)
            {
                throw new InvalidOperationException(string.Format(PropertyNotFoundErrorMsg, this.comparisonProperty));
            }

            var comparisonPropertyValueAsString = comparedProperty.GetValue(validationContext.ObjectInstance).ToString();
            
            double.TryParse(comparisonPropertyValueAsString, out double comparisonPropertyValue);
            if (comparisonPropertyValueAsString != comparisonPropertyValue.ToString())
            {
                throw new InvalidCastException(FailedObjectTypeErrorMsg);
            }

            if (corePropertyValue < comparisonPropertyValue)
            {
                var corePropertyName = validationContext.DisplayName;

                return new ValidationResult(string.Format(FailedValidationErrorMsg, corePropertyName, comparedProperty.Name, comparisonPropertyValue));
            }

            return ValidationResult.Success;
        }
    }
}
