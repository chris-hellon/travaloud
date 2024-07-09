using System.Data;

namespace Travaloud.Application.Identity.Users;

public class SearchStaffByDapperRequest : IRequest<IEnumerable<UserDetailsDto>>
{
    public string Search { get; set; }
    public string TenantId { get; set; }
}

internal class SearchStaffByDapperRequestHandler : IRequestHandler<SearchStaffByDapperRequest, IEnumerable<UserDetailsDto>>
{
    private readonly IDapperRepository _repository;

    public SearchStaffByDapperRequestHandler(IDapperRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<UserDetailsDto>> Handle(SearchStaffByDapperRequest request, CancellationToken cancellationToken)
    {
        var users = await _repository.QueryAsync<UserDetailsDto>(
            sql: "SearchStaff",
            param: new
            {
                request.TenantId,
                request.Search
            },
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken);
        
        return users;
    }
}