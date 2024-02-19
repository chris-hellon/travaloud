using Travaloud.Domain.Catalog.JobVacancies;
using Travaloud.Domain.Common.Events;

namespace Travaloud.Application.Catalog.JobVacancies.Commands;

public class DeleteJobVacancyRequest : IRequest<DefaultIdType>
{
    public DeleteJobVacancyRequest(DefaultIdType id)
    {
        Id = id;
    }

    public DefaultIdType Id { get; set; }
}

public class DeleteJobVacancyRequestHandler : IRequestHandler<DeleteJobVacancyRequest, DefaultIdType>
{
    private readonly IRepositoryFactory<JobVacancy> _repository;
    private readonly IStringLocalizer<DeleteJobVacancyRequestHandler> _localizer;

    public DeleteJobVacancyRequestHandler(IRepositoryFactory<JobVacancy> repository,
        IStringLocalizer<DeleteJobVacancyRequestHandler> localizer)
    {
        _repository = repository;
        _localizer = localizer;
    }

    public async Task<DefaultIdType> Handle(DeleteJobVacancyRequest request, CancellationToken cancellationToken)
    {
        var booking = await _repository.GetByIdAsync(request.Id, cancellationToken);

        _ = booking ?? throw new NotFoundException(_localizer["jobvacancy.notfound"]);

        // Add Domain Events to be raised after the commit
        booking.DomainEvents.Add(EntityDeletedEvent.WithEntity(booking));

        await _repository.DeleteAsync(booking, cancellationToken);

        return request.Id;
    }
}