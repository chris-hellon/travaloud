using System.ComponentModel;
using System.Data;
using ClosedXML.Excel;
using Travaloud.Application.Common.Attributes;

namespace Travaloud.Infrastructure.Common.Export;

public class ExcelWriter : IExcelWriter
{
    public Stream WriteToStream<T>(IList<T> data)
    {
        var properties = TypeDescriptor.GetProperties(typeof(T));
        var table = new DataTable("table", "table");
        foreach (PropertyDescriptor prop in properties)
        {
            var hasExportColumnAttribute = prop.Attributes.OfType<ExportColumnAttribute>().Any();

            if (hasExportColumnAttribute)
            {
                var exportAttribute = prop.Attributes.OfType<ExportColumnAttribute>().First();

                if (!exportAttribute.HideColumn)
                {
                    table.Columns.Add(exportAttribute.ColumnName, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
                }
            }
            else
            {
                table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
            }
        }

        foreach (var item in data)
        {
            var row = table.NewRow();
            foreach (PropertyDescriptor prop in properties)
            {
                var hasExportColumnAttribute = prop.Attributes.OfType<ExportColumnAttribute>().Any();

                if (hasExportColumnAttribute)
                {
                    var exportAttribute = prop.Attributes.OfType<ExportColumnAttribute>().First();

                    if (exportAttribute != null && !exportAttribute.HideColumn && exportAttribute.ColumnName != null)
                    {
                        row[exportAttribute.ColumnName] = prop.GetValue(item) ?? DBNull.Value;
                    }
                }
                else
                {
                    row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
                }
            }

            table.Rows.Add(row);
        }

        using var wb = new XLWorkbook();
        
        var wsDep =  wb.Worksheets.Add(table);
        wsDep.Columns().AdjustToContents();

        var dateFormatColumns = new[] {"Booking Date", "Date of Birth", "Tour Start Date", "Tour End Date"};

        foreach (var dateFormatColumn in dateFormatColumns)
        {
            var column = table.Columns[dateFormatColumn];

            if (column == null) continue;
            
            var index = table.Columns.IndexOf(column);

            if (index < 0) continue;
            
            var format = dateFormatColumn == "Date of Birth" ? "dd/MM/yyyy" : "dd/MM/yyyy HH:mm";
            wsDep.Column(index + 1).Style.DateFormat.Format = format;
        }
        
        Stream stream = new MemoryStream();
        wb.SaveAs(stream);
        stream.Seek(0, SeekOrigin.Begin);
        return stream;
    }
}
