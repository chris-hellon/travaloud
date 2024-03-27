using Travaloud.Application.Catalog.Properties.Commands;
using Travaloud.Domain.Catalog.Properties;
using Travaloud.Domain.Common.Events;

namespace Travaloud.Application.Common.Extensions;

public static class PropertiesExtensions
{
    public static void ProcessDirections(this Property property, IList<PropertyDirectionRequest>? request, DefaultIdType userId)
    {
        var directions = new List<PropertyDirection>();
        
        if (request?.Any() == true)
        {
            foreach (var directionRequest in request)
            {
                var direction = property.Directions?.FirstOrDefault(i => i.Id == directionRequest.Id);

                if (direction == null)
                {
                    var newDirection = new PropertyDirection(directionRequest.Title);

                    if (directionRequest.Content?.Any() == true)
                    {
                        var contents = directionRequest.Content.Select(content => new PropertyDirectionContent(content.Body, content.Style)).ToList();

                        newDirection.Content = contents;
                    }

                    directions.Add(newDirection);
                }
                else
                {
                    direction.Update(directionRequest.Title);

                    var contents = new List<PropertyDirectionContent>();
                    
                    if (directionRequest.Content?.Any() == true)
                    {
                        foreach (var contentRequest in directionRequest.Content)
                        {
                            var content = direction.Content?.FirstOrDefault(i => i.Id == contentRequest.Id);

                            if (content == null)
                            {
                                contents.Add(new PropertyDirectionContent(contentRequest.Body, contentRequest.Style));
                            }
                            else
                            {
                                content.Update(contentRequest.Body, contentRequest.Style);
                                contents.Add(content);
                            }
                        }
                    }
                    
                    var directionsContentToRemove = direction.Content?
                        .Where(existingContent => contents.All(newRoom => newRoom.Id != existingContent.Id))
                        .ToList();

                    if (directionsContentToRemove != null)
                    {
                        foreach (var directionContent in directionsContentToRemove)
                        {
                            directionContent.DomainEvents.Add(EntityDeletedEvent.WithEntity(directionContent));
                            directionContent.FlagAsDeleted(userId);
                            contents.Add(directionContent);
                        }   
                    }
                    
                    direction.Content = contents;

                    directions.Add(direction);
                }
            }
        }
        
        var directionsToRemove = property.Directions?
            .Where(existingRoom => directions.All(newRoom => newRoom.Id != existingRoom.Id))
            .ToList();

        if (directionsToRemove != null && directionsToRemove.Count != 0)
        {
            foreach (var direction in directionsToRemove)
            {
                direction.DomainEvents.Add(EntityDeletedEvent.WithEntity(direction));
                direction.FlagAsDeleted(userId);
                directions.Add(direction);
            }
        }

        property.Directions = directions;
    }
    
    public static void ProcessFacilities(this Property property, IList<PropertyFacilityRequest>? request, DefaultIdType userId)
    {
        var requestFacilities = new List<PropertyFacility>();
        
        if (request?.Any() == true)
        {
            foreach (var facilityRequest in request)
            {
                var facility = property.Facilities?.FirstOrDefault(x => x.Id == facilityRequest.Id);

                if (facility == null)
                {
                    requestFacilities.Add(new PropertyFacility(facilityRequest.Title));
                }
                else
                {
                    facility.Update(facilityRequest.Title);
                    requestFacilities.Add(facility);
                }
            }
        }
        
        var facilitiesToRemove = property.Facilities?
            .Where(existingRoom => requestFacilities.All(newRoom => newRoom.Id != existingRoom.Id))
            .ToList();

        if (facilitiesToRemove != null && facilitiesToRemove.Count != 0)
        {
            foreach (var facility in facilitiesToRemove)
            {
                facility.DomainEvents.Add(EntityDeletedEvent.WithEntity(facility));
                facility.FlagAsDeleted(userId);
                requestFacilities.Add(facility);
            }
        }
        
        property.Facilities = requestFacilities;
    }
    
    // public static void ProcessDestinations(this Property property, IEnumerable<PropertyDestinationLookupRequest> request, DefaultIdType userId)
    // {
    //     var requestDestinations = (from destinationRequest in request let destination = property.PropertyDestinationLookups?.FirstOrDefault(x => x.Id == destinationRequest.Id) select destination ?? new PropertyDestinationLookup(destinationRequest.DestinationId)).ToList();
    //
    //     var destinationsToRemove = property.PropertyDestinationLookups?
    //         .Where(existingRoom => requestDestinations.All(newRoom => newRoom.Id != existingRoom.Id))
    //         .ToList();
    //
    //     if (destinationsToRemove != null && destinationsToRemove.Count != 0)
    //     {
    //         foreach (var destination in destinationsToRemove)
    //         {
    //             destination.DomainEvents.Add(EntityDeletedEvent.WithEntity(destination));
    //             destination.FlagAsDeleted(userId);
    //             requestDestinations.Add(destination);
    //         }
    //     }
    //
    //     property.PropertyDestinationLookups = requestDestinations;
    // }
    //
    public static async Task ProcessRooms(this Property property, IList<PropertyRoomRequest>? request, DefaultIdType userId, IFileStorageService file, CancellationToken cancellationToken)
    {
        var requestRooms = new List<PropertyRoom>();
        
        if (request?.Any() == true)
        {
            foreach (var roomRequest in request)
            {
                var room = property.Rooms?.FirstOrDefault(x => x.Id == roomRequest.Id);

                if (room == null)
                {
                    var roomImagePath = await file.UploadAsync<PropertyRoom>(roomRequest.Image, FileType.Image, cancellationToken);
                    requestRooms.Add(new PropertyRoom(roomRequest.Name, roomRequest.Description, roomRequest.ShortDescription, roomImagePath));
                }
                else
                {
                    if (roomRequest.DeleteCurrentImage)
                    {
                        var currentProductImagePath = roomRequest.ImagePath;
                        if (!string.IsNullOrEmpty(currentProductImagePath))
                        {
                            var root = Directory.GetCurrentDirectory();
                            await file.Remove(Path.Combine(root, currentProductImagePath));
                        }

                        room = room.ClearImagePath();
                    }

                    var roomImagePath = roomRequest.Image is not null
                        ? roomRequest.ImagePath
                        : null;

                    room.Update(roomRequest.Name, roomRequest.Description, roomRequest.ShortDescription, roomImagePath);
                    requestRooms.Add(room);
                }
            }
        }
        
        var roomsToRemove = property.Rooms?
            .Where(existingRoom => requestRooms.All(newRoom => newRoom.Id != existingRoom.Id))
            .ToList();

        if (roomsToRemove != null && roomsToRemove.Count != 0)
        {
            foreach (var room in roomsToRemove)
            {
                room.DomainEvents.Add(EntityDeletedEvent.WithEntity(room));
                room.FlagAsDeleted(userId);
                requestRooms.Add(room);
            }
        }

        property.Rooms = requestRooms;
    }
    
    public static async Task ProcessImages(this Property property, IList<PropertyImageRequest>? request, DefaultIdType userId, IFileStorageService file, CancellationToken cancellationToken)
    {
        var requestImages = new List<PropertyImage>();
        
        if (request?.Any() == true)
        {
            foreach (var requestImage in request)
            {
                var image = property.Images?.FirstOrDefault(x => x.Id == requestImage.Id);

                if (image == null)
                {
                    var imagePath = await file.UploadAsync<PropertyImage>(requestImage.Image, FileType.Image, cancellationToken);
                    requestImages.Add(new PropertyImage(imagePath, imagePath, requestImage.SortOrder, property.Id));
                }
                else
                {
                    image.Update(image.ImagePath, image.ThumbnailImagePath, requestImage.SortOrder);
                    requestImages.Add(image);
                }
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
    
    public static void ProcessDestinations(this Property tour, IEnumerable<PropertyDestinationLookupRequest>? request, DefaultIdType userId)
    {
        var tourDestinationLookups = new List<PropertyDestinationLookup>();

        if (request?.Any() == true)
        {
            tourDestinationLookups.AddRange(request.Select(tourDestinationLookupRequest =>
                new PropertyDestinationLookup(tourDestinationLookupRequest.PropertyId,
                    tourDestinationLookupRequest.DestinationId)));
        }

        var destinationsToRemove = tour.PropertyDestinationLookups?
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

        tour.PropertyDestinationLookups = tourDestinationLookups;
    }


}