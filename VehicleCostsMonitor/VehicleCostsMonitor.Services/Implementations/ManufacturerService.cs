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

    public class ManufacturerService : DataAccessService, IManufacturerService
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

        public async Task<bool> CreateAsync(string name)
        {
            var manufacturer = new Manufacturer { Name = name };
            if (!this.ValidateModelState(manufacturer))
            {
                return false;
            }

            try
            {
                await this.db.AddAsync(manufacturer);
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
            var manufacturer = await this.db.Manufacturers.FindAsync(id);
            if (manufacturer == null)
            {
                return false;
            }

            this.db.Manufacturers.Remove(manufacturer);
            await this.db.SaveChangesAsync();

            return true;
        }

        public async Task<ManufacturerUpdateServiceModel> GetAsync(int id)
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
            if (!this.ValidateModelState(manufacturer))
            {
                return false;
            }

            this.db.Manufacturers.Update(manufacturer);
            await this.db.SaveChangesAsync();

            return true;
        }
    }
}
