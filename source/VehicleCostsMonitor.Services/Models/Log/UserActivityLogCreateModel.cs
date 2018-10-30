namespace VehicleCostsMonitor.Services.Models.Log
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using VehicleCostsMonitor.Common.AutoMapping.Interfaces;
    using VehicleCostsMonitor.Models;

    public class UserActivityLogCreateModel : IAutoMapWith<UserActivityLog>
    {
        [Required]
        [DataType(DataType.DateTime)]
        public DateTime DateTime { get; set; }

        [Required]
        [EmailAddress]
        public string UserEmail { get; set; }

        [Required]
        public string HttpMethod { get; set; }

        [Required]
        public string ControllerName { get; set; }

        [Required]
        public string ActionName { get; set; }

        public string AreaName { get; set; }

        [Url]
        public string Url { get; set; }

        public string QueryString { get; set; }

        public string ActionArguments { get; set; }
    }
}
