namespace VehicleCostsMonitor.Models
{
    using System.ComponentModel.DataAnnotations;

    public class Currency
    {
        [Key]
        [Display(Name = "Display currency")]
        public int Id { get; set; }
        
        public string DisplayName { get; set; }

        [Required]
        [RegularExpression("^[A-Z]{3}$")]
        public string Code { get; set; }

        public override string ToString() => $"{this.DisplayName} ({this.Code})";
    }
}