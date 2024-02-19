using Travaloud.Application.Catalog.Properties.Commands;
using Travaloud.Domain.Catalog.Properties;

namespace Travaloud.Application.Common.Extensions;

public static class PropertiesExtensions
{
    public static void AddDirections(this Property property, CreatePropertyRequest request)
    {
        if (request.Directions?.Any() != true) return;
        var directions = new List<PropertyDirection>();

        foreach (var direction in request.Directions)
        {
            var newDirection = new PropertyDirection(direction.Title);

            if (direction.Content?.Any() == true)
            {
                newDirection.Content = direction.Content
                    .Select(content => new PropertyDirectionContent(content.Body, content.Style)).ToList();
            }

            directions.Add(newDirection);
        }

        property.Directions = directions;
    }

    public static async Task AddRooms(this Property property, CreatePropertyRequest request, IFileStorageService file,
        CancellationToken cancellationToken)
    {
        if (request.Rooms?.Any() == true)
        {
            var rooms = new List<PropertyRoom>();

            foreach (var room in request.Rooms)
            {
                var roomImagePath = await file.UploadAsync<PropertyRoom>(room.Image, FileType.Image, cancellationToken);
                rooms.Add(new PropertyRoom(room.Name, room.Description, room.ShortDescription, roomImagePath));
            }

            property.Rooms = rooms;
        }
    }

    public static void AddFacilities(this Property property, CreatePropertyRequest request)
    {
        if (request.Facilities?.Any() != true) return;
        var facilities = request.Facilities.Select(facility => new PropertyFacility(facility.Title)).ToList();

        property.Facilities = facilities;
    }

    public static void AddDestinations(this Property property, CreatePropertyRequest request)
    {
        if (request.PropertyDestinationLookups?.Any() != true) return;
        var destinations = new List<PropertyDestinationLookup>();

        foreach (var destination in destinations)
        {
            destinations.Add(new PropertyDestinationLookup(destination.DestinationId));
        }

        property.PropertyDestinationLookups = destinations;
    }

    public static async Task AddImages(this Property property, CreatePropertyRequest request, IFileStorageService file,
        CancellationToken cancellationToken)
    {
        if (request.Images?.Any() == true)
        {
            var images = new List<PropertyImage>();

            foreach (var image in request.Images)
            {
                var roomImagePath =
                    await file.UploadAsync<PropertyImage>(image.Image, FileType.Image, cancellationToken);
                images.Add(new PropertyImage(roomImagePath, roomImagePath, image.SortOrder));
            }

            property.Images = images;
        }
    }

    public static void AddDirections(this Property property, UpdatePropertyRequest request)
    {
        if (request.Directions?.Any() != true) return;
        
        var directions = new List<PropertyDirection>();
        foreach (var directionRequest in request.Directions)
        {
            var direction = property.Directions.FirstOrDefault(i => i.Id == directionRequest.Id);

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

                if (directionRequest.Content?.Any() == true)
                {
                    var contents = new List<PropertyDirectionContent>();

                    foreach (var contentRequest in directionRequest.Content)
                    {
                        var content = direction.Content.FirstOrDefault(i => i.Id == contentRequest.Id);

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

                directions.Add(direction);
            }
        }

        property.Directions = directions;
    }
    
    public static void AddFacilities(this Property property, UpdatePropertyRequest request)
    {
        if (request.Facilities?.Any() != true) return;
        
        var facilities = new List<PropertyFacility>();

        foreach (var facilityRequest in request.Facilities)
        {
            var facility = property.Facilities.FirstOrDefault(x => x.Id == facilityRequest.Id);

            if (facility == null)
            {
                facilities.Add(new PropertyFacility(facilityRequest.Title));
            }
            else
            {
                facility.Update(facilityRequest.Title);
                facilities.Add(facility);
            }
        }

        property.Facilities = facilities;
    }
    
    public static void AddDestinations(this Property property, UpdatePropertyRequest request)
    {
        if (request.PropertyDestinationLookups?.Any() != true) return;
        var destinationLookups = (from destinationRequest in request.PropertyDestinationLookups let destination = property.PropertyDestinationLookups.FirstOrDefault(x => x.Id == destinationRequest.Id) where destination == null select new PropertyDestinationLookup(destinationRequest.DestinationId)).ToList();

        property.PropertyDestinationLookups = destinationLookups;
    }

    public static async Task AddRooms(this Property property, UpdatePropertyRequest request, IFileStorageService file, CancellationToken cancellationToken)
    {
        if (request.Rooms?.Any() == true)
        {
            var rooms = new List<PropertyRoom>();
            foreach (var roomRequest in request.Rooms)
            {
                var room = property.Rooms.FirstOrDefault(x => x.Id == roomRequest.Id);

                if (room == null)
                {
                    var roomImagePath = await file.UploadAsync<PropertyRoom>(roomRequest.Image, FileType.Image, cancellationToken);
                    rooms.Add(new PropertyRoom(roomRequest.Name, roomRequest.Description, roomRequest.ShortDescription, roomImagePath));
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
                    rooms.Add(room);
                }
            }

            property.Rooms = rooms;
        }
    }

    public static async Task AddImages(this Property property,UpdatePropertyRequest request, IFileStorageService file, CancellationToken cancellationToken)
    {
        if (request.Images?.Any() == true)
        {
            var images = new List<PropertyImage>();
            foreach (var imageRequest in request.Images)
            {
                var image = property.Images?.FirstOrDefault(x => x.Id == imageRequest.Id);

                if (image == null)
                {
                    var imagePath = await file.UploadAsync<PropertyImage>(imageRequest.Image, FileType.Image, cancellationToken);
                    images.Add(new PropertyImage(imagePath, imagePath, imageRequest.SortOrder, property.Id));
                }
                else
                {
                    if (imageRequest.DeleteCurrentImage)
                    {
                        var currentProductImagePath = imageRequest.ImagePath;
                        if (!string.IsNullOrEmpty(currentProductImagePath))
                        {
                            var root = Directory.GetCurrentDirectory();
                            await file.Remove(Path.Combine(root, currentProductImagePath));
                        }

                        image = image.ClearImagePath();
                    }

                    var imagePath = imageRequest.Image is not null
                        ? image.ImagePath
                        : null;

                    image.Update(imagePath, imagePath, imageRequest.SortOrder, property.Id);
                    images.Add(image);
                }
            }

            property.Images = images;
        }
    }
}