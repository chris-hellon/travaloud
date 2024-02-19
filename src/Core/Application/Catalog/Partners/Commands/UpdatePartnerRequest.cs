using Travaloud.Application.Catalog.Partners.Specification;
using Travaloud.Domain.Catalog.Partners;

namespace Travaloud.Application.Catalog.Partners.Commands;

public class UpdatePartnerRequest : IRequest<DefaultIdType>
{
    public DefaultIdType Id { get; set; }
    public string Name { get; set; } = default!;
    public string? Description { get; set; }
    public string Address { get; set; } = default!;
    public string City { get; set; } = default!;
    public string PrimaryContactName { get; set; } = default!;
    public string PrimaryContactNumber { get; set; } = default!;
    public string PrimaryEmailAddress { get; set; } = default!;
    public IList<UpdatePartnerContactRequest>? PartnerContacts { get; set; }
}

public class UpdatePartnerRequestHandler : IRequestHandler<UpdatePartnerRequest, DefaultIdType>
{
    private readonly IRepositoryFactory<Partner> _repository;

    public UpdatePartnerRequestHandler(IRepositoryFactory<Partner> repository)
    {
        _repository = repository;
    }

    public async Task<DefaultIdType> Handle(UpdatePartnerRequest request, CancellationToken cancellationToken)
    {
        var partner = await _repository.SingleOrDefaultAsync(new PartnerByIdSpec(request.Id), cancellationToken);

        if (partner == null)
        {
            throw new NotFoundException("partner.notfound");
        }

        partner.Update(
            name: request.Name,
            description: request.Description,
            address: request.Address,
            city: request.City,
            primaryContactName: request.PrimaryContactName,
            primaryContactNumber: request.PrimaryContactNumber,
            primaryEmailAddress: request.PrimaryEmailAddress);

        if (request.PartnerContacts?.Any() == true)
        {
            var partnerContacts = new List<PartnerContact>();
            foreach (var contactRequest in request.PartnerContacts)
            {
                var contact = partner.PartnerContacts?.FirstOrDefault(c => c.Id == contactRequest.Id);

                if (contact == null)
                {
                    partnerContacts.Add(new PartnerContact(contactRequest.Name, contactRequest.Description, contactRequest.Address, contactRequest.City, contactRequest.ContactNumber, contactRequest.EmailAddress));
                }
                else
                {
                    contact.Update(
                        name: contactRequest.Name,
                        description: contactRequest.Description,
                        address: contactRequest.Address,
                        city: contactRequest.City,
                        contactNumber: contactRequest.ContactNumber,
                        emailAddress: contactRequest.EmailAddress);

                    partnerContacts.Add(contact);
                }
            }

            partner.PartnerContacts = partnerContacts;
        }

        await _repository.UpdateAsync(partner, cancellationToken);

        return partner.Id;
    }
}