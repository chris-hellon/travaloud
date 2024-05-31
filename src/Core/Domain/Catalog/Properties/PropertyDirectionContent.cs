namespace Travaloud.Domain.Catalog.Properties;

public class PropertyDirectionContent : AuditableEntity, IAggregateRoot
{
    public PropertyDirectionContent(string body, string? style)
    {
        Body = body.FormatTextEditorString();;
        Style = style;
    }

    public DefaultIdType PropertyDirectionId { get; private set; }
    public string Body { get; private set; }
    public string? Style { get; private set; }
    public virtual PropertyDirection PropertyDirection { get; private set; } = default!;

    public PropertyDirectionContent Update(string body, string? style)
    {
        if (body is not null && Body?.Equals(body) is not true) Body = body.FormatTextEditorString();;
        if (style is not null && Style?.Equals(style) is not true) Style = style;
        return this;
    }
}