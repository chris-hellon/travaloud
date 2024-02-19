namespace Travaloud.Domain.Catalog.Enquiries;

public class GeneralEnquiry : AuditableEntity<DefaultIdType>, IAggregateRoot
{
    public string Name { get; private set; } = default!;
    public string Email { get; private set; } = default!;
    public string ContactNumber { get; private set; } = default!;
    public string Message { get; private set; } = default!;
    public bool ResponseSent { get; private set; }

    public GeneralEnquiry(string name, string email, string contactNumber, string message, bool responseSent)
    {
        Name = name;
        Email = email;
        ContactNumber = contactNumber;
        Message = message;
        ResponseSent = responseSent;
    }

    public GeneralEnquiry Update(string? name, string? email, string? contactNumber, string? message, bool? responseSent)
    {
        if (name is not null && Name?.Equals(name) is not true) Name = name;
        if (email is not null && Email?.Equals(email) is not true) Email = email;
        if (contactNumber is not null && ContactNumber?.Equals(contactNumber) is not true) ContactNumber = contactNumber;
        if (message is not null && Message?.Equals(message) is not true) Message = message;
        if (responseSent.HasValue && ResponseSent != responseSent.Value) ResponseSent = responseSent.Value;

        return this;
    }
}