using Travaloud.Domain.Catalog.Tours;

namespace Travaloud.Domain.Catalog.Enquiries;

public class TourEnquiry : AuditableEntity<DefaultIdType>, IAggregateRoot
{
    public DefaultIdType TourId { get; private set; }
    public string Name { get; private set; } = default!;
    public string Email { get; private set; } = default!;
    public string ContactNumber { get; private set; } = default!;
    public DateTime RequestedDate { get; private set; }
    public int NumberOfPeople { get; private set; }
    public string? AdditionalInformation { get; private set; }
    public bool ResponseSent { get; private set; }

    public virtual Tour Tour { get; set; } = default!;

    public TourEnquiry(DefaultIdType tourId, string name, string email, string contactNumber, DateTime requestedDate, int numberOfPeople, string? additionalInformation, bool responseSent)
    {
        TourId = tourId;
        Name = name;
        Email = email;
        ContactNumber = contactNumber;
        RequestedDate = requestedDate;
        NumberOfPeople = numberOfPeople;
        AdditionalInformation = additionalInformation;
        ResponseSent = responseSent;
    }

    public TourEnquiry Update(DefaultIdType? tourId, string? name, string? email, string? contactNumber, DateTime? requestedDate, int? numberOfPeople, string? additionalInformation, bool? responseSent)
    {
        if (name is not null && Name?.Equals(name) is not true) Name = name;
        if (email is not null && Email?.Equals(email) is not true) Email = email;
        if (tourId.HasValue && TourId != tourId) TourId = tourId.Value;
        if (requestedDate.HasValue && !RequestedDate.Equals(requestedDate.Value)) RequestedDate = requestedDate.Value;
        if (numberOfPeople.HasValue && !NumberOfPeople.Equals(numberOfPeople.Value)) NumberOfPeople = numberOfPeople.Value;
        if (additionalInformation is not null && AdditionalInformation?.Equals(additionalInformation) is not true) AdditionalInformation = additionalInformation;
        if (contactNumber is not null && ContactNumber?.Equals(contactNumber) is not true) ContactNumber = contactNumber;
        if (responseSent.HasValue && ResponseSent != responseSent.Value) ResponseSent = responseSent.Value;

        return this;
    }
}