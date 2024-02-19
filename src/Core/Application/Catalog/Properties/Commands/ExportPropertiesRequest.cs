using Travaloud.Application.Catalog.Properties.Specification;
using Travaloud.Application.Common.Exporters;
using Travaloud.Domain.Catalog.Properties;

namespace Travaloud.Application.Catalog.Properties.Commands;

public class ExportPropertiesRequest : BaseFilter, IRequest<Stream>
{
    public string? Name { get; set; }
}

public class ExportPropertiesRequestHandler : IRequestHandler<ExportPropertiesRequest, Stream>
{
    private readonly IRepositoryFactory<Property> _repository;
    private readonly IExcelWriter _excelWriter;

    public ExportPropertiesRequestHandler(IRepositoryFactory<Property> repository, IExcelWriter excelWriter)
    {
        _repository = repository;
        _excelWriter = excelWriter;
    }

    public async Task<Stream> Handle(ExportPropertiesRequest request, CancellationToken cancellationToken)
    {
        var spec = new ExportPropertiesSpec(request);

        var list = await _repository.ListAsync(spec, cancellationToken);

        return _excelWriter.WriteToStream(list);
    }
}