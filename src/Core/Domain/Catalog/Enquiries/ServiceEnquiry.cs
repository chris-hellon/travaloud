using Travaloud.Domain.Catalog.Services;

namespace Travaloud.Domain.Catalog.Enquiries;

public class ServiceEnquiry : AuditableEntity<DefaultIdType>, IAggregateRoot
{
    public ServiceEnquiry(DefaultIdType serviceId, bool responseSent)
    {
        ServiceId = serviceId;
        ResponseSent = responseSent;
    }

    public DefaultIdType ServiceId { get; private set; }
    public bool ResponseSent { get; private set; }

    public virtual IList<ServiceEnquiryField> Fields { get; set; } = default!;
    public virtual Service Service { get; set; } = default!;

    public ServiceEnquiry Update(DefaultIdType? serviceId, bool? responseSent)
    {
        if (serviceId.HasValue && ServiceId != serviceId) ServiceId = serviceId.Value;
        if (responseSent.HasValue && ResponseSent != responseSent.Value) ResponseSent = responseSent.Value;

        return this;
    }
}