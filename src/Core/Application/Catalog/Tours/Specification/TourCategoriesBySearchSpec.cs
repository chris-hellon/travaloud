using Travaloud.Application.Catalog.Tours.Dto;
using Travaloud.Application.Catalog.Tours.Queries;
using Travaloud.Domain.Catalog.Tours;

namespace Travaloud.Application.Catalog.Tours.Specification;

public class TourCategoriesBySearchSpec : EntitiesByPaginationFilterSpec<TourCategory, TourCategoryDto>
{
    public TourCategoriesBySearchSpec(SearchTourCategoriesRequest request)
        : base(request)
    {
        if (request.IsTopLevel.HasValue)
        {
            if (!string.IsNullOrEmpty(request.Name))
            {
                Query
                .OrderBy(c => c.Name, !request.HasOrderBy())
                .Where(p => !request.IsTopLevel.Value ? p.TopLevelCategory == null || p.TopLevelCategory == false : p.TopLevelCategory == request.IsTopLevel.Value && p.Name.Equals(request.Name), request.Name != null);
            }
            else
            {
                Query
                .OrderBy(c => c.Name, !request.HasOrderBy())
                .Where(p => !request.IsTopLevel.Value ? p.TopLevelCategory == null || p.TopLevelCategory == false : p.TopLevelCategory == request.IsTopLevel.Value);
            }
        }
        else
        {
            Query
           .OrderBy(c => c.Name, !request.HasOrderBy())
           .Where(p => p.Name.Equals(request.Name), request.Name != null);
        }
    }
}