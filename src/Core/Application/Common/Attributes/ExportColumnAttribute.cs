namespace Travaloud.Application.Common.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public class ExportColumnAttribute : Attribute
{
    public string? ColumnName { get; }
    public bool HideColumn { get; set; }

    public ExportColumnAttribute(string? columnName, bool hideColumn = false)
    {
        ColumnName = columnName;
        HideColumn = hideColumn;
    }

    public ExportColumnAttribute(bool hideColumn)
    {
        HideColumn = hideColumn;
    }
}