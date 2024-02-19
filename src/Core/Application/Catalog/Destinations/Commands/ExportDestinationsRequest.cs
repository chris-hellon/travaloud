using Travaloud.Application.Catalog.Destinations.Dto;
using Travaloud.Application.Common.Exporters;
using Travaloud.Domain.Catalog.Destinations;

namespace Travaloud.Application.Catalog.Destinations.Commands;

public class ExportDestinationsRequest : BaseFilter, IRequest<Stream>
{
    public string? Name { get; set; }
}

public class ExportDestinationsRequestHandler : IRequestHandler<ExportDestinationsRequest, Stream>
{
    private readonly IRepositoryFactory<Destination> _repository;
    private readonly IExcelWriter _excelWriter;

    public ExportDestinationsRequestHandler(IRepositoryFactory<Destination> repository, IExcelWriter excelWriter)
    {
        _repository = repository;
        _excelWriter = excelWriter;
    }

    public async Task<Stream> Handle(ExportDestinationsRequest request, CancellationToken cancellationToken)
    {
        var spec = new ExportProductsWithBrandsSpecification(request);

        var list = await _repository.ListAsync(spec, cancellationToken);

        return _excelWriter.WriteToStream(list);
    }
}

public class ExportProductsWithBrandsSpecification : EntitiesByBaseFilterSpec<Destination, DestinationExportDto>
{
    public ExportProductsWithBrandsSpecification(ExportDestinationsRequest request)
        : base(request) =>
        Query.Where(d => d.Name == request!.Name);
}