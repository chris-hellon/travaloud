using Travaloud.Application.Catalog.Tours.Dto;
using Travaloud.Application.Common.Exporters;
using Travaloud.Domain.Catalog.Tours;

namespace Travaloud.Application.Catalog.Tours.Commands;

public class ExportToursRequest : BaseFilter, IRequest<Stream>
{
    public string? Name { get; set; }
}

public class ExportToursRequestHandler : IRequestHandler<ExportToursRequest, Stream>
{
    private readonly IExcelWriter _excelWriter;
    private readonly IRepositoryFactory<Tour> _repository;

    public ExportToursRequestHandler(IRepositoryFactory<Tour> repository, IExcelWriter excelWriter)
    {
        _repository = repository;
        _excelWriter = excelWriter;
    }

    public async Task<Stream> Handle(ExportToursRequest request, CancellationToken cancellationToken)
    {
        var spec = new ExportToursSpecification(request);

        var list = await _repository.ListAsync(spec, cancellationToken);

        return _excelWriter.WriteToStream(list);
    }
}

public class ExportToursSpecification : EntitiesByBaseFilterSpec<Tour, TourExportDto>
{
    public ExportToursSpecification(ExportToursRequest request)
        : base(request) =>
        Query.Where(d => d.Name == request!.Name);
}