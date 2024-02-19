using Travaloud.Domain.Catalog.Enquiries;

namespace Travaloud.Application.Catalog.Enquiries.Commands;

public class UpdateGeneralEnquiryRequest : IRequest<DefaultIdType>
{
    public DefaultIdType Id { get; set; }
    public string? Name { get; set; }
    public string? Email { get; set; }
    public string? ContactNumber { get; set; }
    public string? Message { get; set; }
    public bool? ResponseSent { get; set; }
}

public class UpdateGeneralEnquiryRequestHandler : IRequestHandler<UpdateGeneralEnquiryRequest, DefaultIdType>
{
    private readonly IRepositoryFactory<GeneralEnquiry> _repository;

    public UpdateGeneralEnquiryRequestHandler(IRepositoryFactory<GeneralEnquiry> repository)
    {
        _repository = repository;
    }

    public async Task<DefaultIdType> Handle(UpdateGeneralEnquiryRequest request, CancellationToken cancellationToken)
    {
        var enquiry = await _repository.GetByIdAsync(request.Id, cancellationToken);

        if (enquiry == null)
        {
            throw new NotFoundException("generalEnquiry.notfound");
        }

        enquiry.Update(
            request.Name,
            request.Email,
            request.ContactNumber,
            request.Message,
            request.ResponseSent);

        await _repository.UpdateAsync(enquiry, cancellationToken);

        return request.Id;
    }
}