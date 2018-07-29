namespace VehicleCostsMonitor.Web.Areas.Vehicle.Models.CostEntry
{
    using Common.AutoMapping;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Services.Models.Entries.CostEntries;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class CostEntryUpdateViewModel : IAutoMapWith<CostEntryUpdateServiceModel>
    {
        [Required]
        public int Id { get; set; }

        [Display(Name = "Date")]
        public DateTime DateCreated { get; set; }

        public int? Odometer { get; set; }

        public IEnumerable<SelectListItem> AllCostEntryTypes { get; set; }

        [Required]
        [Display(Name = "Type")]
        public int CostEntryTypeId { get; set; }

        [Display(Name = "Price")]
        [DataType(DataType.Currency)]
        public decimal Price { get; set; }

        public string Note { get; set; }

        [Required]
        public int VehicleId { get; set; }
    }
}
