namespace Travaloud.Application.Catalog.Properties.Commands;

public class UpdatePropertyRoomRequest : IRequest<DefaultIdType>
{
    public DefaultIdType Id { get; set; }
    public DefaultIdType PropertyId { get; set; }
    public string Name { get; set; } = default!;
    public string Description { get; set; } = default!;
    public string? ShortDescription { get; set; }
    public string? ImagePath { get; set; }
    public bool DeleteCurrentImage { get; set; }
    public FileUploadRequest? Image { get; set; }
    public string? ImageInBytes { get; set; }
    public string? ImageExtension { get; set; }
}