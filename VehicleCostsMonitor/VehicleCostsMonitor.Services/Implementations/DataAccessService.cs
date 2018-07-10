using VehicleCostsMonitor.Data;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace VehicleCostsMonitor.Services.Implementations
{
    public class DataAccessService
    {
        protected JustMonitorDbContext db;

        public DataAccessService(JustMonitorDbContext db)
        {
            this.db = db;
        }

        protected bool ValidateModelState(object model)
        {
            var validationContext = new System.ComponentModel.DataAnnotations.ValidationContext(model);
            var validationResults = new List<ValidationResult>();

            var isValid = Validator.TryValidateObject(model, validationContext, validationResults, true);

            return isValid;
        }
    }
}
