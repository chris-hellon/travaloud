using System.Data;
using Travaloud.Application.Catalog.Bookings.Specification;
using Travaloud.Domain.Catalog.Bookings;
using Travaloud.Domain.Catalog.Tours;

namespace Travaloud.Application.Dashboard;

public class GetStatsRequest : IRequest<StatsDto>
{
    public string TenantId { get; set; }

    public GetStatsRequest(string tenantId)
    {
        TenantId = tenantId;
    }
}

public class GetStatsRequestHandler : IRequestHandler<GetStatsRequest, StatsDto>
{
    private readonly IDapperRepository _dapperRepository;

    public GetStatsRequestHandler(IDapperRepository dapperRepository)
    {
        _dapperRepository = dapperRepository;
    }

    public async Task<StatsDto> Handle(GetStatsRequest request, CancellationToken cancellationToken)
    {
        return await _dapperRepository.QuerySingleAsync<StatsDto>("GetDashboard", new
        {
            TenantId = request.TenantId
        }, commandType: CommandType.StoredProcedure, cancellationToken: cancellationToken);
    }
}