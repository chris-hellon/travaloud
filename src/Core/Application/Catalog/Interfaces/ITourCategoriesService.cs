using Travaloud.Application.Catalog.Tours.Commands;
using Travaloud.Application.Catalog.Tours.Dto;
using Travaloud.Application.Catalog.Tours.Queries;

namespace Travaloud.Application.Catalog.Interfaces;

public interface ITourCategoriesService : ITransientService
{
    /// <summary>
    /// Search tour categories using available filters.
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    Task<PaginationResponse<TourCategoryDto>> SearchAsync(SearchTourCategoriesRequest request);

    /// <summary>
    /// Get tour category details.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<TourCategoryDto> GetAsync(DefaultIdType id);
    
    /// <summary>
    /// Create a new category.
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    Task<DefaultIdType> CreateAsync(CreateTourCategoryRequest request);

    /// <summary>
    /// Update a tour category.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="request"></param>
    /// <returns></returns>
    Task<DefaultIdType?> UpdateAsync(DefaultIdType id, UpdateTourCategoryRequest request);

    /// <summary>
    /// Delete a tour.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<DefaultIdType> DeleteAsync(DefaultIdType id);
}