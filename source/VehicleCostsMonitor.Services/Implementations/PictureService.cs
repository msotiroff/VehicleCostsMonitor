namespace VehicleCostsMonitor.Services.Implementations
{
    using AutoMapper.QueryableExtensions;
    using Interfaces;
    using Microsoft.EntityFrameworkCore;
    using System.Linq;
    using System.Threading.Tasks;
    using VehicleCostsMonitor.Data;
    using VehicleCostsMonitor.Models;
    using VehicleCostsMonitor.Services.Models.Picture;

    public class PictureService : BaseService, IPictureService
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

            var vehicle = await this.db.Vehicles.FirstOrDefaultAsync(v => v.Id == vehicleId && !v.IsDeleted);
            if (vehicle == null)
            {
                return false;
            }

            try
            {
                this.ValidateEntityState(picture);

                await this.db.Pictures.AddAsync(picture);
                vehicle.PictureId = picture.Id;

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
                .Where(p => p.VehicleId == vehicleId && !p.Vehicle.IsDeleted)
                .ProjectTo<PictureUpdateServiceModel>()
                .FirstOrDefaultAsync();

        public async Task<string> GetPathAsync(int id)
        {
            var picture = await this.db.Pictures.FindAsync(id);

            return picture?.Path;
        }
    }
}
