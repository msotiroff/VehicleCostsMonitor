namespace VehicleCostsMonitor.Web.Infrastructure.Extensions.ExcelExport.Implementations
{
    using ExcelExport.Attributes;
    using Interfaces;
    using Microsoft.AspNetCore.Mvc;
    using OfficeOpenXml;
    using OfficeOpenXml.Style;
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;

    public class ExcelResult<T> : IExcelResult where T : class
    {
        #region Constants
        private const string ResponseContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        private const string ContentDispositionHeaderKey = "content-disposition";
        private const string ContentDispositionHeaderValue = "attachment; filename={0}.xlsx";
        private const string DefaultSheetName = "Sheet";
        private const string SheetHeaderFontFamily = "Times New Roman";
        private const int SheetHeaderFontSize = 14;
        private const int SheetBodyFontSize = 12;
        #endregion

        #region Fields
        private readonly IEnumerable<T> data;
        private readonly string fileName;
        private readonly string sheetName;
        #endregion

        #region Constructors
        public ExcelResult(IEnumerable<T> data)
            : this(data, typeof(T).FullName, typeof(T).FullName)
        {
        }

        public ExcelResult(IEnumerable<T> data, string fileName)
            : this(data, fileName, fileName)
        {
        }

        public ExcelResult(IEnumerable<T> data, string fileName, string sheetName)
        {
            this.data = data;
            this.fileName = fileName;
            this.sheetName = sheetName;
        }
        #endregion

        #region Public methods
        public async Task ExecuteResultAsync(ActionContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var response = context.HttpContext.Response;
            response.ContentType = ResponseContentType;
            response.Headers.Add(ContentDispositionHeaderKey, string.Format(ContentDispositionHeaderValue, this.fileName));

            byte[] serializedData = this.SerializeAsExcelByteArray();

            await response.Body.WriteAsync(serializedData);
        }
        #endregion

        #region Private methods
        private byte[] SerializeAsExcelByteArray()
        {
            using (var application = new ExcelPackage())
            {
                application.Workbook.Properties.Title = this.sheetName ?? DefaultSheetName;
                var sheet = application.Workbook.Worksheets.Add(this.sheetName ?? DefaultSheetName);

                var allowedProperties = typeof(T)
                    .GetProperties()
                    .Select(property => new { Property = property, Attributes = property.GetCustomAttributes(false) })
                    .Where(o => o.Attributes
                        .All(attribute => !(attribute is ExcelIgnoreAttribute)))
                    .ToDictionary(property => property.Property, property => property.Attributes.Cast<Attribute>());

                var headerFont = new Font(SheetHeaderFontFamily, SheetHeaderFontSize, FontStyle.Bold);
                this.GenerateExcelHeader(sheet, allowedProperties, headerFont, Color.SkyBlue);

                var bodyFont = new Font(SheetHeaderFontFamily, SheetBodyFontSize);
                this.GenerateExcelBody(sheet, allowedProperties, bodyFont);

                this.FormatSheet(sheet);

                return application.GetAsByteArray();
            }
        }

        private void FormatSheet(ExcelWorksheet sheet)
        {
            sheet.Cells.AutoFitColumns();
        }

        private void GenerateExcelBody(ExcelWorksheet sheet, Dictionary<PropertyInfo, IEnumerable<Attribute>> properties, Font font)
        {
            var currentRow = 2;
            foreach (var item in this.data)
            {
                var currentColumn = 1;
                foreach (var property in properties)
                {
                    var currentCell = sheet.Cells[currentRow, currentColumn];
                    var propertyValue = property.Key.GetValue(item);

                    var formatAttribute = property.Value
                        .FirstOrDefault(attr => typeof(ExcelValueFormatAttribute).IsAssignableFrom(attr.GetType())) as ExcelValueFormatAttribute;

                    var isPropertyFormattable = typeof(IFormattable).IsAssignableFrom(property.Key.PropertyType);

                    if (formatAttribute != null && isPropertyFormattable)
                    {
                        propertyValue = ((IFormattable)propertyValue).ToString(formatAttribute.Format, CultureInfo.InvariantCulture);
                    }

                    currentCell.Value = propertyValue;
                    currentCell.Style.Font.SetFromFont(font);
                    currentCell.Style.Indent = 1;
                    currentCell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    currentCell.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    currentCell.Style.WrapText = false;

                    currentColumn++;
                }

                currentRow++;
            }
        }

        private void GenerateExcelHeader(ExcelWorksheet sheet, Dictionary<PropertyInfo, IEnumerable<Attribute>> properties, Font font, Color color)
        {
            var currentColumn = 1;

            foreach (var property in properties)
            {
                var currentCell = sheet.Cells[1, currentColumn];

                var displayName = property.Key.GetCustomAttribute(typeof(ExcelDisplayNameAttribute)) != null
                    ? (property.Value.First(attr => attr.GetType() == typeof(ExcelDisplayNameAttribute)) as ExcelDisplayNameAttribute).Name
                    : property.Key.Name;

                currentCell.Value = displayName;

                currentCell.Style.Font.SetFromFont(font);
                currentCell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                currentCell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                currentCell.Style.Fill.BackgroundColor.SetColor(color);

                currentColumn++;
            }
        }
        #endregion
    }
}
