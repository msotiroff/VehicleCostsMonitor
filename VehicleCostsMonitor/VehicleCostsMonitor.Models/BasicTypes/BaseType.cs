using System.ComponentModel.DataAnnotations;

namespace VehicleCostsMonitor.Models
{
    public abstract class BaseType
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public override string ToString() => this.Name;
    }
}
