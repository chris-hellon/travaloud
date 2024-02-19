using Travaloud.Domain.Catalog.Enquiries;
using Travaloud.Domain.Common.Events;

namespace Travaloud.Application.Catalog.Enquiries.Commands;

public class DeleteGeneralEnquiryRequest : IRequest<DefaultIdType>
{
    public DeleteGeneralEnquiryRequest(DefaultIdType id)
    {
        Id = id;
    }

    public DefaultIdType Id { get; set; }
}

public class DeleteGeneralEnquiryRequestHandler : IRequestHandler<DeleteGeneralEnquiryRequest, DefaultIdType>
{
    private readonly IRepositoryFactory<GeneralEnquiry> _repository;
    private readonly IStringLocalizer<DeleteGeneralEnquiryRequestHandler> _localizer;

    public DeleteGeneralEnquiryRequestHandler(IRepositoryFactory<GeneralEnquiry> repository,
        IStringLocalizer<DeleteGeneralEnquiryRequestHandler> localizer)
    {
        _repository = repository;
        _localizer = localizer;
    }

    public async Task<DefaultIdType> Handle(DeleteGeneralEnquiryRequest request, CancellationToken cancellationToken)
    {
        var enquiry = await _repository.GetByIdAsync(request.Id, cancellationToken);

        if (enquiry == null)
        {
            throw new NotFoundException(_localizer["generalEnquiry.notfound"]);
        }

        // Add Domain Events to be raised after the commit
        enquiry.DomainEvents.Add(EntityDeletedEvent.WithEntity(enquiry));

        await _repository.DeleteAsync(enquiry, cancellationToken);

        return request.Id;
    }
}