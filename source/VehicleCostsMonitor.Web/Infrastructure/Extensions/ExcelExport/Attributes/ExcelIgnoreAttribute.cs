namespace VehicleCostsMonitor.Web.Infrastructure.Extensions.ExcelExport.Attributes
{
    using System;

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class ExcelIgnoreAttribute : Attribute
    {
    }
}
