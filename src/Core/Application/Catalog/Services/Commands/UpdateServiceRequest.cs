using Travaloud.Application.Catalog.Services.Specification;
using Travaloud.Domain.Catalog.Services;

namespace Travaloud.Application.Catalog.Services.Commands;

public class UpdateServiceRequest : IRequest<DefaultIdType>
{
    public DefaultIdType Id { get; set; }
    public string? Title { get; set; }
    public string? SubTitle { get; set; }
    public string? Description { get; set; }
    public string? ShortDescription { get; set; }
    public string? BodyHtml { get; set; }
    public string? IconClass { get; set; }

    public IList<UpdateServiceFieldRequest>? ServiceFields { get; set; }
}

public class UpdateServiceRequestHandler : IRequestHandler<UpdateServiceRequest, DefaultIdType>
{
    private readonly IRepositoryFactory<Service> _repository;

    public UpdateServiceRequestHandler(IRepositoryFactory<Service> repository)
    {
        _repository = repository;
    }

    public async Task<DefaultIdType> Handle(UpdateServiceRequest request, CancellationToken cancellationToken)
    {
        var service = await _repository.SingleOrDefaultAsync(new ServiceByIdSpec(request.Id), cancellationToken);

        if (service == null)
        {
            throw new NotFoundException("service.notfound");
        }

        var lastService = await _repository.SingleOrDefaultAsync(new ServiceBySortOrderSpec(), cancellationToken);

        if (lastService != null)
        {
            if (lastService.SortOrder.HasValue)
            {
                service.SortOrder = lastService.SortOrder + 1;
            }
            else
            {
                service.SortOrder = 0;
            }
        }

        service.Update(request.Title, request.SubTitle, request.Description, request.ShortDescription, request.BodyHtml, request.IconClass);

        if (request.ServiceFields?.Any() == true)
        {
            var serviceFields = new List<ServiceField>();
            foreach (var fieldRequest in request.ServiceFields)
            {
                var field = service.ServiceFields?.FirstOrDefault(f => f.Id == fieldRequest.Id);

                if (field == null)
                {
                    serviceFields.Add(new ServiceField(fieldRequest.Label, fieldRequest.FieldType, fieldRequest.Width, fieldRequest.SelectOptions, fieldRequest.IsRequired));
                }
                else
                {
                    field.Update(fieldRequest.Label, fieldRequest.FieldType, fieldRequest.Width, fieldRequest.SelectOptions, fieldRequest.IsRequired);
                    serviceFields.Add(field);
                }
            }

            service.ServiceFields = serviceFields;
        }

        await _repository.UpdateAsync(service, cancellationToken);

        return service.Id;
    }
}