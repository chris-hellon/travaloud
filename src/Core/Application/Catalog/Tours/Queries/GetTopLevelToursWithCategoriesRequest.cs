using System.Data;
using Travaloud.Application.Catalog.Tours.Dto;

namespace Travaloud.Application.Catalog.Tours.Queries;

public class GetTopLevelToursWithCategoriesRequest : IRequest<IEnumerable<TourWithCategoryDto>>
{
    public string TenantId { get; set; }

    public GetTopLevelToursWithCategoriesRequest(string tenantId)
    {
        TenantId = tenantId;
    }
}

internal class GetTopLevelToursWithCategoriesRequestHandler : IRequestHandler<GetTopLevelToursWithCategoriesRequest, IEnumerable<TourWithCategoryDto>>
{
    private readonly IDapperRepository _repository;

    public GetTopLevelToursWithCategoriesRequestHandler(IDapperRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<TourWithCategoryDto>> Handle(GetTopLevelToursWithCategoriesRequest request, CancellationToken cancellationToken)
    {
        return await _repository.QueryAsync<TourWithCategoryDto>(
            sql: "GetTopLevelToursWithCategories",
            param: new
            {
                request.TenantId
            },
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken);
    }
}