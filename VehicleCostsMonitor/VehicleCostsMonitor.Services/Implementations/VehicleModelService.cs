namespace VehicleCostsMonitor.Services.Implementations
{
    using System.Linq;
    using System.Threading.Tasks;
    using AutoMapper.QueryableExtensions;
    using Interfaces;
    using Microsoft.EntityFrameworkCore;
    using VehicleCostsMonitor.Data;
    using VehicleCostsMonitor.Models;
    using VehicleCostsMonitor.Services.Models.VehicleModel;

    public class VehicleModelService : DataAccessService, IVehicleModelService
    {
        public VehicleModelService(JustMonitorDbContext db) 
            : base(db) { }

        public async Task<bool> Create(string modelName, int manufacturerId)
        {
            var model = new Model
            {
                Name = modelName,
                ManufactureId = manufacturerId
            };

            if (!this.ValidateModelState(model))
            {
                return false;
            }

            try
            {
                await this.db.Models.AddAsync(model);
                await this.db.SaveChangesAsync();

                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> Delete(int id)
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

        public async Task<ModelConciseServiceModel> Get(int id)
            => await this.db.Models
                .Where(m => m.Id == id)
                .ProjectTo<ModelConciseServiceModel>()
                .FirstOrDefaultAsync();
        
    }
}
