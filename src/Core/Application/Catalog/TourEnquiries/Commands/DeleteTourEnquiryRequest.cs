using Travaloud.Domain.Catalog.Enquiries;
using Travaloud.Domain.Common.Events;

namespace Travaloud.Application.Catalog.TourEnquiries.Commands;

public class DeleteTourEnquiryRequest : IRequest<DefaultIdType>
{
    public DeleteTourEnquiryRequest(DefaultIdType id)
    {
        Id = id;
    }

    public DefaultIdType Id { get; set; }
}

public class DeleteTourEnquiryRequestHandler : IRequestHandler<DeleteTourEnquiryRequest, DefaultIdType>
{
    private readonly IRepositoryFactory<TourEnquiry> _repository;
    private readonly IStringLocalizer<DeleteTourEnquiryRequestHandler> _localizer;

    public DeleteTourEnquiryRequestHandler(IRepositoryFactory<TourEnquiry> repository,
        IStringLocalizer<DeleteTourEnquiryRequestHandler> localizer)
    {
        _repository = repository;
        _localizer = localizer;
    }

    public async Task<DefaultIdType> Handle(DeleteTourEnquiryRequest request, CancellationToken cancellationToken)
    {
        var enquiry = await _repository.GetByIdAsync(request.Id, cancellationToken);

        if (enquiry == null)
        {
            throw new NotFoundException(_localizer["tourenquiry.notfound"]);
        }

        // Add Domain Events to be raised after the commit
        enquiry.DomainEvents.Add(EntityDeletedEvent.WithEntity(enquiry));

        await _repository.DeleteAsync(enquiry, cancellationToken);

        return request.Id;
    }
}