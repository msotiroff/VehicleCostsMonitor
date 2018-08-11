namespace VehicleCostsMonitor.Web.Infrastructure.Extensions.ExcelExport.Attributes
{
    using System;

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class ExcelValueFormatAttribute : Attribute
    {
        public ExcelValueFormatAttribute(string format)
        {
            this.Format = format;
        }

        public string Format { get; }
    }
}
