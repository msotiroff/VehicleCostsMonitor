namespace VehicleCostsMonitor.Services.Implementations
{
    using AutoMapper;
    using AutoMapper.QueryableExtensions;
    using Data;
    using Interfaces;
    using Microsoft.EntityFrameworkCore;
    using Models.Manufacturer;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using VehicleCostsMonitor.Models;

    public class ManufacturerService : BaseService, IManufacturerService
    {
        public ManufacturerService(JustMonitorDbContext db) 
            : base(db) { }

        public async Task<IEnumerable<ManufacturerConciseListModel>> AllAsync()
        {
            var all = await this.db
            .Manufacturers
            .OrderBy(m => m.Name)
            .ProjectTo<ManufacturerConciseListModel>()
            .ToListAsync();

            return all;
        }

        public async Task<int> CreateAsync(string name)
        {
            var manufacturer = new Manufacturer { Name = name };
            
            try
            {
                this.ValidateEntityState(manufacturer);

                await this.db.AddAsync(manufacturer);
                await this.db.SaveChangesAsync();

                return manufacturer.Id;
            }
            catch
            {
                return default(int);
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var manufacturer = await this.db.Manufacturers.FindAsync(id);
            if (manufacturer == null)
            {
                return false;
            }

            this.db.Manufacturers.Remove(manufacturer);
            await this.db.SaveChangesAsync();

            return true;
        }

        public async Task<ManufacturerUpdateServiceModel> GetForUpdateAsync(int id)
        {
            var manufacturer = await this.db.Manufacturers.FindAsync(id);
            if (manufacturer == null)
            {
                return null;
            }

            return Mapper.Map<ManufacturerUpdateServiceModel>(manufacturer);
        }

        public async Task<ManufacturerDetailsServiceModel> GetDetailedAsync(int id)
            => await this.db
                .Manufacturers
                .Where(m => m.Id == id)
                .ProjectTo<ManufacturerDetailsServiceModel>()
                .FirstOrDefaultAsync();

        public async Task<bool> UpdateAsync(int id, string name)
        {
            var manufacturer = await this.db.Manufacturers.FindAsync(id);
            if (manufacturer == null)
            {
                return false;
            }

            manufacturer.Name = name;

            try
            {
                this.ValidateEntityState(manufacturer);

                this.db.Manufacturers.Update(manufacturer);
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
