namespace VehicleCostsMonitor.Services.Implementations
{
    using Interfaces;
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Threading.Tasks;
    using VehicleCostsMonitor.Data;

    public class DataAccessService : IDataAccessService
    {
        private const string EntityValidationErrorMsg = "Entity validation failed!";
        protected const string FirstFueling = "First fueling";
        protected const string FullFueling = "Full";

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

        public async Task<int> UpdateStatsOnCostEntryChangedAsync(int vehicleId)
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

        public async Task<int> UpdateStatsOnFuelEntryChangedAsync(int vehicleId)
        {
            var vehicle = await this.db.Vehicles
                .Where(v => v.Id == vehicleId)
                .Include(v => v.FuelEntries)
                .ThenInclude(fe => fe.FuelEntryType)
                .FirstOrDefaultAsync();

            var fuelEntries = vehicle.FuelEntries.OrderBy(fe => fe.DateCreated).ToArray();

            vehicle.TotalDistance = fuelEntries.Sum(fe => fe.TripOdometer);
            vehicle.TotalFuelAmount = fuelEntries.Sum(fe => fe.FuelQuantity);
            vehicle.TotalFuelCosts = fuelEntries.Sum(fe => fe.Price);

            var lastFullFuelEntry = fuelEntries.LastOrDefault(fe => fe.FuelEntryType.Name == FullFueling);
            var indexOfLastFullFueling = Math.Max(0, Array.LastIndexOf(fuelEntries, lastFullFuelEntry));

            var reducedCollection = fuelEntries.Take(indexOfLastFullFueling + 1).ToArray();
            var firstFullFuelEntry = reducedCollection.FirstOrDefault(fe => fe.FuelEntryType.Name == FullFueling);
            if (firstFullFuelEntry == null)
            {
                firstFullFuelEntry = reducedCollection.FirstOrDefault(fe => fe.FuelEntryType.Name == FirstFueling);
            }
            var indexOfFirstFullFueling = Math.Max(0, Array.IndexOf(reducedCollection, firstFullFuelEntry));

            var validFuelEntries = fuelEntries
                .Skip(indexOfFirstFullFueling)
                .Take(indexOfLastFullFueling)
                .ToList();

            var quantitiesSum = validFuelEntries.Sum(fe => fe.FuelQuantity);
            var distance = validFuelEntries.Sum(fe => fe.TripOdometer);

            if (distance != 0)
            {
                vehicle.FuelConsumption = quantitiesSum / distance * 100.0;
            }
            else if (fuelEntries.Count() <= 1)
            {
                vehicle.FuelConsumption = 0;
            }

            this.db.Update(vehicle);
            var affectedRows = await this.db.SaveChangesAsync();

            return affectedRows;
        }
    }
}
