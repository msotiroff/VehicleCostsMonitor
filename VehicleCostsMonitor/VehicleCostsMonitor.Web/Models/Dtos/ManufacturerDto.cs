using System.Collections.Generic;

namespace VehicleCostsMonitor.Web.Models.Dtos
{
    public class ManufacturerDto
    {
        public string Name { get; set; }

        public IEnumerable<string> Models { get; set; }
    }
}
