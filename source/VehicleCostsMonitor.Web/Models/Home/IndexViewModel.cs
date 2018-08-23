namespace VehicleCostsMonitor.Web.Models.Home
{
    using Microsoft.AspNetCore.Mvc.Rendering;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class IndexViewModel
    {
        [Display(Name = "Make")]
        public IEnumerable<SelectListItem> AllManufacturers { get; set; }

        [Display(Name = "Make")]
        public int ManufacturerId { get; set; }

        [Display(Name = "Model")]
        public string ModelName { get; set; }
    }
}
