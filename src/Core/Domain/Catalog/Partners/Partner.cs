namespace Travaloud.Domain.Catalog.Partners;

public class Partner : AuditableEntity, IAggregateRoot
{
    public Partner(string name,
        string? description,
        string address,
        string city,
        string primaryContactName,
        string primaryContactNumber,
        string primaryEmailAddress)
    {
        Name = name;
        Description = description;
        Address = address;
        City = city;
        PrimaryContactName = primaryContactName;
        PrimaryContactNumber = primaryContactNumber;
        PrimaryEmailAddress = primaryEmailAddress;
    }

    public string Name { get; private set; }
    public string? Description { get; private set; }
    public string Address { get; private set; }
    public string City { get; private set; }
    public string PrimaryContactName { get; private set; }
    public string PrimaryContactNumber { get; private set; }
    public string PrimaryEmailAddress { get; private set; }

    public virtual IList<PartnerContact> PartnerContacts { get; set; } = default!;

    public Partner Update(string? name, string? description, string? address, string? city, string? primaryContactName, string? primaryContactNumber, string? primaryEmailAddress)
    {
        if (name is not null && Name?.Equals(name) is not true) Name = name;
        if (description is not null && Description?.Equals(description) is not true) Description = description;
        if (address is not null && Address?.Equals(address) is not true) Address = address;
        if (city is not null && City?.Equals(city) is not true) City = city;
        if (primaryContactName is not null && PrimaryContactName?.Equals(primaryContactName) is not true) PrimaryContactName = primaryContactName;
        if (primaryContactNumber is not null && PrimaryContactNumber?.Equals(primaryContactNumber) is not true) PrimaryContactNumber = primaryContactNumber;
        if (primaryEmailAddress is not null && PrimaryEmailAddress?.Equals(primaryEmailAddress) is not true) PrimaryEmailAddress = primaryEmailAddress;

        return this;
    }
}