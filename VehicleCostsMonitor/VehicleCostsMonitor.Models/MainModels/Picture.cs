namespace VehicleCostsMonitor.Models
{
    using System.ComponentModel.DataAnnotations;

    public class Picture
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Path { get; set; }

        [Required]
        public int VehicleId { get; set; }

        public Vehicle Vehicle { get; set; }
    }
}