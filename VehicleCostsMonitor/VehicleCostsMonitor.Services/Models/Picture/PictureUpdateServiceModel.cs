namespace VehicleCostsMonitor.Services.Models.Picture
{
    using Common.AutoMapping.Interfaces;
    using System.ComponentModel.DataAnnotations;
    using VehicleCostsMonitor.Models;

    public class PictureUpdateServiceModel : IAutoMapWith<Picture>
    {
        public int? Id { get; set; }

        [Required]
        public string Path { get; set; }

        [Required]
        public int VehicleId { get; set; }
    }
}
