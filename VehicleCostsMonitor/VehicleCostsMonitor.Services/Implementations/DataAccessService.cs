namespace VehicleCostsMonitor.Services.Implementations
{
    using VehicleCostsMonitor.Data;
    using System.ComponentModel.DataAnnotations;
    using System.Collections.Generic;
    using System;
    using System.Threading.Tasks;
    using System.Linq;
    using Microsoft.EntityFrameworkCore;

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

        protected async Task<int> UpdateStatsOnCostEntryChangedAsync(int vehicleId)
        {
            var vehicle = await this.db.Vehicles
                .Where(v => v.Id == vehicleId)
                .Include(v => v.CostEntries)
                .FirstOrDefaultAsync();

            var costEntrySum = vehicle.CostEntries.Sum(ce => ce.Price);
            vehicle.TotalOtherCosts = costEntrySum;

            this.db.Update(vehicle);
            var affectedRows = await this.db.SaveChangesAsync();

            return affectedRows;
        }

        //protected async Task<int> UpdateStatsOnFuelEntryChangedAsync(int vehicleId)
        //{

        //}
    }
}
