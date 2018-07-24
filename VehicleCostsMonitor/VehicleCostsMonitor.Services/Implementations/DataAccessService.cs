namespace VehicleCostsMonitor.Services.Implementations
{
    using VehicleCostsMonitor.Data;
    using System.ComponentModel.DataAnnotations;
    using System.Collections.Generic;
    using System;

    public class DataAccessService
    {
        private const string EntityValidationErrorMsg = "Entity validation failed!";

        protected JustMonitorDbContext db;

        public DataAccessService(JustMonitorDbContext db)
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
