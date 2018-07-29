namespace VehicleCostsMonitor.Services.Interfaces
{
    using System.Threading.Tasks;
    using VehicleCostsMonitor.Services.Models.Picture;

    public interface IPictureService
    {
        Task<bool> CreateAsync(string path, int vehicleId);

        Task<bool> DeleteAsync(int id);

        Task<PictureUpdateServiceModel> GetByVehicleId(int vehicleId);

        Task<string> GetPathAsync(int id);
    }
}
