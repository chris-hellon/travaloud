using Travaloud.Domain.Catalog.Enquiries;
using Travaloud.Domain.Common.Events;

namespace Travaloud.Application.Catalog.TourEnquiries.Commands;

public class CreateTourEnquiryRequest : IRequest<DefaultIdType>
{
    public DefaultIdType TourId { get; set; }
    public string Name { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string ContactNumber { get; set; } = default!;
    public DateTime RequestedDate { get; set; } = default!;
    public int NumberOfPeople { get; set; } = default!;
    public string? AdditionalInformation { get; set; }
    public bool ResponseSent { get; set; }
}

public class CreateTourEnquiryRequestRequestHandler : IRequestHandler<CreateTourEnquiryRequest, DefaultIdType>
{
    private readonly IRepositoryFactory<TourEnquiry> _repository;

    public CreateTourEnquiryRequestRequestHandler(IRepositoryFactory<TourEnquiry> repository)
    {
        _repository = repository;
    }

    public async Task<DefaultIdType> Handle(CreateTourEnquiryRequest request, CancellationToken cancellationToken)
    {
        var tourEnquiry = new TourEnquiry(request.TourId, request.Name, request.Email, request.ContactNumber, request.RequestedDate, request.NumberOfPeople, request.AdditionalInformation, request.ResponseSent);

        // Add Domain Events to be raised after the commit
        tourEnquiry.DomainEvents.Add(EntityCreatedEvent.WithEntity(tourEnquiry));

        await _repository.AddAsync(tourEnquiry, cancellationToken);

        return tourEnquiry.Id;
    }
}