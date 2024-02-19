using Travaloud.Application.Catalog.PageModals.Commands;
using Travaloud.Application.Catalog.PageModals.Dto;
using Travaloud.Application.Catalog.PageModals.Queries;

namespace Travaloud.Application.Catalog.Interfaces;

public interface IPageModalsService : ITransientService
{
    Task<PaginationResponse<PageModalDto>> SearchAsync(SearchPageModalsRequest request);

    Task<PageModalDetailsDto> GetAsync(DefaultIdType id);

    Task<DefaultIdType> CreateAsync(CreatePageModalRequest request);

    Task<DefaultIdType?> UpdateAsync(DefaultIdType id, UpdatePageModalRequest request);

    Task<DefaultIdType> DeleteAsync(DefaultIdType id);
}