using MediatR;

namespace Travaloud.Infrastructure.Catalog.Services;

public class BaseService
{
    protected readonly ISender Mediator;

    public BaseService(ISender mediator)
    {
        Mediator = mediator;
    }
}