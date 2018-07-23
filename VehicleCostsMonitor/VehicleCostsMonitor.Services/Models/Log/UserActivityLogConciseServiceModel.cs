namespace VehicleCostsMonitor.Services.Models.Log
{
    using Common.AutoMapping;
    using System;
    using System.ComponentModel.DataAnnotations;
    using VehicleCostsMonitor.Models;

    public class UserActivityLogConciseServiceModel : IAutoMapWith<UserActivityLog>
    {
        public int Id { get; set; }
        
        [Display(Name = "Date")]
        public DateTime DateTime { get; set; }

        [Display(Name = "Email")]
        public string UserEmail { get; set; }

        [Display(Name = "Method")]
        public string HttpMethod { get; set; }

        [Display(Name = "Controller")]
        public string ControllerName { get; set; }

        [Display(Name = "Action")]
        public string ActionName { get; set; }

        [Display(Name = "Area")]
        public string AreaName { get; set; }
    }
}
