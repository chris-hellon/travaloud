using System.Data;

namespace Travaloud.Application.Identity.Users;

public class SearchByDapperRequest : PaginationFilter, IRequest<PaginationResponse<UserDetailsDto>>
{
    public string? Role { get; set; }
    public string TenantId { get; set; }
}

internal class SearchByDapperRequestHandler : IRequestHandler<SearchByDapperRequest, PaginationResponse<UserDetailsDto>>
{
    private readonly IDapperRepository _repository;

    public SearchByDapperRequestHandler(IDapperRepository repository)
    {
        _repository = repository;
    }

    public async Task<PaginationResponse<UserDetailsDto>> Handle(SearchByDapperRequest request, CancellationToken cancellationToken)
    {
        var users = await _repository.QueryAsync<UserDetailsDto>(
            sql: "SearchUsers",
            param: new
            {
                request.TenantId,
                request.Role,
                Search = request.Keyword,
                request.PageNumber,
                request.PageSize,
                OrderBy = request.OrderBy != null ? request.OrderBy.First() : "FirstName Ascending"
            },
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken);

        var usersList = users.ToList();
        
        if (users.Any())
            return new PaginationResponse<UserDetailsDto>(usersList, users.First().TotalUsers,
                request.PageNumber, request.PageSize);

        return new PaginationResponse<UserDetailsDto>(usersList, 0, request.PageNumber, request.PageSize);
    }
}