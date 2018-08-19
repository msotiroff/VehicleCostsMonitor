namespace VehicleCostsMonitor.Services.Implementations
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using AutoMapper;
    using AutoMapper.QueryableExtensions;
    using Interfaces;
    using Microsoft.EntityFrameworkCore;
    using VehicleCostsMonitor.Data;
    using VehicleCostsMonitor.Models;
    using VehicleCostsMonitor.Services.Models.Entries.CostEntries;

    public class CostEntryService : BaseService, ICostEntryService
    {
        public CostEntryService(JustMonitorDbContext db) 
            : base(db) { }

        public async Task<bool> CreateAsync(DateTime dateCreated, int costEntryTypeId, int vehicleId, decimal price, int currencyId, string note, int? odometer)
        {
            var costEntry = new CostEntry(dateCreated, costEntryTypeId, vehicleId, price, currencyId, note, odometer);
            
            try
            {
                this.ValidateEntityState(costEntry);

                await this.db.CostEntries.AddAsync(costEntry);
                await this.db.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var costEntry = await this.db.CostEntries.FirstOrDefaultAsync(ce => ce.Id == id);
            if (costEntry == null)
            {
                return false;
            }

            this.db.CostEntries.Remove(costEntry);
            await this.db.SaveChangesAsync();
            
            return true;
        }

        public async Task<IEnumerable<CostEntryType>> GetEntryTypesAsync()
            => await this.db.CostEntryTypes.ToListAsync();

        public async Task<CostEntryDeleteServiceModel> GetForDeleteAsync(int id)
            => await this.db.CostEntries.Where(ce => ce.Id == id).ProjectTo<CostEntryDeleteServiceModel>().FirstOrDefaultAsync();

        public async Task<CostEntryUpdateServiceModel> GetForUpdateAsync(int id)
            => await this.db.CostEntries.Where(ce => ce.Id == id).ProjectTo<CostEntryUpdateServiceModel>().FirstOrDefaultAsync();

        public async Task<bool> UpdateAsync(int id, DateTime dateCreated, int costEntryTypeId, decimal price, int currencyId, string note, int? odometer)
        {
            var costEntry = await this.db.CostEntries.FirstOrDefaultAsync(ce => ce.Id == id);
            if (costEntry == null)
            {
                return false;
            }

            costEntry.DateCreated = dateCreated;
            costEntry.CostEntryTypeId = costEntryTypeId;
            costEntry.Price = price;
            costEntry.CurrencyId = currencyId;
            costEntry.Note = note;
            costEntry.Odometer = odometer;

            try
            {
                this.ValidateEntityState(costEntry);

                this.db.Update(costEntry);
                await this.db.SaveChangesAsync();

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
