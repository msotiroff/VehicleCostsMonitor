namespace VehicleCostsMonitor.Models
{
    using System.ComponentModel.DataAnnotations;

    public class Model
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public int ManufactureId { get; set; }

        public Manufacturer Manufacturer { get; set; }
    }
}