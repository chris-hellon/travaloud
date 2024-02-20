using Travaloud.Application.Catalog.Galleries.Commands;
using Travaloud.Application.Catalog.Galleries.Dto;
using Travaloud.Application.Catalog.Galleries.Queries;

namespace Travaloud.Application.Catalog.Interfaces;

public interface IGalleriesService : ITransientService
{
    Task<PaginationResponse<GalleryDto>> SearchAsync(SearchGalleriesRequest request);

    Task<GalleryDetailsDto> GetAsync(DefaultIdType id);

    Task<DefaultIdType> CreateAsync(CreateGalleryRequest request);

    Task<DefaultIdType?> UpdateAsync(DefaultIdType id, UpdateGalleryRequest request);

    Task<DefaultIdType> DeleteAsync(DefaultIdType id);
}