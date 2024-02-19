using Travaloud.Domain.Catalog.Services;
using Travaloud.Domain.Common.Events;

namespace Travaloud.Application.Catalog.Services.Commands;

public class CreateServiceRequest : IRequest<DefaultIdType>
{
    public string Title { get; set; } = default!;
    public string Description { get; set; } = default!;
    public string? SubTitle { get; set; }
    public string? ShortDescription { get; set; }
    public string? BodyHtml { get; set; }
    public string? IconClass { get; set; }

    public IList<CreateServiceFieldRequest>? ServiceFields { get; set; }
}

public class CreateServiceRequestHandler : IRequestHandler<CreateServiceRequest, DefaultIdType>
{
    private readonly IRepositoryFactory<Service> _repository;

    public CreateServiceRequestHandler(IRepositoryFactory<Service> repository)
    {
        _repository = repository;
    }

    public async Task<DefaultIdType> Handle(CreateServiceRequest request, CancellationToken cancellationToken)
    {
        var service = new Service(
            request.Title,
            request.SubTitle,
            request.Description,
            request.ShortDescription,
            request.BodyHtml,
            request.IconClass);

        if (request.ServiceFields?.Any() == true)
        {
            var serviceFields = request.ServiceFields.Select(field => new ServiceField(field.Label, field.FieldType, field.Width, field.SelectOptions, field.IsRequired)).ToList();

            service.ServiceFields = serviceFields;
        }

        // Add Domain Events to be raised after the commit
        service.DomainEvents.Add(EntityCreatedEvent.WithEntity(service));

        await _repository.AddAsync(service, cancellationToken);

        return service.Id;
    }
}