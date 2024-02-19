using Travaloud.Application.Catalog.Partners.Commands;
using Travaloud.Application.Catalog.Partners.Dto;
using Travaloud.Application.Catalog.Partners.Queries;

namespace Travaloud.Application.Catalog.Interfaces;

public interface IPartnersService : ITransientService
{
    Task<PaginationResponse<PartnerDto>> SearchAsync(SearchPartnersRequest request);

    Task<PartnerDetailsDto> GetAsync(DefaultIdType id);

    Task<DefaultIdType> CreateAsync(CreatePartnerRequest request);

    Task<DefaultIdType?> UpdateAsync(DefaultIdType id, UpdatePartnerRequest request);
    
    Task<DefaultIdType> DeleteAsync(DefaultIdType id);
}