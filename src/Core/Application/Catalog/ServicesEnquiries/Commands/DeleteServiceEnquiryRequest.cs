using Travaloud.Domain.Catalog.Enquiries;
using Travaloud.Domain.Common.Events;

namespace Travaloud.Application.Catalog.ServicesEnquiries.Commands;

public class DeleteServiceEnquiryRequest : IRequest<DefaultIdType>
{
    public DeleteServiceEnquiryRequest(DefaultIdType id)
    {
        Id = id;
    }

    public DefaultIdType Id { get; set; }
}

public class DeleteServiceEnquiryRequestHandler : IRequestHandler<DeleteServiceEnquiryRequest, DefaultIdType>
{
    private readonly IRepositoryFactory<ServiceEnquiry> _repository;
    private readonly IStringLocalizer<DeleteServiceEnquiryRequestHandler> _localizer;

    public DeleteServiceEnquiryRequestHandler(IRepositoryFactory<ServiceEnquiry> repository,
        IStringLocalizer<DeleteServiceEnquiryRequestHandler> localizer)
    {
        _repository = repository;
        _localizer = localizer;
    }

    public async Task<DefaultIdType> Handle(DeleteServiceEnquiryRequest request, CancellationToken cancellationToken)
    {
        var enquiry = await _repository.GetByIdAsync(request.Id, cancellationToken);

        if (enquiry == null)
        {
            throw new NotFoundException(_localizer["serviceenquiry.notfound"]);
        }

        // Add Domain Events to be raised after the commit
        enquiry.DomainEvents.Add(EntityDeletedEvent.WithEntity(enquiry));

        await _repository.DeleteAsync(enquiry, cancellationToken);

        return request.Id;
    }
}