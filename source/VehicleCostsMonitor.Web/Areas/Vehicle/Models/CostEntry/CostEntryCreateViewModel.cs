namespace VehicleCostsMonitor.Web.Areas.Vehicle.Models.CostEntry
{
    using Microsoft.AspNetCore.Mvc.Rendering;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class CostEntryCreateViewModel
    {
        [Display(Name = "Date")]
        [DataType(DataType.DateTime)]
        public DateTime DateCreated { get; set; }

        public int? Odometer { get; set; }

        public IEnumerable<SelectListItem> AllCostEntryTypes { get; set; }

        [Required]
        [Display(Name = "Type")]
        public int CostEntryTypeId { get; set; }

        [Display(Name = "Price")]
        [DataType(DataType.Currency)]
        public decimal Price { get; set; }

        [Required]
        [Display(Name = "Currency")]
        public int? CurrencyId { get; set; }

        public IEnumerable<SelectListItem> AllCurrencies { get; set; }

        public string Note { get; set; }

        [Required]
        public int VehicleId { get; set; }
    }
}
