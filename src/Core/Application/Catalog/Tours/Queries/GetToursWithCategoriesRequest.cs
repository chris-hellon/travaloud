using System.Data;
using Travaloud.Application.Catalog.Tours.Dto;

namespace Travaloud.Application.Catalog.Tours.Queries;

public class GetToursWithCategoriesRequest : IRequest<IEnumerable<TourWithCategoryDto>>
{
    public string TenantId { get; set; }

    public GetToursWithCategoriesRequest(string tenantId)
    {
        TenantId = tenantId;
    }
}

internal class GetToursWithCategoriesRequestHandler : IRequestHandler<GetToursWithCategoriesRequest, IEnumerable<TourWithCategoryDto>>
{
    private readonly IDapperRepository _repository;

    public GetToursWithCategoriesRequestHandler(IDapperRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<TourWithCategoryDto>> Handle(GetToursWithCategoriesRequest request, CancellationToken cancellationToken)
    {
        return await _repository.QueryAsync<TourWithCategoryDto>(
            sql: "GetToursWithCategories",
            param: new
            {
                request.TenantId
            },
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken);
    }
}