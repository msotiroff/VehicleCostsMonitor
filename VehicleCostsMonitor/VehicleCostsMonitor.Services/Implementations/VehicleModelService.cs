namespace VehicleCostsMonitor.Services.Implementations
{
    using AutoMapper;
    using Data;
    using Interfaces;
    using Microsoft.EntityFrameworkCore;
    using Models.Vehicle;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using VehicleCostsMonitor.Models;

    public class VehicleModelService : BaseService, IVehicleModelService
    {
        public VehicleModelService(JustMonitorDbContext db) 
            : base(db) { }

        public async Task<bool> CreateAsync(string modelName, int manufacturerId)
        {
            var newModel = new Model
            {
                Name = modelName,
                ManufacturerId = manufacturerId
            };

            var modelExist = await this.db.Models.AnyAsync(m => m.ManufacturerId == manufacturerId && m.Name == modelName);
            if (modelExist)
            {
                return false;
            }
            
            try
            {
                this.ValidateEntityState(newModel);

                await this.db.Models.AddAsync(newModel);
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
            var model = await this.db.Models.FindAsync(id);
            if (model == null)
            {
                return false;
            }

            this.db.Remove(model);
            await this.db.SaveChangesAsync();

            return true;
        }

        public async Task<ModelConciseServiceModel> GetAsync(int id)
        => Mapper
            .Map<ModelConciseServiceModel>(await this.db
                .Models
                .FirstOrDefaultAsync(m => m.Id == id));

        public async Task<IEnumerable<string>> GetByManufacturerIdAsync(int manufacturerId)
            => await this.db.Models
            .Where(mod => mod.ManufacturerId == manufacturerId)
            .Select(mod => mod.Name)
            .ToListAsync();

        public async Task<bool> UpdateAsync(int id, string name)
        {
            var model = await this.db.Models.FirstOrDefaultAsync(m => m.Id == id);
            if (model == null)
            {
                return false;
            }

            try
            {
                model.Name = name;
                this.ValidateEntityState(model);

                this.db.Update(model);
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
