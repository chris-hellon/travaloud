using Travaloud.Application.Catalog.PageModals.Commands;
using Travaloud.Domain.Catalog.Pages;
using Travaloud.Domain.Common.Events;

namespace Travaloud.Application.Catalog.Extensions;

public static class PageModalsExtensions
{
    public static async Task ProcessPageModalLookups(this PageModal pageModal, IRepositoryFactory<PageModalLookup> lookupRepository, IEnumerable<PageModalPageRequest>? pages)
    {
        if (pages?.Count() > 0)
        {
            var updatedPages = pages.Select(page =>
                new PageModalLookup(page.Id, page.PageId, pageModal.Id)).ToList();

            // Remove products that exist in the database but not in the request
            var pagesToRemove = pageModal.PageModalLookups?
                .Where(existingPage => updatedPages.All(newPage => newPage.Id != existingPage.Id))
                .ToList();

            if (pagesToRemove != null && pagesToRemove.Count != 0)
            {
                foreach (var page in pagesToRemove)
                {
                    page.DomainEvents.Add(EntityDeletedEvent.WithEntity(page));
                    pageModal.PageModalLookups?.Remove(page);
                }
                
                await lookupRepository.DeleteRangeAsync(pagesToRemove);

                // if (pageModal.PageModalLookups != null)
                //     pageModal.PageModalLookups = pageModal.PageModalLookups.Except(pagesToRemove).ToList();
            }

            if (updatedPages.Count != 0)
            {
                foreach (var page in updatedPages.Where(page => pageModal.PageModalLookups != null && pageModal.PageModalLookups.All(x => x.PageId != page.PageId)))
                {
                    pageModal.PageModalLookups?.Add(page);
                }
            }
            
            if ( pageModal.PageModalLookups != null && pageModal.PageModalLookups.Any()) return;
            {
                var newProducts = pages.Select(page => new PageModalLookup(page.Id, page.PageId, pageModal.Id)).ToList();
                pageModal.PageModalLookups = newProducts;
            }
        }
        else
            pageModal.PageModalLookups = Array.Empty<PageModalLookup>();
    }
}