namespace VehicleCostsMonitor.Models
{
    using System.ComponentModel.DataAnnotations;

    public abstract class BaseType
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public override string ToString() => this.Name;
    }
}
