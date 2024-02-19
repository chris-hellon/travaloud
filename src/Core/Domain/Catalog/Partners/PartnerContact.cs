namespace Travaloud.Domain.Catalog.Partners;

public class PartnerContact : AuditableEntity, IAggregateRoot
{
    public PartnerContact(string name,
        string? description,
        string? address,
        string? city,
        string contactNumber,
        string emailAddress)
    {
        Name = name;
        Description = description;
        Address = address;
        City = city;
        ContactNumber = contactNumber;
        EmailAddress = emailAddress;
    }

    public string Name { get; private set; }
    public string? Description { get; private set; }
    public string? Address { get; private set; }
    public string? City { get; private set; }
    public string ContactNumber { get; private set; }
    public string EmailAddress { get; private set; }

    public PartnerContact Update(string? name, string? description, string? address, string? city, string? contactNumber, string? emailAddress)
    {
        if (name is not null && Name?.Equals(name) is not true) Name = name;
        if (description is not null && Description?.Equals(description) is not true) Description = description;
        if (address is not null && Address?.Equals(address) is not true) Address = address;
        if (city is not null && City?.Equals(city) is not true) City = city;
        if (contactNumber is not null && ContactNumber?.Equals(contactNumber) is not true) ContactNumber = contactNumber;
        if (emailAddress is not null && EmailAddress?.Equals(EmailAddress) is not true) EmailAddress = emailAddress;

        return this;
    }
}