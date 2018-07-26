namespace VehicleCostsMonitor.Services.Implementations
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using AutoMapper.QueryableExtensions;
    using Interfaces;
    using Microsoft.EntityFrameworkCore;
    using VehicleCostsMonitor.Data;
    using VehicleCostsMonitor.Models;
    using VehicleCostsMonitor.Services.Models.Picture;

    public class PictureService : DataAccessService, IPictureService
    {
        public PictureService(JustMonitorDbContext db) 
            : base(db) { }

        public async Task<bool> CreateAsync(string path, int vehicleId)
        {
            var picture = new Picture
            {
                Path = path,
                VehicleId = vehicleId
            };

            try
            {
                await this.db.Pictures.AddAsync(picture);
                await this.db.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var picture = await this.db.Pictures.FindAsync(id);
            if (picture == null)
            {
                return false;
            }

            this.db.Remove(picture);
            await this.db.SaveChangesAsync();

            return true;
        }

        public async Task<PictureUpdateServiceModel> GetByVehicleId(int vehicleId)
            => await this.db
                .Pictures
                .Where(p => p.VehicleId == vehicleId)
                .ProjectTo<PictureUpdateServiceModel>()
                .FirstOrDefaultAsync();
    }
}
