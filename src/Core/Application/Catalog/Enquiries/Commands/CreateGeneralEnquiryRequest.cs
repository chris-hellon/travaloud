using Travaloud.Domain.Catalog.Enquiries;
using Travaloud.Domain.Common.Events;

namespace Travaloud.Application.Catalog.Enquiries.Commands;

public class CreateGeneralEnquiryRequest : IRequest<DefaultIdType>
{
    public string Name { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string ContactNumber { get; set; } = default!;
    public string Message { get; set; } = default!;
    public bool ResponseSent { get; set; }
}

public class CreateGeneralEnquiryRequestHandler : IRequestHandler<CreateGeneralEnquiryRequest, DefaultIdType>
{
    private readonly IRepositoryFactory<GeneralEnquiry> _repository;

    public CreateGeneralEnquiryRequestHandler(IRepositoryFactory<GeneralEnquiry> repository)
    {
        _repository = repository;
    }

    public async Task<DefaultIdType> Handle(CreateGeneralEnquiryRequest request, CancellationToken cancellationToken)
    {
        var generalEnquiry = new GeneralEnquiry(
            request.Name,
            request.Email,
            request.ContactNumber,
            request.Message,
            request.ResponseSent);

        // Add Domain Events to be raised after the commit
        generalEnquiry.DomainEvents.Add(EntityCreatedEvent.WithEntity(generalEnquiry));

        await _repository.AddAsync(generalEnquiry, cancellationToken);

        return generalEnquiry.Id;
    }
}