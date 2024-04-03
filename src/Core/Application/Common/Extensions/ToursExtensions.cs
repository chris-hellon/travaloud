using Travaloud.Application.Catalog.Tours.Commands;
using Travaloud.Domain.Catalog.Tours;
using Travaloud.Domain.Common.Events;

namespace Travaloud.Application.Common.Extensions;

public static class ToursExtensions
{
    public static void ProcessTourPricesAndDates(this Tour tour, IList<TourPriceRequest>? prices, IList<TourDateRequest>? dates, int availableSpaces, int? previousAvailableSpaces, DefaultIdType userId)
    {
        var tourPrices = new List<TourPrice>();
        var tourDates = new List<TourDate>();

        if (prices?.Any() == true || dates?.Any() == true)
        {
            if (prices != null)
                foreach (var tourPriceRequest in prices)
                {
                    var price = tour.TourPrices?.FirstOrDefault(tp => tp.Id == tourPriceRequest.Id);

                    if (price is null)
                    {
                        // Create a new TourPrice
                        price = new TourPrice(tourPriceRequest.Price ?? 0m,
                            tourPriceRequest.Title,
                            tourPriceRequest.Description,
                            tourPriceRequest.MonthFrom,
                            tourPriceRequest.MonthTo,
                            tourPriceRequest.DayDuration,
                            tourPriceRequest.NightDuration,
                            tourPriceRequest.HourDuration,
                            tourPriceRequest.Id);
                    }
                    else
                    {
                        // Update an existing TourPrice
                        price.Update(tourPriceRequest.Price,
                            tourPriceRequest.Title,
                            tourPriceRequest.Description,
                            tourPriceRequest.MonthFrom,
                            tourPriceRequest.MonthTo,
                            tourPriceRequest.DayDuration,
                            tourPriceRequest.NightDuration,
                            tourPriceRequest.HourDuration,
                            tour.Id);
                    }

                    tourPrices.Add(price);

                    var priceDates = dates != null ? dates.Where(x => x.TourPriceId == tourPriceRequest.Id) : Array.Empty<TourDateRequest>();
                    var tourDateRequests = priceDates as TourDateRequest[] ?? priceDates.ToArray();
                    
                    if (!tourDateRequests.Any()) continue;

                    foreach (var tourDateRequest in tourDateRequests)
                    {
                        var date = tour.TourDates?.FirstOrDefault(td => td.Id == tourDateRequest.Id);

                        if (tourDateRequest is not
                            {StartDate: not null, EndDate: not null, StartTime: not null}) continue;

                        tourDateRequest.StartDate = tourDateRequest.StartDate.Value.Date + tourDateRequest.StartTime.Value;
                        
                        if (tourDateRequest.EndTime != null)
                            tourDateRequest.EndDate =
                                tourDateRequest.EndDate.Value.Date + tourDateRequest.EndTime.Value;

                        if (date is null)
                        {
                            tourDateRequest.AvailableSpaces = availableSpaces;

                            if (tourDateRequest.AvailableSpaces.HasValue)
                            {
                                date = new TourDate(tourDateRequest.StartDate.Value, tourDateRequest.EndDate.Value,
                                    tourDateRequest.AvailableSpaces.Value, tourDateRequest.PriceOverride, tour.Id,
                                    tourDateRequest.TourPriceId, price);
                            }
                        }
                        else
                        {
                            tourDateRequest.AvailableSpaces = date.AvailableSpaces;

                            if (previousAvailableSpaces.HasValue && previousAvailableSpaces.Value != availableSpaces)
                            {
                                var dateAvailableSpaces = date.AvailableSpaces;
                                var previousAvailableDateSpacesDifference =
                                    previousAvailableSpaces.Value - dateAvailableSpaces;

                                if (previousAvailableDateSpacesDifference == 0)
                                {
                                    tourDateRequest.AvailableSpaces = availableSpaces;
                                }
                                else
                                {
                                    tourDateRequest.AvailableSpaces =
                                        availableSpaces - previousAvailableDateSpacesDifference;
                                }
                            }
                            else
                            {
                                tourDateRequest.AvailableSpaces = availableSpaces;
                            }

                            date.Update(tourDateRequest.StartDate, tourDateRequest.EndDate,
                                tourDateRequest.AvailableSpaces, tourDateRequest.PriceOverride, tour.Id,
                                tourDateRequest.TourPriceId);
                        }

                        if (date is not null)
                        {
                            tourDates.Add(date);
                        }
                    }
                }
        }

        var tourDatesToRemove = tour.TourDates?
            .Where(existingRoom => tourDates.All(newRoom => newRoom.Id != existingRoom.Id))
            .ToList();

        if (tourDatesToRemove != null && tourDatesToRemove.Count != 0)
        {
            foreach (var tourDate in tourDatesToRemove)
            {
                tourDate.DomainEvents.Add(EntityDeletedEvent.WithEntity(tourDate));
                tourDate.FlagAsDeleted(userId);
                tourDates.Add(tourDate);
            }
        }

        var tourPricesToRemove = tour.TourPrices?
            .Where(existingRoom => tourPrices.All(newRoom => newRoom.Id != existingRoom.Id))
            .ToList();

        if (tourPricesToRemove != null && tourPricesToRemove.Count != 0)
        {
            foreach (var tourPrice in tourPricesToRemove)
            {
                tourPrice.DomainEvents.Add(EntityDeletedEvent.WithEntity(tourPrice));
                tourPrice.FlagAsDeleted(userId);
                tourPrices.Add(tourPrice);
            }
        }

        tour.TourDates = tourDates;
        tour.TourPrices = tourPrices;
    }

    public static void ProcessTourCategories(this Tour tour, DefaultIdType? tourCategoryId, List<DefaultIdType>? selectedParentTourCategories, IList<TourCategoryLookupRequest>? request, DefaultIdType userId)
    {
        var tourCategoryLookups = new List<TourCategoryLookup>();

        if (tourCategoryId.HasValue)
        {
            tourCategoryLookups.Add(new TourCategoryLookup(tourCategoryId.Value, null));
        }

        if (selectedParentTourCategories != null)
        {
            foreach (var lookup in selectedParentTourCategories)
            {
                var existingTourCategory = tour.TourCategoryLookups?.FirstOrDefault(td => td.TourCategoryId == lookup);

                if (existingTourCategory is null)
                {
                    // Create a new TourItinerary
                    tourCategoryLookups.Add(new TourCategoryLookup(lookup, null, tour.Id));
                }
                else
                {
                    // Update an existing TourItinerary
                    existingTourCategory.Update(tour.Id, lookup, null);
                    tourCategoryLookups.Add(existingTourCategory);
                }
            }
        }
        else if (request?.Any() == true)
        {
            tourCategoryLookups.AddRange(request.Select(tourCategoryLookupRequest =>
                new TourCategoryLookup(tourCategoryLookupRequest.TourCategoryId,
                    tourCategoryLookupRequest.ParentTourCategoryId)));
        }

        var destinationsToRemove = tour.TourCategoryLookups?
            .Where(existingRoom => tourCategoryLookups.All(newRoom => newRoom.Id != existingRoom.Id))
            .ToList();

        if (destinationsToRemove != null && destinationsToRemove.Count != 0)
        {
            foreach (var destination in destinationsToRemove)
            {
                destination.DomainEvents.Add(EntityDeletedEvent.WithEntity(destination));
                destination.FlagAsDeleted(userId);
                tourCategoryLookups.Add(destination);
            }
        }

        tour.TourCategoryLookups = tourCategoryLookups;
    }

    public static void ProcessTourDestinations(this Tour tour, IEnumerable<TourDestinationLookupRequest>? request, DefaultIdType userId)
    {
        var tourDestinationLookups = new List<TourDestinationLookup>();

        if (request != null)
        {
            var tourDestinationLookupRequests = request as TourDestinationLookupRequest[] ?? request.ToArray();
            if (tourDestinationLookupRequests.Length != 0)
            {
                tourDestinationLookups.AddRange(tourDestinationLookupRequests.Select(tourDestinationLookupRequest =>
                    new TourDestinationLookup(tourDestinationLookupRequest.TourId,
                        tourDestinationLookupRequest.DestinationId)));
            }
        }

        var destinationsToRemove = tour.TourDestinationLookups?
            .Where(existingRoom => tourDestinationLookups.All(newDestination => newDestination.Id != existingRoom.Id))
            .ToList();

        if (destinationsToRemove != null && destinationsToRemove.Count != 0)
        {
            foreach (var destination in destinationsToRemove)
            {
                destination.DomainEvents.Add(EntityDeletedEvent.WithEntity(destination));
                destination.FlagAsDeleted(userId);
                tourDestinationLookups.Add(destination);
            }
        }

        tour.TourDestinationLookups = tourDestinationLookups;
    }

    
    public static async Task ProcessTourItineraries(this Tour tour, IList<TourItineraryRequest>? request, DefaultIdType userId, IFileStorageService file, CancellationToken cancellationToken)
    {
        var itineraries = new List<TourItinerary>();
        
        if (request?.Any() == true)
        {
            foreach (var itineraryRequest in request)
            {
                var itinerary = tour.TourItineraries?.FirstOrDefault(i => i.Id == itineraryRequest.Id);

                if (itinerary == null)
                {
                    List<TourItinerarySection> itinerarySections = [];

                    if (itineraryRequest.Sections?.Any() == true)
                    {
                        foreach (var section in itineraryRequest.Sections)
                        {
                            List<TourItinerarySectionImage> sectionImages = [];

                            if (section.Images?.Any() == true)
                            {
                                foreach (var image in section.Images)
                                {
                                    if (!string.IsNullOrEmpty(image.ImageInBytes))
                                    {
                                        image.Image = new FileUploadRequest() { Data = image.ImageInBytes, Extension = image.ImageExtension ?? string.Empty, Name = $"{section.Title}_{DefaultIdType.NewGuid():N}" };
                                        var imagePath = await file.UploadAsync<TourItinerarySectionImageRequest>(image.Image, FileType.Image, cancellationToken);

                                        sectionImages.Add(new TourItinerarySectionImage(imagePath, imagePath));
                                    }
                                    else
                                    {
                                        sectionImages.Add(new TourItinerarySectionImage(image.ImagePath, image.ThumbnailImagePath));
                                    }
                                }
                            }

                            itinerarySections.Add(new TourItinerarySection(section.Title, section.SubTitle, section.Description, section.Highlights, sectionImages));
                        }
                    }

                    itineraries.Add(new TourItinerary(itineraryRequest.Header, itineraryRequest.Title, itineraryRequest.Description, itinerarySections));
                }
                else
                {
                    // Update an existing TourItinerary
                    List<TourItinerarySection> itinerarySections = [];

                    if (itineraryRequest.Sections?.Any() == true)
                    {
                        foreach (var section in itineraryRequest.Sections)
                        {
                            List<TourItinerarySectionImage> sectionImages = new();

                            if (section.Images?.Any() == true)
                            {
                                foreach (var image in section.Images)
                                {
                                    if (!string.IsNullOrEmpty(image.ImageInBytes))
                                    {
                                        image.Image = new FileUploadRequest() { Data = image.ImageInBytes, Extension = image.ImageExtension ?? string.Empty, Name = $"{section.Title}_{DefaultIdType.NewGuid():N}" };
                                        var imagePath = await file.UploadAsync<TourItinerarySectionImageRequest>(image.Image, FileType.Image, cancellationToken);

                                        sectionImages.Add(new TourItinerarySectionImage(imagePath, imagePath));
                                    }
                                    else
                                    {
                                        sectionImages.Add(new TourItinerarySectionImage(image.ImagePath, image.ThumbnailImagePath));
                                    }
                                }
                            }

                            itinerarySections.Add(new TourItinerarySection(section.Title, section.SubTitle, section.Description, section.Highlights, sectionImages));
                        }
                    }

                    var sectionsToRemove = itinerary.Sections.Where(existingContent => itinerarySections.All(newRoom => newRoom.Id != existingContent.Id)).ToList();

                    foreach (var section in sectionsToRemove)
                    {
                        section.DomainEvents.Add(EntityDeletedEvent.WithEntity(section));
                        section.FlagAsDeleted(userId);
                        itinerarySections.Add(section);
                    }

                    itinerary.Sections = itinerarySections;
                    
                    itinerary.Update(itineraryRequest.Header, itineraryRequest.Title, itineraryRequest.Description, tour.Id, itinerarySections);
                    itineraries.Add(itinerary);
                }
            }
        }
        
        var itinerariesToRemove = tour.TourItineraries?
            .Where(existingRoom => itineraries.All(newRoom => newRoom.Id != existingRoom.Id))
            .ToList();

        if (itinerariesToRemove != null && itinerariesToRemove.Count != 0)
        {
            foreach (var itinerary in itinerariesToRemove)
            {
                itinerary.DomainEvents.Add(EntityDeletedEvent.WithEntity(itinerary));
                itinerary.FlagAsDeleted(userId);
                itineraries.Add(itinerary);
            }
        }

        tour.TourItineraries = itineraries;
    }
    
    public static async Task ProcessImages(this Tour property, IList<TourImageRequest>? request, DefaultIdType userId, IFileStorageService file, CancellationToken cancellationToken)
    {
        if (request?.Any() == true)
        {
            var requestImages = new List<TourImage>();

            foreach (var requestImage in request)
            {
                var image = property.Images?.FirstOrDefault(x => x.Id == requestImage.Id);

                if (image == null)
                {
                    var imagePath =
                        await file.UploadAsync<TourImage>(requestImage.Image, FileType.Image, cancellationToken);
                    requestImages.Add(new TourImage(imagePath, imagePath, requestImage.SortOrder, property.Id));
                }
                else
                {
                    image.Update(image.ImagePath, image.ThumbnailImagePath, requestImage.SortOrder);
                    requestImages.Add(image);
                }
            }

            var imagesToRemove = property.Images?
                .Where(existingImage => requestImages.All(newImage => newImage.Id != existingImage.Id))
                .ToList();

            if (imagesToRemove != null && imagesToRemove.Count != 0)
            {
                foreach (var image in imagesToRemove)
                {
                    image.DomainEvents.Add(EntityDeletedEvent.WithEntity(image));
                    image.FlagAsDeleted(userId);
                    requestImages.Add(image);
                }
            }

            property.Images = requestImages;
        }
        else
            property.Images = Array.Empty<TourImage>();
    }
}