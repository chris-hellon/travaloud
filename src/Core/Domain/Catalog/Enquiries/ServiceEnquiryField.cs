namespace Travaloud.Domain.Catalog.Enquiries;

public class ServiceEnquiryField : AuditableEntity<DefaultIdType>, IAggregateRoot
{
    public DefaultIdType ServiceEnquiryId { get; private set; }
    public string Field { get; private set; } = default!;
    public string Value { get; private set; } = default!;

    public virtual ServiceEnquiry ServiceEnquiry { get; set; } = default!;

    public ServiceEnquiryField(string field, string value)
    {
        Field = field;
        Value = value;
    }

    public ServiceEnquiryField Update(string? field, string? value)
    {
        if (field is not null && Field?.Equals(field) is not true) Field = field;
        if (value is not null && Value?.Equals(value) is not true) Value = value;

        return this;
    }
}