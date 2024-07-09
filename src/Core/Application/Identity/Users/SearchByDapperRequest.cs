using System.Data;
using Dapper;

namespace Travaloud.Application.Identity.Users;

public class SearchByDapperRequest : PaginationFilter, IRequest<PaginationResponse<UserDetailsDto>>
{
    public string? Role { get; set; }
    public string TenantId { get; set; }
    public List<string>? UserIds { get; set; }
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
        var userIds = new DataTable();
        userIds.Columns.Add("Id");

        if (request.UserIds != null)
        {
            foreach (var userId in request.UserIds)
            {
                var dataRow = userIds.NewRow();
                dataRow["Id"] = userId;
                userIds.Rows.Add(dataRow);
            }
        }
        
        var users = await _repository.QueryAsync<UserDetailsDto>(
            sql: "SearchUsers",
            param: new
            {
                request.TenantId,
                request.Role,
                Search = request.Keyword,
                request.PageNumber,
                request.PageSize,
                OrderBy = request.OrderBy != null ? request.OrderBy.First() : "FirstName Ascending",
                //UserIds = userIds.AsTableValuedParameter()
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