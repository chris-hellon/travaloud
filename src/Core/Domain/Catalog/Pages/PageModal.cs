namespace Travaloud.Domain.Catalog.Pages;

public class PageModal : AuditableEntity, IAggregateRoot
{
    public PageModal(string? title,
        string? description,
        string? imagePath,
        string? callToAction,
        DateTime? startDate,
        DateTime? endDate)
    {
        Title = title;
        Description = description;
        ImagePath = imagePath;
        CallToAction = callToAction;

        if (!startDate.HasValue || !endDate.HasValue) return;
        
        var startDateWithTime = startDate.Value.Date + StartTimeSpan;
        var endDateWithTime = endDate.Value.Date + EndTimeSpan;
            
        StartDate = startDateWithTime;
        EndDate = endDateWithTime;
    }

    public string? Title { get; private set; }
    public string? Description { get; private set; }
    public string? ImagePath { get; private set; }
    public string? CallToAction { get; private set; }
    public DateTime? StartDate { get; private set; }
    public DateTime? EndDate { get; private set; }
    private static TimeSpan StartTimeSpan => new(00, 00, 00);
    private static TimeSpan EndTimeSpan => new(23, 59, 00);

    public virtual IList<PageModalLookup>? PageModalLookups { get; set; }

    public PageModal Update(string? title, string? description, string? imagePath, string? callToAction, DateTime? startDate, DateTime? endDate)
    {
        if (title is not null && Title?.Equals(title) is not true) Title = title;
        if (description is not null && Description?.Equals(description) is not true) Description = description;
        if (imagePath is not null && ImagePath?.Equals(imagePath) is not true) ImagePath = !imagePath.Contains("w-1000") ? $"{imagePath}?w=1000" : imagePath;
        if (callToAction is not null && CallToAction?.Equals(callToAction) is not true) CallToAction = callToAction;

        if (startDate is not null)
        {
            var startDateWithTime = startDate.Value.Date + StartTimeSpan;
            if (StartDate?.Equals(startDateWithTime) is not true)
            {
                StartDate = startDateWithTime;
            }
        }

        if (endDate is null) return this;
        
        var endDateWithTime = endDate.Value.Date + EndTimeSpan;
        if (EndDate?.Equals(endDateWithTime) is not true)
        {
            EndDate = endDateWithTime;
        }

        return this;
    }

    // public void ProcessPageModalLookups(List<DefaultIdType>? pages)
    // {
    //     // if (pages == null)
    //     // {
    //     //     PageModalLookups = new List<PageModalLookup>();
    //     //     return;
    //     // }
    //     //
    //     // var pageModalLookupsToRemove = PageModalLookups?.Where(p => !pages.Contains(p.PageId)).ToList();
    //     //
    //     // if (pageModalLookupsToRemove == null || pageModalLookupsToRemove.Count == 0) return;
    //     //
    //     // foreach (var pageModalLookupToRemove in pageModalLookupsToRemove)
    //     // {
    //     //     pageModalLookupToRemove.DomainEvents.Add(EntityDeletedEvent.WithEntity(pageModalLookupToRemove));
    //     // }
    //     
    //     if (products?.Count > 0)
    //     {
    //         var updatedProducts = products.Select(product =>
    //             new SupplierProduct(product.Id, product.Price, supplier.Id, product.ProductId)).ToList();
    //
    //         // Remove products that exist in the database but not in the request
    //         var productsToRemove = supplier.Products?
    //             .Where(existingProduct => updatedProducts.All(newProduct => newProduct.Id != existingProduct.Id))
    //             .ToList();
    //
    //         if (productsToRemove != null && productsToRemove.Count != 0)
    //         {
    //             foreach (var product in productsToRemove)
    //             {
    //                 product.DomainEvents.Add(EntityDeletedEvent.WithEntity(product));
    //             }
    //
    //             supplier.Products = supplier.Products.Except(productsToRemove);
    //         }
    //             
    //         var productsToUpdate = supplier.Products?
    //             .Where(existingProduct =>
    //                 updatedProducts.All(matchedProduct => matchedProduct.Id == existingProduct.Id))
    //             .ToList();
    //
    //         if (productsToUpdate is {Count: <= 0})
    //         {
    //             foreach (var productToUpdate in productsToUpdate)
    //             {
    //                 var matchedProduct = products.SingleOrDefault(x => x.Id == productToUpdate.Id);
    //
    //                 if (matchedProduct == null) continue;
    //                 
    //                 productToUpdate.Update(matchedProduct.Price);
    //                     
    //                 productToUpdate.DomainEvents.Add(EntityUpdatedEvent.WithEntity(productToUpdate));
    //             }
    //         }
    //         
    //         if (supplier.Products != null && supplier.Products.Any()) return;
    //         {
    //             var newProducts = products.Select(product =>  new SupplierProduct(product.Id, product.Price, supplier.Id, product.ProductId)).ToList();
    //             supplier.Products = newProducts;
    //         }
    //     }
    //     else
    //         supplier.Products = Array.Empty<SupplierProduct>();
    // }

    public PageModal ClearImagePath()
    {
        ImagePath = string.Empty;
        return this;
    }
}