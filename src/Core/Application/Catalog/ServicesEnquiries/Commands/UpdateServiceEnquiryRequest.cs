using Travaloud.Domain.Catalog.Enquiries;

namespace Travaloud.Application.Catalog.ServicesEnquiries.Commands;

public class UpdateServiceEnquiryRequest : IRequest<DefaultIdType>
{
    public DefaultIdType Id { get; set; }
    public DefaultIdType ServiceId { get; set; }
    public bool ResponseSent { get; set; }
    public IList<UpdateServiceEnquiryFieldRequest>? Fields { get; set; }
}

public class UpdateServiceEnquiryRequestHandler : IRequestHandler<UpdateServiceEnquiryRequest, DefaultIdType>
{
    private readonly IRepositoryFactory<ServiceEnquiry> _repository;

    public UpdateServiceEnquiryRequestHandler(IRepositoryFactory<ServiceEnquiry> repository)
    {
        _repository = repository;
    }

    public async Task<DefaultIdType> Handle(UpdateServiceEnquiryRequest request, CancellationToken cancellationToken)
    {
        var enquiry = await _repository.GetByIdAsync(request.Id, cancellationToken);

        if (enquiry == null)
        {
            throw new NotFoundException("serviceEnquiry.notfound");
        }

        if (request.Fields?.Any() == true)
        {
            var serviceFields = new List<ServiceEnquiryField>();

            foreach (var field in request.Fields)
            {
                var matchedField = enquiry.Fields.FirstOrDefault(x => x.Id == field.Id);

                if (matchedField != null)
                {
                    matchedField.Update(field.Field, field.Value);
                }
                else
                {
                    matchedField = new ServiceEnquiryField(field.Field, field.Value);
                }

                serviceFields.Add(matchedField);
            }

            enquiry.Fields = serviceFields;
        }

        enquiry.Update(request.ServiceId, request.ResponseSent);

        await _repository.UpdateAsync(enquiry, cancellationToken);

        return request.Id;
    }
}