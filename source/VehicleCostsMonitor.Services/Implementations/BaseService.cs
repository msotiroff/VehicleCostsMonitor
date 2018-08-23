namespace VehicleCostsMonitor.Services.Implementations
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using VehicleCostsMonitor.Data;

    public abstract class BaseService
    {
        private const string EntityValidationErrorMsg = "Entity validation failed!";
        protected const string FirstFueling = "First fueling";
        protected const string FullFueling = "Full";

        protected JustMonitorDbContext db;

        public BaseService(JustMonitorDbContext db)
        {
            this.db = db;
        }

        protected void ValidateEntityState(object model)
        {
            var validationContext = new ValidationContext(model);
            var validationResults = new List<ValidationResult>();

            var isValid = Validator.TryValidateObject(model, validationContext, validationResults, true);

            if (!isValid)
            {
                throw new InvalidOperationException(EntityValidationErrorMsg);
            }
        }
    }
}
