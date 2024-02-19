using Travaloud.Domain.Catalog.Partners;
using Travaloud.Domain.Common.Events;

namespace Travaloud.Application.Catalog.Partners.Commands;

public class CreatePartnerRequest : IRequest<DefaultIdType>
{
    public string Name { get; set; } = default!;
    public string? Description { get; set; }
    public string Address { get; set; } = default!;
    public string City { get; set; } = default!;
    public string PrimaryContactName { get; set; } = default!;
    public string PrimaryContactNumber { get; set; } = default!;
    public string PrimaryEmailAddress { get; set; } = default!;
    public IList<CreatePartnerContactRequest> PartnerContacts { get; set; } = default!;
}

public class CreatePartnerRequestHandler : IRequestHandler<CreatePartnerRequest, DefaultIdType>
{
    private readonly IRepositoryFactory<Partner> _repository;

    public CreatePartnerRequestHandler(IRepositoryFactory<Partner> repository)
    {
        _repository = repository;
    }

    public async Task<DefaultIdType> Handle(CreatePartnerRequest request, CancellationToken cancellationToken)
    {
        var partner = new Partner(request.Name, request.Description, request.Address, request.City, request.PrimaryContactName, request.PrimaryContactNumber, request.PrimaryEmailAddress);

        if (request.PartnerContacts?.Any() == true)
        {
            var partnerContacts = request.PartnerContacts.Select(contactRequest => new PartnerContact(contactRequest.Name, contactRequest.Description, contactRequest.Address, contactRequest.City, contactRequest.ContactNumber, contactRequest.EmailAddress)).ToList();

            partner.PartnerContacts = partnerContacts;
        }

        // Add Domain Events to be raised after the commit
        partner.DomainEvents.Add(EntityCreatedEvent.WithEntity(partner));

        await _repository.AddAsync(partner, cancellationToken);

        return partner.Id;
    }
}