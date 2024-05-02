namespace Travaloud.Application.Identity.Users;

public class UsersByIdsRequest : BaseFilter
{
    public List<string> UserIds { get; set; }

    public UsersByIdsRequest(List<string> userIds)
    {
        UserIds = userIds;
    }
}