namespace VehicleCostsMonitor.Web.Infrastructure.Extensions.ExcelExport.Attributes
{
    using System;

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class ExcelDisplayNameAttribute : Attribute
    {
        public ExcelDisplayNameAttribute(string name)
        {
            this.Name = name;
        }

        public string Name { get; }
    }
}
