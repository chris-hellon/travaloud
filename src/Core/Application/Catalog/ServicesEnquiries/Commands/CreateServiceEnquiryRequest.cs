using Travaloud.Domain.Catalog.Enquiries;
using Travaloud.Domain.Common.Events;

namespace Travaloud.Application.Catalog.ServicesEnquiries.Commands;

public class CreateServiceEnquiryRequest : IRequest<DefaultIdType>
{
    public DefaultIdType ServiceId { get; set; }
    public bool ResponseSent { get; set; }
    public IList<CreateServiceEnquiryFieldRequest>? Fields { get; set; }
}

public class CreateServiceEnquiryRequestRequestHandler : IRequestHandler<CreateServiceEnquiryRequest, DefaultIdType>
{
    private readonly IRepositoryFactory<ServiceEnquiry> _repository;

    public CreateServiceEnquiryRequestRequestHandler(IRepositoryFactory<ServiceEnquiry> repository)
    {
        _repository = repository;
    }

    public async Task<DefaultIdType> Handle(CreateServiceEnquiryRequest request, CancellationToken cancellationToken)
    {
        var serviceEnquiry = new ServiceEnquiry(request.ServiceId, request.ResponseSent);

        if (request.Fields?.Any() == true)
        {
            var serviceFields = request.Fields.Select(field => new ServiceEnquiryField(field.Field, field.Value)).ToList();

            serviceEnquiry.Fields = serviceFields;
        }

        // Add Domain Events to be raised after the commit
        serviceEnquiry.DomainEvents.Add(EntityCreatedEvent.WithEntity(serviceEnquiry));

        await _repository.AddAsync(serviceEnquiry, cancellationToken);

        return serviceEnquiry.Id;
    }
}