using Travaloud.Domain.Catalog.Enquiries;

namespace Travaloud.Application.Catalog.TourEnquiries.Commands;

public class UpdateTourEnquiryRequest : IRequest<DefaultIdType>
{
    public DefaultIdType Id { get; set; }
    public DefaultIdType TourId { get; set; }
    public string Name { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string ContactNumber { get; set; } = default!;
    public DateTime RequestedDate { get; set; } = default!;
    public int NumberOfPeople { get; set; } = default!;
    public string? AdditionalInformation { get; set; }
    public bool ResponseSent { get; set; }
}

public class UpdateTourEnquiryRequestHandler : IRequestHandler<UpdateTourEnquiryRequest, DefaultIdType>
{
    private readonly IRepositoryFactory<TourEnquiry> _repository;

    public UpdateTourEnquiryRequestHandler(IRepositoryFactory<TourEnquiry> repository)
    {
        _repository = repository;
    }

    public async Task<DefaultIdType> Handle(UpdateTourEnquiryRequest request, CancellationToken cancellationToken)
    {
        var enquiry = await _repository.GetByIdAsync(request.Id, cancellationToken);

        if (enquiry == null)
        {
            throw new NotFoundException("generalEnquiry.notfound");
        }

        enquiry.Update(request.TourId, request.Name, request.Email, request.ContactNumber, request.RequestedDate, request.NumberOfPeople, request.AdditionalInformation, request.ResponseSent);

        await _repository.UpdateAsync(enquiry, cancellationToken);

        return request.Id;
    }
}